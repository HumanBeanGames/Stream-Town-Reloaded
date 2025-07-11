using Reflex.Core;
using UnityEngine;

public abstract class GameObjectAccessor : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(this, GetType());
    }

    public void SetActive(bool value) => gameObject.SetActive(value);

    public bool Enabled {
        get
        {
            return gameObject.activeInHierarchy;
        }

        set
        {
            gameObject.SetActive(value);
        }
    }
}