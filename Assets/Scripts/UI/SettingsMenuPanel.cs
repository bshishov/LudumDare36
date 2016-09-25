using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

public class SettingsMenuPanel : MonoBehaviour {

    private int _width = 1280;
    private int _height = 1024;

    public Dropdown ResolutionsDropdown;
    public GameObject ResolutionItemTemplate;
    public ResolutionsList ResolutionsList;

    public GameObject SoundSlider;
    public GameObject WindowedToggle;

    public const string VolumeKey = "volumeValue";
    public const string IsWindowedKey = "isFullScreen";
    public const string ResolutionKey = "resolutionIndex";

    void Start ()
    {
        SoundSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat(VolumeKey, 1f);
        WindowedToggle.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(IsWindowedKey, 1) != 1;
        
        ResolutionsDropdown.AddOptions(ResolutionsList.Resolutions.Select(t => string.Format("{0}x{1}", t.Width, t.Height)).ToList());
        ResolutionsDropdown.value = PlayerPrefs.GetInt(ResolutionKey, 3); // for fullhd by default
        ResolutionsDropdown.interactable = !Screen.fullScreen;
    }

    public void OnWindowedToggled(bool windowed)
    {
        PlayerPrefs.SetInt(IsWindowedKey, windowed ? 0 : 1);
        UpdateWindowSize();
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
    }

    public void OnResolutionChanged(int value)
    {
        var resolution = ResolutionsDropdown.options[value].text;
        var splittedResolution = resolution.Split('x');
        _width = Convert.ToInt32(splittedResolution[0]);
        _height = Convert.ToInt32(splittedResolution[1]);
        PlayerPrefs.SetInt(ResolutionKey, value);
        UpdateWindowSize();
    }

    public void UpdateWindowSize()
    {
        if (PlayerPrefs.GetInt(IsWindowedKey, 1) == 1)
        {
            ResolutionsDropdown.interactable = false;
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else
        {
            ResolutionsDropdown.interactable = true;
            Screen.SetResolution(_width, _height, false);
        }
    }


}
