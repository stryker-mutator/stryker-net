using System;
using System.Text.RegularExpressions;
using Common.ExtensionPoints.Implementation;
using Common.Scripts.ExtensionPoints.PublicAPI;
using Common.Scripts.ExtensionPoints.PublicAPI.Sync;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace Common.ExtensionPoints.Tests
{
	public class SyncWithPayloadExtensionPointTests : ZenjectUnitTestFixture
	{
		public override void Setup()
		{
			base.Setup();
			ExtensionPointsInstaller.Install(Container);
		}

		[Test]
		public void RunPoint_Sync_NormalRun_MultipleInstance()
		{
			Container.BindExtensionPoint<IPayloadSyncExtensionPoint, PayloadSyncExtensionPointImpl>();
			Container.BindExtensionPoint<IPayloadSyncExtensionPoint, PayloadSyncExtensionPointImplSecond>();
			var runExtensionPoint = Container.Resolve<IRunExtensionPoint<IPayloadSyncExtensionPoint, string>>();
			runExtensionPoint.Execute("payloadStr");
			LogAssert.Expect(LogType.Log, "PayloadSyncExtensionPointImpl was executed");
			LogAssert.Expect(LogType.Log, "PayloadSyncExtensionPointImpl2 was executed");
		}

		[Test]
		public void RunPoint_Sync_NormalRun_OneInstance()
		{
			Container.BindExtensionPoint<IPayloadSyncExtensionPoint, PayloadSyncExtensionPointImpl>();
			var runExtensionPoint = Container.Resolve<IRunExtensionPoint<IPayloadSyncExtensionPoint, string>>();
			runExtensionPoint.Execute("payloadStr");
			LogAssert.Expect(LogType.Log, "PayloadSyncExtensionPointImpl was executed");
		}

		[Test]
		public void RunPoint_Sync_NormalRun_NoInstances()
		{
			var runExtensionPoint = Container.Resolve<IRunExtensionPoint<IPayloadSyncExtensionPoint, string>>();
			runExtensionPoint.Execute("payloadStr");
		}

		[Test]
		public void RunPoint_Sync_IsActive_False()
		{
			Container.BindExtensionPoint<IPayloadSyncExtensionPoint, PayloadSyncExtensionPointImpl>();
			Container.BindExtensionPoint<IPayloadSyncExtensionPoint, PayloadSyncExtensionPointImplWithIsActiveFalse>();
			var runExtensionPoint = Container.Resolve<IRunExtensionPoint<IPayloadSyncExtensionPoint, string>>();
			runExtensionPoint.Execute("payloadStr");
			LogAssert.Expect(LogType.Log, "PayloadSyncExtensionPointImpl was executed");
		}

		[Test]
		public void RunPoint_Sync_ExceptionInExecution_AllOtherShouldExecute()
		{
			Container.BindExtensionPoint<IPayloadSyncExtensionPoint, PayloadSyncExtensionPointImpl>();
			Container.BindExtensionPoint<IPayloadSyncExtensionPoint, PayloadSyncExtensionPointImplWithException>();
			var runExtensionPoint = Container.Resolve<IRunExtensionPoint<IPayloadSyncExtensionPoint, string>>();
			runExtensionPoint.Execute("payloadStr");
			LogAssert.Expect(LogType.Log, "PayloadSyncExtensionPointImpl was executed");
			LogAssert.Expect(LogType.Error, new Regex("Exception during execute extension point"));
			
		}

		[Test]
		public void RunPoint_Sync_ExceptionInIsActive_AllOtherShouldExecute()
		{
			Container.BindExtensionPoint<IPayloadSyncExtensionPoint, PayloadSyncExtensionPointImpl>();
			Container.BindExtensionPoint<IPayloadSyncExtensionPoint, PayloadSyncExtensionPointImplWithIsActiveException>();
			var runExtensionPoint = Container.Resolve<IRunExtensionPoint<IPayloadSyncExtensionPoint, string>>();
			runExtensionPoint.Execute("payloadStr");
			LogAssert.Expect(LogType.Log, "PayloadSyncExtensionPointImpl was executed");
			LogAssert.Expect(LogType.Error, new Regex("Exception during IsActive check"));
			
		}

		#region Classes for test
		internal interface IPayloadSyncExtensionPoint : IExtensionPoint<string>
		{
		}

		internal class PayloadSyncExtensionPointImplSecond : IPayloadSyncExtensionPoint
		{
			public bool IsActive(string context)
			{
				return true;
			}

			public void Execute(string context)
			{
				Debug.Log("PayloadSyncExtensionPointImpl2 was executed");
			}
		}

		internal class PayloadSyncExtensionPointImpl : IPayloadSyncExtensionPoint
		{
			public bool IsActive(string context)
			{
				return true;
			}

			public void Execute(string context)
			{
				Debug.Log("PayloadSyncExtensionPointImpl was executed");
			}
		}

		internal class PayloadSyncExtensionPointImplWithException : IPayloadSyncExtensionPoint
		{
			public bool IsActive(string context)
			{
				return true;
			}

			public void Execute(string context)
			{
				throw new Exception("Test exception");
			}
		}

		internal class PayloadSyncExtensionPointImplWithIsActiveException : IPayloadSyncExtensionPoint
		{
			public bool IsActive(string context)
			{
				throw new Exception("Test exception");
			}

			public void Execute(string context)
			{
				Debug.LogError("THis log should not be printed");
			}
		}

		internal class PayloadSyncExtensionPointImplWithIsActiveFalse : IPayloadSyncExtensionPoint
		{
			public bool IsActive(string context)
			{
				return false;
			}

			public void Execute(string context)
			{
				Debug.LogError("THis log should not be printed");
			}
		}
	#endregion
	}
}