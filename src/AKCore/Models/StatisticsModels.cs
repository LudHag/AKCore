using System;

namespace AKCore.Models;

public record StatisticsItemModel(DateTime Created, int Amount);

public enum StatisticsRange
{
    Day,
    Week,
    Month
}