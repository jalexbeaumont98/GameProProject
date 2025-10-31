using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Library")]
    [SerializeField] private List<SoundData> sounds = new List<SoundData>();

    [Header("Startup Music")]
    [SerializeField] private string startupMusicId = ""; // set to valid ID to auto-play
    [SerializeField] private float musicFadeInSeconds = 0f;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;  // looping music
    [SerializeField] private AudioSource sfxSource;    // one-shot SFX

    private Dictionary<string, SoundData> byId;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build lookup
        byId = new Dictionary<string, SoundData>(sounds.Count);
        foreach (var s in sounds)
        {
            if (s == null) continue;
            if (!byId.ContainsKey(s.id)) byId.Add(s.id, s);
            else Debug.LogWarning($"Duplicate Sound ID {s.id} on {s.name}");
        }

        // Auto-create AudioSources if not assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
        }
    }

    private void Start()
    {
        if (startupMusicId != "")
        {
            PlayMusic(startupMusicId, fadeInSeconds: musicFadeInSeconds);
        }
    }

    // ---- Public API ----

    public void PlaySFX(string soundId)
    {
        if (!TryGet(soundId, out var s)) return;

        // Route to mixer if provided
        if (s.outputMixerGroup) sfxSource.outputAudioMixerGroup = s.outputMixerGroup;

        sfxSource.pitch = s.pitch;
        sfxSource.PlayOneShot(s.clip, s.volume);
    }

    public void PlayMusic(string soundId, float fadeInSeconds = 0f)
    {
        if (!TryGet(soundId, out var s)) return;

        if (s.outputMixerGroup) musicSource.outputAudioMixerGroup = s.outputMixerGroup;

        musicSource.loop = true; // music generally loops; can respect s.loop if you want
        musicSource.clip = s.clip;
        musicSource.pitch = s.pitch;
        musicSource.volume = 0f;

        musicSource.Play();

        if (fadeInSeconds > 0f)
            StartCoroutine(FadeTo(musicSource, s.volume, fadeInSeconds));
        else
            musicSource.volume = s.volume;
    }

    public void StopMusic(float fadeOutSeconds = 0f)
    {
        if (fadeOutSeconds > 0f)
            StartCoroutine(FadeTo(musicSource, 0f, fadeOutSeconds, stopOnEnd:true));
        else
            musicSource.Stop();
    }

    // ---- Helpers ----

    private bool TryGet(string id, out SoundData sound)
    {
        if (byId != null && byId.TryGetValue(id, out sound) && sound != null && sound.clip != null)
            return true;

        Debug.LogWarning($"AudioManager: Sound ID {id} not found or has no clip.");
        sound = null;
        return false;
    }

    private System.Collections.IEnumerator FadeTo(AudioSource src, float target, float time, bool stopOnEnd = false)
    {
        float start = src.volume;
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime; // unaffected by pause Time.timeScale
            src.volume = Mathf.Lerp(start, target, t / time);
            yield return null;
        }
        src.volume = target;
        if (stopOnEnd && Mathf.Approximately(target, 0f))
            src.Stop();
    }
}
