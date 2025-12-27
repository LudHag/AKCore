using System;

namespace AKCore.Models;

public record StatisticsItemModel(DateTime Created, int Amount, int Mobile, int Desktop, string Path);


public enum StatisticsRange
{
    Day,
    Week,
    Month
}