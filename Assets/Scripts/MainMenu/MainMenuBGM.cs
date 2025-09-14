using UnityEngine;

public class MainMenuBGMManager : MonoBehaviour
{
    public static MainMenuBGMManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Settings")]
    [Range(0f,1f)] public float bgmVolume = 0.7f;
    [Range(0f,1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        if (bgmSource != null) bgmSource.volume = bgmVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;

        // kalau ada AudioClip sudah assign di bgmSource, langsung play
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmSource != null && !bgmSource.isPlaying) bgmSource.Play();
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }
}
