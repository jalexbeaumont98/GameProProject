using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundData", menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    [Header("Identity")]
    public string id;                     // Unique ID used to play this sound

    [Header("Audio")]
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(.5f, 2f)] public float pitch = 1f;
    public bool loop = false;

    [Header("Routing (optional)")]
    public AudioMixerGroup outputMixerGroup;
}
