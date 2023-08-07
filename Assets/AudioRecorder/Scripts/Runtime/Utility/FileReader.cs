using System.IO;

namespace Mayank.AudioRecorder.Utility
{
    
    // ToDo: Add comments
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