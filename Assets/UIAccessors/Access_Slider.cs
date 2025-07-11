using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public abstract class Access_Slider : UIElementWrapper<Slider>
{
    Slider slider;
    public float val { get; set; } //REVISIT TO MAKE SET PRIVATE

    private void Awake()
    {   
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);
    }

    public void OnValueChanged(float inValue)
    {
        val = inValue;
    }
}
