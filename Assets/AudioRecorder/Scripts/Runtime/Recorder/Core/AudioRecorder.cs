using System;
using System.IO;
using System.Linq;
using Mayank.AudioRecorder.Utility;
using UnityEngine;

namespace Mayank.AudioRecorder.Recorder.Core
{
    public static class AudioRecorder
    {
        /// <summary>
        /// The total time of the current recording audio.
        /// </summary>
        public static float RecordingTime { get; private set; }
        
        /// <summary>
        /// Represents if the recorder is recording or not. 
        /// </summary>
        public static bool IsRecording { get; private set; }

        /// <summary>
        /// The maximum allowed time of recording.
        /// </summary>
        private static int _timeToRecord;
        
        /// <summary>
        /// WAV file header size
        /// </summary>
        private const int HeaderSize = 44;

        /// <summary>
        /// Updates the total recording time.
        /// </summary>
        // ToDo: convert the following method to a asynchronous method
        public static void UpdateRecordingTime()
        {
            RecordingTime += Time.deltaTime;
        }
        
        /// <summary>
        /// Checks if any microphone is available.
        /// </summary>
        /// <returns>A microphone is available or not.</returns>
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
        /// Starts recording the audio file.
        /// </summary>
        /// <param name="audioSource">The intended AudioSource. It records the input audio (via microphone) on its AudioClip.</param>
        /// <param name="timeToRecord">The maximum allowed time of the audio.</param>
        public static void StartRecording(AudioSource audioSource, int timeToRecord)
        {
            _timeToRecord = timeToRecord;
            RecordingTime = 0f;
            IsRecording = true;
            Microphone.End(Microphone.devices[0]);
            audioSource.clip = Microphone.Start(Microphone.devices[0], false, timeToRecord, 44100);
        }

        /// <summary>
        /// Saves the recorded audio.
        /// </summary>
        /// <param name="audioSource">The AudioSource which includes the recorded audio.</param>
        /// <param name="fileName">The intended name of the audio file.</param>
        /// <returns>The result of writing the audio file.</returns>
        public static FileWritingResultModel SaveRecording(AudioSource audioSource, string fileName = "Audio")
        {
            IsRecording = false;
            Microphone.End(Microphone.devices[0]);
            var audioClip = CreateAudioClip(audioSource, fileName);
            var wavWritingResult = TryCreateAudioFile(fileName, audioClip);
            return wavWritingResult;
        }

        /// <summary>
        /// Tries to create audio file.
        /// </summary>
        /// <param name="fileName">The intended name of the audio file.</param>
        /// <param name="audioClip">The recorded audio as an AudioClip.</param>
        /// <returns>The result of creating the audio file.</returns>
        private static FileWritingResultModel TryCreateAudioFile(string fileName, AudioClip audioClip)
        {
            var filePath = Path.Combine(Application.persistentDataPath,
                fileName + " " + DateTime.UtcNow.ToString("yyyy_MM_dd HH_mm_ss_ffff") + ".wav");

            // Delete the file if it exists.
            if (File.Exists(filePath)) File.Delete(filePath);
            FileWritingResultModel wavWritingResult;

            try
            {
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

            return wavWritingResult;
        }

        /// <summary>
        /// Creates an AudioClip.
        /// </summary>
        /// <param name="audioSource">The intended AudioSource.</param>
        /// <param name="fileName">The intended name of the AudioClip.</param>
        /// <returns>The created AudioClip.</returns>
        private static AudioClip CreateAudioClip(AudioSource audioSource, string fileName)
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
                AudioClip.Create(fileName, samplesData.Length, audioSource.clip.channels, 44100, false);
            audioClip.SetData(samplesData, 0);
            return audioClip;
        }
    }
}