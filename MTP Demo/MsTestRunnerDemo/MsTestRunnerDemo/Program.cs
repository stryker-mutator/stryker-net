using System.Reflection;

string testAssemblyPath = Path.GetFullPath("..\\..\\..\\..\\..\\InjectedTestProject\\UnitTests\\bin\\Debug\\net9.0\\UnitTests.dll");
var exits = File.Exists(testAssemblyPath);

// this works, as long as the target framework is the same
var testAssembly = Assembly.LoadFrom(testAssemblyPath);

var entryPoint = testAssembly.GetExportedTypes()
    .Where(t => t.IsClass && !t.IsAbstract)
    .FirstOrDefault(t => t.Name == "Injected");

var instance = Activator.CreateInstance(entryPoint);
var method = entryPoint.GetMethod("Main");

// call the test method multiple times, works fast!
Environment.SetEnvironmentVariable("StrykerActiveMutation", "1");
var result = method.Invoke(instance, new object[] { new string[] { } });
Console.WriteLine(result);

Environment.SetEnvironmentVariable("StrykerActiveMutation", "2");
var result2 = method.Invoke(instance, new object[] { new string[] { } });
Console.WriteLine(result2);

Environment.SetEnvironmentVariable("StrykerActiveMutation", "3");

var result3 = method.Invoke(instance, new object[] { new string[] { } });
Console.WriteLine(result3);

Environment.SetEnvironmentVariable("StrykerActiveMutation", "4");
var result4 = method.Invoke(instance, new object[] { new string[] { } });
Console.WriteLine(result4);

// We don't want this, because it's slow, and doesn't support reruns... But it works for any target framework (as long as it's installed on the host)
//var process = new System.Diagnostics.Process();
//process.StartInfo.FileName = "dotnet";
//process.StartInfo.Arguments = $"exec \"{testAssemblyPath}\"";
//process.Start();
//process.WaitForExit();
//Console.WriteLine($"Process exited with code {process.ExitCode}");
