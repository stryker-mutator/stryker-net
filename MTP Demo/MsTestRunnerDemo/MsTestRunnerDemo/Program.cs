using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Testing.Platform;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.TestHostControllers;
using StreamJsonRpc;

Console.WriteLine("Starting demo....");
string testAssemblyPath = Path.GetFullPath("C:\\Dev\\Repos\\stryker-net\\MTP Demo\\MsTestRunnerDemo\\XUnitRunnerDemo\\bin\\Debug\\net9.0\\XUnitRunnerDemo.dll");
var exits = File.Exists(testAssemblyPath);
if (!exits)
{
    throw new FileNotFoundException($"Test assembly not found at path: {testAssemblyPath}");
}

static async Task<(Process proc, MtpJsonRpcConnection rpc)> StartRunnerAsync(
    string testDllPath, CancellationToken ct)
{
    var rpc = new MtpJsonRpcConnection();
    var psi = new ProcessStartInfo
    {
        FileName = "dotnet",
        Arguments = $"\"{testDllPath}\" --server --client-port {rpc.Port}",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        RedirectStandardInput = true,
    };
    var proc = Process.Start(psi)!;
    Console.WriteLine(proc.Id);

    JsonRpc jsonRpc = JsonRpc.Attach(proc.StandardInput.BaseStream, proc.StandardOutput.BaseStream);
    Console.WriteLine("RPC started");
    var result = await jsonRpc.InvokeWithParameterObjectAsync<string>("initialize", new
    {
        clientInfo = new { name = "Stryker", version = "1.0" },
        capabilities = new[] { "testing/discoverTests", "testing/runTests" }
    });
    //await rpc.AcceptAsync(ct);
    // initialize
    //await rpc.RequestAsync("initialize", new
    //{
    //    clientInfo = new { name = "Stryker", version = "1.0" },
    //    capabilities = new[] { "testing/discoverTests", "testing/runTests" }
    //}, ct);

    return (proc, rpc);
}

var cancellationTokenSource = new CancellationTokenSource();
var startResult = await StartRunnerAsync(testAssemblyPath, cancellationTokenSource.Token); //TODO: In de initialize voert hij al het ophalen van de tests uit.
Console.WriteLine("Test runner started.");
var discoverResult = await DiscoverAsync(startResult.rpc, cancellationTokenSource.Token);
Console.WriteLine("Tests discovered: " + discoverResult.Value.ToString());

static Task<JsonElement?> DiscoverAsync(MtpJsonRpcConnection rpc, CancellationToken ct)
    => rpc.RequestAsync("testing/discoverTests", new { }, ct);

static Task RunAsync(MtpJsonRpcConnection rpc,
    IEnumerable<string> testIds,
    int[] activeMutants,
    CancellationToken ct)
    => rpc.RequestAsync("testing/runTests", new
    {
        tests = testIds.ToArray(),
        // Pass active mutants to the runner as test run parameters.
        // Frameworks surface these via TestContext (MSTest), TestContext.TestParameters (NUnit), etc.
        parameters = new Dictionary<string, string>
        {
            ["Stryker.ActiveMutants"] = string.Join(",", activeMutants)
        }
    }, ct);
