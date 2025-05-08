using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Reglage : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private AudioMixer audioMixer;

    private Resolution[] resolutions;
    private int currentResolution; 

    private void Start()
    {
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
        audioMixer.GetFloat("Master", out float _volume);
        volumeSlider.value = Mathf.Lerp(-80f, 5f, _volume);

        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        resolutionDropdown.onValueChanged.AddListener(UpdateResolution);
        fullscreenToggle.onValueChanged.AddListener(ToggleFullScreen);
    }

    private void OnEnable()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        audioMixer.GetFloat("Master", out float _volume);
        volumeSlider.value = Mathf.InverseLerp(-80f, 5f, _volume);
    }


    private void UpdateVolume(float _value)
    {
        //print("Audio volume : " + _value);
        audioMixer.SetFloat("Master", Mathf.Lerp(-80, 5, _value));
    }

    private void UpdateResolution(int _value)
    {
        currentResolution = _value;
        Screen.SetResolution(resolutions[currentResolution].width,resolutions[currentResolution].height, Screen.fullScreen);
        //print("Resolution ID : " + _value);
    }

    private void ToggleFullScreen(bool _value)
    {
        //print("FullScreen : " + _value);
        Screen.fullScreen = _value;
    }
}
