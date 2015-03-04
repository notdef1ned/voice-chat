using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSpeex;

namespace Server
{
    public static class NSpeexHelper
    {

        private static readonly SpeexEncoder Encoder = CreateEncoder();
        private static SpeexEncoder CreateEncoder()
        {
            var mode = GetBandMode(1000);
            var encoder = new SpeexEncoder(mode) {Quality = 1};

            // set encoding quality to lowest (which will generate the smallest size in the fastest time)
            return encoder;
        }

        /// <summary>
        /// Encodes speech
        /// </summary>
        public static byte[] EncodeSpeech(byte[] buf, int length)
        {
            var mode = GetBandMode(1000);
           
            var dataSize = length/2;

            var data = new short[dataSize];
            var sampleIndex = 0;
            for (var i = 0; i < length; i += 2,sampleIndex++)
                data[sampleIndex] = BitConverter.ToInt16(buf, i);
            dataSize = dataSize - dataSize% Encoder.FrameSize;
            var encodedData = new byte[length];
            var encodedBytes = Encoder.Encode(data, 0, dataSize, encodedData, 0, length);
            if (encodedBytes == 0) 
                return buf;

            // each chunk is laid out as follows
            // | 4-byte total chunk size | 4-byte  encoded buffer size | <encoded-bytes> |
            var inDataSizeBuf = BitConverter.GetBytes(dataSize);
            var sizeBuf = BitConverter.GetBytes(encodedBytes + inDataSizeBuf.Length);
            var returnBuf = new byte[encodedBytes + sizeBuf.Length + inDataSizeBuf.Length];
            sizeBuf.CopyTo(returnBuf, 0);
            inDataSizeBuf.CopyTo(returnBuf, sizeBuf.Length);
            Array.Copy(encodedData, 0, returnBuf, sizeBuf.Length + inDataSizeBuf.Length, encodedBytes);
            return returnBuf;
        }


        private static byte[] DecodeSpeech(byte[] buf)
        {
            var mode = GetBandMode(1000);
            var decoder = new SpeexDecoder(mode);
            var inDataSizeBuf = new byte[4];
            var sizeBuf = new byte[4];
            var encodedBuf = new byte[buf.Length - 8];
            Array.Copy(buf, 0, sizeBuf, 0, 4);
            Array.Copy(buf, 4, inDataSizeBuf, 0, 4);
            Array.Copy(buf, 8, encodedBuf, 0, buf.Length - 8);

            var inDataSize = BitConverter.ToInt32(inDataSizeBuf, 0);
            var decodedBuf = new short[inDataSize];

            decoder.Decode(encodedBuf, 0, encodedBuf.Length, decodedBuf, 0, false);
            
            var returnBuf = new byte[inDataSize * 2];
            for (var index = 0; index < decodedBuf.Length; index++)
            {
                var temp = BitConverter.GetBytes(decodedBuf[index]);
                Array.Copy(temp, 0, returnBuf, index * 2, 2);
            }

            return returnBuf;
        }



        private static BandMode GetBandMode(int sampleRate)
        {
            if (sampleRate <= 8000)
                return BandMode.Narrow;

            if (sampleRate <= 16000)
                return BandMode.Wide;

            return BandMode.UltraWide;
        }
    }
}
