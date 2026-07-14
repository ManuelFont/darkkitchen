using DarkKitchen.Application.Services.Sessions.Dtos;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Domain.Repositories.Sessions;

namespace DarkKitchen.Application.Services.Sessions;

public sealed class SessionService(ISessionRepository sessionRepository, IUserRepository userRepository) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public User GetUserByToken(Guid token)
    {
        var session = _sessionRepository.GetById(token);

        if(session is null)
        {
            throw new ResourceNotFoundException("Session", token);
        }

        if(session.ExpiresAt < DateTime.Now)
        {
            throw new TokenExpiredException("Token expired");
        }

        var user = _userRepository.GetById(session.UserId);

        return user!;
    }

    public LoginResultDto Login(LoginDto dto)
    {
        var userExists = _userRepository.Exists(u => u.Email == dto.Email);
        if(!userExists)
        {
            throw new InvalidArgumentException("Invalid email or password");
        }

        var users = _userRepository.GetAll();
        var user = users.First(u => u.Email == dto.Email);

        if(user.Password.Value != dto.Password)
        {
            throw new InvalidArgumentException("Invalid email or password");
        }

        var session = new Session(user.Id, DateTime.Now.AddHours(1));
        _sessionRepository.Add(session);

        var roleName = _userRepository.GetById(user.Id)!.Role.RoleName;

        return new LoginResultDto { Token = session.Token, RoleName = roleName };
    }

    public void Logout(Guid token)
    {
        _ = _sessionRepository.GetById(token)
          ?? throw new ResourceNotFoundException("Session", token);

        _sessionRepository.Delete(token);
    }
}
