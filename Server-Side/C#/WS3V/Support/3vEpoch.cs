using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS3V.Support
{
    public static class _3vEpoch
    {
        /// <summary>
        /// gets current Echovoice epoch UTC timestamp, in seconds
        /// </summary>
        /// <returns>int, seconds since the Echovoice epoch</returns>
        public static int getEVTime()
        {
            return (int)(((DateTime.UtcNow.Ticks - 621355968000000000) / 10000000) - 1308823200);
        }

        /// <summary>
        /// gets Echovoice epoch UTC timestamp from a given DateTime object, in seconds
        /// </summary>
        /// <param name="dt">DateTime object to convert to Echovoice Epoch</param>
        /// <returns>int, seconds since the Echovoice epoch</returns>
        public static int getEVTime(DateTime dt)
        {
            return (int)(((dt.Ticks - 621355968000000000) / 10000000) - 1308823200);
        }
    }
}
