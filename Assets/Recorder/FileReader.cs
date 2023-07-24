using System.IO;
using UnityEngine;

namespace Recorder
{
    public static class FileReader
    {
        // public static byte[] ConvertWAVtoByteArray(string filePath, int audioSourceSamples, int headerSize)
        public static byte[] ConvertWavToByteArray(string filePath, int audioSourceSamples, int headerSize)
        {
            //Open the stream and read it back.
            // byte[] bytes = new byte[audioSource.clip.samples + HEADER_SIZE];
            byte[] bytes = new byte[audioSourceSamples + headerSize];
            using (FileStream fs = File.OpenRead(filePath))
            {
                fs.Read(bytes, 0, bytes.Length);
            }
            return bytes;
        }
    }
}