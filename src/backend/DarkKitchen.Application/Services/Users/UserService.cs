using DarkKitchen.Application.Services.Users.Dtos;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Domain.Validators;
using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Application.Services.Users;

public sealed class UserService(IUserRepository userRepository, IRoleRepository roleRepository, IPhoneValidator phoneValidator) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IPhoneValidator _phoneValidator = phoneValidator;

    public Guid Register(RegisterUserDto dto)
    {
        var emailExists = _userRepository.Exists(u => u.Email == dto.Email);
        if(emailExists)
        {
            throw new DuplicateResourceException("User", "Email", dto.Email);
        }

        var password = Password.Create(dto.Password);
        var phone = PhoneNumber.Create(dto.Phone, _phoneValidator);
        var user = new User(dto.FirstName, dto.LastName, dto.Email, password, phone);

        _userRepository.Add(user);

        return user.Id;
    }

    public IReadOnlyList<UserSummaryDto> GetUsers(GetUsersDto dto)
    {
        return _userRepository
            .GetByFilters(dto.Search, dto.Role)
            .Select(ToUserSummaryDto)
            .ToList();
    }

    public UserSummaryDto CreateByAdmin(CreateAdminUserDto dto)
    {
        var emailExists = _userRepository.Exists(u => u.Email == dto.Email);
        if(emailExists)
        {
            throw new DuplicateResourceException("User", "Email", dto.Email);
        }

        var role = _roleRepository.GetById(dto.RoleId)
            ?? throw new ResourceNotFoundException("Role", dto.RoleId);

        var password = Password.Create(dto.Password);
        var phone = PhoneNumber.Create(dto.Phone, _phoneValidator);
        var user = new User(dto.FirstName, dto.LastName, dto.Email, password, phone)
        {
            RoleId = role.RoleId,
            Role = role
        };

        _userRepository.Add(user);

        return ToUserSummaryDto(user);
    }

    public UserSummaryDto Update(Guid requesterId, Guid targetId, UpdateUserDto dto)
    {
        if(requesterId == targetId)
        {
            throw new InvalidArgumentException("A user cannot modify themselves");
        }

        var user = _userRepository.GetById(targetId)
            ?? throw new ResourceNotFoundException("User", targetId);

        var role = _roleRepository.GetById(dto.RoleId)
            ?? throw new ResourceNotFoundException("Role", dto.RoleId);

        var emailTaken = _userRepository.Exists(u => u.Email == dto.Email && u.Id != targetId);
        if(emailTaken)
        {
            throw new DuplicateResourceException("User", "Email", dto.Email);
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.Phone = PhoneNumber.Create(dto.Phone, _phoneValidator);
        user.RoleId = role.RoleId;
        user.Role = role;

        if(dto.Password != null)
        {
            user.Password = Password.Create(dto.Password);
        }

        _userRepository.Update(user);

        return ToUserSummaryDto(user);
    }

    public void Delete(Guid requesterId, Guid targetId)
    {
        if(requesterId == targetId)
        {
            throw new InvalidArgumentException("A user cannot delete themselves");
        }

        var exists = _userRepository.Exists(u => u.Id == targetId);
        if(!exists)
        {
            throw new ResourceNotFoundException("User", targetId);
        }

        _userRepository.Delete(targetId);
    }

    private static UserSummaryDto ToUserSummaryDto(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Phone = user.Phone.Value,
        RoleId = user.RoleId,
        RoleName = user.Role.RoleName
    };
}
