using Askstatus.Common.System;
using Refit;

namespace Askstatus.Sdk.System;
public interface ISystemAPI
{
    [Get("/api/system/systemlog?searchTerm={searchTerm}&sortColumn={sortColumn}&desc={desc}&page={page}&pageSize={pageSize}")]
    Task<IApiResponse<SystemLogPagedList>> GetSystemInfo(string searchTerm, string sortColumn, int page, int pageSize, bool desc);

    [Multipart]
    [Post("/api/system/uploadgoogletokenresponsefile")]
    Task<IApiResponse> UploadGoogleTokenResponseFile(StreamPart file);

    [Multipart]
    [Post("/api/system/uploadproductionappsettingsfile")]
    Task<IApiResponse> UploadProductionAppSettingsFile(StreamPart file);

    [Get("/api/system/systeminfo")]
    Task<IApiResponse<SystemInfoDto>> GetSystemInfo();

    [Post("/api/system/sendemail")]
    Task<IApiResponse> SendMail([Body] SystemSendMailRequest request);
}
