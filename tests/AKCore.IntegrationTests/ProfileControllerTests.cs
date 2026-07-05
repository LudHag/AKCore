using System.Net;
using System.Text;
using System.Text.Json;
using AKCore.DataModel;
using AKCore.IntegrationTests.TestData;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKCore.IntegrationTests;

public class ProfileControllerTests
{
    [Fact]
    public async Task ProfileData_RequiresAuthentication()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = TestClients.CreateAnonymousClient(factory);

        var response = await client.GetAsync("/Profile/ProfileData");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProfileData_ReturnsMemberProfile()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.GetAsync("/Profile/ProfileData");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.Equal(TestUsers.MemberUserName, json.RootElement.GetProperty("userName").GetString());
        Assert.Equal("Flöjt", json.RootElement.GetProperty("instrument").GetString());
    }

    [Fact]
    public async Task EditProfile_UpdatesUserFields()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var model = new ProfileModel
        {
            UserName = TestUsers.MemberUserName,
            FirstName = "Updated",
            LastName = "Profile",
            Phone = "0701234567",
            Instrument = "Klarinett",
            OtherInstruments = ["Flöjt"]
        };

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.PostAsync(
            "/Profile/EditProfile",
            new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
        var user = await userManager.FindByNameAsync(TestUsers.MemberUserName);
        Assert.Equal("Updated", user!.FirstName);
        Assert.Equal("Klarinett", user.Instrument);
        Assert.Equal("Flöjt", user.OtherInstruments);
    }

    [Fact]
    public async Task EditProfile_PreservesUnchangedFields()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        using (var setupScope = factory.Services.CreateScope())
        {
            var setupUserManager = setupScope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
            var setupUser = await setupUserManager.FindByNameAsync(TestUsers.MemberUserName);
            setupUser!.Medal = "Gold";
            setupUser.GivenMedal = "Silver";
            setupUser.SlavPoster = "[\"Poster1\"]";
            await setupUserManager.UpdateAsync(setupUser);
        }

        var model = new ProfileModel
        {
            UserName = TestUsers.MemberUserName,
            FirstName = "Test",
            LastName = "Member",
            Instrument = "Flöjt"
        };

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.PostAsync(
            "/Profile/EditProfile",
            new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
        var user = await userManager.FindByNameAsync(TestUsers.MemberUserName);
        var roles = await userManager.GetRolesAsync(user!);

        Assert.Equal("Gold", user!.Medal);
        Assert.Equal("Silver", user.GivenMedal);
        Assert.Equal("[\"Poster1\"]", user.SlavPoster);
        Assert.Contains(AkRoles.Medlem, roles);
    }

    [Fact]
    public async Task EditProfile_RejectsDuplicateInstrument()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();

        var model = new ProfileModel
        {
            UserName = TestUsers.MemberUserName,
            FirstName = "Test",
            LastName = "Member",
            Instrument = "Flöjt",
            OtherInstruments = ["Flöjt"]
        };

        var client = TestClients.CreateMemberClient(factory);
        var response = await client.PostAsync(
            "/Profile/EditProfile",
            new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        Assert.False(json.RootElement.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task EditProfile_UsernameChange_AllowsProfileDataWithNewName()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedMemberAsync();
        const string newUserName = "renamedmember";

        var model = new ProfileModel
        {
            UserName = newUserName,
            FirstName = "Test",
            LastName = "Member",
            Instrument = "Flöjt"
        };

        var client = TestClients.CreateMemberClient(factory);
        var editResponse = await client.PostAsync(
            "/Profile/EditProfile",
            new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, editResponse.StatusCode);

        var editBody = await editResponse.Content.ReadAsStringAsync();
        using var editJson = JsonDocument.Parse(editBody);
        Assert.True(editJson.RootElement.GetProperty("success").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AkUser>>();
        Assert.Null(await userManager.FindByNameAsync(TestUsers.MemberUserName));
        Assert.NotNull(await userManager.FindByNameAsync(newUserName));

        var renamedClient = TestClients.CreateMemberClient(factory, newUserName);
        var profileResponse = await renamedClient.GetAsync("/Profile/ProfileData");
        Assert.Equal(HttpStatusCode.OK, profileResponse.StatusCode);

        var profileBody = await profileResponse.Content.ReadAsStringAsync();
        using var profileJson = JsonDocument.Parse(profileBody);
        Assert.Equal(newUserName, profileJson.RootElement.GetProperty("userName").GetString());
    }
}
