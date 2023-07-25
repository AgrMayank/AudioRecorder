using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
    public class Recorder : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region Constants &  Static Variables

        /// <summary>
        /// Is Recording
        /// </summary>
        // public static bool isRecording = false;
        public bool isRecording = false;
        
        /// <summary>
        /// Audio Source to store Microphone Input, An AudioSource Component is required by default
        /// </summary>
        private static AudioSource audioSource;
        
        /// <summary>
        /// The samples are floats ranging from -1.0f to 1.0f, representing the data in the audio clip
        /// </summary>
        private static float[] samplesData;
        
        /// <summary>
        /// WAV file header size
        /// </summary>
        private const int HEADER_SIZE = 44;

        #endregion

        #region Private Variables
        /// <summary>
        /// Recording Time
        /// </summary>
        // private float recordingTime = 0f;
        public float recordingTime = 0f;
        
        /// <summary>
        /// Recording Time Minute and Seconds
        /// </summary>
        // private int minute = 0, second = 0;
        public int minute = 0, second = 0;

        #endregion

        #region Editor Exposed Variables

        /// <summary>
        /// Set a keyboard key for saving the Audio File
        /// </summary>
        [Tooltip("Set a keyboard key for saving the Audio File")]
        public KeyCode keyCode;
        
        /// <summary>
        /// Audio Player Script for Playing Audio Files
        /// </summary>
        [Tooltip("Audio Player Script for Playing Audio Files")]
        public AudioPlayer audioPlayer;

        /// <summary>
        /// Set a Button to trigger recording or saving the Audio WAV file 
        /// </summary>
        public Button RecordButton;

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
        
        [SerializeField] private View _recorderView;

        #endregion
        
        
        
        #region MonoBehaviour Callbacks

        private void Start()
        {
            _recorderView.Init(this);
            AuthorizeMicrophone();

            // Get the AudioSource component
            audioSource = GetComponent<AudioSource>();
            isRecording = false;
        }

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

        private void Update()
        {
            recordingTime += Time.deltaTime;
            CheckRecordKey();
            CheckRecordingTime();
        }

        private void CheckRecordingTime()
        {
            if (recordingTime >= timeToRecord) StopRecording();
        }

        private void CheckRecordKey()
        {
            if (holdToRecord) return;
            if (!Input.GetKeyDown(keyCode)) return;
            if (isRecording) StopRecording();
            else StartRecording();
        }

        #endregion

        #region Other Functions

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (holdToRecord) return;
            if (isRecording) StopRecording();
            else StartRecording();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (holdToRecord) StartRecording();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (holdToRecord) StopRecording();
        }

        IEnumerator ScaleOverTime(GameObject button, float scaleFactor)
        {
            Vector3 originalScale = button.transform.localScale;
            Vector3 destinationScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            float currentTime = 0.0f;

            do
            {
                button.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / 0.5f);
                currentTime += Time.deltaTime;
                yield return null;
            }
            while (currentTime <= 1f);
        }

        #endregion

        #region Recorder Functions

        public void StartRecording()
        {
            recordingTime = 0f;
            isRecording = true;
            StartCoroutine(ScaleOverTime(RecordButton.gameObject, 1.2f));
            Microphone.End(Microphone.devices[0]);
            audioSource.clip = Microphone.Start(Microphone.devices[0], false, timeToRecord, 44100);
            _recorderView.OnStartRecording();
        }

        public void StopRecording(string fileName = "Audio")
        {
            _recorderView.OnStopRecording();
            SaveRecording();
        }

        private void SaveRecording(string fileName = "Audio")
        {
            if (isRecording)
            {
                StartCoroutine(ScaleOverTime(RecordButton.gameObject, 1f));

                while (!(Microphone.GetPosition(null) > 0)) { }
                samplesData = new float[audioSource.clip.samples * audioSource.clip.channels];
                audioSource.clip.GetData(samplesData, 0);

                // Trim the silence at the end of the recording
                var samples = samplesData.ToList();
                int recordedSamples = (int)(samplesData.Length * (recordingTime / (float)timeToRecord));

                if (recordedSamples < samplesData.Length - 1)
                {
                    samples.RemoveRange(recordedSamples, samplesData.Length - recordedSamples);
                    samplesData = samples.ToArray();
                }

                // Create the audio file after removing the silence
                AudioClip audioClip = AudioClip.Create(fileName, samplesData.Length, audioSource.clip.channels, 44100, false);
                audioClip.SetData(samplesData, 0);

                // Assign Current Audio Clip to Audio Player
                audioPlayer.audioClip = audioClip;
                audioPlayer.UpdateClip();

                string filePath = Path.Combine(Application.persistentDataPath, fileName + " " + DateTime.UtcNow.ToString("yyyy_MM_dd HH_mm_ss_ffff") + ".wav");

                // Delete the file if it exists.
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                try
                {
                    FileWriter.WriteWavFile(audioClip, filePath, HEADER_SIZE);
                    _recorderView.OnRecordingSaved($"Audio saved at {filePath}");
                    Debug.Log("File Saved Successfully at " + filePath);
                }
                catch (DirectoryNotFoundException)
                {
                    Debug.LogError("Persistent Data Path not found!");
                }

                isRecording = false;
                Microphone.End(Microphone.devices[0]);
            }
        }

        #endregion
    }
}