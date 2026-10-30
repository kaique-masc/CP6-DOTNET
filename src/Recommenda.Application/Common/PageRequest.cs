using Recommenda.Application.Exceptions;

namespace Recommenda.Application.Common;

public sealed record PageRequest
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    private PageRequest(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public int Page { get; }
    public int PageSize { get; }

    public static PageRequest Create(int page, int pageSize)
    {
        if (page < 1)
            throw new ValidationException($"O parâmetro 'page' deve ser maior ou igual a 1. Valor recebido: {page}.");

        if (pageSize < 1 || pageSize > MaxPageSize)
            throw new ValidationException($"O parâmetro 'pageSize' deve estar entre 1 e {MaxPageSize}. Valor recebido: {pageSize}.");

        return new PageRequest(page, pageSize);
    }
}
