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

    public Image PlayImage, PauseImage;

    private bool isPaused = false;

    private void Start()
    {
        isPaused = false;

        IsPlaying(false);
    }

    private void IsPlaying(bool isPlaying)
    {
        PlayImage.gameObject.SetActive(!isPlaying);
        PauseImage.gameObject.SetActive(isPlaying);
    }

    public void UpdateClip()
    {
        audioSource.clip = audioClip;

        audioSlider.direction = Slider.Direction.LeftToRight;
        audioSlider.minValue = 0;
        audioSlider.maxValue = audioSource.clip.length;
    }

    public void PlayPauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            IsPlaying(false);
            isPaused = true;
        }
        else if (isPaused)
        {
            IsPlaying(true);
            audioSource.UnPause();
        }
        else
        {
            IsPlaying(true);
            audioSource.Play();
        }
    }

    void Update()
    {
        if (audioClip == null)
        {
            ConsoleText.text = "No Audio Clip Found!\nRecord Something First.";
        }

        audioSlider.value = GetComponent<AudioSource>().time;

        if ((audioSlider.value == audioSlider.maxValue || audioSlider.value == audioSlider.minValue) && !audioSource.isPlaying)
        {
            IsPlaying(false);
            isPaused = false;
        }
    }
}