using Reflex.Core;
using UnityEngine;

public abstract class UIElementWrapper<T> : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(this, GetType());
    }
}