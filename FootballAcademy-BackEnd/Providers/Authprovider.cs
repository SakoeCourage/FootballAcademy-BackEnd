using AutoMapper;
using FootballAcademy_BackEnd.Contracts;
using FootballAcademy_BackEnd.Database;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FootballAcademy_BackEnd.Providers
{
    public class Authprovider
    {
        private readonly FootballAcademyDBContext _dbContext;
        private readonly HttpContext _httpContext;
        private readonly IMapper _mapper;

        public Authprovider(IServiceScopeFactory serviceScopeFactory)
        {
            var scope = serviceScopeFactory.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<FootballAcademyDBContext>();
            _httpContext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        }

        public async Task<GetUserResponse?> GetAuthUser()
        {

            ClaimsIdentity identity = _httpContext?.User.Identity as ClaimsIdentity;

            if (identity == null) return null;

            Claim userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

            var authUserId = userIdClaim?.Value;

            if (authUserId is not null)
            {
                var userJoin = await _dbContext
                    .User
                    .Where(u => u.Id.ToString() == authUserId.ToString())
                        .Join(_dbContext.Role.Include(r => r.Permissions),
                            u => u.RoleId,
                            r => r.Id,
                            (u, r) => new
                            {
                                Role = r,
                                User = u
                            }
                        ).Join(_dbContext.Staff,
                                ur => ur.User.Id,
                                s => s.UserId,
                                (ur, s) => new
                                {
                                    User = ur.User,
                                    Role = ur.Role,
                                    Staff = s
                                })
                    .FirstOrDefaultAsync();

                var UserData = userJoin?.User;
                var StaffData = userJoin?.Staff;
                var UserRole = userJoin?.Role;
                var response = _mapper.Map<GetUserResponse>(UserData);
                response.Staff = StaffData;
                response.Role = UserRole;

                return response;
            }
            return null;
        }
    }
}
