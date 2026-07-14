using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Application.Services.Roles;

public sealed class RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository) : IRoleService
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IPermissionRepository _permissionRepository = permissionRepository;

    public void AssignPermissionToRole(int roleId, int permissionId)
    {
        var role = _roleRepository.GetById(roleId)
            ?? throw new ResourceNotFoundException("Role", roleId);

        var permission = _permissionRepository.GetById(permissionId)
            ?? throw new ResourceNotFoundException("Permission", permissionId);

        role.AddPermission(permission);
        _roleRepository.Update(role);
    }

    public Role GetRoleById(int roleId)
    {
        return _roleRepository.GetById(roleId)
            ?? throw new ResourceNotFoundException("Role", roleId);
    }
}
