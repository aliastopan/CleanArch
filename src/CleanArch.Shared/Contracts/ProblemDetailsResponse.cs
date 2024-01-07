#nullable disable

namespace CleanArch.Shared.Contracts;

public record ProblemDetailsResponse
{
    public string Type { get; init; }
    public string Title { get; init; }
    public int Status { get; init; }
    public string TraceId { get; init; }
    public List<Error> Errors { get; init; }

    public record Error
    {
        public string Message { get; init; }
        public string Severity { get; init; }
    }
}

