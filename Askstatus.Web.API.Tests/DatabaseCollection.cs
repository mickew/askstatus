namespace Askstatus.Web.API.Tests;

/// <summary>
/// This class has no code, and is never created. Its purpose is simply
/// to be the place to apply [CollectionDefinition] and all the
/// ICollectionFixture interfaces.
/// </summary>
[CollectionDefinition(WebAPICollectionDefinition)]
public class DatabaseCollection : ICollectionFixture<IntegrationTestWebAppFactory>
{
    public const string WebAPICollectionDefinition = "WebAPI collection";
}
