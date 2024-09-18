namespace Askstatus.Application.Models.Identity;
public sealed record ApplicationClaimDto(string Issuer, string OriginalIssuer, string Type, string Value, string ValueType);
