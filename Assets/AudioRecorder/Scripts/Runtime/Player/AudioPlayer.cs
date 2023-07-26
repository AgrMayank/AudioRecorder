using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mayank.AudioRecorder.Player
{
    public class AudioPlayer : MonoBehaviour
    {
        // public Slider audioSlider;
        [SerializeField] private Slider audioSlider;
        // public AudioSource audioSource;
        [SerializeField] private AudioSource audioSource;

        [HideInInspector]
        public AudioClip audioClip;

        // public Image PlayImage, PauseImage;
        // public Image PlayImage;
        // [FormerlySerializedAs("PlayImage")] public Image playImage;
        // [FormerlySerializedAs("PlayImage")] public Image playImage;
        [SerializeField] private Image playImage;
        // public Image PauseImage;
        // [FormerlySerializedAs("PauseImage")] public Image pauseImage;
        [SerializeField] private Image pauseImage;

        private bool _isPaused = false;

        private AudioSource _audioSource;

        private void Start()
        {
            
            _audioSource = GetComponent<AudioSource>();
            _isPaused = false;

            IsPlaying(false);
        }

        private void IsPlaying(bool isPlaying)
        {
            playImage.gameObject.SetActive(!isPlaying);
            pauseImage.gameObject.SetActive(isPlaying);
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
                _isPaused = true;
            }
            else if (_isPaused)
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
            // if (audioClip == null)
            // {
            //     ConsoleText.text = "No Audio Clip Found!\nRecord Something First.";
            // }

            // audioSlider.value = GetComponent<AudioSource>().time;
            audioSlider.value = _audioSource.time;

            // if ((audioSlider.value == audioSlider.maxValue || audioSlider.value == audioSlider.minValue) && !audioSource.isPlaying)
            if ((audioSlider.value <= audioSlider.maxValue 
                 && audioSlider.value >= audioSlider.minValue) ||
                audioSource.isPlaying) return;
            IsPlaying(false);
            _isPaused = false;
        }
    }
}