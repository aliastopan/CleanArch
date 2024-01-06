#nullable disable

namespace CleanArch.Shared.Contracts;

public record ProblemDetailsResponse
{
    public string Type { get; init; }
    public string Title { get; init; }
    public int Status { get; init; }
    public string TraceId { get; init; }
    public List<string> Errors { get; init; }
}
