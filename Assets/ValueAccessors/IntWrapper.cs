using Reflex.Core;
using UnityEngine;

public abstract class IntWrapper : MonoBehaviour, IInstaller
{
    public int Val { get; set; }

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(this, GetType());
    }
}