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
    public class TeamspeakChannel
    {
        #region Channel Data Keys

        private const String KEY_NAME = "channel_name";
        private const String KEY_ID   = "cid";

        private const String KEY_PARENT_ID              = "pid";
        private const String KEY_ORDER                  = "channel_order";
        private const String KEY_TOTAL_CLIENTS          = "total_clients";
        private const String KEY_POWER_NEEDED_SUBSCRIBE = "channel_needed_subscribe_power";

        private const String KEY_TOPIC                             = "channel_topic";
        private const String KEY_DESCRIPTION                       = "channel_description";
        private const String KEY_PASSWORD                          = "channel_password";
        private const String KEY_FILE_PATH                         = "channel_filepath";
        private const String KEY_PHONETIC_NAME                     = "channel_name_phonetic";
        private const String KEY_CODEC                             = "channel_codec";
        private const String KEY_CODEC_QUALITY                     = "channel_codec_quality";
        private const String KEY_CODEC_LATENCY_FACTOR              = "channel_codec_latency_factor";
        private const String KEY_MAX_CLIENTS                       = "channel_maxclients";
        private const String KEY_MAX_FAMILY_CLIENTS                = "channel_maxfamilyclients";
        private const String KEY_NEEDED_TALK_POWER                 = "channel_needed_talk_power";
        private const String KEY_ICON_ID                           = "channel_icon_id";
        private const String KEY_IS_CODEC_UNENCRYPTED              = "channel_codec_is_unencrypted";
        private const String KEY_FLAG_PERMANENT                    = "channel_flag_permanent";
        private const String KEY_FLAG_SEMI_PERMANENT               = "channel_flag_semi_permanent";
        private const String KEY_FLAG_DEFAULT                      = "channel_flag_default";
        private const String KEY_FLAG_PASSWORD                     = "channel_flag_password";
        private const String KEY_FLAG_MAX_CLIENTS_UNLIMITED        = "channel_flag_maxclients_unlimited";
        private const String KEY_FLAG_MAX_FAMILY_CLIENTS_UNLIMITED = "channel_flag_maxfamilyclients_unlimited";
        private const String KEY_FLAG_MAX_FAMILY_CLIENTS_INHERITED = "channel_flag_maxfamilyclients_inherited";
        private const String KEY_FORCED_SILENCE                    = "channel_forced_silence";

        #endregion

        public class BasicInfo
        {
            // Data received from a "channelfind" command.
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
            // Data received from a "channellist" command.
            public Int32? Pid              = null;
            public Int32? Order            = null;
            public Int32? TotalClients     = null;
            public Int32? PowerNeededToSub = null;

            // Sets the normal information from a response.
            public void SetData(TeamspeakGroup info, Boolean reset)
            {
                String sValue;
                Int32  iValue;

                if ((sValue = info[KEY_PARENT_ID])              != null && Int32.TryParse(sValue, out iValue)) Pid              = iValue; else if (reset) Pid              = null;
                if ((sValue = info[KEY_ORDER])                  != null && Int32.TryParse(sValue, out iValue)) Order            = iValue; else if (reset) Order            = null;
                if ((sValue = info[KEY_TOTAL_CLIENTS])          != null && Int32.TryParse(sValue, out iValue)) TotalClients     = iValue; else if (reset) TotalClients     = null;
                if ((sValue = info[KEY_POWER_NEEDED_SUBSCRIBE]) != null && Int32.TryParse(sValue, out iValue)) PowerNeededToSub = iValue; else if (reset) PowerNeededToSub = null;
            }
        }
        public class AdvancedInfo
        {
            // Data received from a "channelinfo" command.
            public String   Topic                         = null;
            public String   Description                   = null;
            public String   Password                      = null;
            public String   FilePath                      = null;
            public String   PhoneticName                  = null;
            public Int32?   Codec                         = null;
            public Int32?   CodecQuality                  = null;
            public Int32?   CodecLatencyFactor            = null;
            public Int32?   MaxClients                    = null;
            public Int32?   MaxFamilyClients              = null;
            public Int32?   NeededTalkPower               = null;
            public Int32?   IconId                        = null;
            public Boolean? IsCodecUnencrypted            = null;
            public Boolean? FlagPermanent                 = null;
            public Boolean? FlagSemiPermanent             = null;
            public Boolean? FlagDefault                   = null;
            public Boolean? FlagPassword                  = null;
            public Boolean? FlagMaxClientsUnlimited       = null;
            public Boolean? FlagMaxFamilyClientsUnlimited = null;
            public Boolean? FlagMaxFamilyClientsInherited = null;
            public Boolean? ForcedSilence                 = null;
            
            // Sets the advanced information from a response.
            public void SetData(TeamspeakGroup info, Boolean reset)
            {
                String  sValue;
                Int32   iValue;
                
                if ((sValue = info[KEY_TOPIC])         != null) Topic        = sValue; else if (reset) Topic        = null;
                if ((sValue = info[KEY_DESCRIPTION])   != null) Description  = sValue; else if (reset) Description  = null;
                if ((sValue = info[KEY_PASSWORD])      != null) Password     = sValue; else if (reset) Password     = null;
                if ((sValue = info[KEY_FILE_PATH])     != null) FilePath     = sValue; else if (reset) FilePath     = null;
                if ((sValue = info[KEY_PHONETIC_NAME]) != null) PhoneticName = sValue; else if (reset) PhoneticName = null;
                if ((sValue = info[KEY_CODEC])                != null && Int32.TryParse(sValue, out iValue)) Codec              = iValue; else if (reset) Codec              = null;
                if ((sValue = info[KEY_CODEC_QUALITY])        != null && Int32.TryParse(sValue, out iValue)) CodecQuality       = iValue; else if (reset) CodecQuality       = null;
                if ((sValue = info[KEY_CODEC_LATENCY_FACTOR]) != null && Int32.TryParse(sValue, out iValue)) CodecLatencyFactor = iValue; else if (reset) CodecLatencyFactor = null;
                if ((sValue = info[KEY_MAX_CLIENTS])          != null && Int32.TryParse(sValue, out iValue)) MaxClients         = iValue; else if (reset) MaxClients         = null;
                if ((sValue = info[KEY_MAX_FAMILY_CLIENTS])   != null && Int32.TryParse(sValue, out iValue)) MaxFamilyClients   = iValue; else if (reset) MaxFamilyClients   = null;
                if ((sValue = info[KEY_NEEDED_TALK_POWER])    != null && Int32.TryParse(sValue, out iValue)) NeededTalkPower    = iValue; else if (reset) NeededTalkPower    = null;
                if ((sValue = info[KEY_ICON_ID])              != null && Int32.TryParse(sValue, out iValue)) IconId             = iValue; else if (reset) IconId             = null;
                if ((sValue = info[KEY_IS_CODEC_UNENCRYPTED])              != null) IsCodecUnencrypted            = (sValue == "1") ? true : false; else if (reset) IsCodecUnencrypted            = null;
                if ((sValue = info[KEY_FLAG_PERMANENT])                    != null) FlagPermanent                 = (sValue == "1") ? true : false; else if (reset) FlagPermanent                 = null;
                if ((sValue = info[KEY_FLAG_SEMI_PERMANENT])               != null) FlagSemiPermanent             = (sValue == "1") ? true : false; else if (reset) FlagSemiPermanent             = null;
                if ((sValue = info[KEY_FLAG_DEFAULT])                      != null) FlagDefault                   = (sValue == "1") ? true : false; else if (reset) FlagDefault                   = null;
                if ((sValue = info[KEY_FLAG_PASSWORD])                     != null) FlagPassword                  = (sValue == "1") ? true : false; else if (reset) FlagPassword                  = null;
                if ((sValue = info[KEY_FLAG_MAX_CLIENTS_UNLIMITED])        != null) FlagMaxClientsUnlimited       = (sValue == "1") ? true : false; else if (reset) FlagMaxClientsUnlimited       = null;
                if ((sValue = info[KEY_FLAG_MAX_FAMILY_CLIENTS_UNLIMITED]) != null) FlagMaxFamilyClientsUnlimited = (sValue == "1") ? true : false; else if (reset) FlagMaxFamilyClientsUnlimited = null;
                if ((sValue = info[KEY_FLAG_MAX_FAMILY_CLIENTS_INHERITED]) != null) FlagMaxFamilyClientsInherited = (sValue == "1") ? true : false; else if (reset) FlagMaxFamilyClientsInherited = null;
                if ((sValue = info[KEY_FORCED_SILENCE])                    != null) ForcedSilence                 = (sValue == "1") ? true : false; else if (reset) ForcedSilence                 = null;
            }
        }

        public BasicInfo    Basic    { get; private set; }
        public NormalInfo   Normal   { get; private set; }
        public AdvancedInfo Advanced { get; private set; }
        
        // Initializes a channel with default values.
        public TeamspeakChannel()
        {
            Basic    = new BasicInfo();
            Normal   = new NormalInfo();
            Advanced = new AdvancedInfo();
        }
        // Initializes a channel with the specified data.
        public TeamspeakChannel(TeamspeakGroup info, Boolean reset = true) : this()
        {
            Basic.SetData(info, reset);
            Normal.SetData(info, reset);
            Advanced.SetData(info, reset);
        }
    }
}
