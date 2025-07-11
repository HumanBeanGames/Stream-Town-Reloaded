using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public abstract class Access_Dropdown : UIElementWrapper<TMP_Dropdown>
{
    public TMP_Dropdown dropDown;
    public int val { get; set; } //REVISIT TO MAKE SET PRIVATE

    private void Awake()
    {   
        dropDown = GetComponent<TMP_Dropdown>();
        dropDown.onValueChanged.AddListener(OnValueChanged);
    }

    public virtual void OnValueChanged(int inValue)
    {
        val = inValue;
    }
}
