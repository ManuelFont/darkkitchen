using DarkKitchen.Application.Services.Permissions.Dtos;

namespace DarkKitchen.Application.Services.Permissions;

public interface IPermissionService
{
    int CreatePermission(CreatePermissionDto dto);
}
