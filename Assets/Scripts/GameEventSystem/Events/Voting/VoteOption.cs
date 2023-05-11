namespace GameEventSystem.Events.Voting 
{
	public class VoteOption
	{
		public string OptionName;
		public object OptionData;
		public int Votes;

		public VoteOption(string optionName, object data)
		{
			OptionName = optionName;
			OptionData = data;
			Votes = 0;
		}
	}
}