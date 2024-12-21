using System;

namespace AKCore.Models;

public record StatisticsItemModel(DateTime Created, int Amount, string Path);


public enum StatisticsRange
{
    Day,
    Week,
    Month
}