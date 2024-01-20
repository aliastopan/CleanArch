using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.Application.Identity.Queries.GetUserAccounts;

public class GetUserAccountsQueryHandler : IRequestHandler<GetUserAccountsQuery, Result<GetUserAccountsQueryResponse>>
{
    private readonly IIdentityAggregateHandler _identityAggregateHandler;

    public GetUserAccountsQueryHandler(IIdentityAggregateHandler identityAggregateHandler)
    {
        _identityAggregateHandler = identityAggregateHandler;
    }

    public async ValueTask<Result<GetUserAccountsQueryResponse>> Handle(GetUserAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var tryGetRange = await _identityAggregateHandler.TryGetRangeUserAccountsAsync();
        if (tryGetRange.IsFailure())
        {
            var failure = Result<GetUserAccountsQueryResponse>.Inherit(result: tryGetRange);
            return await ValueTask.FromResult(failure);
        }

        var userAccounts = tryGetRange.Value;

        var userAccountDtos = new List<UserAccountDto>();
        foreach (var userAccount in userAccounts)
        {
            userAccountDtos.Add(new UserAccountDto
            {
                UserAccountId = userAccount.UserAccountId,
                UserRole = userAccount.User.UserRole.ToString(),
                UserPrivileges = userAccount.User.UserPrivileges.Select(privilege => privilege.ToString()).ToList(),
                LastLoggedIn = userAccount.LastSignedIn.DateTime.ToLocalTime()
            });
        }

        var response = new GetUserAccountsQueryResponse(userAccountDtos);
        var ok = Result<GetUserAccountsQueryResponse>.Ok(response);
        return await ValueTask.FromResult(ok);
    }
}
