using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.Application.Identity.Queries.GetUserAccounts;

public class GetUserAccountsQueryHandler : IRequestHandler<GetUserAccountsQuery, Result<GetUserAccountsQueryResponse>>
{
    public GetUserAccountsQueryHandler()
    {

    }

    public async ValueTask<Result<GetUserAccountsQueryResponse>> Handle(GetUserAccountsQuery request,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
