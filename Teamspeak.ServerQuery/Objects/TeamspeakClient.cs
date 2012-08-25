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
    public class TeamspeakClient
    {
        #region Client Data Keys

        private const String KEY_NAME = "client_nickname";
        private const String KEY_ID   = "clid";

        private const String KEY_DATABASE_ID = "client_database_id";
        private const String KEY_CHANNEL_ID  = "cid";
        private const String KEY_TYPE        = "client_type";

        private const String KEY_LOGIN_NAME                  = "client_login_name";
        private const String KEY_UNIQUE_ID                   = "client_unique_identifier";
        private const String KEY_IP_ADDRESS                  = "connection_client_ip";
        private const String KEY_CLIENT_VERSION              = "client_version";
        private const String KEY_CLIENT_PLATFORM             = "client_platform";
        private const String KEY_CLIENT_DESCRIPTION          = "client_description";
        private const String KEY_CLIENT_COUNTRY              = "client_country";
        private const String KEY_CLIENT_META_DATA            = "client_meta_data";
        private const String KEY_DEFAULT_CHANNEL             = "client_default_channel";
        private const String KEY_FLAG_AVATAR                 = "client_flag_avatar";
        private const String KEY_AWAY_MESSAGE                = "client_away_message";
        private const String KEY_TALK_MESSAGE                = "client_talk_request_msg";
        private const String KEY_PHONETIC_NICK               = "client_nickname_phonetic";
        private const String KEY_DEFAULT_TOKEN               = "client_default_token";
        private const String KEY_BASE64_HASH                 = "client_base64HashClientUID";
        private const String KEY_CHANNEL_GROUP_ID            = "client_channel_group_id";
        private const String KEY_SERVER_GROUP_ID             = "client_servergroups";
        private const String KEY_CONNECTION_TIME             = "connection_connected_time";
        private const String KEY_IDLE_TIME                   = "client_idle_time";
        private const String KEY_CREATION_TIME               = "client_created";
        private const String KEY_LAST_CONNECTED              = "client_lastconnected";
        private const String KEY_TOTAL_CONNECTIONS           = "client_totalconnections";
        private const String KEY_TALK_POWER                  = "client_talk_power";
        private const String KEY_NEEDED_QUERY_VIEW_POWER     = "client_needed_serverquery_view_power";
        private const String KEY_UNREAD_MESSAGES             = "client_unread_messages";
        private const String KEY_ICON_ID                     = "client_icon_id";
        private const String KEY_BYTES_UPLOADED_MONTH        = "client_month_bytes_uploaded";
        private const String KEY_BYTES_DOWNLOADED_MONTH      = "client_month_bytes_downloaded";
        private const String KEY_BYTES_UPLOADED_TOTAL        = "client_total_bytes_uploaded";
        private const String KEY_BYTES_DOWNLOADED_TOTAL      = "client_total_bytes_downloaded";
        private const String KEY_FILES_BANDWIDTH_SENT        = "connection_filetransfer_bandwidth_sent";
        private const String KEY_FILES_BANDWIDTH_RECEIVED    = "connection_filetransfer_bandwidth_received";
        private const String KEY_PACKETS_SENT                = "connection_packets_sent_total";
        private const String KEY_PACKETS_RECEIVED            = "connection_packets_received_total";
        private const String KEY_BYTES_SENT                  = "connection_bytes_sent_total";
        private const String KEY_BYTES_RECEIVED              = "connection_bytes_received_total";
        private const String KEY_BANDWIDTH_SENT_LAST_SEC     = "connection_bandwidth_sent_last_second_total";
        private const String KEY_BANDWIDTH_RECEIVED_LAST_SEC = "connection_bandwidth_received_last_second_total";
        private const String KEY_BANDWIDTH_SENT_LAST_MIN     = "connection_bandwidth_sent_last_minute_total";
        private const String KEY_BANDWIDTH_RECEIVED_LAST_MIN = "connection_bandwidth_received_last_minute_total";
        private const String KEY_IS_CHANNEL_COMMANDER        = "client_is_channel_commander";
        private const String KEY_INPUT_MUTED                 = "client_input_muted";
        private const String KEY_OUTPUT_MUTED                = "client_output_muted";
        private const String KEY_OUTPUT_MUTED_ONLY           = "client_outputonly_muted";
        private const String KEY_INPUT_HARDWARE              = "client_input_hardware";
        private const String KEY_OUTPUT_HARDWARE             = "client_output_hardware";
        private const String KEY_IS_RECORDING                = "client_is_recording";
        private const String KEY_IS_AWAY                     = "client_away";
        private const String KEY_TALK_REQUEST                = "client_talk_request";
        private const String KEY_IS_TALKER                   = "client_is_talker";
        private const String KEY_IS_PRIORITY                 = "client_is_priority_speaker";

        #endregion

        public class BasicInfo
        {
            // Data received from a "clientfind" command.
            public String Name = null;
            public Int32? Id   = null;

            // Sets the basic information from a response.
            public void SetData(TeamspeakGroup info, Boolean reset)
            {
                String sValue;
                Int32  iValue;

                if ((sValue = info[KEY_NAME]) != null)                                       Name = sValue; else if (reset) Name = null;
                if ((sValue = info[KEY_ID])   != null && Int32.TryParse(sValue, out iValue)) Id   = iValue; else if (reset) Id   = null;
            }
        }
        public class NormalInfo
        {
            // Data received from a "clientlist" command.
            public Int32? DatabaseId = null;
            public Int32? ChannelId  = null;
            public Int32? Type       = null;

            // Sets the normal information from a response.
            public void SetData(TeamspeakGroup info, Boolean reset)
            {
                String sValue;
                Int32  iValue;

                if ((sValue = info[KEY_DATABASE_ID]) != null && Int32.TryParse(sValue, out iValue)) DatabaseId = iValue; else if (reset) DatabaseId = null;
                if ((sValue = info[KEY_CHANNEL_ID])  != null && Int32.TryParse(sValue, out iValue)) ChannelId  = iValue; else if (reset) ChannelId  = null;
                if ((sValue = info[KEY_TYPE])        != null && Int32.TryParse(sValue, out iValue)) Type       = iValue; else if (reset) Type       = null;
            }
        }
        public class AdvancedInfo
        {
            // Data received from a "clientinfo" command.
            public String   LoginName                   = null;
            public String   UniqueId                    = null;
            public String   IpAddress                   = null;
            public String   ClientVersion               = null;
            public String   ClientPlatform              = null;
            public String   ClientDescription           = null;
            public String   ClientCountry               = null;
            public String   ClientMetaData              = null;
            public String   DefaultChannel              = null;
            public String   FlagAvatar                  = null;
            public String   AwayMessage                 = null;
            public String   TalkMessage                 = null;
            public String   PhoneticNick                = null;
            public String   DefaultToken                = null;
            public String   Base64Hash                  = null;
            public Int32?   ChannelGroupId              = null;
            public Int32?   ServerGroupId               = null;
            public Int32?   ConnectionTime              = null;
            public Int32?   IdleTime                    = null;
            public Int32?   CreationTime                = null;
            public Int32?   LastConnected               = null;
            public Int32?   TotalConnections            = null;
            public Int32?   TalkPower                   = null;
            public Int32?   NeededQueryViewPower        = null;
            public Int32?   UnreadMessages              = null;
            public Int32?   IconId                      = null;
            public Int32?   BytesUploadedMonth          = null;
            public Int32?   BytesDownloadedMonth        = null;
            public Int32?   BytesUploadedTotal          = null;
            public Int32?   BytesDownloadedTotal        = null;
            public Int32?   FilesBandwidthSent          = null;
            public Int32?   FilesBandwidthReceived      = null;
            public Int32?   PacketsSent                 = null;
            public Int32?   PacketsReceived             = null;
            public Int32?   BytesSent                   = null;
            public Int32?   BytesReceived               = null;
            public Int32?   BandwidthSentLastSecond     = null;
            public Int32?   BandwidthReceivedLastSecond = null;
            public Int32?   BandwidthSentLastMinute     = null;
            public Int32?   BandwidthReceivedLastMinute = null;
            public Boolean? IsChannelCommander          = null;
            public Boolean? InputMuted                  = null;
            public Boolean? OutputMuted                 = null;
            public Boolean? OutputMutedOnly             = null;
            public Boolean? InputHardware               = null;
            public Boolean? OutputHardware              = null;
            public Boolean? IsRecording                 = null;
            public Boolean? IsAway                      = null;
            public Boolean? TalkRequest                 = null;
            public Boolean? IsTalker                    = null;
            public Boolean? IsPriority                  = null;
            
            // Sets the advanced information from a response.
            public void SetData(TeamspeakGroup info, Boolean reset)
            {
                String  sValue;
                Int32   iValue;
                
                if ((sValue = info[KEY_LOGIN_NAME])         != null) LoginName         = sValue; else if (reset) LoginName         = null;
                if ((sValue = info[KEY_UNIQUE_ID])          != null) UniqueId          = sValue; else if (reset) UniqueId          = null;
                if ((sValue = info[KEY_IP_ADDRESS])         != null) IpAddress         = sValue; else if (reset) IpAddress         = null;
                if ((sValue = info[KEY_CLIENT_VERSION])     != null) ClientVersion     = sValue; else if (reset) ClientVersion     = null;
                if ((sValue = info[KEY_CLIENT_PLATFORM])    != null) ClientPlatform    = sValue; else if (reset) ClientPlatform    = null;
                if ((sValue = info[KEY_CLIENT_DESCRIPTION]) != null) ClientDescription = sValue; else if (reset) ClientDescription = null;
                if ((sValue = info[KEY_CLIENT_COUNTRY])     != null) ClientCountry     = sValue; else if (reset) ClientCountry     = null;
                if ((sValue = info[KEY_CLIENT_META_DATA])   != null) ClientMetaData    = sValue; else if (reset) ClientMetaData    = null;
                if ((sValue = info[KEY_DEFAULT_CHANNEL])    != null) DefaultChannel    = sValue; else if (reset) DefaultChannel    = null;
                if ((sValue = info[KEY_FLAG_AVATAR])        != null) FlagAvatar        = sValue; else if (reset) FlagAvatar        = null;
                if ((sValue = info[KEY_AWAY_MESSAGE])       != null) AwayMessage       = sValue; else if (reset) AwayMessage       = null;
                if ((sValue = info[KEY_TALK_MESSAGE])       != null) TalkMessage       = sValue; else if (reset) TalkMessage       = null;
                if ((sValue = info[KEY_PHONETIC_NICK])      != null) PhoneticNick      = sValue; else if (reset) PhoneticNick      = null;
                if ((sValue = info[KEY_DEFAULT_TOKEN])      != null) DefaultToken      = sValue; else if (reset) DefaultToken      = null;
                if ((sValue = info[KEY_BASE64_HASH])        != null) Base64Hash        = sValue; else if (reset) Base64Hash        = null;
                if ((sValue = info[KEY_CHANNEL_GROUP_ID])            != null && Int32.TryParse(sValue, out iValue)) ChannelGroupId              = iValue; else if (reset) ChannelGroupId              = null;
                if ((sValue = info[KEY_SERVER_GROUP_ID])             != null && Int32.TryParse(sValue, out iValue)) ServerGroupId               = iValue; else if (reset) ServerGroupId               = null;
                if ((sValue = info[KEY_CONNECTION_TIME])             != null && Int32.TryParse(sValue, out iValue)) ConnectionTime              = iValue; else if (reset) ConnectionTime              = null;
                if ((sValue = info[KEY_IDLE_TIME])                   != null && Int32.TryParse(sValue, out iValue)) IdleTime                    = iValue; else if (reset) IdleTime                    = null;
                if ((sValue = info[KEY_CREATION_TIME])               != null && Int32.TryParse(sValue, out iValue)) CreationTime                = iValue; else if (reset) CreationTime                = null;
                if ((sValue = info[KEY_LAST_CONNECTED])              != null && Int32.TryParse(sValue, out iValue)) LastConnected               = iValue; else if (reset) LastConnected               = null;
                if ((sValue = info[KEY_TOTAL_CONNECTIONS])           != null && Int32.TryParse(sValue, out iValue)) TotalConnections            = iValue; else if (reset) TotalConnections            = null;
                if ((sValue = info[KEY_TALK_POWER])                  != null && Int32.TryParse(sValue, out iValue)) TalkPower                   = iValue; else if (reset) TalkPower                   = null;
                if ((sValue = info[KEY_NEEDED_QUERY_VIEW_POWER])     != null && Int32.TryParse(sValue, out iValue)) NeededQueryViewPower        = iValue; else if (reset) NeededQueryViewPower        = null;
                if ((sValue = info[KEY_UNREAD_MESSAGES])             != null && Int32.TryParse(sValue, out iValue)) UnreadMessages              = iValue; else if (reset) UnreadMessages              = null;
                if ((sValue = info[KEY_ICON_ID])                     != null && Int32.TryParse(sValue, out iValue)) IconId                      = iValue; else if (reset) IconId                      = null;
                if ((sValue = info[KEY_BYTES_UPLOADED_MONTH])        != null && Int32.TryParse(sValue, out iValue)) BytesUploadedMonth          = iValue; else if (reset) BytesUploadedMonth          = null;
                if ((sValue = info[KEY_BYTES_DOWNLOADED_MONTH])      != null && Int32.TryParse(sValue, out iValue)) BytesDownloadedMonth        = iValue; else if (reset) BytesDownloadedMonth        = null;
                if ((sValue = info[KEY_BYTES_UPLOADED_TOTAL])        != null && Int32.TryParse(sValue, out iValue)) BytesUploadedTotal          = iValue; else if (reset) BytesUploadedTotal          = null;
                if ((sValue = info[KEY_BYTES_DOWNLOADED_TOTAL])      != null && Int32.TryParse(sValue, out iValue)) BytesDownloadedTotal        = iValue; else if (reset) BytesDownloadedTotal        = null;
                if ((sValue = info[KEY_FILES_BANDWIDTH_SENT])        != null && Int32.TryParse(sValue, out iValue)) FilesBandwidthSent          = iValue; else if (reset) FilesBandwidthSent          = null;
                if ((sValue = info[KEY_FILES_BANDWIDTH_RECEIVED])    != null && Int32.TryParse(sValue, out iValue)) FilesBandwidthReceived      = iValue; else if (reset) FilesBandwidthReceived      = null;
                if ((sValue = info[KEY_PACKETS_SENT])                != null && Int32.TryParse(sValue, out iValue)) PacketsSent                 = iValue; else if (reset) PacketsSent                 = null;
                if ((sValue = info[KEY_PACKETS_RECEIVED])            != null && Int32.TryParse(sValue, out iValue)) PacketsReceived             = iValue; else if (reset) PacketsReceived             = null;
                if ((sValue = info[KEY_BYTES_SENT])                  != null && Int32.TryParse(sValue, out iValue)) BytesSent                   = iValue; else if (reset) BytesSent                   = null;
                if ((sValue = info[KEY_BYTES_RECEIVED])              != null && Int32.TryParse(sValue, out iValue)) BytesReceived               = iValue; else if (reset) BytesReceived               = null;
                if ((sValue = info[KEY_BANDWIDTH_SENT_LAST_SEC])     != null && Int32.TryParse(sValue, out iValue)) BandwidthSentLastSecond     = iValue; else if (reset) BandwidthSentLastSecond     = null;
                if ((sValue = info[KEY_BANDWIDTH_RECEIVED_LAST_SEC]) != null && Int32.TryParse(sValue, out iValue)) BandwidthReceivedLastSecond = iValue; else if (reset) BandwidthReceivedLastSecond = null;
                if ((sValue = info[KEY_BANDWIDTH_SENT_LAST_MIN])     != null && Int32.TryParse(sValue, out iValue)) BandwidthSentLastMinute     = iValue; else if (reset) BandwidthSentLastMinute     = null;
                if ((sValue = info[KEY_BANDWIDTH_RECEIVED_LAST_MIN]) != null && Int32.TryParse(sValue, out iValue)) BandwidthReceivedLastMinute = iValue; else if (reset) BandwidthReceivedLastMinute = null;
                if ((sValue = info[KEY_IS_CHANNEL_COMMANDER]) != null) IsChannelCommander = (sValue == "1") ? true : false; else if (reset) IsChannelCommander = null;
                if ((sValue = info[KEY_INPUT_MUTED])          != null) InputMuted         = (sValue == "1") ? true : false; else if (reset) InputMuted         = null;
                if ((sValue = info[KEY_OUTPUT_MUTED])         != null) OutputMuted        = (sValue == "1") ? true : false; else if (reset) OutputMuted        = null;
                if ((sValue = info[KEY_OUTPUT_MUTED_ONLY])    != null) OutputMutedOnly    = (sValue == "1") ? true : false; else if (reset) OutputMutedOnly    = null;
                if ((sValue = info[KEY_INPUT_HARDWARE])       != null) InputHardware      = (sValue == "1") ? true : false; else if (reset) InputHardware      = null;
                if ((sValue = info[KEY_OUTPUT_HARDWARE])      != null) OutputHardware     = (sValue == "1") ? true : false; else if (reset) OutputHardware     = null;
                if ((sValue = info[KEY_IS_RECORDING])         != null) IsRecording        = (sValue == "1") ? true : false; else if (reset) IsRecording        = null;
                if ((sValue = info[KEY_IS_AWAY])              != null) IsAway             = (sValue == "1") ? true : false; else if (reset) IsAway             = null;
                if ((sValue = info[KEY_TALK_REQUEST])         != null) TalkRequest        = (sValue == "1") ? true : false; else if (reset) TalkRequest        = null;
                if ((sValue = info[KEY_IS_TALKER])            != null) IsTalker           = (sValue == "1") ? true : false; else if (reset) IsTalker           = null;
                if ((sValue = info[KEY_IS_PRIORITY])          != null) IsPriority         = (sValue == "1") ? true : false; else if (reset) IsPriority         = null;
            }
        }

        public BasicInfo    Basic    { get; private set; }
        public NormalInfo   Normal   { get; private set; }
        public AdvancedInfo Advanced { get; private set; }

        // Initializes a client with default values.
        public TeamspeakClient()
        {
            Basic    = new BasicInfo();
            Normal   = new NormalInfo();
            Advanced = new AdvancedInfo();
        }
        // Initializes a client with the specified data.
        public TeamspeakClient(TeamspeakGroup info, Boolean reset = true) : this()
        {
            Basic.SetData(info, reset);
            Normal.SetData(info, reset);
            Advanced.SetData(info, reset);
        }
    }
}
