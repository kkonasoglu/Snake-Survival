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

        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
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
        PlayBGM();
    }
}
