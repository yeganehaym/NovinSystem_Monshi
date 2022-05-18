using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Monshi.Data.SqlServer;

public class TimeSpanConverter:ValueConverter<TimeSpan,long>
{
    public TimeSpanConverter() :
        base(t => t.Ticks,
            l => new TimeSpan(l))

    {

    }
}