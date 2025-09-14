using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;        
    public AudioSource ambienceSource;   
    public AudioSource sfxSource;        

    [Header("Settings")]
    [Range(0f, 1f)] public float bgmVolume = 0.7f;
    [Range(0f, 1f)] public float ambienceVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Clips")]
    public AudioClip bgmMainMenu;
    public AudioClip bgmRush;
    public AudioClip ambienceRush;

    [Header("Transition Settings")]
    public float fadeTime = 1f; // durasi fade in/out

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // subscribe event
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (bgmSource != null) bgmSource.volume = bgmVolume;
        if (ambienceSource != null) ambienceSource.volume = ambienceVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            CrossfadeBGM(bgmMainMenu, fadeTime);
            StopAmbience(); // ga pake ambience di menu
        }
        else if (scene.name == "RushMode")
        {
            CrossfadeBGM(bgmRush, fadeTime);
            PlayAmbience(ambienceRush);
        }
    }

    #region BGM Controls
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null) return;
        if (bgmSource.isPlaying && bgmSource.clip == clip) return; // anti dupe
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    public void CrossfadeBGM(AudioClip newClip, float duration)
    {
        if (bgmSource == null || newClip == null) return;
        if (bgmSource.clip == newClip && bgmSource.isPlaying) return;
        StartCoroutine(CrossfadeRoutine(newClip, duration));
    }

    private System.Collections.IEnumerator CrossfadeRoutine(AudioClip newClip, float duration)
    {
        // Fade out
        float startVol = bgmSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        bgmSource.Stop();

        // Ganti clip
        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.Play();

        // Fade in
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, t / duration);
            yield return null;
        }
        bgmSource.volume = bgmVolume;
    }
    #endregion

    #region Ambience Controls
    public void PlayAmbience(AudioClip clip, bool loop = true)
    {
        if (ambienceSource == null || clip == null) return;
        if (ambienceSource.isPlaying && ambienceSource.clip == clip) return; // anti dupe
        ambienceSource.clip = clip;
        ambienceSource.loop = loop;
        ambienceSource.volume = ambienceVolume;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        if (ambienceSource != null) ambienceSource.Stop();
    }
    #endregion

    #region SFX Controls
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }
    #endregion
}
