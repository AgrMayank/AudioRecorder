using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Mayank.AudioRecorder.Utility
{
    public static class FileReader
    {
        // public static void LoadFile(string filePath)
        // public static byte[] LoadFile(string filePath)
        public static async UniTask<byte[]> LoadFile(string filePath)
        {
            // FileStream fileStream = File.OpenRead(filePath);
            // fileStream.Read();

            
            
            
            // var fileBytes = File.ReadAllBytes(filePath);


            if (File.Exists(filePath))
            {
                Debug.Log("file exists at "+filePath);
            }
            else
            {
                Debug.Log("file does not exist at "+filePath);
                return null;
            }
            
            

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            

            return fileBytes;



            // AudioClip audioClip = AudioClip.Create("LoadedAudi", fileBytes.Length, 1, 44100, false);


        }


        // public static AudioClip LoadAudioClip(string filePath)
        public static async UniTask<AudioClip> LoadAudioClip(string filePath)
        {
            
            Debug.Log("public static AudioClip LoadAudioClip(string filePath)       public static AudioClip LoadAudioClip(string filePath)");
            
            // var fileBytes = await LoadFile(filePath);
            // var fileBytes = LoadWavFile(filePath);
            var fileBytes = await LoadWavFile(filePath);
            
            Debug.Log("fileBytes.Length    ::::    "+fileBytes.Length);
            
            
            AudioClip audioClip = AudioClip.Create("LoadedAudio", fileBytes.Length, 1, 44100, false);

            
            Debug.Log("audioClip.length      :::   "+audioClip.length);
            

            return audioClip;
            
            
            
            
            
            
            
            
            
        }





        public static async UniTask<AudioClip> LoadWavFileAsAudioClip(string filePath)
        {
            var multimediaWebRequest = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV);
            await multimediaWebRequest.SendWebRequest();
            await UniTask.WaitUntil(() => multimediaWebRequest.isDone);
            return multimediaWebRequest.error != null ? null : DownloadHandlerAudioClip.GetContent((multimediaWebRequest));
        }
        
        
        


        // public static byte[] LoadWavFile(string filePath)
        public static async UniTask<byte[]> LoadWavFile(string filePath)
        {
            // var bytes = new byte[audioSourceSamples + headerSize];

            // byte[] bytes = new byte[];
            
            var fs = File.OpenRead(filePath);
            
            var bytes = new byte[fs.Length];
            
            // fs.Read(bytes, 0, bytes.Length);
            var result = await fs.ReadAsync(bytes, 0, bytes.Length);
            return bytes;
            // return result;
        }
        
        
        
        
        // public 
        
        
        
        
        
        
        /// <summary>
        /// Reads an audio file at the specified path and returns it as a byte array.
        /// </summary>
        /// <param name="filePath">The path of the audio file to read.</param>
        /// <param name="audioSourceSamples">The number of audio source samples.</param>
        /// <param name="headerSize">The size of the audio file header.</param>
        /// <returns>A byte array containing the audio data.</returns>
        public static byte[] ConvertWavToByteArray(string filePath, int audioSourceSamples, int headerSize)
        {
            var bytes = new byte[audioSourceSamples + headerSize];
            var fs = File.OpenRead(filePath);
            fs.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}