using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Oyun içi pause menüsünü yöneten controller.
/// Oyunu duraklat/devam ettir, pause panel'ini göster/gizle.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseButton;

    [Header("Settings")]
    [SerializeField] private bool pauseWithEscKey = true;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource buttonSoundSource;
    [SerializeField] private AudioClip pauseClickSound;
    [SerializeField] private AudioClip resumeClickSound;

    private bool isPaused = false;

    private void Start()
    {
        // Pause panel'i başlangıçta kapat
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Resume butonuna event listener ekle
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }

        // Pause butonuna event listener ekle
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }

        // Oyun normal hızda başlasın
        Time.timeScale = 1f;
    }

    private void Update()
    {
        // ESC tuşu ile pause toggle
        if (pauseWithEscKey && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Pause durumunu toggle eder (aç/kapat)
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Oyunu duraklat
    /// </summary>
    public void PauseGame()
    {
        if (isPaused) return; // Zaten pauseli ise tekrar pause etme

        // Buton ses efekti çal
        PlayPauseSound();

        isPaused = true;
        Time.timeScale = 0f; // Oyunu durdur

        // Pause panel'i göster
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        // Pause butonunu gizle (opsiyonel)
        if (pauseButton != null)
        {
            //pauseButton.gameObject.SetActive(false);
        }

        Debug.Log("Oyun durakladı!");
    }

    /// <summary>
    /// Oyunu devam ettir
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused) return; // Zaten çalışıyorsa tekrar resume etme

        // Buton ses efekti çal
        PlayResumeSound();

        isPaused = false;
        Time.timeScale = 1f; // Oyunu devam ettir

        // Pause panel'i gizle
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Pause butonunu göster
        if (pauseButton != null)
        {
            //pauseButton.gameObject.SetActive(true);
        }

        Debug.Log("Oyun devam ediyor!");
    }

    /// <summary>
    /// Pause durumunu döndürür
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }

    private void OnDestroy()
    {
        // Event listener'ları temizle
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(ResumeGame);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveListener(PauseGame);
        }

        // Oyun hızını normale döndür (güvenlik için)
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Pause buton sesini çal
    /// </summary>
    private void PlayPauseSound()
    {
        if (buttonSoundSource != null && pauseClickSound != null)
        {
            buttonSoundSource.PlayOneShot(pauseClickSound);
        }
    }

    /// <summary>
    /// Resume buton sesini çal
    /// </summary>
    private void PlayResumeSound()
    {
        if (buttonSoundSource != null && resumeClickSound != null)
        {
            buttonSoundSource.PlayOneShot(resumeClickSound);
        }
    }
}