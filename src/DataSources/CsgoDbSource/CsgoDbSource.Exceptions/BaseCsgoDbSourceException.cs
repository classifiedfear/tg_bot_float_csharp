using System;

namespace CsgoDbSource.Exceptions;

public class BaseCsgoDbSourceException(string? message) : Exception(message)
{
    public const string NotFound = "Not found";
    public const string SourceStructureProblem = "Can't parse info from source response!";
}

