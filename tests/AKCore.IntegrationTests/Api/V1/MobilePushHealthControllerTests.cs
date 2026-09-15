using AKCore.Models;
using AKCore.Services;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace AKCore.IntegrationTests.Api.V1;

public class MobilePushHealthControllerTests
{
    [Fact]
    public async Task Get_PushDisabled_ReturnsDisabled()
    {
        await using var factory =
            new CustomWebApplicationFactory();

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.GetAsync(
            "/api/v1/mobile-push/health");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var health = await response.Content
            .ReadFromJsonAsync<HealthResponse>();

        Assert.NotNull(health);
        Assert.Equal("Disabled", health.Status);
        Assert.Null(health.LastSuccessfulSendAt);
        Assert.Null(health.LastFailureAt);
    }

    [Fact]
    public async Task Get_Unhealthy_ReturnsServiceUnavailable()
    {
        await using var factory =
            new CustomWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var health = scope.ServiceProvider
                .GetRequiredService<MobilePushHealth>();

            health.MarkUnhealthy(
                DateTime.UtcNow,
                new InvalidOperationException(
                    "Simulated Firebase failure."));
        }

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.GetAsync(
            "/api/v1/mobile-push/health");

        Assert.Equal(
            HttpStatusCode.ServiceUnavailable,
            response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<HealthResponse>();

        Assert.NotNull(result);
        Assert.Equal("Unhealthy", result.Status);
        Assert.NotNull(result.LastFailureAt);
    }

    [Fact]
    public async Task Get_Healthy_ReturnsHealthy()
    {
        await using var factory =
            new CustomWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var health = scope.ServiceProvider
                .GetRequiredService<MobilePushHealth>();

            health.MarkHealthy(DateTime.UtcNow);
        }

        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.GetAsync(
            "/api/v1/mobile-push/health");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<HealthResponse>();

        Assert.NotNull(result);
        Assert.Equal("Healthy", result.Status);
        Assert.NotNull(result.LastSuccessfulSendAt);
    }

    [Fact]
    public void Validate_PushEnabledWithoutProjectId_FailsClearly()
    {
        var options = new MobilePushOptions
        {
            Enabled = true,
            ProjectId = ""
        };

        var error = Assert.Throws<InvalidOperationException>(
            options.Validate);

        Assert.Contains(
            "ProjectId is not configured",
            error.Message);
    }

    [Fact]
    public void Validate_PushDisabledWithoutProjectId_Succeeds()
    {
        var options = new MobilePushOptions
        {
            Enabled = false,
            ProjectId = ""
        };

        options.Validate();
    }

    [Fact]
    public void Validate_PushEnabledWithProjectId_Succeeds()
    {
        var options = new MobilePushOptions
        {
            Enabled = true,
            ProjectId = "test-project"
        };

        options.Validate();
    }

    [Theory]
    [InlineData(MessagingErrorCode.Unregistered)]
    [InlineData(MessagingErrorCode.SenderIdMismatch)]
    [InlineData(MessagingErrorCode.InvalidArgument)]
    public void FailureClassifier_DeviceErrors_AreNotInfrastructureFailures(
        MessagingErrorCode errorCode)
    {
        Assert.False(
            MobilePushFailureClassifier.IsInfrastructureFailure(
                errorCode));
    }

    [Theory]
    [InlineData(MessagingErrorCode.Internal)]
    [InlineData(MessagingErrorCode.Unavailable)]
    [InlineData(MessagingErrorCode.QuotaExceeded)]
    [InlineData(MessagingErrorCode.ThirdPartyAuthError)]
    public void FailureClassifier_InfrastructureErrors_AreInfrastructureFailures(
        MessagingErrorCode errorCode)
    {
        Assert.True(
            MobilePushFailureClassifier.IsInfrastructureFailure(
                errorCode));
    }

    [Fact]
    public void FailureClassifier_UnknownError_IsInfrastructureFailure()
    {
        Assert.True(
            MobilePushFailureClassifier.IsInfrastructureFailure(null));
    }

    private sealed class HealthResponse
    {
        public string Status { get; set; } = "";

        public DateTime? LastSuccessfulSendAt { get; set; }

        public DateTime? LastFailureAt { get; set; }
    }
}