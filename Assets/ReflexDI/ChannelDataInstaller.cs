using Reflex.Core;
using UnityEngine;

public class ChannelDataInstaller : MonoBehaviour, IInstaller
{

    void IInstaller.InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton<ChannelData>(container => new ChannelData(container));
    }
}