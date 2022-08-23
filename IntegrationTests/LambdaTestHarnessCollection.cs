using IntegrationTests;
using Xunit;

[CollectionDefinition("Lambda test collection")]
public class DatabaseCollection : ICollectionFixture<LambdaTestHarnessFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}