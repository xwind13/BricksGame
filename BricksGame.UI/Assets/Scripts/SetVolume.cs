using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void Start()
    {
        var sliderValue = PlayerPrefs.GetFloat("Sound");
        audioMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);

        var slider = GetComponent<Slider>();
        slider.value = sliderValue;
    }

    public void SetLevel(float sliderValue)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Sound", sliderValue);
    }
}
