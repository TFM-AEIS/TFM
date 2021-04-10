using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider musicSlider, sfxSlider;

    [Range(-80, 0)]
    [SerializeField] private float defaultMusic, defaultSfx;

    private static readonly string firstPlayPref = "firstPlayPref";
    private static readonly string musicPref = "musicPref";
    private static readonly string sfxPref = "sfxPref";

    private bool firstPlayCheck;

    private void Start()
    {
        this.firstPlayCheck = PlayerPrefs.GetInt(firstPlayPref, 1) == 1;

        if (this.firstPlayCheck)
        {
            PlayerPrefs.SetInt(firstPlayPref, 0);

            this.musicSlider.value = this.defaultMusic;
            this.sfxSlider.value = this.defaultSfx;

            PlayerPrefs.SetFloat(musicPref, this.musicSlider.value);
            PlayerPrefs.SetFloat(sfxPref, this.sfxSlider.value);
        }
        else
        {
            this.musicSlider.value = PlayerPrefs.GetFloat(musicPref);
            this.sfxSlider.value = PlayerPrefs.GetFloat(sfxPref);
        }
    }

    public void SetMusic(float volume)
    {
        this.audioMixer.SetFloat("music", volume);
        PlayerPrefs.SetFloat(musicPref, volume);
    }

    public void SetSfx(float volume)
    {
        this.audioMixer.SetFloat("sfx", volume);
        PlayerPrefs.SetFloat(sfxPref, volume);
    }
}
