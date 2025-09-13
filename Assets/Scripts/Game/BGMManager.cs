using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;        // untuk music utama
    public AudioSource ambienceSource;   // untuk ambience

    [Header("Settings")]
    [Range(0f, 1f)] public float bgmVolume = 0.7f;
    [Range(0f, 1f)] public float ambienceVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        if (bgmSource != null)
            bgmSource.volume = bgmVolume;
        if (ambienceSource != null)
            ambienceSource.volume = ambienceVolume;
    }

    #region BGM Controls
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null) return;
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource == null) return;
        bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;
    }
    #endregion

    #region Ambience Controls
    public void PlayAmbience(AudioClip clip, bool loop = true)
    {
        if (ambienceSource == null || clip == null) return;
        ambienceSource.clip = clip;
        ambienceSource.loop = loop;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        if (ambienceSource == null) return;
        ambienceSource.Stop();
    }

    public void SetAmbienceVolume(float volume)
    {
        ambienceVolume = Mathf.Clamp01(volume);
        if (ambienceSource != null)
            ambienceSource.volume = ambienceVolume;
    }
    #endregion

    #region Fade (Optional)
    public void FadeBGM(float targetVolume, float duration)
    {
        if (bgmSource != null)
            StartCoroutine(FadeVolume(bgmSource, targetVolume, duration));
    }

    public void FadeAmbience(float targetVolume, float duration)
    {
        if (ambienceSource != null)
            StartCoroutine(FadeVolume(ambienceSource, targetVolume, duration));
    }

    private System.Collections.IEnumerator FadeVolume(AudioSource source, float target, float duration)
    {
        float startVolume = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, target, t / duration);
            yield return null;
        }
        source.volume = target;
    }
    #endregion
}
