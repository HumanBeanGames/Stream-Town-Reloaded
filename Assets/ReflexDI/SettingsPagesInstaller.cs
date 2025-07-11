using Reflex.Core;
using UnityEngine;

public class SettingsPagesInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private SettingPages _settingPages;
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(_settingPages);
    }
}
