using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Recommenda.Infrastructure.Persistence.Converters;

// O provider MySql.EntityFrameworkCore lê colunas DATE como DateTime.
public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter()
        : base(
            value => value.ToDateTime(TimeOnly.MinValue),
            value => DateOnly.FromDateTime(value))
    {
    }
}
