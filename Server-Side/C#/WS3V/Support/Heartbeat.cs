using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WS3V.Support
{
    /// <summary>
    /// based on ws3v documentation example: http://ws3v.org/spec.json#howdy
    /// by default heartbeats are enabled, frequent and allow for diagnostic collection
    /// </summary>

    public class Heartbeat : IDisposable
    {
        public int heartbeat_min_seconds { get; set; }
        public int heartbeat_max_seconds { get; set; }
        public bool allow_heartbeats_when_busy { get; set; }
        public Action cardiac_arrest { get; set; }

        public bool enabled
        {
            get
            {
                if (heartbeat_min_seconds == -1 || heartbeat_max_seconds == -1)
                    return false;
                else
                    return true;
            }
        }

        // flag used in the loop to determine if the
        // connection is active
        private bool _beat = false;

        // used to determine if the server should respond to a lub
        private int pulse;

        // public void to trigger an external beat
        public void beat()
        {
            _beat = true;

            if (!allow_heartbeats_when_busy)
                pulse = (int)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000) - 1308823200;
        }

        // used to see if a dub should be sent
        public bool respond
        {
            get
            {
                if(!allow_heartbeats_when_busy && ((int)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000) - 1308823200) - pulse < heartbeat_min_seconds)
                    return false;

                return true;
            }
        }

        private bool running = false;
        private Thread heartbeat = null;

        public Heartbeat() : this(false) { }

        /// <summary>
        /// If this connection would like to enable the heartbeat function
        /// this will need to be setup, this object will monitor itself for a dead connection
        /// and trigger a cardiac arrest if the tunnel has been idle and heartbeats are required.
        /// </summary>
        /// <param name="enabled">True enables Heartbeat</param>

        public Heartbeat(bool enabled)
        {
            if (enabled)
            {
                heartbeat_min_seconds = 30;
                heartbeat_max_seconds = 60;
                allow_heartbeats_when_busy = true;
                defibrillator();
            }
            else
            {
                heartbeat_min_seconds = -1;
                heartbeat_max_seconds = -1;
                allow_heartbeats_when_busy = false;
            }
        }

        public Heartbeat(int heartbeat_min_seconds, int heartbeat_max_seconds, bool allow_heartbeats_when_busy)
        {
            if (heartbeat_min_seconds == -1 || heartbeat_max_seconds == -1)
            {
                heartbeat_min_seconds = -1;
                heartbeat_max_seconds = -1;
                allow_heartbeats_when_busy = false;
            }
            else
            {
                this.heartbeat_min_seconds = heartbeat_min_seconds;
                this.heartbeat_max_seconds = heartbeat_max_seconds;
                this.allow_heartbeats_when_busy = allow_heartbeats_when_busy;

                if(!allow_heartbeats_when_busy)
                    pulse = (int)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000) - 1308823200;

                defibrillator();
            }
        }

        // start the heartbeat
        private void defibrillator()
        {
            running = true;
            heartbeat = new Thread(new ThreadStart(pacemaker));
            heartbeat.Start();
        }

        // connection heartbeat Loop
        private void pacemaker()
        {
            while (running)
            {
                // start with sleep cycle, check for running during loop
                int z = heartbeat_max_seconds;
                while (running && z-- > 0)
                    Thread.Sleep(1000);

                // no longer running, leave the thread
                if (!running)
                    return;

                // no beats and the max time has passed, time to die
                if (!_beat)
                {
                    running = false;
                    if(cardiac_arrest != null)
                        cardiac_arrest();
                    return;
                }

                // trigger beat back to false and re-loop
                _beat = false;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('[');
            sb.Append(heartbeat_min_seconds);
            sb.Append(',');
            sb.Append(heartbeat_max_seconds);
            sb.Append(',');
            sb.Append((allow_heartbeats_when_busy) ? "true" : "false");
            sb.Append(']');

            return sb.ToString();
        }

        public void Dispose()
        {
            running = false;

            // wait for heartbeat thread to die
            while (heartbeat.IsAlive)
                Thread.Sleep(100);

            heartbeat.Join(0);
        }
    }
}
