using System;

namespace AKCore.Models;

public record StatisticsRequestModel(DateTime Created, int Amount, int Mobile, int Desktop, string Path);


public enum StatisticsRequestRange
{
    Day,
    Week,
    Month
}

public enum StatisticsGigsRange
{
    Month,
    Year
}