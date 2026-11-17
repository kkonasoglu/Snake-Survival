using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Büyüme Ayarları")]
    [SerializeField] private float hoverScale = 1.15f;    // Fare gelince ne kadar büyüsün (1.15 = %15)
    [SerializeField] private float clickScale = 0.95f;    // Tıklayınca hafif içeri göçsün (%5)
    [SerializeField] private float animationSpeed = 12f;  // Geçiş hızı (akıcılık)

    [Header("Hover (Üzerine Gelme) Sesi")]
    [SerializeField] private AudioClip hoverSound;
    [Range(0f, 1f)][SerializeField] private float hoverVolume = 0.6f;

    [Header("Click (Tıklama) Sesleri")]
    [SerializeField] private AudioClip[] clickSounds;
    [Range(0f, 1f)][SerializeField] private float clickVolume = 0.8f;

    private static AudioSource sharedAudioSource;
    private static AudioClip fallbackHoverSound;
    private static AudioClip[] fallbackClickSounds;
    private static int lastClickIndex = -1;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Coroutine scaleCoroutine;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        if (hoverSound != null && fallbackHoverSound == null)
        {
            fallbackHoverSound = hoverSound;
        }

        if (clickSounds != null && clickSounds.Length > 0 && fallbackClickSounds == null)
        {
            fallbackClickSounds = clickSounds;
        }
    }

    private void OnDisable()
    {
        // Buton kapanırsa veya devre dışı kalırsa orijinal boyutuna dönsün
        transform.localScale = originalScale;
        targetScale = originalScale;
    }

    // Fare butonun üzerine geldiğinde
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale * hoverScale);
        PlayHoverSound();
    }

    private void PlayHoverSound()
    {
        AudioClip clipToPlay = hoverSound != null ? hoverSound : fallbackHoverSound;
        if (clipToPlay != null)
        {
            PlaySound(clipToPlay, hoverVolume);
        }
    }

    // Fare butondan ayrıldığında
    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale);
    }

    // Butona tıklandığı an (Mekanik basılma hissi)
    public void OnPointerDown(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale * clickScale);
        PlayRandomClickSound();
    }

    private void PlayRandomClickSound()
    {
        AudioClip[] clips = (clickSounds != null && clickSounds.Length > 0) ? clickSounds : fallbackClickSounds;
        if (clips == null || clips.Length == 0) return;

        int randomIndex = Random.Range(0, clips.Length);
        if (clips.Length > 1 && randomIndex == lastClickIndex)
        {
            randomIndex = (randomIndex + 1) % clips.Length;
        }
        lastClickIndex = randomIndex;

        AudioClip clipToPlay = clips[randomIndex];
        if (clipToPlay != null)
        {
            PlaySound(clipToPlay, clickVolume);
        }
    }

    private static void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null) return;
        if (sharedAudioSource == null)
        {
            GameObject go = new GameObject("ButtonAudioSource");
            sharedAudioSource = go.AddComponent<AudioSource>();
            sharedAudioSource.playOnAwake = false;
            DontDestroyOnLoad(go);
        }
        sharedAudioSource.PlayOneShot(clip, volume);
    }

    // Tıklama bırakıldığında
    public void OnPointerUp(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale * hoverScale);
    }

    private void StartScaleAnimation(Vector3 newTarget)
    {
        targetScale = newTarget;
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        if (gameObject.activeInHierarchy)
        {
            scaleCoroutine = StartCoroutine(AnimateScale());
        }
    }

    private IEnumerator AnimateScale()
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.005f)
        {
            // Pause menüsünde (Time.timeScale = 0) bile çalışması için unscaledDeltaTime kullanıyoruz
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
            yield return null;
        }

        transform.localScale = targetScale;
        scaleCoroutine = null;
    }
}
