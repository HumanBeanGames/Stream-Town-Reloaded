using Character;

namespace GameEventSystem.Events.Voting 
{
	public class PlayerVote
	{
		public Player Player;
		public VoteOption VoteOption;

		public PlayerVote(Player player, VoteOption option)
		{
			Player = player;
			VoteOption = option;
		}
	}
}