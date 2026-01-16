using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Sahneler arası geçişleri yöneten singleton script.
/// Fade in/out efektleri ile smooth sahne geçişleri sağlar.
/// </summary>
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    
    private bool isTransitioning = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Fade image'i başlangıçta tamamen şeffaf yap
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }
    }

    /// <summary>
    /// Belirtilen sahneye fade efekti ile geçiş yapar.
    /// </summary>
    /// <param name="sceneName">Yüklenecek sahne ismi</param>
    public void FadeToScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToScene(sceneName));
        }
    }

    /// <summary>
    /// Sahne geçiş coroutine'i - fade out, scene load, fade in
    /// </summary>
    private IEnumerator TransitionToScene(string sceneName)
    {
        isTransitioning = true;

        // Fade out
        yield return StartCoroutine(Fade(1f));

        // Sahneyi yükle
        SceneManager.LoadScene(sceneName);

        // Fade in
        yield return StartCoroutine(Fade(0f));

        isTransitioning = false;
    }

    /// <summary>
    /// Fade efekti coroutine'i
    /// </summary>
    /// <param name="targetAlpha">Hedef alpha değeri (0 = şeffaf, 1 = opak)</param>
    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null) yield break;

        float startAlpha = fadeImage.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            
            Color color = fadeImage.color;
            color.a = newAlpha;
            fadeImage.color = color;

            yield return null;
        }

        // Son değeri tam olarak ayarla
        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;
    }

    /// <summary>
    /// Manuel fade in efekti
    /// </summary>
    public void FadeIn()
    {
        StartCoroutine(Fade(0f));
    }

    /// <summary>
    /// Manuel fade out efekti
    /// </summary>
    public void FadeOut()
    {
        StartCoroutine(Fade(1f));
    }
}