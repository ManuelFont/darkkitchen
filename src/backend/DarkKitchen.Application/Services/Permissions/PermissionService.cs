using DarkKitchen.Application.Services.Permissions.Dtos;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Application.Services.Permissions;

public sealed class PermissionService(IPermissionRepository permissionRepository) : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository = permissionRepository;

    public int CreatePermission(CreatePermissionDto dto)
    {
        var exists = _permissionRepository.Exists(p => p.PermissionName == dto.PermissionName);
        if(exists)
        {
            throw new DuplicateResourceException("Permission", "PermissionName", dto.PermissionName);
        }

        var permission = new Permission(dto.PermissionId, dto.PermissionName);
        _permissionRepository.Add(permission);

        return permission.PermissionId;
    }
}
