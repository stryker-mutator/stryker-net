using System.Threading;
using Cysharp.Threading.Tasks;

namespace Common.Scripts.ExtensionPoints.PublicAPI.Async
{
	// ReSharper disable once UnusedTypeParameter
	// TPoint require to bind open generics and do association with one certain point. For example case with two points with the same payload
	public interface IRunExtensionPointAsync<TPoint, in TPayload> where TPoint : IExtensionPointAsync<TPayload>
	{
		public UniTask Execute(TPayload context, CancellationToken cancellationToken);
	}

	// ReSharper disable once UnusedTypeParameter
	// TPoint require to bind open generics and do association with one certain point. For example case with two points with the same payload
	public interface IRunExtensionPointAsync<TPoint> where TPoint : IExtensionPointAsync
	{
		public UniTask Execute(CancellationToken cancellationToken);
	}
}