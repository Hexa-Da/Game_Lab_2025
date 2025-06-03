using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Reglage : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider playerVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private AudioMixer audioMixer;

    private Resolution[] resolutions;
    private int currentResolution; 

    private void Start()
    {
        // Vérifier que l'AudioMixer est assigné
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer n'est pas assigné dans le composant Reglage!");
            return;
        }

        resolutionDropdown.ClearOptions();
        resolutions = Screen.resolutions;
        List<string> _resolutionLabels = new List<string>(); 
        for(int i = 0; i < resolutions.Length; i++)
        {
            _resolutionLabels.Add(resolutions[i].ToString());
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolution = i;
            }
        }
        resolutionDropdown.AddOptions(_resolutionLabels);
        resolutionDropdown.value = currentResolution;
        
        fullscreenToggle.isOn = Screen.fullScreen;

        // Initialiser les sliders à 50%
        volumeSlider.value = 0.5f;
        playerVolumeSlider.value = 0.5f;

        // Appliquer les volumes initiaux
        UpdateVolume(0.5f);
        UpdatePlayerVolume(0.5f);

        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        playerVolumeSlider.onValueChanged.AddListener(UpdatePlayerVolume);
        resolutionDropdown.onValueChanged.AddListener(UpdateResolution);
        fullscreenToggle.onValueChanged.AddListener(ToggleFullScreen);
    }

    private void OnEnable()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        
        // Récupérer les volumes actuels
        float masterVolume;
        if (audioMixer.GetFloat("Master", out masterVolume))
        {
            volumeSlider.value = Mathf.InverseLerp(-25f, 10f, masterVolume);
        }

        float playerVolume;
        if (audioMixer.GetFloat("Player", out playerVolume))
        {
            playerVolumeSlider.value = Mathf.InverseLerp(-25f, 10f, playerVolume);
        }
    }

    private void UpdateVolume(float _value)
    {
        // Convertir la valeur du slider (0-1) en volume dB (-25 à 10)
        float volume = Mathf.Lerp(-25f, 10f, _value);
        audioMixer.SetFloat("Master", volume);
    }

    private void UpdatePlayerVolume(float _value)
    {
        // Convertir la valeur du slider (0-1) en volume dB (-25 à 10)
        float volume = Mathf.Lerp(-25f, 10f, _value);
        audioMixer.SetFloat("Player", volume);
    }

    private void UpdateResolution(int _value)
    {
        currentResolution = _value;
        Screen.SetResolution(resolutions[currentResolution].width,resolutions[currentResolution].height, Screen.fullScreen);
    }

    private void ToggleFullScreen(bool _value)
    {
        Screen.fullScreen = _value;
    }
}
