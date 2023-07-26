using System.IO;

namespace Recorder
{
    public static class FileReader
    {
        public static byte[] ConvertWavToByteArray(string filePath, int audioSourceSamples, int headerSize)
        {
            byte[] bytes = new byte[audioSourceSamples + headerSize];
            using (FileStream fs = File.OpenRead(filePath))
            {
                fs.Read(bytes, 0, bytes.Length);
            }
            return bytes;
        }
    }
}