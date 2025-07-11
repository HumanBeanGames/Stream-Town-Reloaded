using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public abstract class Access_Text : UIElementWrapper<TextMeshProUGUI>
{
    TextMeshProUGUI text;
    public string val { get; set; } //REVISIT TO MAKE SET PRIVATE

    private void Awake()
    {   
        text = GetComponent<TextMeshProUGUI>();
        //REVISIT
        //text.onValueChanged.AddListener(OnValueChanged);
    }

    //REVISIT
    /*public void OnValueChanged(int inValue)
    {
        _val =E inValue;
    }*/
}
