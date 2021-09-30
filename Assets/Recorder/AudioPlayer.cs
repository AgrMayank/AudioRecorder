using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class AudioPlayer : MonoBehaviour
{
    public Slider audioSlider;
    public AudioSource audioSource;

    [HideInInspector]
    public AudioClip audioClip;

    public TMP_Text ConsoleText;

    private bool isPlaying = false;

    public void PlayAudio()
    {
        if (!isPlaying)
        {
            audioSource.clip = audioClip;

            audioSlider.direction = Slider.Direction.LeftToRight;
            audioSlider.minValue = 0;
            audioSlider.maxValue = audioSource.clip.length;

            audioSource.Play();
            isPlaying = true;
        }
        else
        {
            audioSource.UnPause();
        }
    }

    public void PauseAudio()
    {
        audioSource.Pause();
    }

    void Update()
    {
        if (audioClip == null)
        {
            ConsoleText.text = "No Audio Clip Found. Record An Audio Clip First.";
        }

        audioSlider.value = GetComponent<AudioSource>().time;

        if ((audioSlider.value == audioSlider.maxValue || audioSlider.value == audioSlider.minValue) && !audioSource.isPlaying)
        {
            isPlaying = false;
        }
    }
}