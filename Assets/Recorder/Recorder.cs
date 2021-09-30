using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Recorder
{
    /// <summary>
    /// Add this component to a GameObject to Record Mic Input 
    /// </summary>
    [RequireComponent(typeof(AudioSource), typeof(EventTrigger))]
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
            // Request iOS Microphone permission
            Application.RequestUserAuthorization(UserAuthorization.Microphone);

            // Check iOS Microphone permission
            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Debug.Log("Microphone found");
            }
            else
            {
                Debug.Log("Microphone not found");
            }

            // Get the AudioSource component
            audioSource = GetComponent<AudioSource>();

            isRecording = false;
            ConsoleText.text = "";

            if (RecordButton == null)
            {
                return;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(keyCode) && !holdToRecord)
            {
                if (isRecording)
                {
                    SaveRecording();
                }
                else
                {
                    StartRecording();
                }
            }

            if (recordingTime >= timeToRecord)
            {
                SaveRecording();
            }

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

        #endregion

        #region Other Functions

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!holdToRecord)
            {
                if (isRecording)
                {
                    SaveRecording();
                }
                else
                {
                    StartRecording();
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (holdToRecord)
                StartRecording();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (holdToRecord)
                SaveRecording();
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
                    WriteWAVFile(audioClip, filePath);
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

        public static byte[] ConvertWAVtoByteArray(string filePath)
        {
            //Open the stream and read it back.
            byte[] bytes = new byte[audioSource.clip.samples + HEADER_SIZE];
            using (FileStream fs = File.OpenRead(filePath))
            {
                fs.Read(bytes, 0, bytes.Length);
            }
            return bytes;
        }

        // WAV file format from http://soundfile.sapp.org/doc/WaveFormat/
        static void WriteWAVFile(AudioClip clip, string filePath)
        {
            float[] clipData = new float[clip.samples];

            //Create the file.
            using (Stream fs = File.Create(filePath))
            {
                int frequency = clip.frequency;
                int numOfChannels = clip.channels;
                int samples = clip.samples;
                fs.Seek(0, SeekOrigin.Begin);

                //Header

                // Chunk ID
                byte[] riff = Encoding.ASCII.GetBytes("RIFF");
                fs.Write(riff, 0, 4);

                // ChunkSize
                byte[] chunkSize = BitConverter.GetBytes((HEADER_SIZE + clipData.Length) - 8);
                fs.Write(chunkSize, 0, 4);

                // Format
                byte[] wave = Encoding.ASCII.GetBytes("WAVE");
                fs.Write(wave, 0, 4);

                // Subchunk1ID
                byte[] fmt = Encoding.ASCII.GetBytes("fmt ");
                fs.Write(fmt, 0, 4);

                // Subchunk1Size
                byte[] subChunk1 = BitConverter.GetBytes(16);
                fs.Write(subChunk1, 0, 4);

                // AudioFormat
                byte[] audioFormat = BitConverter.GetBytes(1);
                fs.Write(audioFormat, 0, 2);

                // NumChannels
                byte[] numChannels = BitConverter.GetBytes(numOfChannels);
                fs.Write(numChannels, 0, 2);

                // SampleRate
                byte[] sampleRate = BitConverter.GetBytes(frequency);
                fs.Write(sampleRate, 0, 4);

                // ByteRate
                byte[] byteRate = BitConverter.GetBytes(frequency * numOfChannels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
                fs.Write(byteRate, 0, 4);

                // BlockAlign
                ushort blockAlign = (ushort)(numOfChannels * 2);
                fs.Write(BitConverter.GetBytes(blockAlign), 0, 2);

                // BitsPerSample
                ushort bps = 16;
                byte[] bitsPerSample = BitConverter.GetBytes(bps);
                fs.Write(bitsPerSample, 0, 2);

                // Subchunk2ID
                byte[] datastring = Encoding.ASCII.GetBytes("data");
                fs.Write(datastring, 0, 4);

                // Subchunk2Size
                byte[] subChunk2 = BitConverter.GetBytes(samples * numOfChannels * 2);
                fs.Write(subChunk2, 0, 4);

                // Data

                clip.GetData(clipData, 0);
                short[] intData = new short[clipData.Length];
                byte[] bytesData = new byte[clipData.Length * 2];

                int convertionFactor = 32767;

                for (int i = 0; i < clipData.Length; i++)
                {
                    intData[i] = (short)(clipData[i] * convertionFactor);
                    byte[] byteArr = new byte[2];
                    byteArr = BitConverter.GetBytes(intData[i]);
                    byteArr.CopyTo(bytesData, i * 2);
                }

                fs.Write(bytesData, 0, bytesData.Length);
            }
        }

        #endregion
    }
}