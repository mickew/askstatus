namespace Askstatus.Common.Identity;

public sealed record ApplicationClaimVM(string Issuer, string OriginalIssuer, string Type, string Value, string ValueType);
