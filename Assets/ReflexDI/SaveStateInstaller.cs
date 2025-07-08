using Reflex.Core;
using UnityEngine;
public class SaveStateInstaller : MonoBehaviour, IInstaller
{
    void IInstaller.InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(new SaveState());
    }
}