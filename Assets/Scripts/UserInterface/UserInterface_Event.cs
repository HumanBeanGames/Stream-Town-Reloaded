using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface 
{
    public class UserInterface_Event : MonoBehaviour 
	{
        public GameObject EventContainer;
        public TextMeshProUGUI TitleTMP;
        public TextMeshProUGUI DescriptionTMP;
        public Slider Slider;
        public TextMeshProUGUI SliderTMP;

        public void ActivateEventContainer()
        {
            EventContainer.SetActive(true);
        }

        public void DeactivateEventContainer()
        {
            EventContainer.SetActive(false);
        }

		private void Start()
		{
			DeactivateEventContainer();
		}
	}
}