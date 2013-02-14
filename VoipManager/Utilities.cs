using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VoipManager.Teamspeak3.Communication;

namespace VoipManager
{
    internal static class Utilities
    {
        static Utilities()
        {
            Trace.UseGlobalLock = true;
            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.Listeners.Add(new TextWriterTraceListener("log.txt"));
        }

        private static void ModifyLogIndent(Int32 mod)
        {
            foreach (TraceListener listener in Trace.Listeners) {
                listener.IndentSize = 1;
                listener.IndentLevel += mod;
            }
        }

        public static void LogTeamspeak3Request(Teamspeak3Request request)
        {
            Trace.WriteLine("Teamspeak3Request: {");
            ModifyLogIndent(4);
            Trace.WriteLine(String.Format("Command: {0}", request.Command));
            Trace.WriteLine(String.Format("Raw: {0}", request.Raw.TrimEnd()));
            ModifyLogIndent(-4);
            Trace.WriteLine("}");
        }

        public static void LogTeamspeak3Message(Teamspeak3Message message)
        {
            Trace.WriteLine("Teamspeak3Message: {");
            ModifyLogIndent(4);
            foreach (Teamspeak3Pair tPair in message.Error.Pairs) {
                Trace.WriteLine(String.Format("{0}: {1}", tPair.Key, tPair.Value));
            }
            foreach (Teamspeak3Section tSection in message.Sections) {
                Trace.WriteLine("Teamspeak3Section: {");
                ModifyLogIndent(4);
                foreach (Teamspeak3Group tGroup in tSection.Groups) {
                    Trace.WriteLine("Teamspeak3Group: {");
                    ModifyLogIndent(4);
                    foreach (Teamspeak3Pair tPair in tGroup.Pairs) {
                        Trace.WriteLine(String.Format("{0}: {1}", tPair.Key, tPair.Value));
                    }
                    ModifyLogIndent(-4);
                    Trace.WriteLine("}");
                }
                ModifyLogIndent(-4);
                Trace.WriteLine("}");
            }
            ModifyLogIndent(-4);
            Trace.WriteLine("}");
        }

        public static Int32? DateTimeToUtcInteger(DateTime? dateTime)
        {
            if (dateTime.HasValue) {
                DateTime utcEpoch = new DateTime(1970, 1, 1);
                TimeSpan timeDiff = dateTime.Value - utcEpoch;
                return (Int32?)timeDiff.TotalSeconds;
            }
            return null;
        }
        public static DateTime? UtcIntegerToDateTime(Int32? utcInteger)
        {
            if (utcInteger.HasValue) {
                DateTime utcEpoch = new DateTime(1970, 1, 1);
                return utcEpoch.AddSeconds(utcInteger.Value);
            }
            return null;
        }
    }
}
