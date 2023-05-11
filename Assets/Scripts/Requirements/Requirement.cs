namespace Requirements 
{
	/// <summary>
	/// Holds data for any type of requirement for buildings or technology.
	/// </summary>
	[System.Serializable]
    public class Requirement 
	{
		public RequirementType RequirementType;
		public object Data;
    }
}