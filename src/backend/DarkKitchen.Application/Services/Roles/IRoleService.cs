using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Application.Services.Roles;

public interface IRoleService
{
    void AssignPermissionToRole(int roleId, int permissionId);
    Role GetRoleById(int roleId);
}
