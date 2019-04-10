using NAudio.Wave;
using NAudio.Wave.Compression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAudio.Codecs
{
   /// <summary>
   /// 
   /// </summary>
   public abstract class AcmChatCodec
   {
      private readonly WaveFormat encodeFormat;
      private AcmStream encodeStream;
      private AcmStream decodeStream;
      private int decodeSourceBytesLeftovers;
      private int encodeSourceBytesLeftovers;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="recordFormat"></param>
      /// <param name="encodeFormat"></param>
      protected AcmChatCodec(WaveFormat recordFormat, WaveFormat encodeFormat)
      {
         RecordFormat = recordFormat;
         this.encodeFormat = encodeFormat;
      }

      /// <summary>
      /// 
      /// </summary>
      public WaveFormat RecordFormat { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="data"></param>
      /// <param name="offset"></param>
      /// <param name="length"></param>
      /// <returns></returns>
      public byte[] Encode(byte[] data, int offset, int length)
      {
         if (encodeStream == null)
         {
            encodeStream = new AcmStream(RecordFormat, encodeFormat);
         }
         //Debug.WriteLine(String.Format("Encoding {0} + {1} bytes", length, encodeSourceBytesLeftovers));
         return Convert(encodeStream, data, offset, length, ref encodeSourceBytesLeftovers);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="data"></param>
      /// <param name="offset"></param>
      /// <param name="length"></param>
      /// <returns></returns>
      public byte[] Decode(byte[] data, int offset, int length)
      {
         if (decodeStream == null)
         {
            decodeStream = new AcmStream(encodeFormat, RecordFormat);
         }
         //Debug.WriteLine(String.Format("Decoding {0} + {1} bytes", data.Length, decodeSourceBytesLeftovers));
         return Convert(decodeStream, data, offset, length, ref decodeSourceBytesLeftovers);
      }

      private static byte[] Convert(AcmStream conversionStream, byte[] data, int offset, int length, ref int sourceBytesLeftovers)
      {
         int bytesInSourceBuffer = length + sourceBytesLeftovers;
         Array.Copy(data, offset, conversionStream.SourceBuffer, sourceBytesLeftovers, length);
         int sourceBytesConverted;
         int bytesConverted = conversionStream.Convert(bytesInSourceBuffer, out sourceBytesConverted);
         sourceBytesLeftovers = bytesInSourceBuffer - sourceBytesConverted;
         if (sourceBytesLeftovers > 0)
         {
            //Debug.WriteLine(String.Format("Asked for {0}, converted {1}", bytesInSourceBuffer, sourceBytesConverted));
            // shift the leftovers down
            Array.Copy(conversionStream.SourceBuffer, sourceBytesConverted, conversionStream.SourceBuffer, 0, sourceBytesLeftovers);
         }
         byte[] encoded = new byte[bytesConverted];
         Array.Copy(conversionStream.DestBuffer, 0, encoded, 0, bytesConverted);
         return encoded;
      }

      /// <summary>
      /// 
      /// </summary>
      public abstract string Name { get; }

      /// <summary>
      /// 
      /// </summary>
      public int BitsPerSecond
      {
         get
         {
            return encodeFormat.AverageBytesPerSecond * 8;
         }
      }

      /// <summary>
      /// 
      /// </summary>
      public void Dispose()
      {
         if (encodeStream != null)
         {
            encodeStream.Dispose();
            encodeStream = null;
         }
         if (decodeStream != null)
         {
            decodeStream.Dispose();
            decodeStream = null;
         }
      }

      /// <summary>
      /// 
      /// </summary>
      public bool IsAvailable
      {
         get
         {
            // determine if this codec is installed on this PC
            bool available = true;
            try
            {
               using (new AcmStream(RecordFormat, encodeFormat)) { }
               using (new AcmStream(encodeFormat, RecordFormat)) { }
            }
            catch (MmException)
            {
               available = false;
            }
            return available;
         }
      }
   }

   /// <summary>
   /// 
   /// </summary>
   public class AcmMuLawChatCodec : AcmChatCodec
   {
      /// <summary>
      /// 
      /// </summary>
      public AcmMuLawChatCodec()
            : base(new WaveFormat(8000, 16, 1), WaveFormat.CreateMuLawFormat(8000, 1))
      {
      }
      /// <summary>
      /// 
      /// </summary>
      public override string Name
      {
         get { return "ACM G.711 mu-law"; }
      }
   }

   /// <summary>
   /// 
   /// </summary>
   public class MuLawChatCodec
   {
      /// <summary>
      /// 
      /// </summary>
      public string Name
      {
         get { return "G.711 mu-law"; }
      }

      /// <summary>
      /// 
      /// </summary>
      public int BitsPerSecond
      {
         get { return RecordFormat.SampleRate * 8; }
      }

      /// <summary>
      /// 
      /// </summary>
      public WaveFormat RecordFormat
      {
         get { return new WaveFormat(8000, 16, 1); }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="data"></param>
      /// <param name="offset"></param>
      /// <param name="length"></param>
      /// <returns></returns>
      public byte[] Encode(byte[] data, int offset, int length)
      {
         var encoded = new byte[length / 2];
         int outIndex = 0;
         for (int n = 0; n < length; n += 2)
         {
            encoded[outIndex++] = MuLawEncoder.LinearToMuLawSample(BitConverter.ToInt16(data, offset + n));
         }
         return encoded;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="data"></param>
      /// <param name="offset"></param>
      /// <param name="length"></param>
      /// <returns></returns>
      public byte[] Decode(byte[] data, int offset, int length)
      {
         var decoded = new byte[length * 2];
         int outIndex = 0;
         for (int n = 0; n < length; n++)
         {
            short decodedSample = MuLawDecoder.MuLawToLinearSample(data[n + offset]);
            decoded[outIndex++] = (byte)(decodedSample & 0xFF);
            decoded[outIndex++] = (byte)(decodedSample >> 8);
         }
         return decoded;
      }

      /// <summary>
      /// 
      /// </summary>
      public void Dispose()
      {
         // nothing to do
      }

      /// <summary>
      /// 
      /// </summary>
      public bool IsAvailable { get { return true; } }

   }
}
