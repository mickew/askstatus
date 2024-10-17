using System;
using System.Collections.Generic;
using System.Text;
using Askstatus.Common.Users;
using Refit;

namespace Askstatus.Sdk.Users;
public interface IRoleAPI
{
    [Get("/api/role")]
    Task<IApiResponse<IEnumerable<RoleDto>>> GetRoles();

    [Post("/api/role")]
    Task<IApiResponse<RoleDto>> CreateRole(RoleRequest request);

    [Put("/api/role")]
    Task<IApiResponse> UpdateRole(RoleRequest request);

    [Delete("/api/role/{id}")]
    Task<IApiResponse> DeleteRole(string id);

    [Put("/api/role/permission")]
    Task<IApiResponse> UpdatePermissions(RoleRequest roleRequest);

    [Get("/api/role/permissions")]
    Task<IApiResponse<AccessControlVm>> GetPermissions();

}
