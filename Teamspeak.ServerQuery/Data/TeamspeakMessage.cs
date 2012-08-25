/* ************************************************************************** *
 * Teamspeak 3 - Server Query API.
 * Copyright (C) 2012  Cameron Gunnin
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
using System.Linq;

namespace Teamspeak.ServerQuery.Data
{
    public class TeamspeakMessage
    {
        // Error Data
        public TeamspeakGroup Error         { get; private set; }
        public String         Id            { get { return Error["id"]; } }
        public String         Message       { get { return Error["msg"]; } }
        public String         ExtraMessage  { get { return Error["extra_msg"]; } }

        // Message / Sections
        public String                 RawMessage  { get; private set; }
        public Boolean                HasSections { get { return Sections.Count != 0; } }
        public TeamspeakSection       Section     { get { return Sections.FirstOrDefault(); } }
        public List<TeamspeakSection> Sections    { get; private set; }

        // Constants
        public static readonly String Seperator      = "\n\r";
        public static readonly String SeperatorRegex = "\x0A\x0D";


        // Parses a message into sections and the error data.
        public TeamspeakMessage(String rawResponse)
        {
            // Set Class Variables.
            Error      = new TeamspeakGroup("empty");
            RawMessage = rawResponse.Replace(Seperator, TeamspeakQuery.EscapeString(Seperator));
            Sections   = new List<TeamspeakSection>();

            // Sections (separated by '\n\r').
            foreach (String tSection in rawResponse.Split(Seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                if (tSection.Contains("error id=")) Error = new TeamspeakGroup(tSection);
                else                                Sections.Add(new TeamspeakSection(tSection));
        }
    }
}
