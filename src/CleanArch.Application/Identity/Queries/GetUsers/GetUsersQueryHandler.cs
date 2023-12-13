
namespace CleanArch.Application.Identity.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<GetUsersQueryResponse>>
{
    private readonly IAppDbContextFactory<IAppDbContext> _dbContextFactory;

    public GetUsersQueryHandler(IAppDbContextFactory<IAppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async ValueTask<Result<GetUsersQueryResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        Result<GetUsersQueryResponse> result;

        using var dbContext = _dbContextFactory.CreateDbContext();

        var users = await dbContext.GetUsersAsync();
        if(users.Count == 0)
        {
            result = Result<GetUsersQueryResponse>.NotFound();
            return await ValueTask.FromResult(result);
        }

        var userDtos = new List<UserDto>();
        foreach(var user in users)
        {
            var userDto = new UserDto(user.UserId,
                user.Role.ToString(),
                user.LastLoggedIn.DateTime.ToLocalTime());

            userDtos.Add(userDto);
        }

        var response = new GetUsersQueryResponse(userDtos);
        result = Result<GetUsersQueryResponse>.Ok(response);
        return await ValueTask.FromResult(result);
    }
}
