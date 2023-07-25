using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Recorder
{
    public static class AudioRecorder
    {
        // public static float RecordingTime { get; private set; }
        public static float RecordingTime { get; private set; }
        public static bool IsRecording { get; private set; }

        private static int _timeToRecord;
        
        // private static int _headerSize
        
        
        /// <summary>
        /// WAV file header size
        /// </summary>
        private const int HeaderSize = 44;



        private static Coroutine _recordingTimeUpdater;
        
        
        // ToDo: convert the following method to a asyncronous method
        public static void UpdateRecordingTime()
        // public static IEnumerator UpdateRecordingTime()
        {
            // while (IsRecording)
            // {
            //     RecordingTime += 1;
            //     yield return new WaitForSeconds(1.0f);
            // }
            
            RecordingTime += Time.deltaTime;
            
        }
        
        
        
        // public static void StartRecording()
        public static void StartRecording(AudioSource audioSource, int timeToRecord)
        {
            
            Debug.Log("public static void StartRecording(AudioSource audioSource, int timeToRecord)");
            
            _timeToRecord = timeToRecord;
            
            RecordingTime = 0f;
            IsRecording = true;
            Microphone.End(Microphone.devices[0]);
            audioSource.clip = Microphone.Start(Microphone.devices[0], false, timeToRecord, 44100);
            // _recorderView.OnStartRecording();
            
            
            // _recordingTimeUpdater = StartC
        }
        
        
        
        
        
        
        // public void StopRecording(string fileName = "Audio")
        // public static void StopRecording(string fileName = "Audio")
        public static void StopRecording(AudioSource audioSource, string fileName = "Audio")
        {
            // _recorderView.OnStopRecording();
            SaveRecording(audioSource, fileName);
        }
        
        
        
        
        
        // private void SaveRecording(string fileName = "Audio")
        // private static void SaveRecording(string fileName = "Audio")
        // private static void SaveRecording(AudioSource audioSource, string fileName = "Audio")
        // private static string SaveRecording(AudioSource audioSource, string fileName = "Audio")
        public static string SaveRecording(AudioSource audioSource, string fileName = "Audio")
        {
            
            Debug.Log("public static string SaveRecording(AudioSource audioSource, string fileName = )");
            
            IsRecording = false;
            Microphone.End(Microphone.devices[0]);
            
            
            // while (!(Microphone.GetPosition(null) > 0))
            // {
            // }

            var samplesData = new float[audioSource.clip.samples * audioSource.clip.channels];
            audioSource.clip.GetData(samplesData, 0);

            // Trim the silence at the end of the recording
            var samples = samplesData.ToList();
            // int recordedSamples = (int)(samplesData.Length * (recordingTime / (float)timeToRecord));
            // int recordedSamples = (int)(samplesData.Length * (recordingTime / (float)_timeToRecord));
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

            
            
            
            
            
            
            //
            // // Assign Current Audio Clip to Audio Player
            // audioPlayer.audioClip = audioClip;
            // audioPlayer.UpdateClip();
            //
            
            
            
            
            
            
            
            
            
            
            
            string filePath = Path.Combine(Application.persistentDataPath,
                fileName + " " + DateTime.UtcNow.ToString("yyyy_MM_dd HH_mm_ss_ffff") + ".wav");
            
            // Delete the file if it exists.
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            try
            {
                // FileWriter.WriteWavFile(audioClip, filePath, HEADER_SIZE);
                FileWriter.WriteWavFile(audioClip, filePath, HeaderSize);
                // _recorderView.OnRecordingSaved($"Audio saved at {filePath}");

                return filePath;


                // Debug.Log("File Saved Successfully at " + filePath);
            }
            catch (DirectoryNotFoundException)
            {
                // Debug.LogError("Persistent Data Path not found!");

                return "Persistent Data Path not found!";
            }
            catch
            {
                return "Something went wrong while saving audio file!";
            }

            // isRecording = false;
            // IsRecording = false;
            // Microphone.End(Microphone.devices[0]);
        }
        
        
        
        
    }
}
