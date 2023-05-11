using System.Collections.Generic;
using UnityEngine;

namespace Requirements 
{
    [CreateAssetMenu(fileName = "RequirementsDataData", menuName = "ScriptableObjects/RequirementsData", order = 1)]
    public class RequirementsData : ScriptableObject 
	{
		public List<Requirement> Requirements;
    }
}