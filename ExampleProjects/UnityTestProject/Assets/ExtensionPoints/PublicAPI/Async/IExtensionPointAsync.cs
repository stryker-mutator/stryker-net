using System.Threading;
using Cysharp.Threading.Tasks;

namespace Common.Scripts.ExtensionPoints.PublicAPI.Async
{
	public interface IExtensionPointAsync<in TContext>
	{
		public bool IsActive(TContext context);
		public UniTask Execute(TContext context, CancellationToken token);
	}

	public interface IExtensionPointAsync
	{
		public bool IsActive();
		public UniTask Execute(CancellationToken token);
	}
}