using UnityEngine;
using UnityEngine.UI;

namespace Mayank.AudioRecorder.Player
{
    
    // ToDo: Load last audio file.
    
    // This class controls the audio playback and slider UI.
    public class AudioPlayer : MonoBehaviour
    {
        /// <summary>
        /// This slider is used to display the current playback position of the audio file and allows the user to seek to a specific position in the audio file.
        /// </summary>
        [SerializeField] private Slider audioSlider;
        
        /// <summary>
        /// This component is responsible for playing back the audio clip assigned to it and provides various options for modifying the audio playback, such as volume, pitch, and spatialization.
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
        /// This AudioClip represents the audio file that will be played back by the AudioSource component. It contains the actual sound data that will be heard during audio playback.
        /// </summary>
        [HideInInspector] public AudioClip audioClip;
        
        /// <summary>
        /// This boolean variable is used to track the playback state of the audio. When set to true, it means the audio is currently paused, and when set to false, it means the audio is playing.
        /// </summary>
        private bool _isPaused = false;
        
        
        // ToDo: Check it: what is the following variable doing? Why does it exists?
        private AudioSource _audioSource;

        #region MonoCallbacks
        
        /// <summary>
        /// Initializes the audio source and UI elements when the script starts.
        /// This method retrieves the AudioSource component attached to the same GameObject and initializes the necessary variables and UI elements for audio playback.
        /// </summary>
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _isPaused = false;
            SetPlayPauseImagesStatus(false);
        }

        /// <summary>
        /// Updates the audio playback and UI elements based on the current state.
        /// This method is called every frame and handles the logic for updating the audio playback and UI elements, such as the audio slider and play/pause images.
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
        /// Sets the value of the audio slider based on the current playback time of the audio source.
        /// This method updates the value of the audio slider to reflect the current playback time of the audio source.
        /// It retrieves the current playback time from the audio source and sets the value of the slider to match it.
        /// This allows the user to see the current playback position of the audio file and seek to a specific position if needed.
        /// </summary>
        private void SetAudioSliderValue()
        {
            audioSlider.value = _audioSource.time;
        }

        /// <summary>
        /// Updates the play and pause images to reflect the current playback state.
        /// This method sets the play and pause images to be active or inactive based on the provided playing status. If the audio is currently playing, the pause image is set to active, and the play image is set to inactive. If the audio is currently paused, the play image is set to active, and the pause image is set to inactive. This provides a visual indication to the user of the current playback state and allows them to control the audio playback using the associated buttons.
        /// </summary>
        /// <param name="isPlaying">Represents if the audio is playing or not.</param>
        private void SetPlayPauseImagesStatus(bool isPlaying)
        {
            playImage.gameObject.SetActive(!isPlaying);
            pauseImage.gameObject.SetActive(isPlaying);
        }

        /// <summary>
        /// Changes the AudioClip of the AudioSource and adjusts the audio slider parameters accordingly.
        /// This method updates the AudioClip of the AudioSource component to the provided audioClip and adjusts the audio slider parameters to match the length of the new audio clip. It sets the minimum value of the audio slider to 0 and the maximum value to the length of the new audio clip. Additionally, it sets the direction of the audio slider to left-to-right to match the audio playback direction.
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
        /// This method toggles the audio playback between play and pause based on the current state.
        /// If the audio is currently playing, it pauses the audio playback and sets the play/pause images to the inactive state.
        /// If the audio is currently paused, it resumes the audio playback and sets the play/pause images to the active state.
        /// If the audio is not playing, it starts the audio playback and sets the play/pause images to the active state.
        /// This provides a way for the user to control the audio playback using the associated buttons and provides a visual indication of the current playback state.
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