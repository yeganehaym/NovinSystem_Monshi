using Monshi.Domain.Users.Entities;

namespace Monshi.Domain.Users;

public interface IUserService
{
    Task NewUserAsync(User user);
    Task<User> FindUserAsync(int id);
    Task<User> FindUserAsync(string username);
    Task<User> FindUserAsync(string username,string password);

    Task NewOtpCodeAsync(OtpCode otpCode);
    Task<OtpCode> GetOtpCode(string code);
}