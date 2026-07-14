namespace DarkKitchen.Domain.Entities;

public sealed class Role
{
    private readonly List<Permission> _rolePermissions = [];
    private string _roleName = string.Empty;
    public int RoleId { get; private set; }

    public string RoleName
    {
        get => _roleName;
        private set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Role name is required");
            }

            _roleName = value.Trim();
        }
    }

    public IReadOnlyList<Permission> RolePermissions => _rolePermissions.AsReadOnly();

    public Role(int roleId, string roleName)
    {
        if(roleId <= 0)
        {
            throw new ArgumentException("Role id must be greater than zero");
        }

        RoleId = roleId;
        RoleName = roleName;
    }

    public void AddPermission(Permission permission)
    {
        if(permission is null)
        {
            throw new ArgumentException("Permission is required");
        }

        _rolePermissions.Add(permission);
    }
}
