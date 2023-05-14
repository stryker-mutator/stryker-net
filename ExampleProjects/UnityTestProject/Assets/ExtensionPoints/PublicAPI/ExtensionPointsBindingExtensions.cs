using UnityEngine;
using Zenject;

namespace Common.Scripts.ExtensionPoints.PublicAPI
{
	public static class ExtensionPointsBindingExtensions
	{
		public static void BindExtensionPoint<TConcrete, TDestination>(this DiContainer container)
			where TDestination : TConcrete
		{
			container.Bind<TConcrete>().To<TDestination>().AsSingle();
			EmptyMethod();
		}

		public static void EmptyMethod()
		{
			Debug.Log("Do nothin");
		}
	}
}