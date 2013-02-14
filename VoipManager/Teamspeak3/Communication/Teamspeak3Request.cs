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
using System.Text;
using VoipManager.Communication;

namespace VoipManager.Teamspeak3.Communication
{
    public class Teamspeak3Request : IRequest
    {
        // Arguments
        private readonly Dictionary<String, String> mParameters = new Dictionary<String, String>();
        private readonly List<String>               mOptions    = new List<String>();

        // We only want to expose the normalized values for a Teamspeak 3 Request.
        public String Command { get; private set; }
        public String Raw {
            get {
                var request = Command;
                request = mParameters.Aggregate(
                    request,
                    (x, cur) => x + String.Format(" {0}={1}", cur.Key, cur.Value));
                request = mOptions.Aggregate(
                    request,
                    (x, cur) => x + String.Format(" -{0}", cur));
                return request + "\n";
            }
        }

        // If they cast it to a IRequest, they can get the 'raw' values.
        Byte[] IRequest.Raw { 
            get { return Encoding.Default.GetBytes(Raw); }
        }

        /// <summary>
        /// Creates a request with the specified command.
        /// </summary>
        /// <param name="command">The action to request from the server.</param>
        public Teamspeak3Request(String command)
        {
            Command = command;
        }


        /// <summary>
        /// Adds an argument in the form of "key=value".
        /// </summary>
        /// <exception cref="System.ArgumentException"/>
        public void AddParameter(String key, String value)
        {
            if (String.IsNullOrWhiteSpace(key) || String.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException("Neither the key nor value can be null or empty.");
            }

            var tKey   = EscapeString(key.Trim());
            var tValue = EscapeString(value.Trim());
            if (!mParameters.ContainsKey(tKey)) {
                mParameters.Add(tKey, tValue);
            }
            else {
                mParameters[tKey] = tValue;
            }
        }

        /// <summary>
        /// Adds an option in the form of "-option".
        /// </summary>
        /// <exception cref="System.ArgumentException"/>
        public void AddOption(String option)
        {
            if (String.IsNullOrWhiteSpace(option)) {
                throw new ArgumentException("The option can not be null.");
            }

            var tOption = EscapeString(option.Trim());
            if (!mOptions.Contains(tOption)) {
                mOptions.Add(tOption);
            }
        }

        /// <summary>
        /// Removes a parameter with the specified key.
        /// </summary>
        public void RemoveParameter(String key) { mParameters.Remove(key); }

        /// <summary>
        /// Removes an option.
        /// </summary>
        public void RemoveOption(String option) { mOptions.Remove(option); }


        /// <summary>
        /// Used to escape a string in order to send it to a Teamspeak 3 server.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"/>
        public static String EscapeString(String text)
        {
            if (text == null) {
                throw new ArgumentNullException("text");
            }

            return text.Replace("\\", @"\\")
                       .Replace("/", @"\/")
                       .Replace(" ", @"\s")
                       .Replace("|", @"\p")
                       .Replace("\a", @"\a")
                       .Replace("\b", @"\b")
                       .Replace("\f", @"\f")
                       .Replace("\n", @"\n")
                       .Replace("\r", @"\r")
                       .Replace("\t", @"\t")
                       .Replace("\v", @"\v");
        }

        /// <summary>
        /// Used to unescape the response string from a Teamspeak 3 server.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"/>
        public static String UnescapeString(String text)
        {
            if (text == null) {
                throw new ArgumentNullException("text");
            }

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
        public static Teamspeak3Request BuildVersion()
        {
            return new Teamspeak3Request("version");
        }
        public static Teamspeak3Request BuildHostInfo()
        {
            return new Teamspeak3Request("hostinfo");
        }
        public static Teamspeak3Request BuildInstanceInfo()
        {
            return new Teamspeak3Request("instanceinfo");
        }

        // Authorization & Selection.
        public static Teamspeak3Request BuildQuit()
        {
            return new Teamspeak3Request("quit");
        }
        public static Teamspeak3Request BuildLogin(String username, String password)
        {
            var tsLogin = new Teamspeak3Request("login");
            tsLogin.AddParameter("client_login_name",     username);
            tsLogin.AddParameter("client_login_password", password);
            return tsLogin;
        }
        public static Teamspeak3Request BuildLogout()
        {
            return new Teamspeak3Request("logout");
        }
        public static Teamspeak3Request BuildUseId(Int32 serverId)
        {
            var tsUseId = new Teamspeak3Request("use");
            tsUseId.AddParameter("sid", serverId.ToString());
            return tsUseId;
        }
        public static Teamspeak3Request BuildUsePort(Int32 port)
        {
            var tsUsePort = new Teamspeak3Request("use");
            tsUsePort.AddParameter("port", port.ToString());
            return tsUsePort;
        }

        // Server Queries.
        public static Teamspeak3Request BuildServerList()
        {
            return new Teamspeak3Request("serverlist");
        }
        public static Teamspeak3Request BuildServerInfo()
        {
            return new Teamspeak3Request("serverinfo");
        }
        public static Teamspeak3Request BuildRequestConnectionInfo()
        {
            return new Teamspeak3Request("serverrequestconnectioninfo");
        }
        public static Teamspeak3Request BuildServerGetId(Int32 port)
        {
            var tsServerGetId = new Teamspeak3Request("serveridgetbyport");
            tsServerGetId.AddParameter("virtualserver_port", port.ToString());
            return tsServerGetId;
        }
        public static Teamspeak3Request BuildServerCreate(String name)
        {
            var tsServerCreate = new Teamspeak3Request("servercreate");
            tsServerCreate.AddParameter("virtualserver_name", name);
            return tsServerCreate;
        }
        public static Teamspeak3Request BuildServerDelete(Int32 serverId)
        {
            var tsServerDelete = new Teamspeak3Request("serverdelete");
            tsServerDelete.AddParameter("sid", serverId.ToString());
            return tsServerDelete;
        }
        public static Teamspeak3Request BuildServerStart(Int32 serverId)
        {
            var tsServerStart = new Teamspeak3Request("serverstart");
            tsServerStart.AddParameter("sid", serverId.ToString());
            return tsServerStart;
        }
        public static Teamspeak3Request BuildServerStop(Int32 serverId)
        {
            var tsServerStop = new Teamspeak3Request("serverstop");
            tsServerStop.AddParameter("sid", serverId.ToString());
            return tsServerStop;
        }

        // Channel Queries.
        public static Teamspeak3Request BuildChannelList()
        {
            return new Teamspeak3Request("channellist");
        }
        public static Teamspeak3Request BuildChannelInfo(Int32 channelId)
        {
            var tsChannelInfo = new Teamspeak3Request("channelinfo");
            tsChannelInfo.AddParameter("cid", channelId.ToString());
            return tsChannelInfo;
        }
        public static Teamspeak3Request BuildChannelFind(String name)
        {
            var tsChannelFind = new Teamspeak3Request("channelfind");
            tsChannelFind.AddParameter("pattern", name);
            return tsChannelFind;
        }
        public static Teamspeak3Request BuildChannelMove(Int32 channelId, Int32 parentId, Int32? order = null)
        {
            var tsChannelMove = new Teamspeak3Request("channelmove");
            tsChannelMove.AddParameter("cid", channelId.ToString());
            tsChannelMove.AddParameter("cpid", parentId.ToString());
            if (order.HasValue) tsChannelMove.AddParameter("order", order.ToString());
            return tsChannelMove;
        }
        public static Teamspeak3Request BuildChannelCreate(String name, Int32? parentId = null, Int32? order = null)
        {
            var tsChannelCreate = new Teamspeak3Request("channelcreate");
            tsChannelCreate.AddParameter("channel_name", name);
            if (parentId.HasValue) tsChannelCreate.AddParameter("cpid", parentId.ToString());
            if (order.HasValue)    tsChannelCreate.AddParameter("channel_order", order.ToString());
            return tsChannelCreate;
        }
        public static Teamspeak3Request BuildChannelDelete(Int32 channelId, Boolean force = false)
        {
            var tsChannelDelete = new Teamspeak3Request("channeldelete");
            tsChannelDelete.AddParameter("cid", channelId.ToString());
            tsChannelDelete.AddParameter("force", (force) ? "1" : "0");
            return tsChannelDelete;
        }

        // Client Queries.
        public static Teamspeak3Request BuildClientList()
        {
            return new Teamspeak3Request("clientlist");
        }
        public static Teamspeak3Request BuildClientInfo(Int32 clientId)
        {
            var tsClientInfo = new Teamspeak3Request("clientinfo");
            tsClientInfo.AddParameter("clid", clientId.ToString());
            return tsClientInfo;
        }
        public static Teamspeak3Request BuildClientFind(String name)
        {
            var tsClientFind = new Teamspeak3Request("clientfind");
            tsClientFind.AddParameter("pattern", name);
            return tsClientFind;
        }
        public static Teamspeak3Request BuildClientMove(Int32 clientId, Int32 channelId)
        {
            var tsClientMove = new Teamspeak3Request("clientmove");
            tsClientMove.AddParameter("clid", clientId.ToString());
            tsClientMove.AddParameter("cid", channelId.ToString());
            return tsClientMove;
        }
        public static Teamspeak3Request BuildClientKick(Int32 clientId, Teamspeak3.Objects.KickMode type, String message = null)
        {
            var tsClientKick = new Teamspeak3Request("clientkick");
            tsClientKick.AddParameter("clid", clientId.ToString());
            tsClientKick.AddParameter("reasonid", ((Int32)type).ToString());
            if (!String.IsNullOrWhiteSpace(message)) tsClientKick.AddParameter("reasonmsg", message);
            return tsClientKick;
        }
        public static Teamspeak3Request BuildClientPoke(Int32 clientId, String message)
        {
            var tsClientPoke = new Teamspeak3Request("clientpoke");
            tsClientPoke.AddParameter("clid", clientId.ToString());
            tsClientPoke.AddParameter("msg", message);
            return tsClientPoke;
        }

        // Ban Queries.
        public static Teamspeak3Request BuildBanList()
        {
            return new Teamspeak3Request("banlist");
        }
        public static Teamspeak3Request BuildBanDel(Int32 banId)
        {
            var tsBuildBanDel = new Teamspeak3Request("bandel");
            tsBuildBanDel.AddParameter("banid", banId.ToString());
            return tsBuildBanDel;
        }
        public static Teamspeak3Request BuildBanDelAll()
        {
            return new Teamspeak3Request("bandelall");
        }
        public static Teamspeak3Request BuildBanAddIp(String ip, String message, Int32? seconds = null)
        {
            var tsBuildBanAddIp = new Teamspeak3Request("banadd");
            tsBuildBanAddIp.AddParameter("ip", ip);
            tsBuildBanAddIp.AddParameter("banreason", message);
            if (seconds.HasValue) tsBuildBanAddIp.AddParameter("time", seconds.ToString());
            return tsBuildBanAddIp;
        }
        public static Teamspeak3Request BuildBanAddUid(String uid, String message, Int32? seconds = null)
        {
            var tsBuildBanAddIp = new Teamspeak3Request("banadd");
            tsBuildBanAddIp.AddParameter("uid", uid);
            tsBuildBanAddIp.AddParameter("banreason", message);
            if (seconds.HasValue) tsBuildBanAddIp.AddParameter("time", seconds.ToString());
            return tsBuildBanAddIp;
        }
        public static Teamspeak3Request BuildBanAddName(String name, String message, Int32? seconds = null)
        {
            var tsBuildBanAddIp = new Teamspeak3Request("banadd");
            tsBuildBanAddIp.AddParameter("name", name);
            tsBuildBanAddIp.AddParameter("banreason", message);
            if (seconds.HasValue) tsBuildBanAddIp.AddParameter("time", seconds.ToString());
            return tsBuildBanAddIp;
        }
        public static Teamspeak3Request BuildBanClient(Int32 clientId, String message, Int32? seconds = null)
        {
            var tsBanClient = new Teamspeak3Request("banclient");
            tsBanClient.AddParameter("clid", clientId.ToString());
            tsBanClient.AddParameter("banreason", message);
            if (seconds.HasValue) tsBanClient.AddParameter("time", seconds.ToString());
            return tsBanClient;
        }

        // Misc.
        public static Teamspeak3Request BuildWhoAmI()
        {
            return new Teamspeak3Request("whoami");
        }
        public static Teamspeak3Request BuildGlobalMessage(String message)
        {
            var tsGlobalMessage = new Teamspeak3Request("gm");
            tsGlobalMessage.AddParameter("msg", message);
            return tsGlobalMessage;
        }
        public static Teamspeak3Request BuildSendTextMessage(Teamspeak3.Objects.TextMessageTarget target, Int32 targetId, String message)
        {
            var tsSendTextMessage = new Teamspeak3Request("sendtextmessage");
            tsSendTextMessage.AddParameter("targetmode", ((Int32)target).ToString());
            tsSendTextMessage.AddParameter("target", targetId.ToString());
            tsSendTextMessage.AddParameter("msg", message);
            return tsSendTextMessage;
        }
    }
}
