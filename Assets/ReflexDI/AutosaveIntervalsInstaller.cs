using System.Collections.Generic;
using UnityEngine;
using Reflex.Core;

public class AutosaveIntervalsInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private List<int> _autosaveTimeIntervals;

    void IInstaller.InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(new Autosave(_autosaveTimeIntervals));
    }
}
