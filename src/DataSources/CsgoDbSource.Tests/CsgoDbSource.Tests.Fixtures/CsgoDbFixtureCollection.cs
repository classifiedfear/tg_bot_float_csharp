using System;

namespace CsgoDbSource.Tests.Fixtures;

[CollectionDefinition("csgodb")]
public class HtmlPagesCollection : ICollectionFixture<HtmlPagesFixture>, ICollectionFixture<ParserOptionsFixture> { }
