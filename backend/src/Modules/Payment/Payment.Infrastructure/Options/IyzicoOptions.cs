namespace Payment.Infrastructure.Options;

public class IyzicoOptions
{
    public const string SectionName = "Iyzico";

    public string ApiKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string BaseUrl { get; set; } = "https://sandbox-api.iyzipay.com";
}
