using UnityEngine;
using UnityEngine.Rendering.Universal;
using Reflex.Core;

public class ForwardRendererInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private UniversalRendererData _forwardRenderer;

    void IInstaller.InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(_forwardRenderer);
    }
}
