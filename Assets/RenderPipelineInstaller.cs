using Reflex.Core;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RenderPipelineInstaller : MonoBehaviour, IInstaller
{   [SerializeField]
    UniversalRenderPipelineAsset renderPipelineAsset;
    void IInstaller.InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(renderPipelineAsset);
    }
}
