namespace Askstatus.Common.Authorization;
public static class PolicyNameHelper
{
    public const string Prefix = "Permissions";

    public static bool IsValidPolicyName(string? policyName)
    {
        return policyName != null && policyName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase);
    }

    public static string GeneratePolicyNameFor(Permissions permissions)
    {
        return $"{Prefix}{(int)permissions}";
    }

    public static Permissions GetPermissionsFrom(string policyName)
    {
        var permissionsValue = int.Parse(policyName.Substring(Prefix.Length));

        return (Permissions)permissionsValue;
    }
}
