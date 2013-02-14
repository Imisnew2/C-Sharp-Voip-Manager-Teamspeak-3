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
using System.Linq;
using System.Text;

namespace VoipManager
{
    public class Teamspeak3Settings
    {
        // Used in Teamspeak3Connection.
        public Int32   ConnectTimeout   { get; set; }
        public Boolean ParallelRequests { get; set; }

        // Used in Teamspeak3VoipManager.
        //public Int32? RefreshClientListInterval  { get; set; }
        //public Int32? RefreshChannelListInterval { get; set; }
        //public Int32? RefreshBanListInterval     { get; set; }

        public Teamspeak3Settings()
        {
            ParallelRequests = false;
            ConnectTimeout   = 10000;

            //RefreshClientListInterval  = null;
            //RefreshChannelListInterval = null;
            //RefreshBanListInterval     = null;
        }
    }
}