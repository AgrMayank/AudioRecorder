using System;
using System.IO;
using System.Linq;
using Mayank.AudioRecorder.Utility;
using UnityEngine;

namespace Mayank.AudioRecorder.Recorder.Core
{
    // ToDo: Convert Debug.Log ... to Debug.UnityLogger ...
    // ToDo: Add comments.
    public static class AudioRecorder
    {
        public static float RecordingTime { get; private set; }
        public static bool IsRecording { get; private set; }

        private static int _timeToRecord;
        
        /// <summary>
        /// WAV file header size
        /// </summary>
        private const int HeaderSize = 44;

        private static Coroutine _recordingTimeUpdater;

        // ToDo: convert the following method to a asyncronous method
        public static void UpdateRecordingTime()
        {
            RecordingTime += Time.deltaTime;
        }

        public static bool MicrophoneIsAvailable()
        {
            if (Microphone.devices == null || Microphone.devices.Length == 0)
            {
                // Debug.LogError("No microphone found.");
                Debug.unityLogger.LogError("Microphone", "No microphone found.");
                return false;
            }
            
            // Debug.Log("Microphone is available");
            Debug.unityLogger.Log(LogType.Log, "Microphone is available");
            return true;
        }

        public static void StartRecording(AudioSource audioSource, int timeToRecord)
        {
            _timeToRecord = timeToRecord;
            RecordingTime = 0f;
            IsRecording = true;
            Microphone.End(Microphone.devices[0]);
            audioSource.clip = Microphone.Start(Microphone.devices[0], false, timeToRecord, 44100);
        }

        public static FileWritingResultModel SaveRecording(AudioSource audioSource, string fileName = "Audio")
        {
            IsRecording = false;
            Microphone.End(Microphone.devices[0]);
            var audioClip = CreateAudioClip(audioSource, fileName);
            var wavWritingResult = TryCreateAudioFile(fileName, audioClip);
            return wavWritingResult;
        }

        private static FileWritingResultModel TryCreateAudioFile(string fileName, AudioClip audioClip)
        {
            var filePath = Path.Combine(Application.persistentDataPath,
                fileName + " " + DateTime.UtcNow.ToString("yyyy_MM_dd HH_mm_ss_ffff") + ".wav");

            // Delete the file if it exists.
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

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