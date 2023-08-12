using System.IO;
using Cysharp.Threading.Tasks;
using Mayank.AudioRecorder.Utility.Result;
using UnityEngine;
using UnityEngine.Networking;

namespace Mayank.AudioRecorder.Utility
{
    public static class FileReader
    {
        /// <summary>
        /// Loads a WAV file from the specified path and converts it into an audio clip.
        /// </summary>
        /// <param name="filePath">The path of the WAV file.</param>
        /// <returns>The result of loading the audio clip from the file path.</returns>
        public static async UniTask<AudioClipFileReadingResultModel> LoadWavFileAsAudioClip(string filePath)
        {
            var multimediaWebRequest = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV);
            await multimediaWebRequest.SendWebRequest();
            await UniTask.WaitUntil(() => multimediaWebRequest.isDone);
            var audioClipFileReadingResultModel = new AudioClipFileReadingResultModel();

            if (multimediaWebRequest.error != null)
            {
                audioClipFileReadingResultModel.status = false;
                audioClipFileReadingResultModel.error = multimediaWebRequest.error;
                audioClipFileReadingResultModel.result = null;
            }
            else
            {
                audioClipFileReadingResultModel.status = true;
                audioClipFileReadingResultModel.error = null;
                audioClipFileReadingResultModel.result = DownloadHandlerAudioClip.GetContent(multimediaWebRequest);
            }

            return audioClipFileReadingResultModel;
        }
    }
}