using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Recorder
{
    /// <summary>
    /// Add this component to a GameObject to Record Mic Input 
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioRecordHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region Static Variables
        /// <summary>
        /// Audio Source to store Microphone Input, An AudioSource Component is required by default
        /// </summary>
        private static AudioSource audioSource;

        /// <summary>
        /// The samples are floats ranging from -1.0f to 1.0f, representing the data in the audio clip
        /// </summary>
        private static float[] samplesData;
        #endregion

        
        #region Editor Exposed Variables
        /// <summary>
        /// Set a keyboard key for saving the Audio File
        /// </summary>
        [Tooltip("Set a keyboard key for saving the Audio File")]
        public KeyCode keyCode;

        /// <summary>
        /// Set max duration of the audio file in seconds
        /// </summary>
        [Tooltip("Set max duration of the audio file in seconds")]
        public int timeToRecord = 30;

        /// <summary>
        /// Hold Button to Record
        /// </summary>
        [Tooltip("Press and Hold Record button to Record")]
        public bool holdToRecord = false;

        /// <summary>
        /// The component that Handles recorder UI
        /// </summary>
        [Tooltip("AudioRecorderView component for recorder")]
        [SerializeField] private View recorderView;
        #endregion


        #region MonoBehaviour Callbacks
        private void Start()
        {
            AuthorizeMicrophone();
            audioSource = GetComponent<AudioSource>();
        }
        
        private void Update()
        {
            CheckRecordKey();
            if (!AudioRecorder.IsRecording) return;
            AudioRecorder.UpdateRecordingTime();
            CheckRecordingTime();
        }
        #endregion MonoBehaviour Callbacks

        
        #region Event Functions
        public void OnPointerClick(PointerEventData eventData)
        {
            if (holdToRecord) return;
            if (AudioRecorder.IsRecording) StartCoroutine(StopRecording());
            else StartRecording();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (holdToRecord) StartRecording();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (holdToRecord) StartCoroutine(StopRecording());
        }
        #endregion Event Functions

        
        #region Recorder Functions
        private static void AuthorizeMicrophone()
        {
            // Check iOS Microphone permission
            if (Application.HasUserAuthorization(UserAuthorization.Microphone)) Debug.Log("Microphone found");
            else
            {
                Debug.Log("Microphone not found");
                // Request iOS Microphone permission
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
        }
        
        private void CheckRecordingTime()
        {
            if (AudioRecorder.RecordingTime >= timeToRecord) StartCoroutine(StopRecording());
        }

        private void CheckRecordKey()
        {
            if (holdToRecord) return;
            if (!Input.GetKeyDown(keyCode)) return;
            if (AudioRecorder.IsRecording) StartCoroutine(StopRecording());
            else StartRecording();
        }
        
        private void StartRecording()
        {
            if (!AudioRecorder.MicrophoneIsAvailable()) return;
            AudioRecorder.StartRecording(audioSource, timeToRecord);
            recorderView.OnStartRecording();
        }

        private IEnumerator StopRecording(string fileName = "Audio")
        {
            if (!AudioRecorder.IsRecording) yield break;
            recorderView.OnStopRecording();
            FileWritingResultModel writingResult = null; 

            yield return new WaitUntil(() =>
            {
                writingResult = AudioRecorder.SaveRecording(audioSource, fileName);
                return writingResult != null;
            });

            recorderView.OnRecordingSaved(writingResult.status
                ? $"Audio saved at {writingResult.result}"
                : $"Something went wrong while saving audio file \n {writingResult.result}");
        }
        #endregion Recorder Functions
    }
}