using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Lambda.TestUtilities;
using AppLambda;

namespace IntegrationTests
{
    public class LambdaTestHarnessFixture : IDisposable
    {
        private readonly AmazonLambdaClient _client;
        private readonly string _functionName;

        public LambdaTestHarnessFixture()
        {
            // pull region endpoint and function name from config, if you like
            _client = new AmazonLambdaClient(RegionEndpoint.USWest2);
            _functionName = "test-lambda-fn";
        }

        public async Task<string> InvokeAsync(string request)
        {
            if (System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local")
            {
                return await new Function().FunctionHandler(request, new TestLambdaContext());
            }

            var invocationResponse = await _client.InvokeAsync(new InvokeRequest
            {
                FunctionName = _functionName,
                Payload = request
            });

            using var streamReader = new StreamReader(invocationResponse.Payload);
            return await streamReader.ReadToEndAsync();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

