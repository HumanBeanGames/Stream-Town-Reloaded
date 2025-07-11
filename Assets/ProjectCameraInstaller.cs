using Reflex.Core;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class ProjectCameraInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private Camera _initialCamera;
    [SerializeField] UniversalAdditionalCameraData _initialCameraData;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(c => new ProjectCamera(_initialCamera, _initialCameraData));
    }
}