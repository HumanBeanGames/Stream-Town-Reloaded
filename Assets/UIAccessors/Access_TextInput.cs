using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public abstract class Access_TextInput : UIElementWrapper<TMP_InputField>
{
    TMP_InputField textField;
    public string text { get; set; } //REVISIT TO MAKE SET PRIVATE

    private void Awake()
    {   
        textField = GetComponent<TMP_InputField>();
        textField.onValueChanged.AddListener(OnValueChanged);
    }

    public void OnValueChanged(string inValue)
    {
        text = inValue;
    }
}
