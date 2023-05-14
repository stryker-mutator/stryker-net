using Common.Scripts.ExtensionPoints.PublicAPI.Async;
using Common.Scripts.ExtensionPoints.PublicAPI.Sync;
using Zenject;

namespace Common.ExtensionPoints.Implementation
{
	public class ExtensionPointsInstaller : Installer<ExtensionPointsInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind(typeof(IRunExtensionPoint<,>)).To(typeof(RunExtensionPoint<,>)).AsSingle();
			Container.Bind(typeof(IRunExtensionPoint<>)).To(typeof(RunExtensionPoint<>)).AsSingle();
		}
	}
}