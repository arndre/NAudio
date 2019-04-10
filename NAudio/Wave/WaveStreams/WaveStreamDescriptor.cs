using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAudio.Wave
{
   /// <summary>
   /// Additional meta data for wave stream (arno 2018)
   /// </summary>
   public class WaveStreamDescriptor
   {
      /// <summary>
      /// Name
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// ChannelOrigin
      /// </summary>
      public int[] Channels { get; set; }

      /// <summary>
      /// TrainingRoomId
      /// </summary>
      public string TrainingRoomId { get; set; }
   }
}
