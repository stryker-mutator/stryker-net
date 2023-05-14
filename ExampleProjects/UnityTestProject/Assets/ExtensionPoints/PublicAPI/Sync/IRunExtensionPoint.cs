namespace Common.Scripts.ExtensionPoints.PublicAPI.Sync
{
	// ReSharper disable once UnusedTypeParameter
	// TPoint require to bind open generics and do association with one certain point. For example case with two points with the same payload
	public interface IRunExtensionPoint<TPoint, in TPayload> where TPoint : IExtensionPoint<TPayload>
	{
		public void Execute(TPayload context);
	}

	// ReSharper disable once UnusedTypeParameter
	// TPoint require to bind open generics and do association with one certain point. For example case with two points with the same payload
	public interface IRunExtensionPoint<TPoint> where TPoint : IExtensionPoint
	{
		public void Execute();
	}
}