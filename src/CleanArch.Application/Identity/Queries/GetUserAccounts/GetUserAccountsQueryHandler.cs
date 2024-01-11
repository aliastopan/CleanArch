using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.Application.Identity.Queries.GetUserAccounts;

public class GetUserAccountsQueryHandler : IRequestHandler<GetUserAccountsQuery, Result<GetUserAccountsQueryResponse>>
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;

    public GetUserAccountsQueryHandler(IAppDbContextFactory<IAppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async ValueTask<Result<GetUserAccountsQueryResponse>> Handle(GetUserAccountsQuery request,
        CancellationToken cancellationToken)
    {
        Result<GetUserAccountsQueryResponse> result;

        using var dbContext = _dbContextFactory.CreateDbContext();

        var userAccounts = await dbContext.GetUserAccountAsync();
        if (userAccounts.Count == 0)
        {
            result = Result<GetUserAccountsQueryResponse>.NotFound();
            return await ValueTask.FromResult(result);
        }

        var userAccountDtos = new List<UserAccountDto>();
        foreach (var userAccount in userAccounts)
        {
            var userAccountDto = new UserAccountDto
            {
                UserAccountId = userAccount.UserAccountId,
                UserRole = userAccount.UserRole.ToString(),
                UserPrivileges = userAccount.UserPrivileges.Select(privilege => privilege.ToString()).ToList(),
                LastLoggedIn = userAccount.LastSignedIn.DateTime.ToLocalTime()
            };

            userAccountDtos.Add(userAccountDto);

            Log.Warning("USER: {0}", userAccount.User.Username);
        }

        var response = new GetUserAccountsQueryResponse(userAccountDtos);
        result = Result<GetUserAccountsQueryResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }
}
