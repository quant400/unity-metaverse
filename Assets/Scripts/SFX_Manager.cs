using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFX_Manager : MonoBehaviour
{
    private static AudioSource _audioSrc;

    [SerializeField] private  static Dictionary<string, AudioClip> _audioList;
    
    void Awake()
    {
        _audioSrc = GetComponent<AudioSource>();
        _audioList = GetAudioFx();
        Debug.Log(string.Join(", ", _audioList.Keys));
    }

    private Dictionary<string, AudioClip> GetAudioFx()
    {
        return Resources.LoadAll<AudioClip>("Audio/SFX").ToDictionary(
            auxAudio => auxAudio.name,
            auxAudio => auxAudio);
    }

    public static void Play(string soundName)
    {
        _audioSrc.PlayOneShot(_audioList[soundName]);
    }
}
