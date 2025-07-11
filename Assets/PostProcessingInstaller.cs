using Reflex.Core;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingInstaller : MonoBehaviour, IInstaller
{
    [SerializeField]
    private Volume _volume;
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(_volume);
    }
}