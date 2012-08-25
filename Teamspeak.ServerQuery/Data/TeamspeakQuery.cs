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
using System.Text;

namespace Teamspeak.ServerQuery
{
    // Contains the information to build a teamspeak query.
    public sealed class TeamspeakQuery
    {
        // Public
        public String Command { get; private set; }

        // Private
        private Dictionary<String, String> mParameters = null;
        private List<String>               mOptions    = null;


        // Creates a Teamspeak Query for the specified command.
        public TeamspeakQuery(String command)
        {
            Command     = command;
            mParameters = new Dictionary<String, String>();
            mOptions    = new List<String>();
        }

        // Modifies the Teamspeak Query.
        public void AddParameter(String key, String value)
        {
            String tKey   = key.Trim();
            String tValue = value.Trim();
            if (!String.IsNullOrEmpty(tKey) && !String.IsNullOrEmpty(tValue))
                if (!mParameters.ContainsKey(tKey))
                    mParameters.Add(EscapeString(tKey), EscapeString(tValue));
        }
        public void AddOption(String option)
        {
            String tOption = option.Trim();
            if (!String.IsNullOrEmpty(tOption))
                mOptions.Add(EscapeString(tOption));
        }

        // Returns the query object as a string or bytes.
        public override String ToString()
        {
            String query = Command;
            foreach (KeyValuePair<String, String> p in mParameters)
                query += String.Format(" {0}={1}", p.Key, p.Value);
            foreach (String o in mOptions)
                query += String.Format(" -{0}", o);
            return query + "\n";
        }


        // Escapes and Unescapes strings for use in Teamspeak Queries.
        public static String EscapeString(String text)
        {
            return text.Replace("\\", @"\\")
                       .Replace("/",  @"\/")
                       .Replace(" ",  @"\s")
                       .Replace("|",  @"\p")
                       .Replace("\a", @"\a")
                       .Replace("\b", @"\b")
                       .Replace("\f", @"\f")
                       .Replace("\n", @"\n")
                       .Replace("\r", @"\r")
                       .Replace("\t", @"\t")
                       .Replace("\v", @"\v");
        }
        public static String UnescapeString(String text)
        {
            return text.Replace(@"\\", "\\")
                       .Replace(@"\/", "/")
                       .Replace(@"\s", " ")
                       .Replace(@"\p", "|")
                       .Replace(@"\a", "\a")
                       .Replace(@"\b", "\b")
                       .Replace(@"\f", "\f")
                       .Replace(@"\n", "\n")
                       .Replace(@"\r", "\r")
                       .Replace(@"\t", "\t")
                       .Replace(@"\v", "\v");
        }

        // Instance Information.
        public static TeamspeakQuery BuildVersion()
        {
            return new TeamspeakQuery("version");
        }
        public static TeamspeakQuery BuildHostInfo()
        {
            return new TeamspeakQuery("hostinfo");
        }
        public static TeamspeakQuery BuildInstanceInfo()
        {
            return new TeamspeakQuery("instanceinfo");
        }

        // Authorization & Selection.
        public static TeamspeakQuery BuildQuit()
        {
            return new TeamspeakQuery("quit");
        }
        public static TeamspeakQuery BuildLogin(String username, String password)
        {
            TeamspeakQuery tsLogin = new TeamspeakQuery("login");
            tsLogin.AddParameter("client_login_name", username);
            tsLogin.AddParameter("client_login_password", password);
            return tsLogin;
        }
        public static TeamspeakQuery BuildLogout()
        {
            return new TeamspeakQuery("logout");
        }
        public static TeamspeakQuery BuildUseId(Int32 serverId)
        {
            TeamspeakQuery tsUseId = new TeamspeakQuery("use");
            tsUseId.AddParameter("sid", serverId.ToString());
            return tsUseId;
        }
        public static TeamspeakQuery BuildUsePort(Int32 port)
        {
            TeamspeakQuery tsUsePort = new TeamspeakQuery("use");
            tsUsePort.AddParameter("port", port.ToString());
            return tsUsePort;
        }

        // Server Queries.
        public static TeamspeakQuery BuildServerList()
        {
            return new TeamspeakQuery("serverlist");
        }
        public static TeamspeakQuery BuildServerInfo()
        {
            return new TeamspeakQuery("serverinfo");
        }
        public static TeamspeakQuery BuildRequestConnectionInfo()
        {
            return new TeamspeakQuery("serverrequestconnectioninfo");
        }
        public static TeamspeakQuery BuildServerGetId(Int32 port)
        {
            TeamspeakQuery tsServerGetId = new TeamspeakQuery("serveridgetbyport");
            tsServerGetId.AddParameter("virtualserver_port", port.ToString());
            return tsServerGetId;
        }
        public static TeamspeakQuery BuildServerCreate(String name)
        {
            TeamspeakQuery tsServerCreate = new TeamspeakQuery("servercreate");
            tsServerCreate.AddParameter("virtualserver_name", name);
            return tsServerCreate;
        }
        public static TeamspeakQuery BuildServerDelete(Int32 serverId)
        {
            TeamspeakQuery tsServerDelete = new TeamspeakQuery("serverdelete");
            tsServerDelete.AddParameter("sid", serverId.ToString());
            return tsServerDelete;
        }
        public static TeamspeakQuery BuildServerStart(Int32 serverId)
        {
            TeamspeakQuery tsServerStart = new TeamspeakQuery("serverstart");
            tsServerStart.AddParameter("sid", serverId.ToString());
            return tsServerStart;
        }
        public static TeamspeakQuery BuildServerStop(Int32 serverId)
        {
            TeamspeakQuery tsServerStop = new TeamspeakQuery("serverstop");
            tsServerStop.AddParameter("sid", serverId.ToString());
            return tsServerStop;
        }

        // Channel Queries.
        public static TeamspeakQuery BuildChannelList()
        {
            return new TeamspeakQuery("channellist");
        }
        public static TeamspeakQuery BuildChannelInfo(Int32 channelId)
        {
            TeamspeakQuery tsChannelInfo = new TeamspeakQuery("channelinfo");
            tsChannelInfo.AddParameter("cid", channelId.ToString());
            return tsChannelInfo;
        }
        public static TeamspeakQuery BuildChannelFind(String name)
        {
            TeamspeakQuery tsChannelFind = new TeamspeakQuery("channelfind");
            tsChannelFind.AddParameter("pattern", name);
            return tsChannelFind;
        }
        public static TeamspeakQuery BuildChannelMove(Int32 channelId, Int32 parentId, Int32? order = null)
        {
            TeamspeakQuery tsChannelMove = new TeamspeakQuery("channelmove");
            tsChannelMove.AddParameter("cid", channelId.ToString());
            tsChannelMove.AddParameter("cpid", parentId.ToString());
            if (order.HasValue) tsChannelMove.AddParameter("order", order.ToString());
            return tsChannelMove;
        }
        public static TeamspeakQuery BuildChannelCreate(String name, Int32? parentId = null, Int32? order = null)
        {
            TeamspeakQuery tsChannelCreate = new TeamspeakQuery("channelcreate");
            tsChannelCreate.AddParameter("channel_name", name);
            if (parentId.HasValue) tsChannelCreate.AddParameter("cpid", parentId.ToString());
            if (order.HasValue)    tsChannelCreate.AddParameter("channel_order", order.ToString());
            return tsChannelCreate;
        }
        public static TeamspeakQuery BuildChannelDelete(Int32 channelId, Boolean force = false)
        {
            TeamspeakQuery tsChannelDelete = new TeamspeakQuery("channeldelete");
            tsChannelDelete.AddParameter("cid", channelId.ToString());
            tsChannelDelete.AddParameter("force", (force) ? "1" : "0");
            return tsChannelDelete;
        }

        // Client Queries.
        public static TeamspeakQuery BuildClientList()
        {
            return new TeamspeakQuery("clientlist");
        }
        public static TeamspeakQuery BuildClientInfo(Int32 clientId)
        {
            TeamspeakQuery tsClientInfo = new TeamspeakQuery("clientinfo");
            tsClientInfo.AddParameter("clid", clientId.ToString());
            return tsClientInfo;
        }
        public static TeamspeakQuery BuildClientFind(String name)
        {
            TeamspeakQuery tsClientFind = new TeamspeakQuery("clientfind");
            tsClientFind.AddParameter("pattern", name);
            return tsClientFind;
        }
        public static TeamspeakQuery BuildClientMove(Int32 clientId, Int32 channelId)
        {
            TeamspeakQuery tsClientMove = new TeamspeakQuery("clientmove");
            tsClientMove.AddParameter("clid", clientId.ToString());
            tsClientMove.AddParameter("cid", channelId.ToString());
            return tsClientMove;
        }
        public static TeamspeakQuery BuildClientKick(Int32 clientId, Int32 reasonId, String message = null)
        {
            TeamspeakQuery tsClientKick = new TeamspeakQuery("clientkick");
            tsClientKick.AddParameter("clid", clientId.ToString());
            tsClientKick.AddParameter("reasonid", reasonId.ToString());
            if (!String.IsNullOrWhiteSpace(message)) tsClientKick.AddParameter("reasonmsg", message);
            return tsClientKick;
        }
        public static TeamspeakQuery BuildClientPoke(Int32 clientId, String message)
        {
            TeamspeakQuery tsClientPoke = new TeamspeakQuery("clientpoke");
            tsClientPoke.AddParameter("clid", clientId.ToString());
            tsClientPoke.AddParameter("msg", message.ToString());
            return tsClientPoke;
        }

        // Ban Queries.
        public static TeamspeakQuery BuildBanList()
        {
            return new TeamspeakQuery("banlist");
        }
        public static TeamspeakQuery BuildBanDel(Int32 banId)
        {
            TeamspeakQuery tsBuildBanDel = new TeamspeakQuery("bandel");
            tsBuildBanDel.AddParameter("banid", banId.ToString());
            return tsBuildBanDel;
        }
        public static TeamspeakQuery BuildBanDelAll()
        {
            return new TeamspeakQuery("bandelall");
        }
        public static TeamspeakQuery BuildBanAddIp(String ip, String message, Int32? seconds = null)
        {
            TeamspeakQuery tsBuildBanAddIp = new TeamspeakQuery("banadd");
            tsBuildBanAddIp.AddParameter("ip", ip);
            tsBuildBanAddIp.AddParameter("banreason", message);
            if (seconds.HasValue) tsBuildBanAddIp.AddParameter("time", seconds.ToString());
            return tsBuildBanAddIp;
        }
        public static TeamspeakQuery BuildBanAddUid(String uid, String message, Int32? seconds = null)
        {
            TeamspeakQuery tsBuildBanAddIp = new TeamspeakQuery("banadd");
            tsBuildBanAddIp.AddParameter("uid", uid);
            tsBuildBanAddIp.AddParameter("banreason", message);
            if (seconds.HasValue) tsBuildBanAddIp.AddParameter("time", seconds.ToString());
            return tsBuildBanAddIp;
        }
        public static TeamspeakQuery BuildBanAddName(String name, String message, Int32? seconds = null)
        {
            TeamspeakQuery tsBuildBanAddIp = new TeamspeakQuery("banadd");
            tsBuildBanAddIp.AddParameter("name", name);
            tsBuildBanAddIp.AddParameter("banreason", message);
            if (seconds.HasValue) tsBuildBanAddIp.AddParameter("time", seconds.ToString());
            return tsBuildBanAddIp;
        }
        public static TeamspeakQuery BuildBanClient(Int32 clientId, String message, Int32? seconds = null)
        {
            TeamspeakQuery tsBanClient = new TeamspeakQuery("banclient");
            tsBanClient.AddParameter("clid", clientId.ToString());
            tsBanClient.AddParameter("banreason", message);
            if (seconds.HasValue) tsBanClient.AddParameter("time", seconds.ToString());
            return tsBanClient;
        }

        // Misc.
        public static TeamspeakQuery BuildWhoAmI()
        {
            return new TeamspeakQuery("whoami");
        }
        public static TeamspeakQuery BuildGlobalMessage(String message)
        {
            TeamspeakQuery tsGlobalMessage = new TeamspeakQuery("gm");
            tsGlobalMessage.AddParameter("msg", message);
            return tsGlobalMessage;
        }
        public static TeamspeakQuery BuildSendTextMessage(Int32 mode, Int32 target, String message)
        {
            TeamspeakQuery tsSendTextMessage = new TeamspeakQuery("sendtextmessage");
            tsSendTextMessage.AddParameter("targetmode", mode.ToString());
            tsSendTextMessage.AddParameter("target", target.ToString());
            tsSendTextMessage.AddParameter("msg", message);
            return tsSendTextMessage;
        }

        // Use Type Enumeration.
        public enum UseType { Id, Port };
    }
}
