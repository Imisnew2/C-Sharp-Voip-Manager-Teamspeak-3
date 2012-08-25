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
using Teamspeak.ServerQuery.Data;

namespace Teamspeak.ServerQuery.Objects
{
    public class TeamspeakServer
    {
        #region Server Data Keys

        private const String KEY_SERVER_NAME                 = "virtualserver_name";
        private const String KEY_SERVER_ID                   = "virtualserver_id";
        private const String KEY_SERVER_PORT                 = "virtualserver_port";
        private const String KEY_SERVER_MACHINE_ID           = "virtualserver_machine_id";
        private const String KEY_SERVER_STATUS               = "virtualserver_status";
        private const String KEY_SERVER_UPTIME               = "virtualserver_uptime";
        private const String KEY_SERVER_CLIENTS_ONLINE       = "virtualserver_clientsonline";
        private const String KEY_SERVER_QUERY_CLIENTS_ONLINE = "virtualserver_queryclientsonline";
        private const String KEY_SERVER_MAX_CLIENTS          = "virtualserver_maxclients";
        private const String KEY_SERVER_AUTO_START           = "virtualserver_autostart";

        #endregion
        
        public class NormalInfo
        {
            // Data received from a "serverlist" command.
            public String   Name               = null;
            public String   Status             = null;
            public Int32?   Id                 = null;
            public Int32?   Port               = null;
            public Int32?   MachineId          = null;
            public Int32?   UpTime             = null;
            public Int32?   ClientsOnline      = null;
            public Int32?   QueryClientsOnline = null;
            public Int32?   MaxClients         = null;
            public Boolean? AutoStart          = null;

            // Sets the basic information from a response.
            public void SetData(TeamspeakGroup info, Boolean reset)
            {
                String  sValue;
                Int32   iValue;
                
                if ((sValue = info[KEY_SERVER_NAME])   != null) Name   = sValue; else if (reset) Name   = null;
                if ((sValue = info[KEY_SERVER_STATUS]) != null) Status = sValue; else if (reset) Status = null;
                if ((sValue = info[KEY_SERVER_ID])                   != null && Int32.TryParse(sValue, out iValue)) Id                 = iValue; else if (reset) Id                 = null;
                if ((sValue = info[KEY_SERVER_PORT])                 != null && Int32.TryParse(sValue, out iValue)) Port               = iValue; else if (reset) Port               = null;
                if ((sValue = info[KEY_SERVER_MACHINE_ID])           != null && Int32.TryParse(sValue, out iValue)) MachineId          = iValue; else if (reset) MachineId          = null;
                if ((sValue = info[KEY_SERVER_UPTIME])               != null && Int32.TryParse(sValue, out iValue)) UpTime             = iValue; else if (reset) UpTime             = null;
                if ((sValue = info[KEY_SERVER_CLIENTS_ONLINE])       != null && Int32.TryParse(sValue, out iValue)) ClientsOnline      = iValue; else if (reset) ClientsOnline      = null;
                if ((sValue = info[KEY_SERVER_QUERY_CLIENTS_ONLINE]) != null && Int32.TryParse(sValue, out iValue)) QueryClientsOnline = iValue; else if (reset) QueryClientsOnline = null;
                if ((sValue = info[KEY_SERVER_MAX_CLIENTS])          != null && Int32.TryParse(sValue, out iValue)) MaxClients         = iValue; else if (reset) MaxClients         = null;
                if ((sValue = info[KEY_SERVER_AUTO_START]) != null) AutoStart = (sValue == "1") ? true : false; else if (reset) AutoStart = null;
            }
        }

        public NormalInfo Normal { get; private set; }

        // Initializes a server with default values.
        public TeamspeakServer()
        {
            Normal = new NormalInfo();
        }
        // Initializes a server with the specified data.
        public TeamspeakServer(TeamspeakGroup info, Boolean reset = true) : this()
        {
            Normal.SetData(info, reset);
        }
    }
}
