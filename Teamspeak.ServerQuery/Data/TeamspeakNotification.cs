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
    public class TeamspeakNotification
    {
        // Notification Data
        public String Event { get; private set; }

        // Group / Values
        public String              RawNotification { get; private set; }
        public Boolean             HasPairs        { get { return Pairs.Count != 0; } }
        public List<TeamspeakPair> Pairs           { get; private set; }

        // Extra accessor
        public String this[String key] {
            get {
                return Pairs.Exists(x => x.Key == key) ? 
                       Pairs.Single(x => x.Key == key).Value :
                       null;
        } }

        // Constants
        public static readonly String Seperator      = " ";
        public static readonly String SeperatorRegex = "\x20";


        // Parses a notification into pairs.
        public TeamspeakNotification(String rawNotification)
        {
            // Set Class Variables.
            RawNotification = rawNotification.Replace(TeamspeakMessage.Seperator, TeamspeakQuery.EscapeString(TeamspeakMessage.Seperator));
            Pairs           = new List<TeamspeakPair>();

            // Pairs (separated by ' ').
            foreach (String tPair in rawNotification.Split(Seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
                if (Event == null)
                    Event = tPair;
                else {
                    TeamspeakPair tNewPair = new TeamspeakPair(tPair);
                    TeamspeakPair tOldPair = Pairs.SingleOrDefault(x => x.Key == tNewPair.Key);
                    if (tOldPair != null) Pairs.Remove(tOldPair);
                    Pairs.Add(tNewPair);
                }
            }
        }
    }
}
