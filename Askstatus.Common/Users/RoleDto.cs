using Askstatus.Common.Authorization;

namespace Askstatus.Common.Users;
public sealed class RoleDto
{
    public RoleDto()
    {
        Id = string.Empty;
        Name = string.Empty;
        Permissions = Permissions.None;
    }

    public RoleDto(string id, string name, Permissions permissions)
    {
        Id = id;
        Name = name;
        Permissions = permissions;
    }

    public string Id { get; init; }

    public string Name { get; init; }

    public Permissions Permissions { get; private set; }

    public bool Has(Permissions permission)
    {
        return Permissions.HasFlag(permission); ;
    }

    public void Set(Permissions permission, bool granted)
    {
        if (granted)
        {
            Grant(permission);
        }
        else
        {
            Revoke(permission);
        }
    }

    public void Grant(Permissions permission)
    {
        Permissions |= permission;
    }

    public void Revoke(Permissions permission)
    {
        Permissions ^= permission;
    }

    //public class RoleDtoFluentValidator : AbstractValidator<RoleDto>
    //{
    //    public RoleDtoFluentValidator()
    //    {
    //        RuleFor(x => x.Name)
    //            .NotEmpty().Length(50);
    //    }
    //    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    //    {
    //        var result = await ValidateAsync(ValidationContext<RoleDto>.CreateWithOptions((RoleDto)model, x => x.IncludeProperties(propertyName)));
    //        if (result.IsValid)
    //            return Array.Empty<string>();
    //        return result.Errors.Select(e => e.ErrorMessage);
    //    };
    //}
}
