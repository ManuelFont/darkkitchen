using DarkKitchen.Application.Services.Users.Dtos;

namespace DarkKitchen.Application.Services.Users;

public interface IUserService
{
    Guid Register(RegisterUserDto dto);
    IReadOnlyList<UserSummaryDto> GetUsers(GetUsersDto dto);
    UserSummaryDto CreateByAdmin(CreateAdminUserDto dto);
    UserSummaryDto Update(Guid requesterId, Guid targetId, UpdateUserDto dto);
    void Delete(Guid requesterId, Guid targetId);
}
