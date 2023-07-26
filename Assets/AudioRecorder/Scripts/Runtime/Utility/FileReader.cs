using System.IO;

namespace Mayank.AudioRecorder.Utility
{
    public static class FileReader
    {
        public static byte[] ConvertWavToByteArray(string filePath, int audioSourceSamples, int headerSize)
        {
            var bytes = new byte[audioSourceSamples + headerSize];
            var fs = File.OpenRead(filePath);
            fs.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}