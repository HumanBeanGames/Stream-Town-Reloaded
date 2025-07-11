using Reflex.Core;
using System.Collections.Generic;
using UnityEngine;

public class PresetButtonsInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private List<GameObject> _presetButtons;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(_presetButtons);
    }
}
