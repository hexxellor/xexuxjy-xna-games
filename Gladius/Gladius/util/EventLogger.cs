﻿using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace xexuxjy.Gladius.util
{
    public class EventLogger 
    {
        public EventLogger(Game game,String filename)
        {
            EventMask = EventTypes.Action | EventTypes.LifeCycle | EventTypes.Waypoints | EventTypes.ContentExceptions;
            #if WINDOWS
            if (filename == null)
            {
                filename = @"../../../logs/action-logs.txt";
            }
            FileStream filestream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            m_streamWriter = new StreamWriter(filestream);
            m_streamWriter.AutoFlush = true;
            #endif
        }


        public void LogEvent(EventTypes eventType ,String eventDetails)
        {
#if WINDOWS
            if (LoggingType(eventType)) 
            {
                m_streamWriter.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " : " + eventDetails);

            }
#endif
        }

#if WINDOWS
        StreamWriter m_streamWriter = null;


        public void Dispose()
        {
            m_streamWriter.Flush();
            m_streamWriter.Close();

        }
#endif
        public bool LoggingType(EventTypes eventType)
        {
            return Enabled && ((EventMask & eventType) != 0);
        }


        public EventTypes EventMask
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }

    }
    [Flags]
    public enum EventTypes
    {
        Action =1,
        LifeCycle =2 ,
        Waypoints=4,
        Animation=8,
        ContentExceptions = 16
    }

}
