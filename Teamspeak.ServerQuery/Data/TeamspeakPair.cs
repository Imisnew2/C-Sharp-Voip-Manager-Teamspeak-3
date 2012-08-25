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

namespace Teamspeak.ServerQuery.Data
{
    public class TeamspeakPair
    {
        // Pair / Key-Value
        public String RawPair { get; private set; }
        public String Key     { get; private set; }
        public String Value   { get; private set; }

        // Constants
        public static readonly String Seperator      = "=";
        public static readonly String SeperatorRegex = "\x3D";


        // Parses a pair from a response into a key-value pair.
        public TeamspeakPair(String rawPair)
        {
            // Set Class Variables.
            RawPair = rawPair;
            Key     = TeamspeakQuery.UnescapeString(rawPair);

            // Key-Value Pairs (separated by '=').
            if (rawPair.Contains(Seperator)) {
                String[] tPair = rawPair.Split(Seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                Key   = TeamspeakQuery.UnescapeString(tPair[0]);
                Value = TeamspeakQuery.UnescapeString(tPair[1]);
            }
        }
    }
}
