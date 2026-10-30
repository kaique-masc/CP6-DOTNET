using Recommenda.Application.Common;
using Recommenda.Application.DTOs.Contents;

namespace Recommenda.Application.Services;

public interface IContentService
{
    Task<IReadOnlyList<ContentResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<ContentResponse>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ContentDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ContentDetailResponse> CreateAsync(ContentRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, ContentRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
