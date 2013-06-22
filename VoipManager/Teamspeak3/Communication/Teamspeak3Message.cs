/* ************************************************************************** *
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace VoipManager.Teamspeak3.Communication
{
    public sealed class Teamspeak3Message : Teamspeak3Response
    {
        // Examples: version=3.0.6.1 build=1340956745 platform=Windows\n\rerror id=0 msg=ok\n\r
        internal static readonly Regex MessageRegex = new Regex(String.Format("^((.+?{0})*?)error id=.+?{0}", SeperatorRegex));
        internal static readonly Regex BannedRegex  = new Regex(String.Format("^((.+?{0})*?)error id=(3331|3329).+", SeperatorRegex));

        public const String Seperator = "\n\r";
        public const String SeperatorRegex = "\\n\\r";

        private readonly List<Teamspeak3Section>     mSections = new List<Teamspeak3Section>(); 
        public Teamspeak3Section                     Section  { get { return mSections.FirstOrDefault(); } }
        public ReadOnlyCollection<Teamspeak3Section> Sections { get { return mSections.AsReadOnly(); } }  


        /// <summary>
        /// The status of the response and related information.
        /// </summary>
        public readonly Teamspeak3Group Error;
        public UInt32 Id           { get { return UInt32.Parse(Error["id"]); } }
        public String Message      { get { return Error["msg"]; } }
        public String ExtraMessage { get { return Error["extra_msg"]; } }


        /// <summary>
        /// Parses the raw response as a message response.
        /// </summary>
        /// <param name="raw">The raw response from the server.</param>
        /// <exception cref="System.ArgumentNullException"/>
        public Teamspeak3Message(String rawText) : base(rawText)
        {
            foreach (var strSection in rawText.Split(Seperator.ToArray(), StringSplitOptions.RemoveEmptyEntries)) {

                String strSectionTrimmed = strSection.Trim();
                if (strSectionTrimmed.StartsWith("error")) {
                    Error = new Teamspeak3Group(strSectionTrimmed.Remove(0, "error".Length));
                    continue;
                }
                mSections.Add(new Teamspeak3Section(strSection));
            }
        }
    }

    public sealed class Teamspeak3Section
    {
        public const String Seperator      = "|";
        public const String SeperatorRegex = "\\|";

        private readonly List<Teamspeak3Group>     mGroups = new List<Teamspeak3Group>();
        public Teamspeak3Group                     Group  { get { return mGroups.FirstOrDefault(); } }
        public ReadOnlyCollection<Teamspeak3Group> Groups { get { return mGroups.AsReadOnly(); } }


        /// <summary>
        /// Represents a collection of objects in a response.
        /// </summary>
        /// <param name="raw">This section's raw response from the server.</param>
        public Teamspeak3Section(String rawText)
        {
            foreach (var strGroup in rawText.Split(Seperator.ToArray(), StringSplitOptions.RemoveEmptyEntries)) {
                mGroups.Add(new Teamspeak3Group(strGroup));
            }
        }
    }

    public sealed class Teamspeak3Group
    {
        public static readonly String Seperator      = " ";
        public static readonly String SeperatorRegex = " ";

        private readonly Dictionary<String, String> mPairs = new Dictionary<String, String>();
        public String this[String key]    { get { return mPairs.Get(key); } }
        public IEnumerable<String> Keys   { get { return mPairs.Keys;     } }
        public IEnumerable<String> Values { get { return mPairs.Values;   } }


        /// <summary>
        /// Represents a single object of a response.
        /// </summary>
        /// <param name="raw">This group's raw response from the server.</param>
        public Teamspeak3Group(String rawText)
        {
            foreach (var strPair in rawText.Split(Seperator.ToArray(), StringSplitOptions.RemoveEmptyEntries)) {
                Teamspeak3Pair pair = new Teamspeak3Pair(strPair);
                mPairs.AddOrUpdate(pair.Key, pair.Value);
            }
        }
    }

    public sealed class Teamspeak3Pair
    {
        public static readonly String Seperator      = "=";
        public static readonly String SeperatorRegex = "=";

        public readonly String Key;
        public readonly String Value;

        /// <summary>
        /// Represents a single property of an object.
        /// </summary>
        /// <param name="raw">This pair's raw response from the server.</param>
        public Teamspeak3Pair(String rawText)
        {
            String[] pair = rawText.Split(Seperator.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            Key   = pair[0];
            Value = pair.Length > 1 ? pair[1] : String.Empty;
        }
    }
}
