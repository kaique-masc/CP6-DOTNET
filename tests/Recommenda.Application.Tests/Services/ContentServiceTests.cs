using System.Linq.Expressions;
using Moq;
using Recommenda.Application.Abstractions.Persistence;
using Recommenda.Application.DTOs.Contents;
using Recommenda.Application.Exceptions;
using Recommenda.Application.Services;
using Recommenda.Domain.Entities;
using Recommenda.Domain.Enums;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Application.Tests.Services;

public class ContentServiceTests
{
    private readonly Mock<IContentRepository> _contents = new();
    private readonly Mock<IRepository<Genre>> _genres = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly ContentService _service;

    public ContentServiceTests()
    {
        _service = new ContentService(_contents.Object, _genres.Object, _unitOfWork.Object);
    }

    private static ContentRequest Request(params Guid[] genreIds) => new()
    {
        Title = "O Iluminado",
        Synopsis = "Um escritor cuida de um hotel isolado.",
        Type = ContentType.Movie,
        ReleaseYear = 1980,
        DurationMinutes = 146,
        AgeRating = 16,
        GenreIds = genreIds.ToList()
    };

    [Fact]
    public async Task CreateAsync_GenreDoesNotExist_ThrowsResourceNotFoundAndDoesNotPersist()
    {
        // Arrange
        var missingGenre = Guid.NewGuid();
        _genres.Setup(g => g.FindAsync(It.IsAny<Expression<Func<Genre, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Genre>());

        // Act
        var act = () => _service.CreateAsync(Request(missingGenre));

        // Assert
        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(act);
        Assert.Equal(missingGenre, exception.Key);
        _contents.Verify(c => c.AddAsync(It.IsAny<Content>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsOnce()
    {
        // Arrange
        var genre = new Genre("Terror");
        _genres.Setup(g => g.FindAsync(It.IsAny<Expression<Func<Genre, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Genre> { genre });

        Content? added = null;
        _contents.Setup(c => c.AddAsync(It.IsAny<Content>(), It.IsAny<CancellationToken>()))
            .Callback<Content, CancellationToken>((content, _) => added = content);
        _contents.Setup(c => c.GetWithGenresAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => added);

        // Act
        var result = await _service.CreateAsync(Request(genre.Id));

        // Assert
        Assert.Equal("O Iluminado", result.Title);
        _contents.Verify(c => c.AddAsync(It.Is<Content>(x => x.ContentGenres.Count == 1), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ContentDoesNotExist_ThrowsResourceNotFoundAndDoesNotSave()
    {
        // Arrange
        _contents.Setup(c => c.GetWithGenresAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Content?)null);

        // Act
        var act = () => _service.UpdateAsync(Guid.NewGuid(), Request(Guid.NewGuid()));

        // Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(act);
        _contents.Verify(c => c.Update(It.IsAny<Content>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    [InlineData(1, 9999)]
    public async Task GetPagedAsync_InvalidPageOrPageSize_ThrowsValidationExceptionWithoutQuerying(int page, int pageSize)
    {
        // Act
        var act = () => _service.GetPagedAsync(page, pageSize);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _contents.Verify(c => c.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_ValidRange_ReturnsEnvelopeWithTotals()
    {
        // Arrange
        var items = new List<Content>
        {
            new("A Bruxa", null, ContentType.Movie, 2015, 92, 14),
            new("Hereditário", null, ContentType.Movie, 2018, 127, 16)
        };
        _contents.Setup(c => c.GetPagedAsync(2, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, 15));

        // Act
        var result = await _service.GetPagedAsync(2, 2);

        // Assert
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(15, result.TotalItems);
        Assert.Equal(8, result.TotalPages);
        Assert.True(result.HasPrevious);
        Assert.True(result.HasNext);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_PageBeyondTotal_ReturnsEmptyItems()
    {
        // Arrange
        _contents.Setup(c => c.GetPagedAsync(50, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Content>(), 15));

        // Act
        var result = await _service.GetPagedAsync(50, 20);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasNext);
    }
}
