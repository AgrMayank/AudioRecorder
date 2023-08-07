using UnityEngine;
using UnityEngine.UI;

namespace Mayank.AudioRecorder.Player
{
    
    // ToDo: Add comments
    // ToDo: Load last audio file.
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private Slider audioSlider;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Image playImage;
        [SerializeField] private Image pauseImage;

        [HideInInspector] public AudioClip audioClip;
        
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
            audioSlider.value = _audioSource.time;
            if ((audioSlider.value <= audioSlider.maxValue 
                 && audioSlider.value >= audioSlider.minValue) ||
                audioSource.isPlaying) return;
            IsPlaying(false);
            _isPaused = false;
        }
    }
}