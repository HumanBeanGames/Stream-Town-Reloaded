using UnityEngine;
using Utils;
using World;

namespace Buildings
{
	/// <summary>
	/// Probes the surface below the probe.
	/// </summary>
	public class PlacementProbe : MonoBehaviour
	{
		/// <summary>
		/// Height offset for surface checks.
		/// </summary>
		private const float HEIGHT_OFFSET = 0.0f;

		[SerializeField]
		private SurfaceType _desiredSurface;

		/// <summary>
		/// The desired surface of the probe.
		/// </summary>
		public SurfaceType DesiredSurface => _desiredSurface;

		/// <summary>
		/// Returns true if probe is on the desired surface.
		/// </summary>
		public bool OnDesiredSurface => GetSurface() == _desiredSurface;

		/// <summary>
		/// Returns the surface directly below the probe.
		/// </summary>
		/// <returns></returns>
		public SurfaceType GetSurface()
		{
			Vector3 adjustedPostion = transform.position + (Vector3.up * HEIGHT_OFFSET);

			if (WorldUtils.OnGroundCheck(adjustedPostion))
				return SurfaceType.Ground;
			else if (WorldUtils.OnShoreLineCheck(adjustedPostion))
				return SurfaceType.Shoreline;
			else if (WorldUtils.UnderWaterCheck(adjustedPostion))
				return SurfaceType.Underwater;
			else
			{
				Debug.LogWarning($"Probe returned an undefined surface type", this);
				return SurfaceType.Undefined;
			}
		}
	}
}