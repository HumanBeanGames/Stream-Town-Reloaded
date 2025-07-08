using System.Collections.Generic;
using UnityEngine;
using Reflex.Core;

public class AutosaveIntervalsInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private List<int> _autosaveTimeIntervals;

    void IInstaller.InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton<Autosave>(container => new Autosave(container, _autosaveTimeIntervals));
    }
}
