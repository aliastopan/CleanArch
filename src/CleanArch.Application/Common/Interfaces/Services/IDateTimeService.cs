namespace CleanArch.Application.Common.Interfaces.Services;

public interface IDateTimeService
{
    DateTime UtcNow { get; }
    DateTimeOffset DateTimeOffsetNow { get; }
}
