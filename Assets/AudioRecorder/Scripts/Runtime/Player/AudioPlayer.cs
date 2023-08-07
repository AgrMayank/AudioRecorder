using UnityEngine;
using UnityEngine.UI;

namespace Mayank.AudioRecorder.Player
{
    
    // ToDo: Load last audio file.
    
    // This class controls the audio playback and slider UI.
    public class AudioPlayer : MonoBehaviour
    {
        /// <summary>
        /// Displays the current playback position and allows seeking.
        /// </summary>
        [SerializeField] private Slider audioSlider;
        
        /// <summary>
        /// Plays back the assigned audio clip.
        /// </summary>
        [SerializeField] private AudioSource audioSource;
        
        /// <summary>
        /// This image provides a visual indication to the user that the audio playback can be resumed by pressing the button associated with it.
        /// </summary>
        [SerializeField] private Image playImage;
        
        /// <summary>
        /// This image provides a visual indication to the user that the audio playback can be paused by pressing the button associated with it.
        /// </summary>
        [SerializeField] private Image pauseImage;
        
        /// <summary>
        /// The audio file that will be played.
        /// </summary>
        [HideInInspector] public AudioClip audioClip;
        
        /// <summary>
        /// Tracks the playback state.
        /// </summary>
        private bool _isPaused = false;
        
        
        // ToDo: Check it: what is the following variable doing? Why does it exists?
        private AudioSource _audioSource;

        #region MonoCallbacks
        
        /// <summary>
        /// Initializes the audio source and UI elements.
        /// </summary>
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _isPaused = false;
            SetPlayPauseImagesStatus(false);
        }

        /// <summary>
        /// Updates the audio playback and UI elements based on the current state.
        /// </summary>
        private void Update()
        {
            // If the audio is currently playing, update the audio slider value.
            if (audioSource.isPlaying) SetAudioSliderValue();
            
            // If the audio slider value is within the valid range or the audio is playing, do nothing.
            if ((audioSlider.value <= audioSlider.maxValue && audioSlider.value >= audioSlider.minValue) || audioSource.isPlaying) return;
            
            // If the audio is not playing, set the play/pause images to the inactive state and reset the paused state.
            SetPlayPauseImagesStatus(false);
            _isPaused = false;
        }
        
        #endregion MonoCallbacks

        /// <summary>
        /// Sets the value of the audio slider based on the current playback time.
        /// </summary>
        private void SetAudioSliderValue()
        {
            audioSlider.value = _audioSource.time;
        }

        /// <summary>
        /// Updates the play and pause images to reflect the current playback state.
        /// </summary>
        /// <param name="isPlaying">Indicates if the audio is currently playing or not.</param>
        private void SetPlayPauseImagesStatus(bool isPlaying)
        {
            playImage.gameObject.SetActive(!isPlaying);
            pauseImage.gameObject.SetActive(isPlaying);
        }

        /// <summary>
        /// Changes the AudioClip of the AudioSource and adjusts the audio slider parameters accordingly.
        /// </summary>
        public void UpdateClip()
        {
            // Set the AudioClip of the AudioSource component to the provided audioClip.
            audioSource.clip = audioClip;
            // Adjust the audio slider parameters to match the length of the new audio clip.
            audioSlider.direction = Slider.Direction.LeftToRight;
            audioSlider.minValue = 0;
            audioSlider.maxValue = audioSource.clip.length;
        }

        /// <summary>
        /// Toggles the audio playback between play and pause based on the current state.
        /// </summary>
        public void PlayPauseAudio()
        {
            // If the audio is currently playing, pause the audio playback and set the play/pause images to the inactive state.
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                SetPlayPauseImagesStatus(false);
                _isPaused = true;
            }
            // If the audio is currently paused, resume the audio playback and set the play/pause images to the active state.
            else if (_isPaused)
            {
                SetPlayPauseImagesStatus(true);
                audioSource.UnPause();
            }
            // If the audio is not playing, start the audio playback and set the play/pause images to the active state.
            else
            {
                SetPlayPauseImagesStatus(true);
                audioSource.Play();
            }
        }
    }
}