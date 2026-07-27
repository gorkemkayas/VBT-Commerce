using BuildingBlocks.Application.Security;
using Moq;

namespace Cart.Application.Tests;

internal static class TestCurrentUserServiceFactory
{
    public static ICurrentUserService Create(Guid userId)
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(x => x.UserId).Returns(userId);
        return mock.Object;
    }
}
