using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip bgmClip;
    [Range(0f, 1f)][SerializeField] private float musicVolume = 0.5f;

    [Header("Sound Effects (SFX)")]
    [SerializeField] private AudioClip appleEatClip;
    [SerializeField] private AudioClip bananaClip;
    [SerializeField] private AudioClip devilFruitClip;
    [SerializeField] private AudioClip bombClip;
    [SerializeField] private AudioClip deathClip;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 1.0f;

    [Header("Muffle (Boğuklaştırma - Pause)")]
    [SerializeField] private float normalCutoff = 22000f;
    [SerializeField] private float muffledCutoff = 750f;
    [SerializeField] private float muffleDuration = 0.2f;

    private AudioLowPassFilter musicLowPassFilter;
    private Coroutine muffleCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        musicLowPassFilter = musicSource.GetComponent<AudioLowPassFilter>();
        if (musicLowPassFilter == null)
        {
            musicLowPassFilter = musicSource.gameObject.AddComponent<AudioLowPassFilter>();
        }
        musicLowPassFilter.cutoffFrequency = normalCutoff;
        musicLowPassFilter.lowpassResonanceQ = 1f;
    }

    private void Start()
    {
        PlayBGM();
    }

    private void OnEnable()
    {
        SnakeMovement.OnSnakeDied += HandleSnakeDied;
        SnakeMovement.OnSnakeReset += HandleSnakeReset;
    }

    private void OnDisable()
    {
        SnakeMovement.OnSnakeDied -= HandleSnakeDied;
        SnakeMovement.OnSnakeReset -= HandleSnakeReset;
    }

    public void PlayBGM()
    {
        if (bgmClip != null && musicSource != null)
        {
            musicSource.clip = bgmClip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void StopBGM()
    {
        if (musicSource != null) musicSource.Stop();
    }

    public void SetMusicMuffled(bool muffled)
    {
        if (musicLowPassFilter == null) return;
        if (muffleCoroutine != null) StopCoroutine(muffleCoroutine);
        muffleCoroutine = StartCoroutine(TransitionMuffle(muffled ? muffledCutoff : normalCutoff));
    }

    private System.Collections.IEnumerator TransitionMuffle(float targetCutoff)
    {
        float startCutoff = musicLowPassFilter.cutoffFrequency;
        float elapsed = 0f;

        while (elapsed < muffleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicLowPassFilter.cutoffFrequency = Mathf.Lerp(startCutoff, targetCutoff, elapsed / muffleDuration);
            yield return null;
        }

        musicLowPassFilter.cutoffFrequency = targetCutoff;
        muffleCoroutine = null;
    }

    public void PlayAppleEat() => PlaySFX(appleEatClip);
    public void PlayBanana() => PlaySFX(bananaClip);
    public void PlayDevilFruit() => PlaySFX(devilFruitClip);
    public void PlayBomb() => PlaySFX(bombClip);
    public void PlayDeath() => PlaySFX(deathClip);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    private void HandleSnakeDied()
    {
        StopBGM();
        PlayDeath();
    }

    private void HandleSnakeReset()
    {
        SetMusicMuffled(false);
        PlayBGM();
    }
}
