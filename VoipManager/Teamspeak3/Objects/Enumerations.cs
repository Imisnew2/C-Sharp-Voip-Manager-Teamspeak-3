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

namespace VoipManager.Teamspeak3.Objects
{
    /// <summary>
    /// What type of message this was in the log.
    /// </summary>
    public enum LogLevel
    {
        Critical  = 0x00,
        Error     = 0x01, 
        Warning   = 0x02,
        Debug     = 0x03,
        Info      = 0x04,
        Developer = 0x05
    }

    /// <summary>
    /// The type of token.
    /// </summary>
    public enum TokenType
    {
        ServerGroup  = 0x00,
        ChannelGroup = 0x01
    }

    /// <summary>
    /// The type of codec for the channel.
    /// </summary>
    public enum CodecType
    {
        SpeexNarrowBand    = 0x00,
        SpeexWideBand      = 0x01,
        SpeexUltraWideBand = 0x02,
        CeltMono           = 0x03
    }

    /// <summary>
    /// How codec encryption is handled in the server.
    /// </summary>
    public enum CodecEncryptionMode
    {
        Individual = 0x00,
        Disabled   = 0x01,
        Enabled    = 0x02
    }

    /// <summary>
    /// How to kick a client.
    /// </summary>
    public enum KickMode
    {
        Channel = 0x04,
        Server  = 0x05
    }

    /// <summary>
    /// The target of a text message.
    /// </summary>
    public enum TextMessageTarget
    {
        Client  = 0x01,
        Channel = 0x02,
        Server  = 0x03
    }

    /// <summary>
    /// How to perform a global message.
    /// </summary>
    public enum HostMessageMode
    {
        None      = 0x00,
        Log       = 0x01,
        Modal     = 0x02,
        ModalQuit = 0x03
    }

    /// <summary>
    /// How the banner is handled.
    /// </summary>
    public enum HostBannerMode
    {
        NoAdjust     = 0x00,
        IgnoreAspect = 0x01,
        KeepAspect   = 0x02
    }

    /// <summary>
    /// The type of client.
    /// </summary>
    public enum ClientType
    {
        Regular     = 0x00,
        ServerQuery = 0x01
    }

    /// <summary>
    /// The type of group in the db (template = virtual server).
    /// </summary>
    public enum GroupDbType
    {
        Template    = 0x00,
        Regular     = 0x01,
        ServerQuery = 0x02
    }

    /// <summary>
    /// How to display the group name.
    /// </summary>
    public enum GroupNameMode
    {
        Hidden = 0x00,
        Before = 0x01,
        Behind = 0x02
    }

    /// <summary>
    /// Iunno.
    /// </summary>
    public enum GroupIdentify
    {
        Strongest = 0x01,
        Weakest   = 0x02
    }

    /// <summary>
    /// The type of permission.
    /// </summary>
    public enum PermissionType
    {
        ServerGroup   = 0x00,
        Client        = 0x01,
        Channel       = 0x02,
        ChannelGroup  = 0x03,
        ChannelClient = 0x04
    }

    /// <summary>
    /// The category the permission falls under.
    /// </summary>
    public enum PermissionCategory
    {
        Global                 = 0x10,
        GlobalInformation      = 0x11,
        GlobalServerManagement = 0x12,
        GlobalAdminActions     = 0x13,
        GlobalSettings         = 0x14,

        Server             = 0x20,
        ServerInformation  = 0x21,
        ServerAdminActions = 0x22,
        ServerSettings     = 0x23,

        Channel            = 0x30,
        ChannelInformation = 0x31,
        ChannelCreate      = 0x32,
        ChannelModify      = 0x33,
        ChannelDelete      = 0x34,
        ChannelAccess      = 0x35,

        Group            = 0x40,
        GroupInformation = 0x41,
        GroupCreate      = 0x42,
        GroupModify      = 0x43,
        GroupDelete      = 0x44,

        Client             = 0x50,
        ClientInformation  = 0x51,
        ClientAdminActions = 0x52,
        ClientBasics       = 0x53,
        ClientModify       = 0x54,

        FileTransfer = 0x60,

        NeededModifyPower = 0xFF
    }

    /// <summary>
    /// The type of file.
    /// </summary>
    public enum FileType
    {
        Directory = 0x00,
        Regular   = 0x01
    }

    /// <summary>
    /// Not sure.
    /// </summary>
    public enum Snapshot
    {
        String      = 0x00,
        Base64      = 0x01,
        Hexadecimal = 0x02,
    }

    /// <summary>
    /// The type of spacer.
    /// </summary>
    public enum SpacerType
    {
        SolidLine      = 0x00,
        DashLine       = 0x01,
        DotLine        = 0x02,
        DashDotLine    = 0x03,
        DashDotDotLine = 0x04,
        Custom         = 0x05
    }

    /// <summary>
    /// How the spacer is aligned.
    /// </summary>
    public enum SpacerAlignment
    {
        Left   = 0x00,
        Right  = 0x01,
        Center = 0x02,
        Repeat = 0x03
    }

    /// <summary>
    /// Reasons for notifications.
    /// </summary>
    public enum Reason
    {
        None               = 0x00,
        Move               = 0x01,
        Subscription       = 0x02,
        Timeout            = 0x03,
        ChannelKick        = 0x04,
        ServerKick         = 0x05,
        ServerBan          = 0x06,
        ServerStop         = 0x07,
        Disconnect         = 0x08,
        ChannelUpdate      = 0x09,
        ChannelEdit        = 0x0A,
        DisconnectShutdown = 0x0B
    }
}
