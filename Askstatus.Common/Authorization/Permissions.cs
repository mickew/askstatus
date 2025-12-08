namespace Askstatus.Common.Authorization;

[Flags]
public enum Permissions
{
    None = 0,
    ViewRoles = 1,
    ManageRoles = 2,
    ViewUsers = 4,
    ManageUsers = 8,
    ConfigureAccessControl = 16,
    ViewAccessControl = 32,
    ConfigurePowerDevices = 64,
    ViewPowerDevices = 128,
    DiscoverDevices = 256,
    System = 512,
    DiscoverSensors = 256,
    ConfigureSensors = 512,
    ViewSensors = 1024,
    All = ~None
}
