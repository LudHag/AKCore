using System.Net;
using System.Text.RegularExpressions;
using AKCore.IntegrationTests.TestData;

namespace AKCore.IntegrationTests;

public class MediaControllerTests
{
    [Fact]
    public async Task MediaPickerList_Empty_ReturnsNoItemsAndSinglePage()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedEditorAsync();

        var client = TestClients.CreateEditorClient(factory);
        var response = await client.GetAsync("/Media/MediaPickerList?Page=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(0, CountMediaItems(body));
        Assert.Contains("data-page=\"1\"", body);
    }

    [Fact]
    public async Task MediaPickerList_FirstPage_ReturnsUpToTwelveItems()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedEditorAsync();
        await TestMedias.SeedManyAsync(factory, 15);

        var client = TestClients.CreateEditorClient(factory);
        var response = await client.GetAsync("/Media/MediaPickerList?Page=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(12, CountMediaItems(body));
        Assert.Contains("class=\"active\"", body);
        Assert.Contains("data-page=\"2\"", body);
    }

    [Fact]
    public async Task MediaPickerList_LastPage_ReturnsRemainingItems()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedEditorAsync();
        await TestMedias.SeedManyAsync(factory, 15);

        var client = TestClients.CreateEditorClient(factory);
        var response = await client.GetAsync("/Media/MediaPickerList?Page=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(3, CountMediaItems(body));
    }

    [Fact]
    public async Task MediaPickerList_PageLessThanOne_NormalizesToOne()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedEditorAsync();
        await TestMedias.SeedManyAsync(factory, 3);

        var client = TestClients.CreateEditorClient(factory);
        var response = await client.GetAsync("/Media/MediaPickerList?Page=0");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(3, CountMediaItems(body));
        Assert.DoesNotContain("data-page=\"0\"", body);
    }

    [Fact]
    public async Task MediaPickerList_PageBeyondTotal_ClampsToLastPage()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.SeedEditorAsync();
        await TestMedias.SeedManyAsync(factory, 15);

        var client = TestClients.CreateEditorClient(factory);
        var response = await client.GetAsync("/Media/MediaPickerList?Page=99");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(3, CountMediaItems(body));
        Assert.Contains("<li class=\"active\"><a href=\"#\" data-page=\"2\">2</a></li>", body);
    }

    private static int CountMediaItems(string html) =>
        Regex.Matches(html, @"<div class=""col-sm-2 image-box""").Count;
}
