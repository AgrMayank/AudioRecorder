using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Mayank.AudioRecorder.Utility
{
    
    // ToDo: Add comments
    public static class FileWriter
    {
        public static void WriteWavFile(AudioClip clip, string filePath, int headerSize)
        {
            var clipData = new float[clip.samples];

            //Create the file.
            Stream fs = File.Create(filePath);
            var frequency = clip.frequency;
            var numOfChannels = clip.channels;
            var samples = clip.samples;
            fs.Seek(0, SeekOrigin.Begin);

            //Header

            // Chunk ID
            var riff = Encoding.ASCII.GetBytes("RIFF");
            fs.Write(riff, 0, 4);

            // ChunkSize
            // byte[] chunkSize = BitConverter.GetBytes((HEADER_SIZE + clipData.Length) - 8);
            var chunkSize = BitConverter.GetBytes((headerSize + clipData.Length) - 8);
            fs.Write(chunkSize, 0, 4);

            // Format
            var wave = Encoding.ASCII.GetBytes("WAVE");
            fs.Write(wave, 0, 4);

            // Subchunk1ID
            var fmt = Encoding.ASCII.GetBytes("fmt ");
            fs.Write(fmt, 0, 4);

            // Subchunk1Size
            var subChunk1 = BitConverter.GetBytes(16);
            fs.Write(subChunk1, 0, 4);

            // AudioFormat
            var audioFormat = BitConverter.GetBytes(1);
            fs.Write(audioFormat, 0, 2);

            // NumChannels
            var numChannels = BitConverter.GetBytes(numOfChannels);
            fs.Write(numChannels, 0, 2);

            // SampleRate
            var sampleRate = BitConverter.GetBytes(frequency);
            fs.Write(sampleRate, 0, 4);

            // ByteRate
            var byteRate = BitConverter.GetBytes(frequency * numOfChannels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
            fs.Write(byteRate, 0, 4);

            // BlockAlign
            var blockAlign = (ushort)(numOfChannels * 2);
            fs.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            // BitsPerSample
            ushort bps = 16;
            var bitsPerSample = BitConverter.GetBytes(bps);
            fs.Write(bitsPerSample, 0, 2);

            // Subchunk2ID
            var datastring = Encoding.ASCII.GetBytes("data");
            fs.Write(datastring, 0, 4);

            // Subchunk2Size
            var subChunk2 = BitConverter.GetBytes(samples * numOfChannels * 2);
            fs.Write(subChunk2, 0, 4);

            // Data

            clip.GetData(clipData, 0);
            var intData = new short[clipData.Length];
            var bytesData = new byte[clipData.Length * 2];

            var convertionFactor = 32767;

            for (int i = 0; i < clipData.Length; i++)
            {
                intData[i] = (short)(clipData[i] * convertionFactor);
                byte[] byteArr = new byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            fs.Write(bytesData, 0, bytesData.Length);
        }
        
    }
}