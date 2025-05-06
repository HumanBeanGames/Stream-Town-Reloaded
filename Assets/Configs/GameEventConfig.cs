using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/GameEvent Manager Config")]
    public class GameEventConfig : Config<GameEventConfig>
    {
        public float rulerVoteMinTime = 3600f;
    }
}
