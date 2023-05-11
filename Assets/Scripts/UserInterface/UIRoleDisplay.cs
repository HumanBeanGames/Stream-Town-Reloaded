using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UserInterface 
{
    public class UIRoleDisplay : MonoBehaviour 
	{
		[SerializeField]
		private TextMeshProUGUI _roleName;

		[SerializeField]
		private TextMeshProUGUI _roleAmount;

		[SerializeField]
		private Image _icon;

		public TextMeshProUGUI RoleNameTMP => _roleName;
		public TextMeshProUGUI RoleAmount => _roleAmount;
		public Image Icon => _icon;
    }
}