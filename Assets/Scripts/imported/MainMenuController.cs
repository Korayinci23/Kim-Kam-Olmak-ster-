using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ana menü kontrolcüsü. Menü butonlarını ve müziği yönetir.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Audio Settings")]
    [SerializeField] private AudioSource menuMusicSource;
    [SerializeField] private AudioSource buttonSoundSource;
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Button References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        // Butonlara event listener ekle
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }

        // Menü müziğini başlat
        if (menuMusicSource != null && !menuMusicSource.isPlaying)
        {
            menuMusicSource.Play();
        }
    }

    /// <summary>
    /// Play butonuna basıldığında çağrılır. Oyun sahnesine geçiş yapar.
    /// </summary>
    public void PlayGame()
    {
        // Buton ses efekti çal
        PlayButtonSound();
        
        Debug.Log("Play Game button clicked!");
        
        // Play butonunu gizle (tekrar tıklanmasını önlemek için)
        if (playButton != null)
        {
            playButton.gameObject.SetActive(false);
        }
        
        // Menü müziğini durdur
        if (menuMusicSource != null)
        {
            menuMusicSource.Stop();
        }

        // Sahne geçişini başlat
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.FadeToScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning("SceneTransition Instance bulunamadı! Sahne normal şekilde yükleniyor.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }
    }

    /// <summary>
    /// Ayarlar butonuna basıldığında çağrılır.
    /// İleride ayarlar menüsü için kullanılabilir.
    /// </summary>
    public void OpenSettings()
    {
        // Buton ses efekti çal
        PlayButtonSound();
        
        Debug.Log("Settings button clicked!");
        // TODO: Ayarlar menüsünü aç
        // Örnek: SettingsPanel.SetActive(true);
    }

    /// <summary>
    /// Çıkış butonuna basıldığında çağrılır.
    /// </summary>
    public void ExitGame()
    {
        // Buton ses efekti çal
        PlayButtonSound();
        
        Debug.Log("Exit button clicked!");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        // Event listener'ları temizle
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(PlayGame);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OpenSettings);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(ExitGame);
        }
    }

    /// <summary>
    /// Buton tıklama sesini çal
    /// </summary>
    private void PlayButtonSound()
    {
        if (buttonSoundSource != null && buttonClickSound != null)
        {
            buttonSoundSource.PlayOneShot(buttonClickSound);
        }
    }
}