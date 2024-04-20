namespace Payme.Extensions;

public static class DateTimeExtension
{
    public static long GetUnixTicks(this DateTime value)
    {
        return new DateTimeOffset(value)
            .ToUnixTimeMilliseconds();
    }
}