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
        // Constants
        public static readonly String Seperator      = "\n\r";
        public static readonly String SeperatorRegex = "\x0A\x0D";

        /// <example>version=3.0.6.1 build=1340956745 platform=Windows\n\rerror id=0 msg=ok\n\r</example>
        internal static readonly Regex MessageRegex = new Regex(String.Format("^((.+?{0})*?)error id=.+?{0}", SeperatorRegex));
        /// <example>version=3.0.6.1 build=1340956745 platform=Windows\n\rerror id=0 msg=ok\n\r</example>
        internal static readonly Regex BannedRegex = new Regex(String.Format("^((.+?{0})*?)error id=(3331|3329).+", SeperatorRegex));

        // Sections
        private readonly List<Teamspeak3Section>     mSections = new List<Teamspeak3Section>(); 
        public Teamspeak3Section                     Section  { get { return mSections.FirstOrDefault(); } }
        public ReadOnlyCollection<Teamspeak3Section> Sections { get { return mSections.AsReadOnly(); } }  

        // Error Information
        public readonly Teamspeak3Group Error;
        public UInt32 Id           { get { return UInt32.Parse(Error["id"]); } }
        public String Message      { get { return Error["msg"]; } }
        public String ExtraMessage { get { return Error["extra_msg"]; } }


        /// <summary>
        /// Parses the raw response as a message response.
        /// </summary>
        /// <param name="raw">The raw response from the server.</param>
        /// <exception cref="System.ArgumentNullException"/>
        public Teamspeak3Message(String raw) : base(raw)
        {
            foreach (var strSection in raw.Split(Seperator.ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())) {
                if (strSection.StartsWith("error")) {
                    Error = new Teamspeak3Group(strSection.Remove(0, "error".Length));
                    continue;
                }
                mSections.Add(new Teamspeak3Section(strSection));
            }
        }
    }

    public sealed class Teamspeak3Section
    {
        // Constants
        public static readonly String Seperator      = "|";
        public static readonly String SeperatorRegex = "\x7C";

        // Groups
        private readonly List<Teamspeak3Group>     mGroups = new List<Teamspeak3Group>();
        public Teamspeak3Group                     Group  { get { return mGroups.FirstOrDefault(); } }
        public ReadOnlyCollection<Teamspeak3Group> Groups { get { return mGroups.AsReadOnly(); } }


        /// <summary>
        /// Represents a collection of objects in the response.
        /// </summary>
        /// <param name="raw">This section's raw response from the server.</param>
        public Teamspeak3Section(String raw)
        {
            foreach (var strGroup in raw.Split(Seperator.ToArray(), StringSplitOptions.RemoveEmptyEntries)) {
                mGroups.Add(new Teamspeak3Group(strGroup));
            }
        }
    }

    public sealed class Teamspeak3Group
    {
        // Constants
        public static readonly String Seperator      = " ";
        public static readonly String SeperatorRegex = "\x20";

        // Pairs
        private readonly List<Teamspeak3Pair>     mPairs = new List<Teamspeak3Pair>();
        public Teamspeak3Pair                     Pair  { get { return mPairs.FirstOrDefault(); } }
        public ReadOnlyCollection<Teamspeak3Pair> Pairs { get { return mPairs.AsReadOnly(); } }

        // Indexer
        public String this[String key] {
            get {
                var pair = mPairs.LastOrDefault(x => x.Key == key);
                return pair != null
                    ? pair.Value
                    : null;
        } }

        /// <summary>
        /// Represents a single object in the response.
        /// </summary>
        /// <param name="raw">This group's raw response from the server.</param>
        public Teamspeak3Group(String raw)
        {
            foreach (var strPair in raw.Split(Seperator.ToArray(), StringSplitOptions.RemoveEmptyEntries)) {
                mPairs.Add(new Teamspeak3Pair(strPair));
            }
        }
    }

    public sealed class Teamspeak3Pair
    {
        // Constants
        public static readonly String Seperator      = "=";
        public static readonly String SeperatorRegex = "\x3D";

        // Key / Value
        public readonly String Key;
        public readonly String Value;


        /// <summary>
        /// Represents a single variable in the response.
        /// </summary>
        /// <param name="raw">This variable's raw response from the server.</param>
        public Teamspeak3Pair(String raw)
        {
            var pair = raw.Split(Seperator.ToArray());
            Key = pair[0];
            if (pair.Length > 1) {
                Value = pair[1];
            }
        }
    }
}
