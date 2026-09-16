using System.Reflection;

namespace Askstatus.Common.System;

public static class SystemHelper
{
    public static string GetVersion(Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        bool isPrerelease = attribute?.InformationalVersion?.Contains('-') ?? false;

        string version = "?.?.?";
        if (isPrerelease)
        {
            version = attribute!.InformationalVersion!.Split('+')[0];
        }
        else
        {
            version = $"{attribute!.InformationalVersion!.Split('.')[0]}.{attribute!.InformationalVersion!.Split('.')[1]}.{attribute!.InformationalVersion!.Split('.')[2]}";
        }
        return version ?? "?.?.?";
    }
}
