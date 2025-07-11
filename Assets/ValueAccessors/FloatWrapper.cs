using Reflex.Core;
using UnityEngine;

public abstract class FloatWrapper : MonoBehaviour, IInstaller
{
    public float Val { get; set; }

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(this, GetType());
    }
}