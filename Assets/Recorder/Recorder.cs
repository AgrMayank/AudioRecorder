using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        /// Audio Source to store Microphone Input, An AudioSource Component is required by default
        /// </summary>
        static AudioSource audioSource;
        /// <summary>
        /// The samples are floats ranging from -1.0f to 1.0f, representing the data in the audio clip
        /// </summary>
        static float[] samplesData;
        /// <summary>
        /// WAV file header size
        /// </summary>
        const int HEADER_SIZE = 44;

        #endregion

        #region Private Variables

        /// <summary>
        /// Is Recording
        /// </summary>
        public static bool isRecording = false;
        /// <summary>
        /// Recording Time
        /// </summary>
        private float recordingTime = 0f;
        /// <summary>
        /// Recording Time Minute and Seconds
        /// </summary>
        private int minute = 0, second = 0;

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
        /// Show the Filepath on the screen, etc 
        /// </summary>
        public TMP_Text ConsoleText;
        /// <summary>
        /// Show the Recording Time on the screen
        /// </summary>
        public TMP_Text RecordingTimeText;
        /// <summary>
        /// Set a Button to trigger recording or saving the Audio WAV file 
        /// </summary>
        public Button RecordButton;
        /// <summary>
        /// Record or Save Image for the Record Button
        /// </summary>
        public Image RecordImage, SaveImage;
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

        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            AuthorizeMicrophone();

            // Get the AudioSource component
            audioSource = GetComponent<AudioSource>();

            isRecording = false;
            ConsoleText.text = "";
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
            CheckRecordKey();
            CheckRecordingTime();
            SetTexts();
        }

        private void SetTexts()
        {
            if (isRecording)
            {
                ConsoleText.text = "";
                recordingTime += Time.deltaTime;

                minute = (int)(recordingTime / 60);
                second = (int)(recordingTime % 60);

                if (minute < 10)
                {
                    if (second < 10)
                    {
                        RecordingTimeText.text = "0" + minute + ":0" + second;
                    }
                    else
                    {
                        RecordingTimeText.text = "0" + minute + ":" + second;
                    }
                }
                else if (second < 10)
                {
                    RecordingTimeText.text = minute + ":0" + second;
                }
                else
                {
                    RecordingTimeText.text = minute + ":" + second;
                }

                RecordImage.gameObject.SetActive(false);
                SaveImage.gameObject.SetActive(true);
            }
            else
            {
                RecordingTimeText.text = "00:00";

                RecordImage.gameObject.SetActive(true);
                SaveImage.gameObject.SetActive(false);
            }
        }

        private void CheckRecordingTime()
        {
            if (recordingTime >= timeToRecord) SaveRecording();
        }

        private void CheckRecordKey()
        {
            if (holdToRecord) return;
            if (!Input.GetKeyDown(keyCode)) return;
            if (isRecording) SaveRecording();
            else StartRecording();
        }

        #endregion

        #region Other Functions

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (holdToRecord) return;
            if (isRecording) SaveRecording();
            else StartRecording();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (holdToRecord) StartRecording();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (holdToRecord) SaveRecording();
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
        }

        public void SaveRecording(string fileName = "Audio")
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
                    // WriteWAVFile(audioClip, filePath);
                    FileWriter.WriteWavFile(audioClip, filePath, HEADER_SIZE);
                    ConsoleText.text = "Audio Saved at: " + filePath;
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