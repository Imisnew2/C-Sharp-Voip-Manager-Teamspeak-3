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

namespace VoipManager.Teamspeak3.Objects
{
    using VoipManager.Teamspeak3.Communication;

    public class Teamspeak3Client
    {
        #region Client Data Keys

        private const String KEY_NAME = "client_nickname";
        private const String KEY_ID   = "clid";

        private const String KEY_DATABASE_ID = "client_database_id";
        private const String KEY_CHANNEL_ID  = "cid";
        private const String KEY_TYPE        = "client_type";

        private const String KEY_LOGIN_NAME                         = "client_login_name";
        private const String KEY_UNIQUE_ID                          = "client_unique_identifier";
        private const String KEY_IP_ADDRESS                         = "connection_client_ip";
        private const String KEY_CLIENT_VERSION                     = "client_version";
        private const String KEY_CLIENT_PLATFORM                    = "client_platform";
        private const String KEY_CLIENT_DESCRIPTION                 = "client_description";
        private const String KEY_CLIENT_COUNTRY                     = "client_country";
        private const String KEY_CLIENT_META_DATA                   = "client_meta_data";
        private const String KEY_DEFAULT_CHANNEL                    = "client_default_channel";
        private const String KEY_FLAG_AVATAR                        = "client_flag_avatar";
        private const String KEY_AWAY_MESSAGE                       = "client_away_message";
        private const String KEY_TALK_MESSAGE                       = "client_talk_request_msg";
        private const String KEY_PHONETIC_NICK                      = "client_nickname_phonetic";
        private const String KEY_DEFAULT_TOKEN                      = "client_default_token";
        private const String KEY_BASE64_HASH                        = "client_base64HashClientUID";
        private const String KEY_CHANNEL_GROUP_INHERITED_CHANNEL_ID = "client_channel_group_inherited_channel_id";
        private const String KEY_CHANNEL_GROUP_ID                   = "client_channel_group_id";
        private const String KEY_SERVER_GROUP_ID                    = "client_servergroups";
        private const String KEY_IDLE_TIME                          = "client_idle_time";
        private const String KEY_CONNECTION_TIME                    = "connection_connected_time";
        private const String KEY_CREATION_TIME                      = "client_created";
        private const String KEY_LAST_CONNECTED                     = "client_lastconnected";
        private const String KEY_TOTAL_CONNECTIONS                  = "client_totalconnections";
        private const String KEY_TALK_POWER                         = "client_talk_power";
        private const String KEY_NEEDED_QUERY_VIEW_POWER            = "client_needed_serverquery_view_power";
        private const String KEY_ICON_ID                            = "client_icon_id";
        private const String KEY_BYTES_UPLOADED_MONTH               = "client_month_bytes_uploaded";
        private const String KEY_BYTES_DOWNLOADED_MONTH             = "client_month_bytes_downloaded";
        private const String KEY_BYTES_UPLOADED_TOTAL               = "client_total_bytes_uploaded";
        private const String KEY_BYTES_DOWNLOADED_TOTAL             = "client_total_bytes_downloaded";
        private const String KEY_FILES_BANDWIDTH_SENT               = "connection_filetransfer_bandwidth_sent";
        private const String KEY_FILES_BANDWIDTH_RECEIVED           = "connection_filetransfer_bandwidth_received";
        private const String KEY_PACKETS_SENT                       = "connection_packets_sent_total";
        private const String KEY_PACKETS_RECEIVED                   = "connection_packets_received_total";
        private const String KEY_BYTES_SENT                         = "connection_bytes_sent_total";
        private const String KEY_BYTES_RECEIVED                     = "connection_bytes_received_total";
        private const String KEY_BANDWIDTH_SENT_LAST_SEC            = "connection_bandwidth_sent_last_second_total";
        private const String KEY_BANDWIDTH_RECEIVED_LAST_SEC        = "connection_bandwidth_received_last_second_total";
        private const String KEY_BANDWIDTH_SENT_LAST_MIN            = "connection_bandwidth_sent_last_minute_total";
        private const String KEY_BANDWIDTH_RECEIVED_LAST_MIN        = "connection_bandwidth_received_last_minute_total";
        private const String KEY_IS_CHANNEL_COMMANDER               = "client_is_channel_commander";
        private const String KEY_INPUT_MUTED                        = "client_input_muted";
        private const String KEY_OUTPUT_MUTED                       = "client_output_muted";
        private const String KEY_OUTPUT_MUTED_ONLY                  = "client_outputonly_muted";
        private const String KEY_INPUT_HARDWARE                     = "client_input_hardware";
        private const String KEY_OUTPUT_HARDWARE                    = "client_output_hardware";
        private const String KEY_IS_RECORDING                       = "client_is_recording";
        private const String KEY_IS_AWAY                            = "client_away";
        private const String KEY_TALK_REQUEST                       = "client_talk_request";
        private const String KEY_IS_TALKER                          = "client_is_talker";
        private const String KEY_IS_PRIORITY                        = "client_is_priority_speaker";

        #endregion

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                            Raw Classes                            *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public class RawBasicInfo
        {
            public String Name;
            public Int32? Id;

            public void SetData(Teamspeak3Group info, Boolean reset)
            {
                String sValue;
                Int32  iValue;

                if (reset) {
                    Name = null;
                    Id   = null;
                }

                if ((sValue = info[KEY_NAME]) != null)                                       Name = sValue;
                if ((sValue = info[KEY_ID])   != null && Int32.TryParse(sValue, out iValue)) Id   = iValue;
            }
        }
        public class RawNormalInfo
        {
            public Int32? DatabaseId;
            public Int32? ChannelId;
            public Int32? Type;

            public void SetData(Teamspeak3Group info, Boolean reset)
            {
                String sValue;
                Int32  iValue;

                if (reset) {
                    DatabaseId = null;
                    ChannelId  = null;
                    Type       = null;
                }

                if ((sValue = info[KEY_DATABASE_ID]) != null && Int32.TryParse(sValue, out iValue)) DatabaseId = iValue;
                if ((sValue = info[KEY_CHANNEL_ID])  != null && Int32.TryParse(sValue, out iValue)) ChannelId  = iValue;
                if ((sValue = info[KEY_TYPE])        != null && Int32.TryParse(sValue, out iValue)) Type       = iValue;
            }
        }
        public class RawAdvancedInfo
        {
            public String   LoginName;
            public String   UniqueId;
            public String   IpAddress;
            public String   ClientVersion;
            public String   ClientPlatform;
            public String   ClientDescription;
            public String   ClientCountry;
            public String   ClientMetaData;
            public String   DefaultChannelName;
            public String   FlagAvatar;
            public String   AwayMessage;
            public String   TalkMessage;
            public String   PhoneticNick;
            public String   DefaultToken;
            public String   Base64Hash;
            public Int32?   ChannelGroupInheritedFromChannelId;
            public Int32?   ChannelGroupId;
            public Int32?   ServerGroupId;
            public Int32?   IdleTime;
            public Int32?   ConnectionTime;
            public Int32?   TotalConnections;
            public Int32?   TalkPower;
            public Int32?   NeededQueryViewPower;
            public Int32?   IconId;
            public Int32?   BytesUploadedMonth;
            public Int32?   BytesDownloadedMonth;
            public Int32?   BytesUploadedTotal;
            public Int32?   BytesDownloadedTotal;
            public Int32?   FilesBandwidthSent;
            public Int32?   FilesBandwidthReceived;
            public Int32?   PacketsSent;
            public Int32?   PacketsReceived;
            public Int32?   BytesSent;
            public Int32?   BytesReceived;
            public Int32?   BandwidthSentLastSecond;
            public Int32?   BandwidthReceivedLastSecond;
            public Int32?   BandwidthSentLastMinute;
            public Int32?   BandwidthReceivedLastMinute;
            public UInt32?  CreationTime;
            public UInt32?  LastConnected;
            public Boolean? IsChannelCommander;
            public Boolean? InputMuted;
            public Boolean? OutputMuted;
            public Boolean? OutputMutedOnly;
            public Boolean? InputHardware;
            public Boolean? OutputHardware;
            public Boolean? IsRecording;
            public Boolean? IsAway;
            public Boolean? TalkRequest;
            public Boolean? IsTalker;
            public Boolean? IsPriority;
            
            public void SetData(Teamspeak3Group info, Boolean reset)
            {
                String  sValue;
                Int32   iValue;
                UInt32  uiValue;

                if (reset) {
                    LoginName                          = null;
                    UniqueId                           = null;
                    IpAddress                          = null;
                    ClientVersion                      = null;
                    ClientPlatform                     = null;
                    ClientDescription                  = null;
                    ClientCountry                      = null;
                    ClientMetaData                     = null;
                    DefaultChannelName                 = null;
                    FlagAvatar                         = null;
                    AwayMessage                        = null;
                    TalkMessage                        = null;
                    PhoneticNick                       = null;
                    DefaultToken                       = null;
                    Base64Hash                         = null;
                    ChannelGroupInheritedFromChannelId = null;
                    ChannelGroupId                     = null;
                    ServerGroupId                      = null;
                    IdleTime                           = null;
                    LastConnected                      = null;
                    TotalConnections                   = null;
                    TalkPower                          = null;
                    NeededQueryViewPower               = null;
                    IconId                             = null;
                    BytesUploadedMonth                 = null;
                    BytesDownloadedMonth               = null;
                    BytesUploadedTotal                 = null;
                    BytesDownloadedTotal               = null;
                    FilesBandwidthSent                 = null;
                    FilesBandwidthReceived             = null;
                    PacketsSent                        = null;
                    PacketsReceived                    = null;
                    BytesSent                          = null;
                    BytesReceived                      = null;
                    BandwidthSentLastSecond            = null;
                    BandwidthReceivedLastSecond        = null;
                    BandwidthSentLastMinute            = null;
                    BandwidthReceivedLastMinute        = null;
                    ConnectionTime                     = null;
                    CreationTime                       = null;
                    IsChannelCommander                 = null;
                    InputMuted                         = null;
                    OutputMuted                        = null;
                    OutputMutedOnly                    = null;
                    InputHardware                      = null;
                    OutputHardware                     = null;
                    IsRecording                        = null;
                    IsAway                             = null;
                    TalkRequest                        = null;
                    IsTalker                           = null;
                    IsPriority                         = null;
                }
                
                if ((sValue = info[KEY_LOGIN_NAME])         != null) LoginName          = sValue;
                if ((sValue = info[KEY_UNIQUE_ID])          != null) UniqueId           = sValue;
                if ((sValue = info[KEY_IP_ADDRESS])         != null) IpAddress          = sValue;
                if ((sValue = info[KEY_CLIENT_VERSION])     != null) ClientVersion      = sValue;
                if ((sValue = info[KEY_CLIENT_PLATFORM])    != null) ClientPlatform     = sValue;
                if ((sValue = info[KEY_CLIENT_DESCRIPTION]) != null) ClientDescription  = sValue;
                if ((sValue = info[KEY_CLIENT_COUNTRY])     != null) ClientCountry      = sValue;
                if ((sValue = info[KEY_CLIENT_META_DATA])   != null) ClientMetaData     = sValue;
                if ((sValue = info[KEY_DEFAULT_CHANNEL])    != null) DefaultChannelName = sValue;
                if ((sValue = info[KEY_FLAG_AVATAR])        != null) FlagAvatar         = sValue;
                if ((sValue = info[KEY_AWAY_MESSAGE])       != null) AwayMessage        = sValue;
                if ((sValue = info[KEY_TALK_MESSAGE])       != null) TalkMessage        = sValue;
                if ((sValue = info[KEY_PHONETIC_NICK])      != null) PhoneticNick       = sValue;
                if ((sValue = info[KEY_DEFAULT_TOKEN])      != null) DefaultToken       = sValue;
                if ((sValue = info[KEY_BASE64_HASH])        != null) Base64Hash         = sValue;
                if ((sValue = info[KEY_CHANNEL_GROUP_INHERITED_CHANNEL_ID]) != null && Int32.TryParse(sValue, out iValue)) ChannelGroupInheritedFromChannelId = iValue;
                if ((sValue = info[KEY_CHANNEL_GROUP_ID])                   != null && Int32.TryParse(sValue, out iValue)) ChannelGroupId                     = iValue;
                if ((sValue = info[KEY_SERVER_GROUP_ID])                    != null && Int32.TryParse(sValue, out iValue)) ServerGroupId                      = iValue;
                if ((sValue = info[KEY_IDLE_TIME])                          != null && Int32.TryParse(sValue, out iValue)) IdleTime                           = iValue;
                if ((sValue = info[KEY_CONNECTION_TIME])                    != null && Int32.TryParse(sValue, out iValue)) ConnectionTime                     = iValue;
                if ((sValue = info[KEY_TOTAL_CONNECTIONS])                  != null && Int32.TryParse(sValue, out iValue)) TotalConnections                   = iValue;
                if ((sValue = info[KEY_TALK_POWER])                         != null && Int32.TryParse(sValue, out iValue)) TalkPower                          = iValue;
                if ((sValue = info[KEY_NEEDED_QUERY_VIEW_POWER])            != null && Int32.TryParse(sValue, out iValue)) NeededQueryViewPower               = iValue;
                if ((sValue = info[KEY_ICON_ID])                            != null && Int32.TryParse(sValue, out iValue)) IconId                             = iValue;
                if ((sValue = info[KEY_BYTES_UPLOADED_MONTH])               != null && Int32.TryParse(sValue, out iValue)) BytesUploadedMonth                 = iValue;
                if ((sValue = info[KEY_BYTES_DOWNLOADED_MONTH])             != null && Int32.TryParse(sValue, out iValue)) BytesDownloadedMonth               = iValue;
                if ((sValue = info[KEY_BYTES_UPLOADED_TOTAL])               != null && Int32.TryParse(sValue, out iValue)) BytesUploadedTotal                 = iValue;
                if ((sValue = info[KEY_BYTES_DOWNLOADED_TOTAL])             != null && Int32.TryParse(sValue, out iValue)) BytesDownloadedTotal               = iValue;
                if ((sValue = info[KEY_FILES_BANDWIDTH_SENT])               != null && Int32.TryParse(sValue, out iValue)) FilesBandwidthSent                 = iValue;
                if ((sValue = info[KEY_FILES_BANDWIDTH_RECEIVED])           != null && Int32.TryParse(sValue, out iValue)) FilesBandwidthReceived             = iValue;
                if ((sValue = info[KEY_PACKETS_SENT])                       != null && Int32.TryParse(sValue, out iValue)) PacketsSent                        = iValue;
                if ((sValue = info[KEY_PACKETS_RECEIVED])                   != null && Int32.TryParse(sValue, out iValue)) PacketsReceived                    = iValue;
                if ((sValue = info[KEY_BYTES_SENT])                         != null && Int32.TryParse(sValue, out iValue)) BytesSent                          = iValue;
                if ((sValue = info[KEY_BYTES_RECEIVED])                     != null && Int32.TryParse(sValue, out iValue)) BytesReceived                      = iValue;
                if ((sValue = info[KEY_BANDWIDTH_SENT_LAST_SEC])            != null && Int32.TryParse(sValue, out iValue)) BandwidthSentLastSecond            = iValue;
                if ((sValue = info[KEY_BANDWIDTH_RECEIVED_LAST_SEC])        != null && Int32.TryParse(sValue, out iValue)) BandwidthReceivedLastSecond        = iValue;
                if ((sValue = info[KEY_BANDWIDTH_SENT_LAST_MIN])            != null && Int32.TryParse(sValue, out iValue)) BandwidthSentLastMinute            = iValue;
                if ((sValue = info[KEY_BANDWIDTH_RECEIVED_LAST_MIN])        != null && Int32.TryParse(sValue, out iValue)) BandwidthReceivedLastMinute        = iValue;
                if ((sValue = info[KEY_CREATION_TIME])  != null && UInt32.TryParse(sValue, out uiValue)) CreationTime  = uiValue;
                if ((sValue = info[KEY_LAST_CONNECTED]) != null && UInt32.TryParse(sValue, out uiValue)) LastConnected = uiValue;
                if ((sValue = info[KEY_IS_CHANNEL_COMMANDER]) != null) IsChannelCommander = (sValue == "1");
                if ((sValue = info[KEY_INPUT_MUTED])          != null) InputMuted         = (sValue == "1");
                if ((sValue = info[KEY_OUTPUT_MUTED])         != null) OutputMuted        = (sValue == "1");
                if ((sValue = info[KEY_OUTPUT_MUTED_ONLY])    != null) OutputMutedOnly    = (sValue == "1");
                if ((sValue = info[KEY_INPUT_HARDWARE])       != null) InputHardware      = (sValue == "1");
                if ((sValue = info[KEY_OUTPUT_HARDWARE])      != null) OutputHardware     = (sValue == "1");
                if ((sValue = info[KEY_IS_RECORDING])         != null) IsRecording        = (sValue == "1");
                if ((sValue = info[KEY_IS_AWAY])              != null) IsAway             = (sValue == "1");
                if ((sValue = info[KEY_TALK_REQUEST])         != null) TalkRequest        = (sValue == "1");
                if ((sValue = info[KEY_IS_TALKER])            != null) IsTalker           = (sValue == "1");
                if ((sValue = info[KEY_IS_PRIORITY])          != null) IsPriority         = (sValue == "1");
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                          Parsed Classes                           *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public class BasicInfo : RawBasicInfo
        {
        }
        public class NormalInfo : RawNormalInfo
        {
            new public ClientType? Type {
                get { return (ClientType?)base.Type; }
                set { base.Type = (Int32?)value; }
            }
        }
        public class AdvancedInfo : RawAdvancedInfo
        {
            new public TimeSpan? IdleTime {
                get { return (base.IdleTime.HasValue)  ? TimeSpan.FromMilliseconds(base.IdleTime.Value) : default(TimeSpan?); }
                set { base.IdleTime = (value.HasValue) ? (Int32?)value.Value.TotalMilliseconds          : default(Int32?); }
            }
            new public TimeSpan? ConnectionTime {
                get { return  (base.ConnectionTime.HasValue) ? TimeSpan.FromMilliseconds(base.ConnectionTime.Value) : default(TimeSpan?); }
                set { base.ConnectionTime = (value.HasValue) ? (Int32?)value.Value.TotalMilliseconds                : default(Int32?); }
            }
            new public DateTime? CreationTime {
                get { return Utilities.UtcIntegerToDateTime(base.CreationTime); }
                set { base.CreationTime = Utilities.DateTimeToUtcInteger(value); }
            }
            new public DateTime? LastConnected {
                get { return Utilities.UtcIntegerToDateTime(base.LastConnected); }
                set { base.LastConnected = Utilities.DateTimeToUtcInteger(value); }
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                        Teamspeak 3 Client                         *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>Info from a "clientfind" command.</summary>
        public BasicInfo Basic { get; private set; }

        /// <summary>Info from a "clientlist" command.</summary>
        public NormalInfo Normal { get; private set; }

        /// <summary>Info from a "clientinfo" command.</summary>
        public AdvancedInfo Advanced { get; private set; }


        /// <summary>
        /// Initializes a client with the specified data.
        /// </summary>
        public Teamspeak3Client(Teamspeak3Group info = null)
        {
            Basic    = new BasicInfo();
            Normal   = new NormalInfo();
            Advanced = new AdvancedInfo();
            Update(info);
        }


        /// <summary
        /// >Updates a client with the specified data.
        /// </summary>
        public void Update(Teamspeak3Group info, Boolean clear = false)
        {
            if (info != null) {
                Basic.SetData(info, clear);
                Normal.SetData(info, clear);
                Advanced.SetData(info, clear);
            }
        }
    }
}
