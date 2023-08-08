using System.Collections;
using Mayank.AudioRecorder.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Mayank.AudioRecorder.Recorder.Handler
{
    /// <summary>
    /// Add this component to a GameObject to Record Mic Input 
    /// </summary>
    public class AudioRecordHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// An array of floating-point values ranging from -1.0f to 1.0f, representing the audio data in the clip.
        /// </summary>
        private static float[] samplesData;
        
        // #region Static Variables
        //
        // /// <summary>
        // /// An array of floating-point values ranging from -1.0f to 1.0f, representing the audio data in the clip.
        // /// </summary>
        // private static float[] samplesData;
        //
        // #endregion


        #region Editor Exposed Variables

        /// <summary>
        /// Audio Source to store Microphone Input.
        /// </summary>
        [Tooltip("Set a AudioSource component to store the microphone input.")]
        [SerializeField] private AudioSource _audioSource;
        
        /// <summary>
        /// The keyboard key used to start and stop recording the audio file.
        /// </summary>
        [Tooltip("Set a keyboard key to start and stop recording the audio file.")]
        [SerializeField] private KeyCode _keyCode;
        
        
        
        
        
        
        
        
        

        /// <summary>
        /// Set max duration of the audio file in seconds
        /// </summary>
        [Tooltip("Set max duration of the audio file in seconds")]
        [SerializeField] private int _timeToRecord = 30;
        // public int timeToRecord = 30;

        /// <summary>
        /// Hold Button to Record
        /// </summary>
        [Tooltip("Press and Hold Record button to Record")]
        [SerializeField] private bool _holdToRecord = false;
        // public bool holdToRecord = false;

        /// <summary>
        /// The component that Handles recorder UI
        /// </summary>
        [Tooltip("AudioRecorderView component for recorder")] [SerializeField]
        private View.RecorderView _recorderRecorderView;

        #endregion


        #region MonoBehaviour Callbacks

        /// <summary>
        /// Authorizes the microphone and 
        /// </summary>
        private void Start()
        {
            AuthorizeMicrophone();
            // _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Checks any thing to start or stop recording.
        /// </summary>
        private void Update()
        {
            CheckRecordKey();
            if (!Core.AudioRecorder.IsRecording) return;
            Core.AudioRecorder.UpdateRecordingTime();
            CheckRecordingTime();
        }

        #endregion MonoBehaviour Callbacks


        #region Event Functions

        /// <summary>
        /// This method is called when the user clicks on the record button. If the hold-to-record option is enabled, it does nothing.
        /// If the audio recorder is currently recording, it stops the recording. Otherwise, it starts the recording.
        /// </summary>
        /// <param name="eventData">The data of the event. It's useless in this function.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_holdToRecord) return;
            if (Core.AudioRecorder.IsRecording) StartCoroutine(StopRecording());
            else StartRecording();
        }

        /// <summary>
        /// It starts recording if _holdToRecord is true.
        /// </summary>
        /// <param name="eventData">The data of the event. It's useless in this function.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_holdToRecord) StartRecording();
        }

        /// <summary>
        /// It stops recording, if _hodToRecord is true.
        /// </summary>
        /// <param name="eventData">The data of the event. It's useless in this function.</param>
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (_holdToRecord) StartCoroutine(StopRecording());
        }

        #endregion Event Functions


        #region Recorder Functions

        /// <summary>
        /// Authorizes the microphone.
        /// </summary>
        private static void AuthorizeMicrophone()
        {
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }

        /// <summary>
        /// Stops recording if its duration be bigger than the maximum time of the recording.
        /// </summary>
        private void CheckRecordingTime()
        {
            if (Core.AudioRecorder.RecordingTime >= _timeToRecord) StartCoroutine(StopRecording());
        }

        /// <summary>
        /// Starts or stop recording if _holdToRecord is false and the keyCode be gotten.
        /// </summary>
        private void CheckRecordKey()
        {
            if (_holdToRecord) return;
            if (!Input.GetKeyDown(_keyCode)) return;
            if (Core.AudioRecorder.IsRecording) StartCoroutine(StopRecording());
            else StartRecording();
        }

        /// <summary>
        /// Starts recording.
        /// </summary>
        private void StartRecording()
        {
            if (!Core.AudioRecorder.MicrophoneIsAvailable()) return;
            Core.AudioRecorder.StartRecording(_audioSource, _timeToRecord);
            _recorderRecorderView.OnStartRecording();
        }

        /// <summary>
        /// Stops recording.
        /// </summary>
        /// <param name="fileName">The intended name of the recorded audio file.</param>
        /// <returns>IEnumerator</returns>
        private IEnumerator StopRecording(string fileName = "Audio")
        {
            if (!Core.AudioRecorder.IsRecording) yield break;
            _recorderRecorderView.OnStopRecording();
            FileWritingResultModel writingResult = null;

            yield return new WaitUntil(() =>
            {
                writingResult = Core.AudioRecorder.SaveRecording(_audioSource, fileName);
                return writingResult != null;
            });

            _recorderRecorderView.OnRecordingSaved(writingResult.status
                ? $"Audio saved at {writingResult.result}"
                : $"Something went wrong while saving audio file \n {writingResult.result}");
        }

        #endregion Recorder Functions
    }
}