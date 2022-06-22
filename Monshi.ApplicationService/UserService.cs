using Microsoft.EntityFrameworkCore;
using Monshi.Data.SqlServer;
using Monshi.Domain.Users;
using Monshi.Domain.Users.Entities;

namespace Monshi.ApplicationService;

public class UserService:IUserService
{
    private ApplicationDbContext _applicationDbContext;

    public UserService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task NewUserAsync(User user)
    {
        await _applicationDbContext.AddAsync(user);
    }

    public async Task<User> FindUserAsync(int id)
    {
        return await _applicationDbContext.Users.FindAsync(id);
    }

    public async Task<User> FindUserAsync(string username)
    {
        return await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

    }

    public async Task<User> FindUserAsync(string username, string password)
    {
        return await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Username == username
        && x.Password==password);

    }

    public async Task NewOtpCodeAsync(OtpCode otpCode)
    {
        await _applicationDbContext.AddAsync(otpCode);
    }

    public async Task<OtpCode> GetOtpCode(string code)
    {
        return await _applicationDbContext.OtpCodes
                
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x => x.Code == code);
    }
    
    public async Task<OtpCode> GetOtpCode2(string code)
    {
        var join= await _applicationDbContext.OtpCodes
            .AsNoTracking()
                
            .Join(_applicationDbContext.Users,
                x=>x.User.Id,
                y=>y.Id,
                (x, y) => new
            {
                OTp=x,
                User=y
            })
            .FirstOrDefaultAsync(x => x.OTp.Code==code);

        return null;
    }
}