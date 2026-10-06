using System;

namespace AKCore.Models;

public record StatisticsRequestModel(DateTime Created, int Amount, int Mobile, int Desktop, string Path);

public record StatisticsUsageModel(DateTime Created, int Amount, string Type);

public enum StatisticsRequestRange
{
    Day,
    Week,
    Month
}

public enum StatisticsGigsRange
{
    Month,
    Year,
    AllTime
}