﻿/* ************************************************************************** *
 * Voip Manager.
 * Copyright (C) 2012-2013  Cameron Gunnin
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * ************************************************************************** */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VoipManager
{
    using VoipManager.Teamspeak3.Communication;

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
            Trace.WriteLine(String.Format("Raw: {0}", request.RawText.TrimEnd()));
            ModifyLogIndent(-4);
            Trace.WriteLine("}");
        }

        public static void LogTeamspeak3Message(Teamspeak3Message message)
        {
            Trace.WriteLine("Teamspeak3Message: {");
            ModifyLogIndent(4);
            foreach (String tKey in message.Error.Keys) {
                Trace.WriteLine(String.Format("{0}: {1}", tKey, message.Error[tKey]));
            }
            foreach (Teamspeak3Section tSection in message.Sections) {
                Trace.WriteLine("Teamspeak3Section: {");
                ModifyLogIndent(4);
                foreach (Teamspeak3Group tGroup in tSection.Groups) {
                    Trace.WriteLine("Teamspeak3Group: {");
                    ModifyLogIndent(4);
                    foreach (String tKey in tGroup.Keys) {
                        Trace.WriteLine(String.Format("{0}: {1}", tKey, tGroup[tKey]));
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

        public static UInt32? DateTimeToUtcInteger(DateTime? dateTime)
        {
            if (dateTime.HasValue) {
                DateTime utcEpoch = new DateTime(1970, 1, 1);
                TimeSpan timeDiff = dateTime.Value - utcEpoch;
                return (UInt32?)timeDiff.TotalSeconds;
            }
            return null;
        }
        public static DateTime? UtcIntegerToDateTime(UInt32? utcInteger)
        {
            if (utcInteger.HasValue) {
                DateTime utcEpoch = new DateTime(1970, 1, 1);
                return utcEpoch.AddSeconds(utcInteger.Value);
            }
            return null;
        }

        public static bool AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key)) {
                dictionary[key] = value;
                return false;
            }
            dictionary.Add(key, value);
            return true;
        }
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue fallback = default(TValue))
        {
            if (dictionary.ContainsKey(key)) {
                return dictionary[key];
            }
            return fallback;
        }

        public static void Require<TException>(Boolean predicate, String message = null)
            where TException : Exception
        {
            if (!predicate) {
                if (message != null) {
                    throw (TException)Activator.CreateInstance(typeof(TException), message);
                }
                throw (TException)Activator.CreateInstance(typeof(TException));
            }
        }
    }
}
