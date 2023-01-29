using System;

namespace AKCore.Models;

public class AkValidationError : Exception
{
    public AkValidationError(string message) : base(message) { }
}