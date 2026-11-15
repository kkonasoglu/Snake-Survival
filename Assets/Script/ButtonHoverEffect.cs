using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Büyüme Ayarları")]
    [SerializeField] private float hoverScale = 1.15f;    // Fare gelince ne kadar büyüsün (1.15 = %15)
    [SerializeField] private float clickScale = 0.95f;    // Tıklayınca hafif içeri göçsün (%5)
    [SerializeField] private float animationSpeed = 12f;  // Geçiş hızı (akıcılık)

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Coroutine scaleCoroutine;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
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
