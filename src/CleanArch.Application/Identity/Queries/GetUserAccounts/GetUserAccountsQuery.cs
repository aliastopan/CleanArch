using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.Application.Identity.Queries.GetUserAccounts;

public class GetUserAccountsQuery() : IRequest<Result<GetUserAccountsQueryResponse>>;
