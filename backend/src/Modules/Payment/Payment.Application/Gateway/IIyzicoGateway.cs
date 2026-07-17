namespace Payment.Application.Gateway;

/// <summary>
/// Abstraction over the iyzico sandbox SDK, implemented in Payment.Infrastructure — keeps the
/// Application layer free of the Iyzipay NuGet package, same "abstract the external dependency"
/// principle as Identity's IPasswordHasher.
/// </summary>
public interface IIyzicoGateway
{
    Task<IyzicoChargeResult> ChargeAsync(IyzicoChargeRequest request, CancellationToken cancellationToken);

    Task<IyzicoRefundResult> RefundAsync(IyzicoRefundRequest request, CancellationToken cancellationToken);
}
