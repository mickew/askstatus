using Askstatus.Common.System;
using Refit;

namespace Askstatus.Sdk.System;
public interface ISystemAPI
{
    [Get("/api/system/systemlog?searchTerm={searchTerm}&sortColumn={sortColumn}&desc={desc}&page={page}&pageSize={pageSize}")]
    Task<IApiResponse<SystemLogPagedList>> GetSystemInfo(string searchTerm, string sortColumn, int page, int pageSize, bool desc);
}
