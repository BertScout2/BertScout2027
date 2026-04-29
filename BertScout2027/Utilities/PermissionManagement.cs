namespace BertScout2027.Utilities;

public static class PermissionManagement
{
    /*
    Android requires runtime permissions for accessing storage.
    Long-click on the app icon, go to App Info / Permissions / Storage to enable storage permissions.
    */

    private static PermissionStatus statusRead = PermissionStatus.Unknown;

    public static async Task<bool> CheckAndRequestStoragePermissionsAsync()
    {
        if (statusRead != PermissionStatus.Unknown)
        {
            return true; // Permissions already granted
        }
        statusRead = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
        if (statusRead == PermissionStatus.Granted)
        {
            return true; // Permission granted
        }
        return false; // Permission denied
    }
}
