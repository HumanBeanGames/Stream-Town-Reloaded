using Reflex.Core;
using System.Collections.Generic;
using UnityEngine;

public class SettingPresetsInstaller : MonoBehaviour, IInstaller
{
    [SerializeField]
    List<SettingPreset> presets;
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(presets.ToArray());
    }
}
