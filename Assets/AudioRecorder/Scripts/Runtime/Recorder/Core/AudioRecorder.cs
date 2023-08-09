using System;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mayank.AudioRecorder.Utility;
using UnityEngine;

namespace Mayank.AudioRecorder.Recorder.Core
{
    public static class AudioRecorder
    {
        /// <summary>
        /// The total duration of the current recording audio.
        /// </summary>
        public static float RecordingTime { get; private set; }
        
        /// <summary>
        /// Indicates whether the recorder is currently recording or not.
        /// </summary>
        public static bool IsRecording { get; private set; }

        /// <summary>
        /// The maximum allowed recording time, in seconds.
        /// </summary>
        private static int _timeToRecord;
        
        /// <summary>
        /// The name of the audio file; 
        /// </summary>
        public static string saveFileName { get; private set; }
        // private static string _fileName;
        
        
        
        
        
        /// <summary>
        /// The path of the directory which the audio file will be saved at.
        /// </summary>
        public static string saveDirectoryPath { get; private set; }
        // private static string _saveDirectoryPath;
        
        
        
        
        
        /// <summary>
        /// The size of the WAV file header, in bytes.
        /// </summary>
        private const int HeaderSize = 44;

        /// <summary>
        /// Updates the total recording time by adding the time elapsed since the last update.
        /// </summary>
        // ToDo: convert the following method to a asynchronous method
        public static void UpdateRecordingTime()
        {
            RecordingTime += Time.deltaTime;
        }
        
        /// <summary>
        /// Checks if any microphone is available.
        /// </summary>
        /// <returns>True if a microphone is available, false otherwise.</returns>
        public static bool MicrophoneIsAvailable()
        {
            if (Microphone.devices == null || Microphone.devices.Length == 0)
            {
                Debug.unityLogger.LogError("Microphone", "No microphone found.");
                return false;
            }
            
            Debug.unityLogger.Log(LogType.Log, "Microphone is available");
            return true;
        }

        /// <summary>
        /// Starts recording audio from the microphone and assigns it to the provided AudioSource.
        /// </summary>
        /// <param name="audioSource">The AudioSource to record the input audio on its AudioClip.</param>
        /// <param name="timeToRecord">The maximum allowed recording time, in seconds.</param>
        public static void StartRecording(AudioSource audioSource, int timeToRecord)
        {
            _timeToRecord = timeToRecord;
            RecordingTime = 0f;
            IsRecording = true;
            Microphone.End(Microphone.devices[0]);
            audioSource.clip = Microphone.Start(Microphone.devices[0], false, timeToRecord, 44100);
        }

        /// <summary>
        /// Saves the recorded audio to a WAV file.
        /// </summary>
        /// <param name="audioSource">The AudioSource containing the recorded audio.</param>
        /// <param name="fileName">The name of the audio file to save.</param>
        /// <returns>The result of writing the audio file.</returns>
        // public static FileWritingResultModel SaveRecording(AudioSource audioSource, string fileName = "Audio")
        // public static FileWritingResultModel SaveRecording(AudioSource audioSource, string directoryPath = "", string fileName = "Audio")
        // public static FileWritingResultModel SaveRecording(AudioSource audioSource, string directoryPath, string fileName = "Audio")
        public static async UniTask<FileWritingResultModel> SaveRecording(AudioSource audioSource, string directoryPath, string fileName = "Audio")
        {
            saveDirectoryPath = directoryPath;
            // sdfsdf
            // _fileName = fileName;
            saveFileName = fileName;
            IsRecording = false;
            Microphone.End(Microphone.devices[0]);
            var audioClip = CreateAudioClip(audioSource);
            // var wavWritingResult = TryCreateAudioFile(audioClip);
            var wavWritingResult = await TryCreateAudioFile(audioClip);
            return wavWritingResult;
        }

        /// <summary>
        /// Tries to create a WAV file from the recorded audio.
        /// </summary>
        /// <param name="audioClip">The recorded audio as an AudioClip.</param>
        /// <returns>The result of creating the audio file.</returns>
        // private static FileWritingResultModel TryCreateAudioFile(AudioClip audioClip)
        private static async UniTask<FileWritingResultModel> TryCreateAudioFile(AudioClip audioClip)
        {
            // var filePath = Path.Combine(Application.persistentDataPath, saveFileName + ".wav");


            Debug.Log(Path.Combine(saveDirectoryPath, saveFileName + ".wav"));
            
            var filePath = Path.Combine(saveDirectoryPath, saveFileName + ".wav");

            // Delete the file if it exists.
            if (File.Exists(filePath)) File.Delete(filePath);
            FileWritingResultModel wavWritingResult;

            try
            {
                // FileWriter.WriteWavFile(audioClip, filePath, HeaderSize);
                FileWriter.WriteWavFile(audioClip, filePath, HeaderSize);

                wavWritingResult = new FileWritingResultModel()
                {
                    status = true,
                    result = filePath,
                    error = null
                };
            }
            catch (Exception exception)
            {
                wavWritingResult = new FileWritingResultModel()
                {
                    status = false,
                    result = null,
                    error = exception.Message
                };
            }

            await UniTask.WaitUntil(() => wavWritingResult.result != null);

            return wavWritingResult;
        }

        /// <summary>
        /// Creates an AudioClip from the recorded audio data and trims the silence at the end of the recording.
        /// </summary>
        /// <param name="audioSource">The AudioSource that includes the recorded audio data.</param>
        /// <returns>The created AudioClip.</returns>
        private static AudioClip CreateAudioClip(AudioSource audioSource)
        {
            var samplesData = new float[audioSource.clip.samples * audioSource.clip.channels];
            audioSource.clip.GetData(samplesData, 0);

            // Trim the silence at the end of the recording
            var samples = samplesData.ToList();
            var recordedSamples = (int)(samplesData.Length * (RecordingTime / (float)_timeToRecord));

            if (recordedSamples < samplesData.Length - 1)
            {
                samples.RemoveRange(recordedSamples, samplesData.Length - recordedSamples);
                samplesData = samples.ToArray();
            }

            // Create the audio file after removing the silence
            var audioClip =
                AudioClip.Create(saveFileName, samplesData.Length, audioSource.clip.channels, 44100, false);
            audioClip.SetData(samplesData, 0);
            return audioClip;
        }
    }
}