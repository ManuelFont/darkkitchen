namespace DarkKitchen.Domain.Entities;

public sealed class Permission
{
    private string _permissionName = string.Empty;
    public int PermissionId { get; private set; }

    public string PermissionName
    {
        get => _permissionName;
        private set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Permission name is required");
            }

            _permissionName = value.Trim();
        }
    }

    public Permission(int permissionId, string permissionName)
    {
        if(permissionId <= 0)
        {
            throw new ArgumentException("Permission id must be greater than zero");
        }

        PermissionId = permissionId;
        PermissionName = permissionName;
    }
}
