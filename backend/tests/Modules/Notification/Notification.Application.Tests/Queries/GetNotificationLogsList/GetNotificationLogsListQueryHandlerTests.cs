using FluentAssertions;
using Notification.Application.Queries.GetNotificationLogsList;
using Notification.Domain.Entities;
using Xunit;

namespace Notification.Application.Tests.Queries.GetNotificationLogsList;

public class GetNotificationLogsListQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoFilter_ReturnsAllOrderedByCreatedAtDescending()
    {
        using var dbContext = TestNotificationDbContextFactory.Create();
        var older = NotificationLog.Create("OrderConfirmed", Guid.NewGuid(), "a@example.com", "s1", "b1", true, null);
        var newer = NotificationLog.Create("PasswordResetRequested", Guid.NewGuid(), "b@example.com", "s2", "b2", false, "err");
        dbContext.NotificationLogs.AddRange(older, newer);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetNotificationLogsListQueryHandler(dbContext);
        var query = new global::Notification.Application.Queries.GetNotificationLogsList.GetNotificationLogsListQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithIsSuccessFilter_ReturnsOnlyMatchingLogs()
    {
        using var dbContext = TestNotificationDbContextFactory.Create();
        var success = NotificationLog.Create("OrderConfirmed", Guid.NewGuid(), "a@example.com", "s1", "b1", true, null);
        var failure = NotificationLog.Create("OrderConfirmed", Guid.NewGuid(), "b@example.com", "s2", "b2", false, "err");
        dbContext.NotificationLogs.AddRange(success, failure);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetNotificationLogsListQueryHandler(dbContext);
        var query = new global::Notification.Application.Queries.GetNotificationLogsList.GetNotificationLogsListQuery(IsSuccess: false);

        var result = await handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle();
        result.Items.Single().IsSuccess.Should().BeFalse();
        result.Items.Single().ErrorMessage.Should().Be("err");
    }

    [Fact]
    public async Task Handle_WithPaging_ReturnsCorrectPageAndTotalPages()
    {
        using var dbContext = TestNotificationDbContextFactory.Create();
        for (var i = 0; i < 5; i++)
        {
            dbContext.NotificationLogs.Add(
                NotificationLog.Create("OrderConfirmed", Guid.NewGuid(), $"user{i}@example.com", "s", "b", true, null));
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetNotificationLogsListQueryHandler(dbContext);
        var query = new global::Notification.Application.Queries.GetNotificationLogsList.GetNotificationLogsListQuery(PageNumber: 2, PageSize: 2);

        var result = await handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Items.Should().HaveCount(2);
        result.TotalPages.Should().Be(3);
    }
}
