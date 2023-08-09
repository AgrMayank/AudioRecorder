using System.IO;
using Mayank.AudioRecorder.Utility;
using UnityEngine;
using UnityEngine.UI;
// using AudioRecorder = Mayank.AudioRecorder.Recorder.Core.AudioRecorder;

namespace Mayank.AudioRecorder.Player
{
    /// <summary>
    /// This class controls the audio playback and slider UI.
    /// </summary>
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
        /// Indicates if the audio playback can be resumed.
        /// </summary>
        [SerializeField] private Image playImage;
        
        /// <summary>
        /// Indicates if the audio playback can be paused.
        /// </summary>
        [SerializeField] private Image pauseImage;
        
        /// <summary>
        /// The audio file that will be played.
        /// </summary>
        [HideInInspector] public AudioClip audioClip;
        
        /// <summary>
        /// Tracks the playback state.
        /// </summary>
        private bool _isPaused;
        
        
        #region MonoCallbacks
        
        /// <summary>
        /// Initializes the audio source and UI elements.
        /// </summary>
        private void Start()
        {
            _isPaused = false;
            SetPlayPauseImagesStatus(false);
        }

        /// <summary>
        /// Updates the audio playback and UI elements based on the current state.
        /// </summary>
        private void Update()
        {
            if (audioSource.isPlaying) SetAudioSliderValue();
            if ((audioSlider.value <= audioSlider.maxValue && audioSlider.value >= audioSlider.minValue) || audioSource.isPlaying) return;
            SetPlayPauseImagesStatus(false);
            _isPaused = false;
        }
        
        #endregion MonoCallbacks

        /// <summary>
        /// Sets the value of the audio slider based on the current playback time.
        /// </summary>
        private void SetAudioSliderValue()
        {
            audioSlider.value = audioSource.time;
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
        /// Changes the audio clip of the AudioSource and adjusts the audio slider parameters accordingly.
        /// </summary>
        public void UpdateClip()
        {
            audioSource.clip = audioClip;
            audioSlider.direction = Slider.Direction.LeftToRight;
            audioSlider.minValue = 0;
            audioSlider.maxValue = audioSource.clip.length;
        }

        /// <summary>
        /// Toggles the audio playback between play and pause based on the current state.
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







        // public void SetAudioFile()
        public async void SetAudioFile()
        {
            Debug.Log("public void SetAudioFile()            public void SetAudioFile()");
            
            var audioClip = await FileReader.LoadWavFileAsAudioClip(Path.Combine(Recorder.Core.AudioRecorder.saveDirectoryPath, Recorder.Core.AudioRecorder.saveFileName + ".wav"));
            // var audioClip = await FileReader.LoadAudioClip(Path.Combine(Recorder.Core.AudioRecorder.saveDirectoryPath, Recorder.Core.AudioRecorder.saveFileName + ".wav"));
            // var audioClip = FileReader.LoadAudioClip(Path.Combine(Recorder.Core.AudioRecorder.saveDirectoryPath, Recorder.Core.AudioRecorder.saveFileName + ".wav"));
            // var audioClip = FileReader.LoadAudioClip(Path.Combine(Recorder.Core.AudioRecorder.saveDirectoryPath, Recorder.Core.AudioRecorder.saveFileName));
            // FileReader.LoadAudioClip(Path.Combine(AudioRecorder.saveDirectoryPath, Recorder.Core.AudioRecorder.saveFileName));

            Debug.Log("audioClip is loaded . . . ");

            audioSource.clip = audioClip;

        }
    }
}