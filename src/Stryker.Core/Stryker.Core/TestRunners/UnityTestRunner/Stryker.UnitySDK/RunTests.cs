using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.TestTools;

namespace Stryker.UnitySDK
{
    public static class RunTests
	{
		public static bool TestsInProgress = false;

        [InitializeOnLoadMethod]
        public static void Run()
		{
			EditorCoroutine.Start(Coroutine());
		}

		private static IEnumerator Coroutine()
		{
			Console.WriteLine("[Stryker] Run coroutine");
			var textFileToListen = Environment.GetEnvironmentVariable("Stryker.Unity.PathToListen");

            if (string.IsNullOrEmpty(textFileToListen) || !File.Exists(textFileToListen))
            {
                yield break;
            }

			string _runnedPathToOutput = string.Empty;
			var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
			testRunnerApi.RegisterCallbacks(new TestCallbacks(() => _runnedPathToOutput));

			while (true)
			{

                var command = File.ReadAllText(textFileToListen);
				if (command == "exit")
				{
					Console.WriteLine("[Stryker] Got exit. Close unity");

					EditorApplication.Exit(0);
					yield break;
				}
                if (command == "reloadDomain")
                {
                    Console.WriteLine($"[Stryker][{DateTime.Now.ToLongTimeString()}] Got reloadDomain");

                    File.WriteAllText(textFileToListen, string.Empty);

                    EditorUtility.RequestScriptReload();
                }
				else if (!string.IsNullOrWhiteSpace(command))
				{
					_runnedPathToOutput = command;
					Console.WriteLine($"[Stryker][{DateTime.Now.ToLongTimeString()}] Got RequestToRunTests");
					Console.WriteLine("[Stryker] Start testRunnerApi.Execute with path " + command);

					var executionSettings = new ExecutionSettings(new Filter() { testMode = TestMode.EditMode });
					testRunnerApi.Execute(executionSettings);
					TestsInProgress = true;

                    while (TestsInProgress)
                    {
                        yield return new WaitForSeconds(1f);
                    }
                    File.WriteAllText(textFileToListen, string.Empty);
                }
				else
				{
					// Console.WriteLine("[Stryker] did not Got RequestToRun. wait 1s");
					yield return new WaitForSeconds(1f);
				}
			}
		}
	}

	public class EditorCoroutine
	{
		private IEnumerator routine;

		private EditorCoroutine(IEnumerator routine)
		{
			this.routine = routine;
		}

		public static EditorCoroutine Start(IEnumerator routine)
		{
			var coroutine = new EditorCoroutine(routine);
			coroutine.Start();
			return coroutine;
		}

		private void Start()
		{
			UnityEditor.EditorApplication.update += Update;
		}

		public void Stop()
		{
			UnityEditor.EditorApplication.update -= Update;
		}

		private void Update()
		{
			if (!routine.MoveNext())
			{
				Stop();
			}
		}
	}

	public class TestCallbacks : ICallbacks
	{
		private readonly Func<string> _getOutputFileName;

		public TestCallbacks(Func<string> getOutputFileName)
		{
			_getOutputFileName = getOutputFileName;
		}

		public void RunStarted(ITestAdaptor testsToRun)
		{
			Console.WriteLine("[Stryker] Run started");
		}

		public void RunFinished(ITestResultAdaptor result)
		{
			var sts = new XmlWriterSettings()
			{
				Indent = true,
			};
			Console.WriteLine("[Stryker] Run finished");

			var outputFileName = _getOutputFileName.Invoke();
			using var writer = XmlWriter.Create(outputFileName, sts);
			writer.WriteStartDocument();

			result.ToXml().WriteTo(writer);
			writer.WriteEndDocument();
			Console.WriteLine("[Stryker] Run finished and was write at " + outputFileName);

			RunTests.TestsInProgress = false;
		}

		public void TestStarted(ITestAdaptor test)
		{
		}

		public void TestFinished(ITestResultAdaptor result)
		{
		}
	}
}
