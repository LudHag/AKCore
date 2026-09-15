using System;

namespace AKCore.Services;

public class InvalidMobilePushTokenException : Exception
{
    public InvalidMobilePushTokenException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}