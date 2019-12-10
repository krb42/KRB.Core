using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRB.Core.Toolset
{
   public class Hash
   {
      /// <summary>
      /// Implementation of a super fast hash http://landman-code.blogspot.com.au/2009/02/c-superfasthash-and-murmurhash2.html
      /// </summary>
      /// <param name="dataToHash">
      /// Bytes to hash
      /// </param>
      /// <returns>
      /// Hash code
      /// </returns>
      public static uint SuperFastHash(byte[] dataToHash)
      {
         var dataLength = dataToHash.Length;
         if (dataLength == 0) return 0;

         var hash = Convert.ToUInt32(dataLength);
         var remainingBytes = dataLength & 3; // mod 4
         var numberOfLoops = dataLength >> 2; // div 4
         var currentIndex = 0;
         while (numberOfLoops > 0)
         {
            hash += BitConverter.ToUInt16(dataToHash, currentIndex);
            var tmp = (uint)(BitConverter.ToUInt16(dataToHash, currentIndex + 2) << 11) ^ hash;
            hash = (hash << 16) ^ tmp;
            hash += hash >> 11;
            currentIndex += 4;
            numberOfLoops--;
         }

         switch (remainingBytes)
         {
            case 3:
               hash += BitConverter.ToUInt16(dataToHash, currentIndex);
               hash ^= hash << 16;
               hash ^= ((uint)dataToHash[currentIndex + 2]) << 18;
               hash += hash >> 11;
               break;

            case 2:
               hash += BitConverter.ToUInt16(dataToHash, currentIndex);
               hash ^= hash << 11;
               hash += hash >> 17;
               break;

            case 1:
               hash += dataToHash[currentIndex];
               hash ^= hash << 10;
               hash += hash >> 1;
               break;

            default:
               break;
         }

         /* Force "avalanching" of final 127 bits */
         hash ^= hash << 3;
         hash += hash >> 5;
         hash ^= hash << 4;
         hash += hash >> 17;
         hash ^= hash << 25;
         hash += hash >> 6;

         return hash;
      }
   }
}
