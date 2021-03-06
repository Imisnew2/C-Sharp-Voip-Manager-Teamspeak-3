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

    public class Teamspeak3Server
    {
        #region Server Data Keys

        private const String KEY_NAME                 = "virtualserver_name";
        private const String KEY_ID                   = "virtualserver_id";
        private const String KEY_PORT                 = "virtualserver_port";
        private const String KEY_MACHINE_ID           = "virtualserver_machine_id";
        private const String KEY_STATUS               = "virtualserver_status";
        private const String KEY_UPTIME               = "virtualserver_uptime";
        private const String KEY_CLIENTS_ONLINE       = "virtualserver_clientsonline";
        private const String KEY_QUERY_CLIENTS_ONLINE = "virtualserver_queryclientsonline";
        private const String KEY_MAX_CLIENTS          = "virtualserver_maxclients";
        private const String KEY_AUTO_START           = "virtualserver_autostart";
        
        private const String KEY_UNIQUE_ID                             = "virtualserver_unique_identifier";
        private const String KEY_WELCOME_MESSAGE                       = "virtualserver_welcomemessage";
        private const String KEY_SERVER_PLATFORM                       = "virtualserver_platform";
        private const String KEY_SERVER_VERSION                        = "virtualserver_version";
        private const String KEY_PASSWORD                              = "virtualserver_password";
        private const String KEY_CHANNELS_ONLINE                       = "virtualserver_channelsonline";
        private const String KEY_CREATION_TIME                         = "virtualserver_created";
        private const String KEY_ENCRYPTION_MODE                       = "virtualserver_codec_encryption_mode";
        private const String KEY_HOSTMESSAGE                           = "virtualserver_hostmessage";
        private const String KEY_HOSTMESSAGE_MODE                      = "virtualserver_hostmessage_mode";
        private const String KEY_FILE_BASE                             = "virtualserver_filebase";
        private const String KEY_DEFAULT_SERVER_GROUP                  = "virtualserver_default_server_group";
        private const String KEY_DEFAULT_CHANNEL_GROUP                 = "virtualserver_default_channel_group";
        private const String KEY_FLAG_PASSWORD                         = "virtualserver_flag_password";
        private const String KEY_DEFAULT_CHANNEL_ADMIN_GROUP           = "virtualserver_default_channel_admin_group";
        private const String KEY_MAX_DOWNLOAD_TOTAL_BANDWIDTH          = "virtualserver_max_download_total_bandwidth";
        private const String KEY_MAX_UPLOAD_TOTAL_BANDWIDTH            = "virtualserver_max_upload_total_bandwidth";
        private const String KEY_HOSTBANNER_MODE                       = "virtualserver_hostbanner_mode";
        private const String KEY_HOSTBANNER_URL                        = "virtualserver_hostbanner_url";
        private const String KEY_HOSTBANNER_GFX_URL                    = "virtualserver_hostbanner_gfx_url";
        private const String KEY_HOSTBANNER_GFX_INTERVAL               = "virtualserver_hostbanner_gfx_interval";
        private const String KEY_COMPLAIN_AUTOBAN_COUNT                = "virtualserver_complain_autoban_count";
        private const String KEY_COMPLAIN_AUTOBAN_TIME                 = "virtualserver_complain_autoban_time";
        private const String KEY_COMPLAIN_REMOVE_TIME                  = "virtualserver_complain_remove_time";
        private const String KEY_CLIENTS_NEEDED_BEFORE_FORCED_SILENCE  = "virtualserver_min_clients_in_channel_before_forced_silence";
        private const String KEY_PRIORITY_SPEAKER_DIMM_MODIFICATOR     = "virtualserver_priority_speaker_dimm_modificator";
        private const String KEY_ANTIFLOOD_POINTS_TICK_REDUCE          = "virtualserver_antiflood_points_tick_reduce";
        private const String KEY_ANTIFLOOD_POINTS_NEEDED_COMMAND_BLOCK = "virtualserver_antiflood_points_needed_command_block";
        private const String KEY_ANTIFLOOD_POINTS_NEEDED_IP_BLOCK      = "virtualserver_antiflood_points_needed_ip_block";
        private const String KEY_CLIENT_CONNECTIONS                    = "virtualserver_client_connections";
        private const String KEY_QUERY_CLIENT_CONNECTIONS              = "virtualserver_query_client_connections";
        private const String KEY_HOSTBUTTON_TOOLTIP                    = "virtualserver_hostbutton_tooltip";
        private const String KEY_HOSTBUTTON_URL                        = "virtualserver_hostbutton_url";
        private const String KEY_HOSTBUTTON_GFX_URL                    = "virtualserver_hostbutton_gfx_url";
        private const String KEY_DOWNLOAD_QUOTA                        = "virtualserver_download_quota";
        private const String KEY_UPLOAD_QUOTA                          = "virtualserver_upload_quota";
        private const String KEY_MONTH_BYTES_DOWNLOADED                = "virtualserver_month_bytes_downloaded";
        private const String KEY_MONTH_BYTES_UPLOADED                  = "virtualserver_month_bytes_uploaded";
        private const String KEY_TOTAL_BYTES_DOWNLOADED                = "virtualserver_total_bytes_downloaded";
        private const String KEY_TOTAL_BYTES_UPLOADED                  = "virtualserver_total_bytes_uploaded";
        private const String KEY_NEEDED_IDENTITY_SECURITY_LEVEL        = "virtualserver_needed_identity_security_level";
        private const String KEY_LOG_CLIENT                            = "virtualserver_log_client";
        private const String KEY_LOG_QUERY                             = "virtualserver_log_query";
        private const String KEY_LOG_CHANNEL                           = "virtualserver_log_channel";
        private const String KEY_LOG_PERMISSIONS                       = "virtualserver_log_permissions";
        private const String KEY_LOG_SERVER                            = "virtualserver_log_server";
        private const String KEY_LOG_FILETRANSFER                      = "virtualserver_log_filetransfer";
        private const String KEY_MIN_CLIENT_VERSION                    = "virtualserver_min_client_version";
        private const String KEY_NAME_PHONETIC                         = "virtualserver_name_phonetic";
        private const String KEY_ICON_ID                               = "virtualserver_icon_id";
        private const String KEY_RESERVED_SLOTS                        = "virtualserver_reserved_slots";
        private const String KEY_TOTAL_PACKETLOSS_SPEECH               = "virtualserver_total_packetloss_speech";
        private const String KEY_TOTAL_PACKETLOSS_KEEPALIVE            = "virtualserver_total_packetloss_keepalive";
        private const String KEY_TOTAL_PACKETLOSS_CONTROL              = "virtualserver_total_packetloss_control";
        private const String KEY_TOTAL_PACKATLOSS_TOTAL                = "virtualserver_total_packetloss_total";
        private const String KEY_TOTAL_PING                            = "virtualserver_total_ping";
        private const String KEY_IP                                    = "virtualserver_ip";
        private const String KEY_WEBLIST_ENABLED                       = "virtualserver_weblist_enabled";
        private const String KEY_ASK_FOR_PRIVILEGEKEY                  = "virtualserver_ask_for_privilegekey";
        private const String KEY_FILETRANSFER_BANDWIDTH_SENT           = "connection_filetransfer_bandwidth_sent";
        private const String KEY_FILETRANSFER_BANDWIDTH_RECEIVED       = "connection_filetransfer_bandwidth_received";
        private const String KEY_FILETRANSFER_BYTES_SENT_TOTAL         = "connection_filetransfer_bytes_sent_total";
        private const String KEY_FILETRANSFER_BYTES_RECEIVED_TOTAL     = "connection_filetransfer_bytes_received_total";
        private const String KEY_PACKETS_SENT_SPEECH                   = "connection_packets_sent_speech";
        private const String KEY_PACKETS_RECEIVED_SPEECH               = "connection_packets_received_speech";
        private const String KEY_BYTES_SENT_SPEECH                     = "connection_bytes_sent_speech";
        private const String KEY_BYTES_RECEIVED_SPEECH                 = "connection_bytes_received_speech";
        private const String KEY_PACKETS_SENT_KEEPALIVE                = "connection_packets_sent_keepalive";
        private const String KEY_PACKETS_RECEIVED_KEEPALIVE            = "connection_packets_received_keepalive";
        private const String KEY_BYTES_SENT_KEEPALIVE                  = "connection_bytes_sent_keepalive";
        private const String KEY_BYTES_RECEIVED_KEEPALIVE              = "connection_bytes_received_keepalive";
        private const String KEY_PACKETS_SENT_CONTROL                  = "connection_packets_sent_control";
        private const String KEY_PACKETS_RECEIVED_CONTROL              = "connection_packets_received_control";
        private const String KEY_BYTES_SENT_CONTROL                    = "connection_bytes_sent_control";
        private const String KEY_BYTES_RECEIVED_CONTROL                = "connection_bytes_received_control";
        private const String KEY_PACKETS_SENT_TOTAL                    = "connection_packets_sent_total";
        private const String KEY_PACKETS_RECEIVED_TOTAL                = "connection_packets_received_total";
        private const String KEY_BYTES_SENT_TOTAL                      = "connection_bytes_sent_total";
        private const String KEY_BYTES_RECEIVED_TOTAL                  = "connection_bytes_received_total";
        private const String KEY_BANDWIDTH_SENT_LAST_SECOND_TOTAL      = "connection_bandwidth_sent_last_second_total";
        private const String KEY_BANDWIDTH_RECEIVED_LAST_SECOND_TOTAL  = "connection_bandwidth_received_last_second_total";
        private const String KEY_BANDWIDTH_SENT_LAST_MINUTE_TOTAL      = "connection_bandwidth_sent_last_minute_total";
        private const String KEY_BANDWIDTH_RECEIVED_LAST_MINUTE_TOTAL  = "connection_bandwidth_received_last_minute_total";

        #endregion

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                            Raw Classes                            *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public class RawNormalInfo
        {
            public String   Name;
            public String   Status;
            public Int32?   Id;
            public Int32?   Port;
            public Int32?   MachineId;
            public Int32?   UpTime;
            public Int32?   ClientsOnline;
            public Int32?   QueryClientsOnline;
            public Int32?   MaxClients;
            public Boolean? AutoStart;

            public void SetData(Teamspeak3Group info, Boolean reset)
            {
                String sValue;
                Int32  iValue;

                if (reset) {
                    Name               = null;
                    Status             = null;
                    Id                 = null;
                    Port               = null;
                    MachineId          = null;
                    UpTime             = null;
                    ClientsOnline      = null;
                    QueryClientsOnline = null;
                    MaxClients         = null;
                    AutoStart          = null;
                }
                
                if ((sValue = info[KEY_NAME])   != null) Name   = sValue;
                if ((sValue = info[KEY_STATUS]) != null) Status = sValue;
                if ((sValue = info[KEY_ID])                   != null && Int32.TryParse(sValue, out iValue)) Id                 = iValue;
                if ((sValue = info[KEY_PORT])                 != null && Int32.TryParse(sValue, out iValue)) Port               = iValue;
                if ((sValue = info[KEY_MACHINE_ID])           != null && Int32.TryParse(sValue, out iValue)) MachineId          = iValue;
                if ((sValue = info[KEY_UPTIME])               != null && Int32.TryParse(sValue, out iValue)) UpTime             = iValue;
                if ((sValue = info[KEY_CLIENTS_ONLINE])       != null && Int32.TryParse(sValue, out iValue)) ClientsOnline      = iValue;
                if ((sValue = info[KEY_QUERY_CLIENTS_ONLINE]) != null && Int32.TryParse(sValue, out iValue)) QueryClientsOnline = iValue;
                if ((sValue = info[KEY_MAX_CLIENTS])          != null && Int32.TryParse(sValue, out iValue)) MaxClients         = iValue;
                if ((sValue = info[KEY_AUTO_START]) != null) AutoStart = (sValue == "1");
            }
        }
        public class RawAdvancedInfo
        {
            public String   Ip;
            public String   UniqueId;
            public String   PhoneticName;
            public String   WelcomeMessage;
            public String   Platform;
            public String   Version;
            public String   MinimumClientVersion;
            public String   Password;
            public String   FilePath;
            public String   HostMessage;
            public String   HostBannerUrl;
            public String   HostBannerGfxUrl;
            public String   HostButtonToolTip;
            public String   HostButtonUrl;
            public String   HostButtonGfxUrl;
            public Int32?   IconId;
            public Int32?   HostMessageMode;
            public Int32?   HostBannerMode;
            public Int32?   EncryptionMode;
            public Int32?   HostBannerGfxInterval;
            public Int32?   DefaultServerGroup;
            public Int32?   DefaultChannelGroup;
            public Int32?   DefaultChannelAdminGroup;
            public Int32?   ComplainAutoBanCount;
            public Int32?   ComplainAutoBanTime;
            public Int32?   ComplainRemoveTime;
            public Int32?   MinClientsNeededBeforeForcedSilence;
            public Int32?   AntifloodPointsTickReduce;
            public Int32?   AntifloodPointsNeededCommandBlock;
            public Int32?   AntifloodPointsNeededIpBlock;
            public Int32?   LogClient;
            public Int32?   LogQuery;
            public Int32?   LogChannel;
            public Int32?   LogPermissions;
            public Int32?   LogServer;
            public Int32?   LogFiletransfer;
            public Int32?   ReservedSlots;
            public Int32?   RequestsForPrivilegeKey;
            public Int32?   MinimumSecurityLevel;
            public Int32?   FiletransferBandwidthSent;
            public Int32?   FiletransferBandwidthReceived;
            public Int32?   FiletransferBytesSentTotal;
            public Int32?   FiletransferBytesReceivedTotal;
            public Int32?   SpeechPacketsSent;
            public Int32?   SpeechPacketsReceived;
            public Int32?   SpeechBytesSent;
            public Int32?   SpeechBytesReceived;
            public Int32?   KeepalivePacketsSent;
            public Int32?   KeepalivePacketsReceived;
            public Int32?   KeepaliveBytesSent;
            public Int32?   KeepaliveBytesReceived;
            public Int32?   ControlPacketsSent;
            public Int32?   ControlPacketsReceived;
            public Int32?   ControlBytesSent;
            public Int32?   ControlBytesReceived;
            public Int32?   PacketsSentTotal;
            public Int32?   PacketsReceivedTotal;
            public Int32?   BytesSentTotal;
            public Int32?   BytesReceivedTotal;
            public Int32?   BandwidthSentLastSecond;
            public Int32?   BandwidthReceivedLastSecond;
            public Int32?   BandwidthSentLastMinute;
            public Int32?   BandwidthReceivedLastMinute;
            public UInt32?  CreationTime;
            public UInt32?  ChannelsOnline;
            public UInt32?  TotalClientConnections;
            public UInt32?  TotalQueryClientConnections;
            public UInt64?  MaxDownloadBandwidth;
            public UInt64?  MaxUploadBandwidth;
            public UInt64?  DownloadQuota;
            public UInt64?  UploadQuota;
            public UInt64?  BytesDownloadedMonth;
            public UInt64?  BytesUploadedMonth;
            public UInt64?  BytesDownloadedTotal;
            public UInt64?  BytesUploadedTotal;
            public Double?  Ping;
            public Double?  TotalSpeechPacketloss;
            public Double?  TotalKeepalivePacketloss;
            public Double?  TotalControlPacketloss;
            public Double?  TotalPacketlossAll;
            public Double?  PrioritySpeakerDimmModificator;
            public Boolean? IsPassworded;
            public Boolean? IsWebListEnabled;

            public void SetData(Teamspeak3Group info, Boolean reset)
            {
                String sValue;
                Int32  iValue;
                UInt32 uiValue;
                UInt64 ulValue;
                Double dValue;

                if (reset) {
                    Ip                                  = null;
                    UniqueId                            = null;
                    PhoneticName                        = null;
                    WelcomeMessage                      = null;
                    Platform                            = null;
                    Version                             = null;
                    MinimumClientVersion                = null;
                    Password                            = null;
                    FilePath                            = null;
                    HostMessage                         = null;
                    HostBannerUrl                       = null;
                    HostBannerGfxUrl                    = null;
                    HostButtonToolTip                   = null;
                    HostButtonUrl                       = null;
                    HostButtonGfxUrl                    = null;
                    IconId                              = null;
                    HostMessageMode                     = null;
                    HostBannerMode                      = null;
                    EncryptionMode                      = null;
                    HostBannerGfxInterval               = null;
                    DefaultServerGroup                  = null;
                    DefaultChannelGroup                 = null;
                    DefaultChannelAdminGroup            = null;
                    ComplainAutoBanCount                = null;
                    ComplainAutoBanTime                 = null;
                    ComplainRemoveTime                  = null;
                    MinClientsNeededBeforeForcedSilence = null;
                    AntifloodPointsTickReduce           = null;
                    AntifloodPointsNeededCommandBlock   = null;
                    AntifloodPointsNeededIpBlock        = null;
                    LogClient                           = null;
                    LogQuery                            = null;
                    LogChannel                          = null;
                    LogPermissions                      = null;
                    LogServer                           = null;
                    LogFiletransfer                     = null;
                    ReservedSlots                       = null;
                    RequestsForPrivilegeKey             = null;
                    MinimumSecurityLevel                = null;
                    FiletransferBandwidthSent           = null;
                    FiletransferBandwidthReceived       = null;
                    FiletransferBytesSentTotal          = null;
                    FiletransferBytesReceivedTotal      = null;
                    SpeechPacketsSent                   = null;
                    SpeechPacketsReceived               = null;
                    SpeechBytesSent                     = null;
                    SpeechBytesReceived                 = null;
                    KeepalivePacketsSent                = null;
                    KeepalivePacketsReceived            = null;
                    KeepaliveBytesSent                  = null;
                    KeepaliveBytesReceived              = null;
                    ControlPacketsSent                  = null;
                    ControlPacketsReceived              = null;
                    ControlBytesSent                    = null;
                    ControlBytesReceived                = null;
                    PacketsSentTotal                    = null;
                    PacketsReceivedTotal                = null;
                    BytesSentTotal                      = null;
                    BytesReceivedTotal                  = null;
                    BandwidthSentLastSecond             = null;
                    BandwidthReceivedLastSecond         = null;
                    BandwidthSentLastMinute             = null;
                    BandwidthReceivedLastMinute         = null;
                    CreationTime                        = null;
                    ChannelsOnline                      = null;
                    TotalClientConnections              = null;
                    TotalQueryClientConnections         = null;
                    MaxDownloadBandwidth                = null;
                    MaxUploadBandwidth                  = null;
                    DownloadQuota                       = null;
                    UploadQuota                         = null;
                    BytesDownloadedMonth                = null;
                    BytesUploadedMonth                  = null;
                    BytesDownloadedTotal                = null;
                    BytesUploadedTotal                  = null;
                    Ping                                = null;
                    TotalSpeechPacketloss               = null;
                    TotalKeepalivePacketloss            = null;
                    TotalControlPacketloss              = null;
                    TotalPacketlossAll                  = null;
                    PrioritySpeakerDimmModificator      = null;
                    IsPassworded                        = null;
                    IsWebListEnabled                    = null;
                }
                
                if ((sValue = info[KEY_IP])                 != null) Ip                   = sValue;
                if ((sValue = info[KEY_UNIQUE_ID])          != null) UniqueId             = sValue;
                if ((sValue = info[KEY_NAME_PHONETIC])      != null) PhoneticName         = sValue;
                if ((sValue = info[KEY_WELCOME_MESSAGE])    != null) WelcomeMessage       = sValue;
                if ((sValue = info[KEY_SERVER_PLATFORM])    != null) Platform             = sValue;
                if ((sValue = info[KEY_SERVER_VERSION])     != null) Version              = sValue;
                if ((sValue = info[KEY_MIN_CLIENT_VERSION]) != null) MinimumClientVersion = sValue;
                if ((sValue = info[KEY_PASSWORD])           != null) Password             = sValue;
                if ((sValue = info[KEY_FILE_BASE])          != null) FilePath             = sValue;
                if ((sValue = info[KEY_HOSTMESSAGE])        != null) HostMessage          = sValue;
                if ((sValue = info[KEY_HOSTBANNER_URL])     != null) HostBannerUrl        = sValue;
                if ((sValue = info[KEY_HOSTBANNER_GFX_URL]) != null) HostBannerGfxUrl     = sValue;
                if ((sValue = info[KEY_HOSTBUTTON_TOOLTIP]) != null) HostButtonToolTip    = sValue;
                if ((sValue = info[KEY_HOSTBANNER_URL])     != null) HostButtonUrl        = sValue;
                if ((sValue = info[KEY_HOSTBUTTON_GFX_URL]) != null) HostButtonGfxUrl     = sValue;
                if ((sValue = info[KEY_ICON_ID])                               != null && Int32.TryParse(sValue, out iValue)) IconId                              = iValue;
                if ((sValue = info[KEY_HOSTMESSAGE_MODE])                      != null && Int32.TryParse(sValue, out iValue)) HostMessageMode                     = iValue;
                if ((sValue = info[KEY_HOSTBANNER_MODE])                       != null && Int32.TryParse(sValue, out iValue)) HostBannerMode                      = iValue;
                if ((sValue = info[KEY_ENCRYPTION_MODE])                       != null && Int32.TryParse(sValue, out iValue)) EncryptionMode                      = iValue;
                if ((sValue = info[KEY_HOSTBANNER_GFX_INTERVAL])               != null && Int32.TryParse(sValue, out iValue)) HostBannerGfxInterval               = iValue;
                if ((sValue = info[KEY_DEFAULT_SERVER_GROUP])                  != null && Int32.TryParse(sValue, out iValue)) DefaultServerGroup                  = iValue;
                if ((sValue = info[KEY_DEFAULT_CHANNEL_GROUP])                 != null && Int32.TryParse(sValue, out iValue)) DefaultChannelGroup                 = iValue;
                if ((sValue = info[KEY_DEFAULT_CHANNEL_ADMIN_GROUP])           != null && Int32.TryParse(sValue, out iValue)) DefaultChannelAdminGroup            = iValue;
                if ((sValue = info[KEY_COMPLAIN_AUTOBAN_COUNT])                != null && Int32.TryParse(sValue, out iValue)) ComplainAutoBanCount                = iValue;
                if ((sValue = info[KEY_COMPLAIN_AUTOBAN_TIME])                 != null && Int32.TryParse(sValue, out iValue)) ComplainAutoBanTime                 = iValue;
                if ((sValue = info[KEY_COMPLAIN_REMOVE_TIME])                  != null && Int32.TryParse(sValue, out iValue)) ComplainRemoveTime                  = iValue;
                if ((sValue = info[KEY_CLIENTS_NEEDED_BEFORE_FORCED_SILENCE])  != null && Int32.TryParse(sValue, out iValue)) MinClientsNeededBeforeForcedSilence = iValue;
                if ((sValue = info[KEY_ANTIFLOOD_POINTS_TICK_REDUCE])          != null && Int32.TryParse(sValue, out iValue)) AntifloodPointsTickReduce           = iValue;
                if ((sValue = info[KEY_ANTIFLOOD_POINTS_NEEDED_COMMAND_BLOCK]) != null && Int32.TryParse(sValue, out iValue)) AntifloodPointsNeededCommandBlock   = iValue;
                if ((sValue = info[KEY_ANTIFLOOD_POINTS_NEEDED_IP_BLOCK])      != null && Int32.TryParse(sValue, out iValue)) AntifloodPointsNeededIpBlock        = iValue;
                if ((sValue = info[KEY_LOG_CLIENT])                            != null && Int32.TryParse(sValue, out iValue)) LogClient                           = iValue;
                if ((sValue = info[KEY_LOG_QUERY])                             != null && Int32.TryParse(sValue, out iValue)) LogQuery                            = iValue;
                if ((sValue = info[KEY_LOG_CHANNEL])                           != null && Int32.TryParse(sValue, out iValue)) LogChannel                          = iValue;
                if ((sValue = info[KEY_LOG_PERMISSIONS])                       != null && Int32.TryParse(sValue, out iValue)) LogPermissions                      = iValue;
                if ((sValue = info[KEY_LOG_SERVER])                            != null && Int32.TryParse(sValue, out iValue)) LogServer                           = iValue;
                if ((sValue = info[KEY_LOG_FILETRANSFER])                      != null && Int32.TryParse(sValue, out iValue)) LogFiletransfer                     = iValue;
                if ((sValue = info[KEY_RESERVED_SLOTS])                        != null && Int32.TryParse(sValue, out iValue)) ReservedSlots                       = iValue;
                if ((sValue = info[KEY_ASK_FOR_PRIVILEGEKEY])                  != null && Int32.TryParse(sValue, out iValue)) RequestsForPrivilegeKey             = iValue;
                if ((sValue = info[KEY_NEEDED_IDENTITY_SECURITY_LEVEL])        != null && Int32.TryParse(sValue, out iValue)) MinimumSecurityLevel                = iValue;
                if ((sValue = info[KEY_FILETRANSFER_BANDWIDTH_SENT])           != null && Int32.TryParse(sValue, out iValue)) FiletransferBandwidthSent           = iValue;
                if ((sValue = info[KEY_FILETRANSFER_BANDWIDTH_RECEIVED])       != null && Int32.TryParse(sValue, out iValue)) FiletransferBandwidthReceived       = iValue;
                if ((sValue = info[KEY_FILETRANSFER_BYTES_SENT_TOTAL])         != null && Int32.TryParse(sValue, out iValue)) FiletransferBytesSentTotal          = iValue;
                if ((sValue = info[KEY_FILETRANSFER_BYTES_RECEIVED_TOTAL])     != null && Int32.TryParse(sValue, out iValue)) FiletransferBytesReceivedTotal      = iValue;
                if ((sValue = info[KEY_PACKETS_SENT_SPEECH])                   != null && Int32.TryParse(sValue, out iValue)) SpeechPacketsSent                   = iValue;
                if ((sValue = info[KEY_PACKETS_RECEIVED_SPEECH])               != null && Int32.TryParse(sValue, out iValue)) SpeechPacketsReceived               = iValue;
                if ((sValue = info[KEY_BYTES_SENT_SPEECH])                     != null && Int32.TryParse(sValue, out iValue)) SpeechBytesSent                     = iValue;
                if ((sValue = info[KEY_BYTES_RECEIVED_SPEECH])                 != null && Int32.TryParse(sValue, out iValue)) SpeechBytesReceived                 = iValue;
                if ((sValue = info[KEY_PACKETS_SENT_KEEPALIVE])                != null && Int32.TryParse(sValue, out iValue)) KeepalivePacketsSent                = iValue;
                if ((sValue = info[KEY_PACKETS_RECEIVED_KEEPALIVE])            != null && Int32.TryParse(sValue, out iValue)) KeepalivePacketsReceived            = iValue;
                if ((sValue = info[KEY_BYTES_SENT_KEEPALIVE])                  != null && Int32.TryParse(sValue, out iValue)) KeepaliveBytesSent                  = iValue;
                if ((sValue = info[KEY_BYTES_RECEIVED_KEEPALIVE])              != null && Int32.TryParse(sValue, out iValue)) KeepaliveBytesReceived              = iValue;
                if ((sValue = info[KEY_PACKETS_SENT_CONTROL])                  != null && Int32.TryParse(sValue, out iValue)) ControlPacketsSent                  = iValue;
                if ((sValue = info[KEY_PACKETS_RECEIVED_CONTROL])              != null && Int32.TryParse(sValue, out iValue)) ControlPacketsReceived              = iValue;
                if ((sValue = info[KEY_BYTES_SENT_CONTROL])                    != null && Int32.TryParse(sValue, out iValue)) ControlBytesSent                    = iValue;
                if ((sValue = info[KEY_BYTES_RECEIVED_CONTROL])                != null && Int32.TryParse(sValue, out iValue)) ControlBytesReceived                = iValue;
                if ((sValue = info[KEY_PACKETS_SENT_TOTAL])                    != null && Int32.TryParse(sValue, out iValue)) PacketsSentTotal                    = iValue;
                if ((sValue = info[KEY_PACKETS_RECEIVED_TOTAL])                != null && Int32.TryParse(sValue, out iValue)) PacketsReceivedTotal                = iValue;
                if ((sValue = info[KEY_BYTES_SENT_TOTAL])                      != null && Int32.TryParse(sValue, out iValue)) BytesSentTotal                      = iValue;
                if ((sValue = info[KEY_BYTES_RECEIVED_TOTAL])                  != null && Int32.TryParse(sValue, out iValue)) BytesReceivedTotal                  = iValue;
                if ((sValue = info[KEY_BANDWIDTH_SENT_LAST_SECOND_TOTAL])      != null && Int32.TryParse(sValue, out iValue)) BandwidthSentLastSecond             = iValue;
                if ((sValue = info[KEY_BANDWIDTH_RECEIVED_LAST_SECOND_TOTAL])  != null && Int32.TryParse(sValue, out iValue)) BandwidthReceivedLastSecond         = iValue;
                if ((sValue = info[KEY_BANDWIDTH_SENT_LAST_MINUTE_TOTAL])      != null && Int32.TryParse(sValue, out iValue)) BandwidthSentLastMinute             = iValue;
                if ((sValue = info[KEY_BANDWIDTH_RECEIVED_LAST_MINUTE_TOTAL])  != null && Int32.TryParse(sValue, out iValue)) BandwidthReceivedLastMinute         = iValue;
                if ((sValue = info[KEY_CREATION_TIME])            != null && UInt32.TryParse(sValue, out uiValue)) CreationTime                = uiValue;
                if ((sValue = info[KEY_CHANNELS_ONLINE])          != null && UInt32.TryParse(sValue, out uiValue)) ChannelsOnline              = uiValue;
                if ((sValue = info[KEY_CLIENT_CONNECTIONS])       != null && UInt32.TryParse(sValue, out uiValue)) TotalClientConnections      = uiValue;
                if ((sValue = info[KEY_QUERY_CLIENT_CONNECTIONS]) != null && UInt32.TryParse(sValue, out uiValue)) TotalQueryClientConnections = uiValue;
                if ((sValue = info[KEY_MAX_DOWNLOAD_TOTAL_BANDWIDTH]) != null && UInt64.TryParse(sValue, out ulValue)) MaxDownloadBandwidth = ulValue;
                if ((sValue = info[KEY_MAX_UPLOAD_TOTAL_BANDWIDTH])   != null && UInt64.TryParse(sValue, out ulValue)) MaxUploadBandwidth   = ulValue;
                if ((sValue = info[KEY_DOWNLOAD_QUOTA])               != null && UInt64.TryParse(sValue, out ulValue)) DownloadQuota        = ulValue;
                if ((sValue = info[KEY_UPLOAD_QUOTA])                 != null && UInt64.TryParse(sValue, out ulValue)) UploadQuota          = ulValue;
                if ((sValue = info[KEY_MONTH_BYTES_DOWNLOADED])       != null && UInt64.TryParse(sValue, out ulValue)) BytesDownloadedMonth = ulValue;
                if ((sValue = info[KEY_MONTH_BYTES_UPLOADED])         != null && UInt64.TryParse(sValue, out ulValue)) BytesUploadedMonth   = ulValue;
                if ((sValue = info[KEY_TOTAL_BYTES_DOWNLOADED])       != null && UInt64.TryParse(sValue, out ulValue)) BytesDownloadedTotal = ulValue;
                if ((sValue = info[KEY_TOTAL_BYTES_UPLOADED])         != null && UInt64.TryParse(sValue, out ulValue)) BytesUploadedTotal   = ulValue;
                if ((sValue = info[KEY_TOTAL_PING])                        != null && Double.TryParse(sValue, out dValue)) Ping                           = dValue;
                if ((sValue = info[KEY_TOTAL_PACKETLOSS_SPEECH])           != null && Double.TryParse(sValue, out dValue)) TotalSpeechPacketloss          = dValue;
                if ((sValue = info[KEY_TOTAL_PACKETLOSS_KEEPALIVE])        != null && Double.TryParse(sValue, out dValue)) TotalKeepalivePacketloss       = dValue;
                if ((sValue = info[KEY_TOTAL_PACKETLOSS_CONTROL])          != null && Double.TryParse(sValue, out dValue)) TotalControlPacketloss         = dValue;
                if ((sValue = info[KEY_TOTAL_PACKATLOSS_TOTAL])            != null && Double.TryParse(sValue, out dValue)) TotalPacketlossAll             = dValue;
                if ((sValue = info[KEY_PRIORITY_SPEAKER_DIMM_MODIFICATOR]) != null && Double.TryParse(sValue, out dValue)) PrioritySpeakerDimmModificator = dValue;
                if ((sValue = info[KEY_FLAG_PASSWORD])   != null) IsPassworded     = (sValue == "1");
                if ((sValue = info[KEY_WEBLIST_ENABLED]) != null) IsWebListEnabled = (sValue == "1");
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                          Parsed Classes                           *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public class NormalInfo : RawNormalInfo
        {
            new public TimeSpan? UpTime {
                get { return  (base.UpTime.HasValue) ? TimeSpan.FromSeconds(base.UpTime.Value) : default(TimeSpan?); }
                set { base.UpTime = (value.HasValue) ? (Int32?)value.Value.TotalSeconds : default(Int32?); }
            }
        }
        public class AdvancedInfo : RawAdvancedInfo
        {
            new public DateTime? CreationTime {
                get { return Utilities.UtcIntegerToDateTime(base.CreationTime); }
                set { base.CreationTime = Utilities.DateTimeToUtcInteger(value); }
            }
            new public HostMessageMode? HostMessageMode {
                get { return (HostMessageMode?)base.HostMessageMode; }
                set { base.HostMessageMode = (Int32?)value; }
            }
            new public HostBannerMode? HostBannerMode {
                get { return (HostBannerMode?)base.HostBannerMode; }
                set { base.HostBannerMode = (Int32?)value; }
            }
            new public CodecEncryptionMode? EncryptionMode {
                get { return (CodecEncryptionMode?)base.EncryptionMode; }
                set { base.EncryptionMode = (Int32?)value; }
            }
            new public LogLevel? LogClient {
                get { return (LogLevel?)base.LogClient; }
                set { base.LogClient = (Int32?)value; }
            }
            new public LogLevel? LogQuery {
                get { return (LogLevel?)base.LogQuery; }
                set { base.LogQuery = (Int32?)value; }
            }
            new public LogLevel? LogChannel {
                get { return (LogLevel?)base.LogChannel; }
                set { base.LogChannel = (Int32?)value; }
            }
            new public LogLevel? LogPermissions {
                get { return (LogLevel?)base.LogPermissions; }
                set { base.LogPermissions = (Int32?)value; }
            }
            new public LogLevel? LogServer {
                get { return (LogLevel?)base.LogServer; }
                set { base.LogServer = (Int32?)value; }
            }
            new public LogLevel? LogFiletransfer {
                get { return (LogLevel?)base.LogFiletransfer; }
                set { base.LogFiletransfer = (Int32?)value; }
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                        Teamspeak 3 Server                         *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>Info from a "serverlist" command.</summary>
        public NormalInfo Normal { get; private set; }

        /// <summary>Info from a "serverinfo" command.</summary>
        public AdvancedInfo Advanced { get; private set; }


        /// <summary>
        /// Initializes a server with the specified data.
        /// </summary>
        public Teamspeak3Server(Teamspeak3Group info = null)
        {
            Normal   = new NormalInfo();
            Advanced = new AdvancedInfo();
            Update(info);
        }


        /// <summary>
        /// Updates a server with the specified data.
        /// </summary>
        public void Update(Teamspeak3Group info, Boolean clear = false)
        {
            if (info != null) {
                Normal.SetData(info, clear);
                Advanced.SetData(info, clear);
            }
        }
    }
}
