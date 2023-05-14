namespace Common.Scripts.ExtensionPoints.PublicAPI.Sync
{
	public interface IExtensionPoint<in TContext>
	{
		public bool IsActive(TContext context);
		public void Execute(TContext context);
	}

	public interface IExtensionPoint
	{
		public bool IsActive();
		public void Execute();
	}
}