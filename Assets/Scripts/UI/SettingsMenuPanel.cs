using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsMenuPanel : MonoBehaviour {

    private int _width = 1280;
    private int _height = 1024;

    public Dropdown ResolutionsDropdown;
    public GameObject ResolutionItemTemplate;
    public ResolutionsList ResolutionsList;

    public GameObject SoundSlider;
    public GameObject WindowedToggle;

    void Start ()
    {
        SoundSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("volumeValue", 1f);
        WindowedToggle.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("isFullScreen", 1) != 1;
        
        foreach (var item in ResolutionsList.Resolutions)
        {
            var option = Instantiate(ResolutionItemTemplate);
            option.transform.FindChild("Item Label").gameObject.GetComponentInChildren<Text>().text = string.Format("{0}x{1}", item.Width, item.Height);
        }
        ResolutionsDropdown.value = PlayerPrefs.GetInt("resolutionIndex", 3); // for fullhd by default
        ResolutionsDropdown.interactable = !Screen.fullScreen;
    }

    public void OnWindowedToggled(bool windowed)
    {
        PlayerPrefs.SetInt("isFullScreen", windowed ? 0 : 1);
        UpdateWindowSize();
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("volumeValue", value);
    }

    public void OnResolutionChanged(int value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("volumeValue", value);
    }

    public void UpdateWindowSize()
    {
        if (PlayerPrefs.GetInt("isFullScreen", 1) == 1)
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
