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
using System.Linq;
using System.Text.RegularExpressions;

namespace VoipManager.Teamspeak3.Communication
{
    public sealed class Teamspeak3Notification : Teamspeak3Response
    {
        // Examples: notifyclientleftview cfid=1 ctid=0 reasonid=8 reasonmsg=leaving clid=5\n\r
        internal static readonly Regex NotificationRegex = new Regex(String.Format("^notify.+?{0}", Teamspeak3Message.SeperatorRegex));

        public static readonly String Seperator      = Teamspeak3Group.Seperator;
        public static readonly String SeperatorRegex = Teamspeak3Group.SeperatorRegex;


        /// <summary>
        /// Information related to the notification.
        /// </summary>
        public readonly Teamspeak3Group Notification;
        public String Event               { get { return Notification.Keys.FirstOrDefault(); } }
        public String this[String key]    { get { return Notification[key]; } }
        public IEnumerable<String> Keys   { get { return Notification.Keys; } }
        public IEnumerable<String> Values { get { return Notification.Values; } }


        /// <summary>
        /// Parses the raw response as a notification response.
        /// </summary>
        /// <param name="raw">This variable's raw response from the server.</param>
        public Teamspeak3Notification(String rawText) : base(rawText)
        {
            Notification = new Teamspeak3Group(rawText.Remove(0, "notify".Length));
        }
    }
}
