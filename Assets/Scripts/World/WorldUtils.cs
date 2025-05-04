using Managers;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace World
{
    /// <summary>
    /// Holds utility functions relating to the world.
    /// </summary>
    public static class WorldUtils
    {
        public static LayerMask GroundLayerMask { get; set; }

        /// <summary>
        /// Returns true if the position is on the ground as well as the height of the ground.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static (bool, float) OnGroundCheckHeight(Vector3 position)
        {
            float height = GetTerrainHeightAtPosition(position);

            return (height >= 0, height);
        }

        /// <summary>
        /// Returns true if position is on or above the ground.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool OnGroundCheck(Vector3 position)
        {
            return OnGroundCheckHeight(position).Item1;
        }

        /// <summary>
        /// Returns true if the position is on the shore line as well as the height of the shoreline.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static (bool, float) OnShoreLineCheckHeight(Vector3 position)
        {
            float height = GetTerrainHeightAtPosition(position);

            return (height >= -1.0f && height < 0, height);
        }

        /// <summary>
        /// Returns true if position is on the shoreline.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool OnShoreLineCheck(Vector3 position)
        {
            return OnShoreLineCheckHeight(position).Item1;
        }

        /// <summary>
        /// Returns true if the position is underwater and not obstructed by terrain as well as the height of the ground underneath.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static (bool, float) UnderWaterCheckHeight(Vector3 position)
        {
            float height = GetTerrainHeightAtPosition(position);
            return (height < -1.0f, height);
        }

        /// <summary>
        /// Returns true if the position is underwater.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool UnderWaterCheck(Vector3 position)
        {
            return UnderWaterCheckHeight(position).Item1;
        }

        /// <summary>
        /// Returns the terrain height at the given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static float GetTerrainHeightAtPosition(Vector3 position)
        {
            if (Physics.Raycast(position + Vector3.up * 1, Vector3.down, out RaycastHit hit, 15, GroundLayerMask))
            {
                return hit.point.y;
            }

            return 0;
        }

        /// <summary>
        /// Sets the objects transform from another transform
        /// </summary>
        /// <param name="obj">The object whos transform is being changed</param>
        /// <param name="targetTransform">The transform to be changed to</param>
        public static void SetTransformFromTransform(GameObject obj, Transform targetTransform)
        {
            obj.transform.position = targetTransform.position;
            obj.transform.rotation = targetTransform.rotation;
            obj.transform.localScale = targetTransform.localScale;
        }
        
		public static bool IsPointerOverUI(EventSystem eventSystem)
		{
			return eventSystem.IsPointerOverGameObject() || eventSystem.currentSelectedGameObject != null;
		}

		public static double CurrentTime => TimeManager.WorldTimePassed;
	}
}