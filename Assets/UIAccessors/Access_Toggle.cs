using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public abstract class Access_Toggle : UIElementWrapper<Toggle>
{
    Toggle toggle;
    public bool isOn { get; set; } //REVISIT TO MAKE SET PRIVATE
    private void Awake()
    {   
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    public void OnValueChanged(bool inValue)
    {
        isOn = inValue;
    }
}
