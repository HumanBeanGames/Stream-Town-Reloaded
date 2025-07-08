using Reflex.Core;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerInstaller : MonoBehaviour, IInstaller
{
    [SerializeField]
    private AudioMixer _audioMixer;
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton<AudioMixer>(c => _audioMixer, typeof(AudioMixer));
    }
}
