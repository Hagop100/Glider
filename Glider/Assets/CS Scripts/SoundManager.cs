using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// This script was originally attached to GameSessionManager but for whatever reason it was failing to grab the slider's value attribute
    /// the code would not give errors but it would not grab the value and it wasn't working
    /// possible hypotheses may include the gameobject holding mutliple scripts or being prefabbed??
    /// Really not sure but upon creating a brand new game object, the issue has been resolved and we have been saved from eternal damnation
    /// Well it seems the prefab is what caused it to break. But I'm not sure exactly why.
    /// </summary>
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Text sliderValueText;

    private const string MASTER_VOLUME = "masterVolume";

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey(MASTER_VOLUME))
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME, 0.5f);
            LoadVolume();
            
        }
        else
        {
            LoadVolume();
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = masterVolumeSlider.value;
        sliderValueText.text = (masterVolumeSlider.value * 100).ToString("F0");
        SaveVolume();
    }

    public void LoadVolume()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat(MASTER_VOLUME);
        AudioListener.volume = masterVolumeSlider.value;
        sliderValueText.text = (masterVolumeSlider.value * 100).ToString("F0");
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME, masterVolumeSlider.value);
    }
}
