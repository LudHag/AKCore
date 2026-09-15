using System;

namespace AKCore.Models;

public class MobilePushOptions
{
    public const string SectionName = "MobilePush";

    public bool Enabled { get; set; }

    public string ProjectId { get; set; } = "";

    public void Validate()
    {
        if (Enabled &&
            string.IsNullOrWhiteSpace(ProjectId))
        {
            throw new InvalidOperationException(
                "MobilePush is enabled but ProjectId is not configured.");
        }
    }
}
