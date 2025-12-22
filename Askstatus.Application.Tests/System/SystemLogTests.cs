using System.Linq.Expressions;
using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.System;
using Askstatus.Common.Models;
using Askstatus.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;
public class SystemLogTests
{
    [Fact]
    public async Task GetSystemLogsQueryHandler_ReturnsSystemLogPagedList()
    {
        // Arrange
        var logger = new Mock<ILogger<GetSystemLogsQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var pagedListMock = new Mock<IPagedList<SystemLog>>();
        pagedListMock.Setup(x => x.TotalCount).Returns(1);
        pagedListMock.Setup(x => x.Page).Returns(1);
        pagedListMock.Setup(x => x.PageSize).Returns(10);
        pagedListMock.Setup(x => x.HasNextPage).Returns(false);
        pagedListMock.Setup(x => x.HasPreviousPage).Returns(false);
        pagedListMock.Setup(x => x.Items).Returns(new List<SystemLog>
        {
            new SystemLog
            {
                Id = 1,
                EventTime = DateTime.Now,
                EventType = SystemLogEventType.SetDeviceState,
                User = "User",
                Message = "Message"
            }
        });
        unitOfWork.Setup(x => x.SystemLogRepository.GetListBy(It.IsAny<Expression<Func<SystemLog, bool>>>(), It.IsAny<Expression<Func<SystemLog, object>>>(), 1, 10, false)).ReturnsAsync(pagedListMock.Object);
        var handler = new GetSystemLogsQueryHandler(logger.Object, unitOfWork.Object);
        var query = new GetSystemLogsQuery("", "", 1, 10);
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().HaveCount(1);
        result.Value.TotalCount.Should().Be(1);
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
        result.Value.HasNextPage.Should().BeFalse();
        result.Value.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetSystemLogsQueryHandler_ReturnsBadRequest()
    {
        // Arrange
        var logger = new Mock<ILogger<GetSystemLogsQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SystemLogRepository.GetListBy(It.IsAny<Expression<Func<SystemLog, bool>>>(), It.IsAny<Expression<Func<SystemLog, object>>>(), 1, 10, false)).ReturnsAsync((IPagedList<SystemLog>)null!);
        var handler = new GetSystemLogsQueryHandler(logger.Object, unitOfWork.Object);
        var query = new GetSystemLogsQuery("", "", 1, 10);
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeOfType<BadRequestError>();
        result.Errors.First().Message.Should().Be("Failed to get system logs");
    }
}
