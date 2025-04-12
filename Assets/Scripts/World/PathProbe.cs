
using Pathfinding;
using UnityEngine;

namespace World
{
    /// <summary>
    /// Used to check if a path is valid to probes location.
    /// </summary>
    public class PathProbe : MonoBehaviour
    {
        private GraphNode _thisNode = null;

        /// <summary>
        /// Returns true if a path is possible to this probe.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool CanPathTo(GraphNode from) => PathUtilities.IsPathPossible(from, _thisNode);

        private void Start()
        {
            _thisNode = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;
            GameManager.Instance.AddPathProbe(this);
        }
    }
}