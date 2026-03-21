using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    // References to UI elements
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Dropdown graphicsQualityDropdown;

    public AudioMixer mainMixer;

    void Start()
    {
        // Load and set initial settings
        LoadSettings();

        // Add listeners to update settings when UI elements are changed
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        graphicsQualityDropdown.onValueChanged.AddListener(SetGraphicsQuality);
    }

    private void LoadSettings()
    {
        // Load settings and update UI elements
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 0.5f);

        mainMixer.SetFloat("SoundEffectVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", 1.0f)) * 20);
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1.0f)) * 20);

        graphicsQualityDropdown.value = PlayerPrefs.GetInt("GraphicsQuality", 2) - 1;

        // Set frame rate according to graphics quality
        GameManager.Instance.SetFrameRate();

        // Set game sensitivity
        GameManager.Instance.sensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
    }

    // Set Music Volume
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    // Set Sound Effects Volume
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
        mainMixer.SetFloat("SoundEffectVolume", Mathf.Log10(volume) * 20);
    }

    // Set Sensitivity
    public void SetSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.Save();
        GameManager.Instance.sensitivity = sensitivity;
    }

    // Set Graphics Quality
    public void SetGraphicsQuality(int quality)
    {
        int adjustedQuality = quality + 1; // Adjust for dropdown index (0 = Low, 1 = Medium, 2 = High)
        PlayerPrefs.SetInt("GraphicsQuality", adjustedQuality);
        PlayerPrefs.Save();
        QualitySettings.SetQualityLevel(adjustedQuality);
        GameManager.Instance.SetFrameRate();
        Debug.Log("Graphics quality is now " + adjustedQuality);
    }

    // Reset All Settings to Default
    public void ResetAllSettings()
    {
        SetMusicVolume(1.0f);
        SetSFXVolume(1.0f);
        SetSensitivity(1.0f);
        SetGraphicsQuality(2);
        LoadSettings(); // Refresh UI elements with default values
    }
}
