using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public Slider difficultySlider;
    public Slider volumeSlider;
    public Toggle muteMusicToggle;

    private void Start()
    {
        difficultySlider.value = GameSettings.difficultyLevel;
        volumeSlider.value = GameSettings.volumeLevel;
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene(1);
    }

    public void difficultyChanged()
    {
        GameSettings.difficultyLevel = (int)difficultySlider.value;
    }

    public void VolumeChanged()
    {
       GameSettings.volumeLevel = volumeSlider.value;
       Sound[] sounds = FindObjectOfType<AudioManager>().sounds;

        foreach(Sound s in sounds)
        {
            s.source.volume = s.maxVolume * volumeSlider.value;
        }
    }

    public void MuteMusic()
    {
        GameSettings.musicMuted = muteMusicToggle.isOn;
        Sound[] sounds = FindObjectOfType<AudioManager>().sounds;
        Sound s = Array.Find(sounds, sound => sound.name == "Theme");
        if (s == null)
            return;
        s.source.mute = muteMusicToggle.isOn;
    }

}
