using System;
using System.Collections;
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

		public static void Run()
		{
			var enumerator = Coroutine();
			EditorCoroutine.Start(enumerator);
		}

		private static IEnumerator Coroutine()
		{
			Console.WriteLine("[Stryker] Run coroutine");
			var textFileToListen = Environment.GetEnvironmentVariable("Stryker.Unity.PathToListen");

			string _runnedPathToOutput = string.Empty;
			var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
			testRunnerApi.RegisterCallbacks(new TestCallbacks(() => _runnedPathToOutput));

			while (true)
			{
				// if (IsActiveVariable("Stryker.Unity.RequestToExit"))
				// {
				// 	Console.WriteLine("[Stryker] Got RequestToExit");
				// 	yield break;
				// }

				if (TestsInProgress)
				{
					yield return new WaitForSeconds(1f);
					continue;
				}


				var pathToSaveOutput = File.ReadAllText(textFileToListen);
				if (pathToSaveOutput.Contains("exit"))
				{
					Console.WriteLine("[Stryker] Got exit. Close unity");

					EditorApplication.Exit(0);
					yield break;
				}
				else if (pathToSaveOutput != _runnedPathToOutput)
				{
					_runnedPathToOutput = pathToSaveOutput;
					Console.WriteLine("[Stryker] Got RequestToRun");
					Console.WriteLine("[Stryker] Start testRunnerApi.Execute with path " + pathToSaveOutput);

					var executionSettings = new ExecutionSettings(new Filter() { testMode = TestMode.EditMode });
					testRunnerApi.Execute(executionSettings);
					TestsInProgress = true;
					Console.WriteLine("[Stryker] Finish testRunnerApi.Execute");
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
