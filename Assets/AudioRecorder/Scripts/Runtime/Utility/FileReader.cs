using System.IO;

namespace Mayank.AudioRecorder.Utility
{
    public static class FileReader
    {
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