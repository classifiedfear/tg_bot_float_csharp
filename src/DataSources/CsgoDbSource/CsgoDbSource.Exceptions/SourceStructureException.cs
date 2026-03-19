using System;

namespace CsgoDbSource.Exceptions;

public class SourceStructureException(string message) : BaseCsgoDbSourceException(message) { }
