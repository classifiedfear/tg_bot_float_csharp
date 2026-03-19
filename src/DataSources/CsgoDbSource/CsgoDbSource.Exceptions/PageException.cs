using System;

namespace CsgoDbSource.Exceptions;

public class PageException(string message) : BaseCsgoDbSourceException(message) { }
