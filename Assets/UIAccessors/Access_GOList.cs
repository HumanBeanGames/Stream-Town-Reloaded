using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Access_GOList : UIElementWrapper<List<GameObject>>
{
    [HideInInspector]
    public List<GameObject> list;

    private void Awake()
    {   
        list = new List<GameObject>();

        foreach (Transform child in transform)
            list.Add(child.gameObject);
    }
}
