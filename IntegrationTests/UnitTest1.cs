using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace IntegrationTests;

[Collection("Lambda test collection")]
public class Tests
{

    private readonly LambdaTestHarnessFixture _testHarnessFixture;

    public Tests(LambdaTestHarnessFixture testHarnessFixture)
    {
        _testHarnessFixture = testHarnessFixture;
    }

    [Fact]
    public async Task TestLambda_ReturnsExpectedResponse()
    {
        var result = await _testHarnessFixture.InvokeAsync(JsonSerializer.Serialize("data"));

        result.Should().Contain("DATA");
    }
}
