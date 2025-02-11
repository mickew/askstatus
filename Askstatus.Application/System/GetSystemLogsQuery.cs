using System.Linq.Expressions;
using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.System;
using Askstatus.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.System;
public sealed record GetSystemLogsQuery(string SearchTerm, string SortColumn, int Page, int PageSize, bool Desc = false) : IRequest<Result<SystemLogPagedList>>;

public sealed class GetSystemLogsQueryHandler : IRequestHandler<GetSystemLogsQuery, Result<SystemLogPagedList>>
{
    private readonly ILogger<GetSystemLogsQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetSystemLogsQueryHandler(ILogger<GetSystemLogsQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SystemLogPagedList>> Handle(GetSystemLogsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<SystemLog, bool>> expression = null!;
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            expression = l => l.User!.Contains(request.SearchTerm) || l.EventType.ToString().Contains(request.SearchTerm);
        }

        Expression<Func<SystemLog, object>> keySelector = request.SortColumn?.ToLower() switch
        {
            "eventtime" => systemLog => systemLog.EventTime,
            "eventtype" => systemLog => systemLog.EventType,
            "user" => systemLog => systemLog.User!,
            "message" => systemLog => systemLog.Message!,
            _ => systemLog => systemLog.Id
        };

        var logList = await _unitOfWork.SystemLogRepository.GetListBy(expression, keySelector, request.Page, Math.Min(request.PageSize, 50), request.Desc);
        if (logList == null)
        {
            _logger.LogError("Failed to get system logs");
            return Result.Fail(new BadRequestError("Failed to get system logs"));
        }
        return Result.Ok(new SystemLogPagedList
        {
            Data = logList.Items.Select(l => new SystemLogDto(l.Id.ToString(), l.EventTime, l.EventType, l.User!, l.Message!)).ToList(),
            TotalCount = logList.TotalCount,
            Page = logList.Page,
            PageSize = logList.PageSize,
            HasNextPage = logList.HasNextPage,
            HasPreviousPage = logList.HasPreviousPage
        });
    }
}
