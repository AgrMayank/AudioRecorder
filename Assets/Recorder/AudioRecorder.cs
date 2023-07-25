using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Recorder
{
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

        public static void StartRecording(AudioSource audioSource, int timeToRecord)
        {
            _timeToRecord = timeToRecord;
            RecordingTime = 0f;
            IsRecording = true;
            Microphone.End(Microphone.devices[0]);
            audioSource.clip = Microphone.Start(Microphone.devices[0], false, timeToRecord, 44100);
        }

        // public static string SaveRecording(AudioSource audioSource, string fileName = "Audio")
        public static FileWritingResultModel SaveRecording(AudioSource audioSource, string fileName = "Audio")
        {
            IsRecording = false;
            Microphone.End(Microphone.devices[0]);

            var samplesData = new float[audioSource.clip.samples * audioSource.clip.channels];
            audioSource.clip.GetData(samplesData, 0);

            // Trim the silence at the end of the recording
            var samples = samplesData.ToList();
            int recordedSamples = (int)(samplesData.Length * (RecordingTime / (float)_timeToRecord));

            if (recordedSamples < samplesData.Length - 1)
            {
                samples.RemoveRange(recordedSamples, samplesData.Length - recordedSamples);
                samplesData = samples.ToArray();
            }

            // Create the audio file after removing the silence
            AudioClip audioClip =
                AudioClip.Create(fileName, samplesData.Length, audioSource.clip.channels, 44100, false);
            audioClip.SetData(samplesData, 0);

            string filePath = Path.Combine(Application.persistentDataPath,
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


                // return wavWritingResult;
                
                
                // return filePath;
            }
            // catch (DirectoryNotFoundException)
            // {
            //     // wavWritingResult = new FileWritingResultModel()
            //     // {
            //     //     status = true,
            //     //     result = filePath,
            //     //     error = null
            //     // };
            //     return "Persistent Data Path not found!";
            // }
            // // catch
            catch(Exception exception)
            {
                wavWritingResult = new FileWritingResultModel()
                {
                    status = false,
                    result = null,
                    error = exception.Message
                };
                
                // return "Something went wrong while saving audio file!";
            }

            return wavWritingResult;
        }
    }
}
