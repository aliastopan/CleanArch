namespace CleanArch.Infrastructure.Services;

internal sealed class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
