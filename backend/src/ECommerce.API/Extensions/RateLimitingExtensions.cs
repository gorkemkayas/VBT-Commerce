using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.API.Extensions;

public static class RateLimitingExtensions
{
    /// <summary>Named policy for credential-stuffing/brute-force-prone endpoints (login, register, refresh, forgot/reset password).</summary>
    public const string AuthPolicy = "auth";

    // Docker's default bridge networks are always carved out of this block, so any peer here is a
    // sibling container reachable only over the compose network — never a real internet client (those
    // only ever reach us through nginx, which UseForwardedHeaders already resolves to the true client
    // IP below). The frontend's SSR calls hit the API directly over this network (see
    // SERVER_API_BASE_URL/INTERNAL_API_URL in the frontend), so without this exemption every visitor's
    // server-rendered page load would appear to share one IP and blow through the per-IP budget.
    private static readonly IPNetwork DockerNetwork = IPNetwork.Parse("172.16.0.0/12");

    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Applies to every request regardless of endpoint — baseline flood protection per client IP.
            // Behind the reverse proxy, UseForwardedHeaders (registered earlier in the pipeline) already
            // resolves RemoteIpAddress to the real client IP rather than the proxy's.
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var remoteIp = httpContext.Connection.RemoteIpAddress;
                if (remoteIp != null && DockerNetwork.Contains(remoteIp))
                    return RateLimitPartition.GetNoLimiter(GetClientKey(httpContext));

                return RateLimitPartition.GetSlidingWindowLimiter(
                    GetClientKey(httpContext),
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 4,
                        QueueLimit = 0
                    });
            });

            // Stricter, separate budget layered on top of the global limiter for auth endpoints —
            // those are the primary brute-force/credential-stuffing target, so they need a tighter
            // per-IP ceiling than general browsing/API traffic.
            options.AddPolicy(AuthPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetClientKey(httpContext),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();

                context.HttpContext.Response.ContentType = "application/problem+json";
                await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Too Many Requests",
                    Detail = "Rate limit exceeded. Please try again later."
                }, cancellationToken);
            };
        });

        return services;
    }

    private static string GetClientKey(HttpContext httpContext)
        => httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
