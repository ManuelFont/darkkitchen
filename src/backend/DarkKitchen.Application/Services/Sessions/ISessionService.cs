using DarkKitchen.Application.Services.Sessions.Dtos;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Application.Services.Sessions;

public interface ISessionService
{
    User GetUserByToken(Guid token);
    LoginResultDto Login(LoginDto dto);
    void Logout(Guid token);
}
