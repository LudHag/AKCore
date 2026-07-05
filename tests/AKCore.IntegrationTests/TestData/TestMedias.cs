using AKCore.DataModel;

namespace AKCore.IntegrationTests.TestData;

public static class TestMedias
{
    public static Media Create(string name, DateTime created) =>
        new()
        {
            Name = name,
            Type = "Image",
            Tag = "test",
            Created = created
        };

    public static Task SeedManyAsync(CustomWebApplicationFactory factory, int count)
    {
        var baseTime = DateTime.UtcNow;
        return factory.SeedAsync(db =>
        {
            for (var i = 0; i < count; i++)
            {
                db.Medias.Add(Create($"media-{i:D3}.jpg", baseTime.AddMinutes(-i)));
            }
            return Task.CompletedTask;
        });
    }
}
