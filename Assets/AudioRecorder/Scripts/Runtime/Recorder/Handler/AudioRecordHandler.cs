using System;
using System.Collections;
using Cysharp.Threading.Tasks;
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
        /// Set max duration of the audio file in seconds.
        /// </summary>
        [Tooltip("Set max duration of the audio file in seconds")]
        [SerializeField] private int _timeToRecord = 30;

        /// <summary>
        /// Determines whether the record button needs to be held down to start recording.
        /// </summary>
        [Tooltip("Enable this option to require holding down the record button to start recording.")]
        [SerializeField] private bool _holdToRecord = false;

        /// <summary>
        /// The component responsible for handling the recorder UI.
        /// </summary>
        [Tooltip("The RecorderView component used for the recorder.")]
        [SerializeField] private View.RecorderView _recorderRecorderView;

        #endregion


        #region MonoBehaviour Callbacks

        /// <summary>
        /// Authorizes the use of the microphone.
        /// </summary>
        private void Start()
        {
            AuthorizeMicrophone();
        }

        /// <summary>
        /// Checks whether to start or stop recording based on user input and recording status.
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
        /// Handles user input when the record button is clicked.
        /// </summary>
        /// <param name="eventData">Unused.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_holdToRecord) return;
            // if (Core.AudioRecorder.IsRecording) StartCoroutine(StopRecording());
            if (Core.AudioRecorder.IsRecording) StopRecording();
            else StartRecording();
        }

        /// <summary>
        /// Starts recording if the hold-to-record option is enabled.
        /// </summary>
        /// <param name="eventData">Unused.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_holdToRecord) StartRecording();
        }

        /// <summary>
        /// Stops recording if the hold-to-record option is enabled.
        /// </summary>
        /// <param name="eventData">Unused.</param>
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            // if (_holdToRecord) StartCoroutine(StopRecording());
            if (_holdToRecord) StopRecording();
        }

        #endregion Event Functions


        #region Recorder Functions

        /// <summary>
        /// Requests user authorization to use the microphone if it hasn't been granted already.
        /// </summary>
        private static void AuthorizeMicrophone()
        {
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }

        /// <summary>
        /// Stops recording if the recording time exceeds the maximum duration.
        /// </summary>
        private void CheckRecordingTime()
        {
            // if (Core.AudioRecorder.RecordingTime >= _timeToRecord) StartCoroutine(StopRecording());
            if (Core.AudioRecorder.RecordingTime >= _timeToRecord) StopRecording();
        }

        /// <summary>
        /// Starts or stops recording based on user input and recording status.
        /// </summary>
        private void CheckRecordKey()
        {
            if (_holdToRecord) return;
            if (!Input.GetKeyDown(_keyCode)) return;
            // if (Core.AudioRecorder.IsRecording) StartCoroutine(StopRecording());
            if (Core.AudioRecorder.IsRecording) StopRecording();
            else StartRecording();
        }

        /// <summary>
        /// Starts recording audio from the microphone if it's available.
        /// </summary>
        private void StartRecording()
        {
            if (!Core.AudioRecorder.MicrophoneIsAvailable()) return;
            Core.AudioRecorder.StartRecording(_audioSource, _timeToRecord);
            _recorderRecorderView.OnStartRecording();
        }

        /// <summary>
        /// Stops recording audio and saves the recorded audio file to disk.
        /// </summary>
        /// <param name="fileName">The intended name of the recorded audio file.</param>
        /// <returns>An IEnumerator object.</returns>
        // private IEnumerator StopRecording(string fileName = "Audio")
        // private async UniTask StopRecording(string fileName = "Audio")
        private async UniTask StopRecording(string fileName = "Audio")
        {
            // if (!Core.AudioRecorder.IsRecording) yield break;
            if (!Core.AudioRecorder.IsRecording) return;
            _recorderRecorderView.OnStopRecording();
            FileWritingResultModel writingResult = null;
            fileName = fileName + " " + DateTime.UtcNow.ToString("yyyy_MM_dd HH_mm_ss_ffff");
            
            writingResult = Core.AudioRecorder.SaveRecording(_audioSource, fileName);
            // return writingResult != null;


            await UniTask.WaitUntil(() => writingResult != null);
            
            // yield return new WaitUntil(() =>
            // {
            //     writingResult = Core.AudioRecorder.SaveRecording(_audioSource, fileName);
            //     return writingResult != null;
            // });

            _recorderRecorderView.OnRecordingSaved(writingResult.status
                ? $"Audio saved at {writingResult.result}"
                : $"Something went wrong while saving audio file \n {writingResult.result}");
        }

        #endregion Recorder Functions
    }
}