using System;
using System.Collections.Generic;
using System.Linq;
using Common.Scripts.ExtensionPoints.PublicAPI.Sync;
using UnityEngine;

namespace Common.ExtensionPoints.Implementation
{
	public class RunExtensionPoint<TPoint, TContext> : IRunExtensionPoint<TPoint, TContext>
		where TPoint : IExtensionPoint<TContext>
	{
		private readonly IEnumerable<TPoint> _points;

		public RunExtensionPoint(IEnumerable<TPoint> points)
		{
			_points = points;
		}

		public void Execute(TContext context)
		{
			foreach (var point in _points.Where(point => IsActiveSafeCall(context, point)))
			{
				try
				{
					point.Execute(context);
				}
				catch (Exception e)
				{
					Debug.LogError("Exception during execute extension point " + e);
				}
			}
		}

		private bool IsActiveSafeCall(TContext context, TPoint point)
		{
			try
			{
				return point.IsActive(context);
			}
			catch (Exception e)
			{
				Debug.LogError("Exception during IsActive check" + e);

				return false;
			}
		}
	}

	public class RunExtensionPoint<TPoint> : IRunExtensionPoint<TPoint> where TPoint : IExtensionPoint
	{
		private readonly IEnumerable<TPoint> _points;

		public RunExtensionPoint(IEnumerable<TPoint> points)
		{
			_points = points;
		}

		public void Execute()
		{
			foreach (var point in _points.Where(IsActiveSafeCall))
			{
				try
				{
					point.Execute();
				}
				catch (Exception e)
				{
					Debug.LogError("Exception during execute extension point" + e);
				}
			}
		}

		private bool IsActiveSafeCall(TPoint point)
		{
			try
			{
				return point.IsActive();
			}
			catch (Exception e)
			{
				Debug.LogError("Exception during IsActive check" + e);

				return false;
			}
		}
	}
}