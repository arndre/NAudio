using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudio.Exceptions
{
   /// <summary>
   /// 
   /// </summary>
   [Serializable]
   public class WaveFileTooLargeException : Exception
   {
      /// <summary>
      /// 
      /// </summary>
      public WaveFileTooLargeException()
      { }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="message"></param>
      public WaveFileTooLargeException(string message) : base(message)
      { }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="message"></param>
      /// <param name="innerException"></param>
      public WaveFileTooLargeException(string message, Exception innerException) : base(message, innerException)
      { }
   }
}
