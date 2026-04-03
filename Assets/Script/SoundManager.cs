using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private bool dontDestroyOnLoad = true;

    [Header("Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip uiClickSfx;
    [SerializeField] private AudioClip loseSfx;
    [SerializeField] private AudioClip bombSfx;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        EnsureAudioSources();
        StartBackgroundMusic();
    }

    private void EnsureAudioSources()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        bgmSource.loop = true;
    }

    public void StartBackgroundMusic()
    {
        if (bgmSource == null || backgroundMusic == null)
            return;

        if (bgmSource.clip == backgroundMusic && bgmSource.isPlaying)
            return;

        bgmSource.clip = backgroundMusic;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayUiClick()
    {
        PlaySfx(uiClickSfx);
    }

    /// <summary>UILose vừa hiện.</summary>
    public void PlayLose()
    {
        PlaySfx(loseSfx);
    }

    /// <summary>Player không khớp obstacle (va chạm / nổ).</summary>
    public void PlayBomb()
    {
        PlaySfx(bombSfx);
    }

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }
}

