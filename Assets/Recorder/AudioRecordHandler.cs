using System;
using System.Collections;
using System.IO;
using System.Linq;
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
        /// <summary>
        /// A flag that represents if the recorder is recording a voice or not.
        /// </summary>
        public bool isRecording { get; private set; }


        /// <summary>
        /// Recording Time
        /// </summary>
        public float recordingTime { get; private set; }


        #region Constants &  Static Variables

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
            if (isRecording)
            {
                AudioRecorder.UpdateRecordingTime();
                recordingTime += Time.deltaTime;
                CheckRecordingTime();
            }
            else CheckRecordKey();
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
            Debug.Log("void IPointerUpHandler.OnPointerUp(PointerEventData eventData)");
            
            // if (holdToRecord) StopRecording();
            // if (holdToRecord) StartCoroutine(nameof(StopRecording));
            if (holdToRecord) StartCoroutine(StopRecording());
        }

        #endregion

        #region Recorder Functions

        public void StartRecording()
        {
            // recordingTime = 0f;
            // isRecording = true;
            // Microphone.End(Microphone.devices[0]);
            // audioSource.clip = Microphone.Start(Microphone.devices[0], false, timeToRecord, 44100);

            
            
            isRecording = true;
            
            AudioRecorder.StartRecording(audioSource, timeToRecord);
            
            _recorderView.OnStartRecording();
        }

        // public void StopRecording(string fileName = "Audio")
        public IEnumerator StopRecording(string fileName = "Audio")
        {

            Debug.Log("public IEnumerator StopRecording(string fileName = ");
            
            isRecording = false;
            
            
            
            _recorderView.OnStopRecording();
            // SaveRecording();
            
            // AudioRecorder.StopRecording(fileName);

            var filePath = "";
            
            // AudioRecorder.StopRecording(audioSource, fileName);
            
            // yield return WaitUntil(() => AudioRecorder.StopRecording(audioSource, fileName) != "")
            // yield return new WaitUntil(() => AudioRecorder.SaveRecording(audioSource, fileName) != "");
            yield return new WaitUntil(() =>
            {
                filePath = AudioRecorder.SaveRecording(audioSource, fileName);
                // return AudioRecorder.SaveRecording(audioSource, fileName) != "";
                // return filePath != "";
                return !string.IsNullOrEmpty(filePath);
            });
            
            

            // yield return WaitUntil(() => string.IsNullOrEmpty(filePath));
            
            
            _recorderView.OnRecordingSaved($"Audio saved at {filePath}");
            
        }

        
        
        // private void SaveRecording(string fileName = "Audio")
        // {
        //     // while (!(Microphone.GetPosition(null) > 0))
        //     // {
        //     // }
        //     //
        //     // samplesData = new float[audioSource.clip.samples * audioSource.clip.channels];
        //     // audioSource.clip.GetData(samplesData, 0);
        //     //
        //     // // Trim the silence at the end of the recording
        //     // var samples = samplesData.ToList();
        //     // int recordedSamples = (int)(samplesData.Length * (recordingTime / (float)timeToRecord));
        //     //
        //     // if (recordedSamples < samplesData.Length - 1)
        //     // {
        //     //     samples.RemoveRange(recordedSamples, samplesData.Length - recordedSamples);
        //     //     samplesData = samples.ToArray();
        //     // }
        //     //
        //     // // Create the audio file after removing the silence
        //     // AudioClip audioClip =
        //     //     AudioClip.Create(fileName, samplesData.Length, audioSource.clip.channels, 44100, false);
        //     // audioClip.SetData(samplesData, 0);
        //
        //     
        //     
        //     
        //     
        //     
        //     //
        //     // // Assign Current Audio Clip to Audio Player
        //     // // audioPlayer.audioClip = audioClip;
        //     // audioPlayer.audioClip = audioClip;
        //     // audioPlayer.UpdateClip();
        //
        //     
        //     
        //     
        //     
        //     // string filePath = Path.Combine(Application.persistentDataPath,
        //     //     fileName + " " + DateTime.UtcNow.ToString("yyyy_MM_dd HH_mm_ss_ffff") + ".wav");
        //     //
        //     // // Delete the file if it exists.
        //     // if (File.Exists(filePath))
        //     // {
        //     //     File.Delete(filePath);
        //     // }
        //     //
        //     // try
        //     // {
        //     //     FileWriter.WriteWavFile(audioClip, filePath, HEADER_SIZE);
        //     //     _recorderView.OnRecordingSaved($"Audio saved at {filePath}");
        //     //     Debug.Log("File Saved Successfully at " + filePath);
        //     // }
        //     // catch (DirectoryNotFoundException)
        //     // {
        //     //     Debug.LogError("Persistent Data Path not found!");
        //     // }
        //     //
        //     // isRecording = false;
        //     // Microphone.End(Microphone.devices[0]);
        // }

        #endregion
    }
}