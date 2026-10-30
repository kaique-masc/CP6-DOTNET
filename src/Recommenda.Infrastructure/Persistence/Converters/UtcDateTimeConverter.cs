using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Recommenda.Infrastructure.Persistence.Converters;

// O MySQL não guarda o Kind do DateTime; todas as datas são gravadas e lidas como UTC.
public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(
            value => value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime(),
            value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
    {
    }
}
