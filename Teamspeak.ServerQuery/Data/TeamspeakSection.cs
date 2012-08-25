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
    public class TeamspeakSection
    {
        // Section / Groups
        public String               RawSection { get; private set; }
        public Boolean              HasGroups  { get { return Groups.Count != 0; } }
        public TeamspeakGroup       Group      { get { return Groups.FirstOrDefault(); } }
        public List<TeamspeakGroup> Groups     { get; private set; }

        // Constants
        public static readonly String Seperator      = "|";
        public static readonly String SeperatorRegex = "\x7C";


        // Parses a section from a response into groups.
        public TeamspeakSection(String rawSection)
        {
            // Set Class Variables.
            RawSection = rawSection;
            Groups     = new List<TeamspeakGroup>();

            // Groups (separated by '|').
            foreach (String tGroup in rawSection.Split(Seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                Groups.Add(new TeamspeakGroup(tGroup));
        }
    }
}
