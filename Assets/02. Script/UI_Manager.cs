using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LeiaUnity;

/// <summary>
/// Manages the UI components and settings for the game
/// Handles 3D display toggle, audio settings, and menu navigation
/// </summary>
public class UI_Manager : MonoBehaviour
{
    [Header("Leia Display")]
    [SerializeField] private LeiaDisplay leiaDisplay;

    [Header("UI Buttons")]
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button closeButton;

    [Header("Settings Controls")]
    [SerializeField] private Toggle toggle3D;
    [SerializeField] private Toggle toggleBgmSound;
    [SerializeField] private Slider sliderBgmSound;

    [Header("UI Elements")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private DoTEffect settingEffect;

    private Audio_Manager audioManager;
    private bool is3DEnabled = true;
    private bool isMuted = false;

    private void Awake()
    {
        // Find audio manager once and cache it
        audioManager = FindObjectOfType<Audio_Manager>();

        if (audioManager == null)
        {
            Debug.LogError("Audio_Manager not found in scene!");
            return;
        }
    }

    private void Start()
    {
        // Initialize audio slider with current volume
        sliderBgmSound.value = audioManager.Current_BGM_Volume;

        // Setup UI button listeners
        SetupButtonListeners();

        // Set initial toggle states
        toggle3D.isOn = is3DEnabled;
        toggleBgmSound.isOn = isMuted;
    }

    /// <summary>
    /// Sets up all the button and UI event listeners
    /// </summary>
    private void SetupButtonListeners()
    {
        // Button listeners
        optionButton.onClick.AddListener(OpenOptions);
        exitButton.onClick.AddListener(ExitGame);
        restartButton.onClick.AddListener(RestartGame);
        closeButton.onClick.AddListener(CloseOptions);

        // Toggle listeners
        toggle3D.onValueChanged.AddListener(OnToggle3D);
        toggleBgmSound.onValueChanged.AddListener(OnToggleBgmSound);

        // Slider listener
        sliderBgmSound.onValueChanged.AddListener(OnBgmVolumeChanged);
    }

    /// <summary>
    /// Opens the options panel
    /// </summary>
    private void OpenOptions()
    {
        optionPanel.SetActive(true);
    }

    /// <summary>
    /// Closes the options panel with effect
    /// </summary>
    private void CloseOptions()
    {
        settingEffect.EndDoT();
    }

    /// <summary>
    /// Exits the application
    /// </summary>
    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Restarts the current scene
    /// </summary>
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Handles 3D toggle changes
    /// </summary>
    private void OnToggle3D(bool isOn)
    {
        is3DEnabled = isOn;
        leiaDisplay.Set3DMode(is3DEnabled);
    }

    /// <summary>
    /// Handles BGM mute toggle changes
    /// </summary>
    private void OnToggleBgmSound(bool isMuted)
    {
        this.isMuted = isMuted;
        audioManager.SetMute(isMuted);

        // Update slider visibility
        if (isMuted)
        {
            sliderBgmSound.value = 0;
        }
        else if (sliderBgmSound.value == 0)
        {
            // If unmuting from 0, set to default value
            sliderBgmSound.value = 0.5f;
        }
    }

    /// <summary>
    /// Handles BGM volume slider changes
    /// </summary>
    private void OnBgmVolumeChanged(float value)
    {
        // Update audio volume
        if (audioManager != null)
        {
            audioManager.SetBGMVolume(value);
        }

        // Update mute toggle based on volume
        bool shouldBeMuted = value <= 0.01f;
        if (toggleBgmSound.isOn != shouldBeMuted)
        {
            toggleBgmSound.isOn = shouldBeMuted;
        }
    }
}