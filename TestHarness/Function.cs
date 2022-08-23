using Amazon.Lambda.Core;
using Xunit.Runners;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TestHarness;

public class Function
{

    private object consoleLock = new object();

    private ManualResetEvent finished = new ManualResetEvent(false);

    private int result = 0;

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public int FunctionHandler(string input, ILambdaContext context)
    {
        var dir = Directory.GetCurrentDirectory();
        using (var runner = AssemblyRunner.WithoutAppDomain("IntegrationTests.dll"))
        {
            runner.OnDiscoveryComplete = OnDiscoveryComplete;
            runner.OnExecutionComplete = OnExecutionComplete;
            runner.OnTestFailed = OnTestFailed;
            runner.OnTestSkipped = OnTestSkipped;

            Console.WriteLine("Discovering...");
            runner.Start();

            finished.WaitOne();
            finished.Dispose();

            while (runner.Status != AssemblyRunnerStatus.Idle)
            {
                // Wait for assembly runner to finish.
                // If we try to dispose while runner is executing,
                // it will throw an error.
                Thread.Sleep(1000);
            }

            return result;
        }
    }

    private void OnDiscoveryComplete(DiscoveryCompleteInfo info)
    {
        lock (consoleLock)
        {
            Console.WriteLine($"Running {info.TestCasesToRun} of {info.TestCasesDiscovered} tests...");
        }
    }

    private void OnExecutionComplete(ExecutionCompleteInfo info)
    {
        lock (consoleLock)
        {
            Console.WriteLine($"Finished: {info.TotalTests} tests in {Math.Round(info.ExecutionTime, 3)}s ({info.TestsFailed} failed, {info.TestsSkipped} skipped)");
        }

        finished.Set();
    }

    private void OnTestFailed(TestFailedInfo info)
    {
        lock (consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("[FAIL] {0}: {1}", info.TestDisplayName, info.ExceptionMessage);
            if (info.ExceptionStackTrace != null)
                Console.WriteLine(info.ExceptionStackTrace);

            Console.ResetColor();
        }

        result = 1;
    }

    private void OnTestSkipped(TestSkippedInfo info)
    {
        lock (consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[SKIP] {0}: {1}", info.TestDisplayName, info.SkipReason);
            Console.ResetColor();
        }
    }
}
