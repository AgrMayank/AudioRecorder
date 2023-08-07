using UnityEngine;
using UnityEngine.UI;

namespace Mayank.AudioRecorder.Player
{
    
    // ToDo: Add comments
    // ToDo: Load last audio file.
    public class AudioPlayer : MonoBehaviour
    {
        /// <summary>
        /// The slider which represents the passed time of the audio file as well as whole time and remaining time of the audio file.
        /// </summary>
        [SerializeField] private Slider audioSlider;
        
        /// <summary>
        /// The AudioSource component which controls audio file  playback.
        /// </summary>
        [SerializeField] private AudioSource audioSource;
        
        /// <summary>
        /// The image which shown when the audio is paused. It represents that the audio will play if user press the button.
        /// </summary>
        [SerializeField] private Image playImage;
        
        /// <summary>
        /// The image which shown when the audio is playing. It represents that the audi will pause if user press the button.
        /// </summary>
        [SerializeField] private Image pauseImage;
        
        /// <summary>
        /// The AudioClip that will play in the AudiSource component.
        /// </summary>
        [HideInInspector] public AudioClip audioClip;
        
        /// <summary>
        /// Represents if the audio is playing or not.
        /// </summary>
        private bool _isPaused = false;
        
        
        // ToDo: Check it: what is the following variable doing? Why does it exists?
        private AudioSource _audioSource;

        #region MonoCallbacks
        
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _isPaused = false;
            SetPlayPauseImagesStatus(false);
        }

        private void Update()
        {
            // audioSlider.value = _audioSource.time;

            if (audioSource.isPlaying) SetAudioSliderValue();
            
            
            
            
            if ((audioSlider.value <= audioSlider.maxValue 
                 && audioSlider.value >= audioSlider.minValue) ||
                audioSource.isPlaying) return;
            SetPlayPauseImagesStatus(false);
            _isPaused = false;
        }
        
        #endregion MonoCallbacks

        /// <summary>
        /// Sets the value of the audio slider.
        /// </summary>
        private void SetAudioSliderValue()
        {
            audioSlider.value = _audioSource.time;
        }

        /// <summary>
        /// Sets the play and pause images active or inactive.
        /// </summary>
        /// <param name="isPlaying"></param>
        // private void IsPlaying(bool isPlaying)
        private void SetPlayPauseImagesStatus(bool isPlaying)
        {
            playImage.gameObject.SetActive(!isPlaying);
            pauseImage.gameObject.SetActive(isPlaying);
        }

        /// <summary>
        /// Changes the AudioClip of the AudioSource.
        /// </summary>
        public void UpdateClip()
        {
            audioSource.clip = audioClip;
            audioSlider.direction = Slider.Direction.LeftToRight;
            audioSlider.minValue = 0;
            audioSlider.maxValue = audioSource.clip.length;
        }

        /// <summary>
        /// Plays or pauses the audio.
        /// </summary>
        public void PlayPauseAudio()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                SetPlayPauseImagesStatus(false);
                _isPaused = true;
            }
            else if (_isPaused)
            {
                SetPlayPauseImagesStatus(true);
                audioSource.UnPause();
            }
            else
            {
                SetPlayPauseImagesStatus(true);
                audioSource.Play();
            }
        }

        
    }
}