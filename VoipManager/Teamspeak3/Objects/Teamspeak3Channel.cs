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
using VoipManager.Teamspeak3.Communication;

namespace VoipManager.Teamspeak3.Objects
{
    public class Teamspeak3Channel
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
            public Int32? Pid;
            public Int32? Order;
            public Int32? TotalClients;
            public Int32? PowerNeededToSub;
            
            public void SetData(Teamspeak3Group info, Boolean reset)
            {
                String sValue;
                Int32  iValue;

                if (reset) {
                    Pid              = null;
                    Order            = null;
                    TotalClients     = null;
                    PowerNeededToSub = null;
                }

                if ((sValue = info[KEY_PARENT_ID])              != null && Int32.TryParse(sValue, out iValue)) Pid              = iValue;
                if ((sValue = info[KEY_ORDER])                  != null && Int32.TryParse(sValue, out iValue)) Order            = iValue;
                if ((sValue = info[KEY_TOTAL_CLIENTS])          != null && Int32.TryParse(sValue, out iValue)) TotalClients     = iValue;
                if ((sValue = info[KEY_POWER_NEEDED_SUBSCRIBE]) != null && Int32.TryParse(sValue, out iValue)) PowerNeededToSub = iValue;
            }
        }
        public class RawAdvancedInfo
        {
            public String   Topic;
            public String   Description;
            public String   Password;
            public String   FilePath;
            public String   PhoneticName;
            public Int32?   Codec;
            public Int32?   CodecQuality;
            public Int32?   CodecLatencyFactor;
            public Int32?   MaxClients;
            public Int32?   MaxFamilyClients;
            public Int32?   NeededTalkPower;
            public Int32?   IconId;
            public Boolean? IsCodecUnencrypted;
            public Boolean? IsPermanent;
            public Boolean? IsSemiPermanent;
            public Boolean? IsDefault;
            public Boolean? IsPassworded;
            public Boolean? AreMaxClientsUnlimited;
            public Boolean? AreMaxFamilyClientsUnlimited;
            public Boolean? AreMaxFamilyClientsInherited;
            public Boolean? ForcedSilence;
            
            public void SetData(Teamspeak3Group info, Boolean reset)
            {
                String  sValue;
                Int32   iValue;

                if (reset) {
                    Topic                        = null;
                    Description                  = null;
                    Password                     = null;
                    FilePath                     = null;
                    PhoneticName                 = null;
                    Codec                        = null;
                    CodecQuality                 = null;
                    CodecLatencyFactor           = null;
                    MaxClients                   = null;
                    MaxFamilyClients             = null;
                    NeededTalkPower              = null;
                    IconId                       = null;
                    IsCodecUnencrypted           = null;
                    IsPermanent                  = null;
                    IsSemiPermanent              = null;
                    IsDefault                    = null;
                    IsPassworded                   = null;
                    AreMaxClientsUnlimited       = null;
                    AreMaxFamilyClientsUnlimited = null;
                    AreMaxFamilyClientsInherited = null;
                    ForcedSilence                = null;
                }
                
                if ((sValue = info[KEY_TOPIC])         != null) Topic        = sValue;
                if ((sValue = info[KEY_DESCRIPTION])   != null) Description  = sValue;
                if ((sValue = info[KEY_PASSWORD])      != null) Password     = sValue;
                if ((sValue = info[KEY_FILE_PATH])     != null) FilePath     = sValue;
                if ((sValue = info[KEY_PHONETIC_NAME]) != null) PhoneticName = sValue;
                if ((sValue = info[KEY_CODEC])                != null && Int32.TryParse(sValue, out iValue)) Codec              = iValue;
                if ((sValue = info[KEY_CODEC_QUALITY])        != null && Int32.TryParse(sValue, out iValue)) CodecQuality       = iValue;
                if ((sValue = info[KEY_CODEC_LATENCY_FACTOR]) != null && Int32.TryParse(sValue, out iValue)) CodecLatencyFactor = iValue;
                if ((sValue = info[KEY_MAX_CLIENTS])          != null && Int32.TryParse(sValue, out iValue)) MaxClients         = iValue;
                if ((sValue = info[KEY_MAX_FAMILY_CLIENTS])   != null && Int32.TryParse(sValue, out iValue)) MaxFamilyClients   = iValue;
                if ((sValue = info[KEY_NEEDED_TALK_POWER])    != null && Int32.TryParse(sValue, out iValue)) NeededTalkPower    = iValue;
                if ((sValue = info[KEY_ICON_ID])              != null && Int32.TryParse(sValue, out iValue)) IconId             = iValue;
                if ((sValue = info[KEY_IS_CODEC_UNENCRYPTED])              != null) IsCodecUnencrypted           = (sValue == "1");
                if ((sValue = info[KEY_FLAG_PERMANENT])                    != null) IsPermanent                  = (sValue == "1");
                if ((sValue = info[KEY_FLAG_SEMI_PERMANENT])               != null) IsSemiPermanent              = (sValue == "1");
                if ((sValue = info[KEY_FLAG_DEFAULT])                      != null) IsDefault                    = (sValue == "1");
                if ((sValue = info[KEY_FLAG_PASSWORD])                     != null) IsPassworded                   = (sValue == "1");
                if ((sValue = info[KEY_FLAG_MAX_CLIENTS_UNLIMITED])        != null) AreMaxClientsUnlimited       = (sValue == "1");
                if ((sValue = info[KEY_FLAG_MAX_FAMILY_CLIENTS_UNLIMITED]) != null) AreMaxFamilyClientsUnlimited = (sValue == "1");
                if ((sValue = info[KEY_FLAG_MAX_FAMILY_CLIENTS_INHERITED]) != null) AreMaxFamilyClientsInherited = (sValue == "1");
                if ((sValue = info[KEY_FORCED_SILENCE])                    != null) ForcedSilence                = (sValue == "1");
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                          Parsed Classes                           *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        public class BasicInfo
        {
            public RawBasicInfo Raw { get; private set; }

            public String Name { get { return Raw.Name; } set { Raw.Name = value; } }
            public Int32? Id   { get { return Raw.Id;   } set { Raw.Id   = value; } }

            public BasicInfo() { Raw = new RawBasicInfo(); }
            

            public void SetData(Teamspeak3Group info, Boolean reset)
            {
                Raw.SetData(info, reset);
            }
        }
        public class NormalInfo
        {
            public RawNormalInfo Raw { get; private set; }

            public Int32? Pid              { get { return Raw.Pid;              } set { Raw.Pid              = value; } }
            public Int32? Order            { get { return Raw.Order;            } set { Raw.Order            = value; } }
            public Int32? TotalClients     { get { return Raw.TotalClients;     } set { Raw.TotalClients     = value; } }
            public Int32? PowerNeededToSub { get { return Raw.PowerNeededToSub; } set { Raw.PowerNeededToSub = value; } }

            public NormalInfo() { Raw = new RawNormalInfo(); }
            

            public void SetData(Teamspeak3Group info, Boolean reset)
            {
                Raw.SetData(info, reset);
            }
        }
        public class AdvancedInfo
        {
            public RawAdvancedInfo Raw { get; private set; }

            public String   Topic                        { get { return Raw.Topic;                        } set { Raw.Topic                        = value; } }
            public String   Description                  { get { return Raw.Description;                  } set { Raw.Description                  = value; } }
            public String   Password                     { get { return Raw.Password;                     } set { Raw.Password                     = value; } }
            public String   FilePath                     { get { return Raw.FilePath;                     } set { Raw.FilePath                     = value; } }
            public String   PhoneticName                 { get { return Raw.PhoneticName;                 } set { Raw.PhoneticName                 = value; } }
            public Int32?   CodecQuality                 { get { return Raw.CodecQuality;                 } set { Raw.CodecQuality                 = value; } }
            public Int32?   CodecLatencyFactor           { get { return Raw.CodecLatencyFactor;           } set { Raw.CodecLatencyFactor           = value; } }
            public Int32?   MaxClients                   { get { return Raw.MaxClients;                   } set { Raw.MaxClients                   = value; } }
            public Int32?   MaxFamilyClients             { get { return Raw.MaxFamilyClients;             } set { Raw.MaxFamilyClients             = value; } }
            public Int32?   NeededTalkPower              { get { return Raw.NeededTalkPower;              } set { Raw.NeededTalkPower              = value; } }
            public Int32?   IconId                       { get { return Raw.IconId;                       } set { Raw.IconId                       = value; } }
            public Boolean? IsCodecUnencrypted           { get { return Raw.IsCodecUnencrypted;           } set { Raw.IsCodecUnencrypted           = value; } }
            public Boolean? IsPermanent                  { get { return Raw.IsPermanent;                  } set { Raw.IsPermanent                  = value; } }
            public Boolean? IsSemiPermanent              { get { return Raw.IsSemiPermanent;              } set { Raw.IsSemiPermanent              = value; } }
            public Boolean? IsDefault                    { get { return Raw.IsDefault;                    } set { Raw.IsDefault                    = value; } }
            public Boolean? IsPassworded                 { get { return Raw.IsPassworded;                 } set { Raw.IsPassworded                 = value; } }
            public Boolean? AreMaxClientsUnlimited       { get { return Raw.AreMaxClientsUnlimited;       } set { Raw.AreMaxClientsUnlimited       = value; } }
            public Boolean? AreMaxFamilyClientsUnlimited { get { return Raw.AreMaxFamilyClientsUnlimited; } set { Raw.AreMaxFamilyClientsUnlimited = value; } }
            public Boolean? AreMaxFamilyClientsInherited { get { return Raw.AreMaxFamilyClientsInherited; } set { Raw.AreMaxFamilyClientsInherited = value; } }
            public Boolean? ForcedSilence                { get { return Raw.ForcedSilence;                } set { Raw.ForcedSilence                = value; } }

            public CodecType? Codec { 
                get { return (CodecType?)Raw.Codec; }
                set { Raw.Codec = (Int32?)value; }
            }

            public AdvancedInfo() { Raw = new RawAdvancedInfo(); }
            

            public void SetData(Teamspeak3Group info, Boolean reset)
            {
                Raw.SetData(info, reset);
            }
        }


        /// <summary>Info from a "channelfind" command.</summary>
        public BasicInfo Basic { get; private set; }
        /// <summary>Info from a "channellist" command.</summary>
        public NormalInfo Normal { get; private set; }
        /// <summary>Info from a "channelinfo" command.</summary>
        public AdvancedInfo Advanced { get; private set; }

        /// <summary>Initializes a channel with default values.</summary>
        public Teamspeak3Channel()
        {
            Basic    = new BasicInfo();
            Normal   = new NormalInfo();
            Advanced = new AdvancedInfo();
        }

        /// <summary>Initializes a channel with the specified data.</summary>
        public Teamspeak3Channel(Teamspeak3Group info) : this()
        {
            Basic.SetData(info, true);
            Normal.SetData(info, true);
            Advanced.SetData(info, true);
        }


        /// <summary>Updates a client with the specified data.</summary>
        public void Update(Teamspeak3Group info)
        {
            Basic.SetData(info, false);
            Normal.SetData(info, false);
            Advanced.SetData(info, false);
        }
    }
}
