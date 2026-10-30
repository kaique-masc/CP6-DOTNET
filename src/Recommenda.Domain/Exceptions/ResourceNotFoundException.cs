namespace Recommenda.Domain.Exceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string resource, object key)
        : base($"{resource} com identificador '{key}' não foi encontrado.")
    {
        Resource = resource;
        Key = key;
    }

    public string Resource { get; }
    public object Key { get; }
}
