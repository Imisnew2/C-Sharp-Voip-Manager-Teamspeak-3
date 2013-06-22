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
using System.Diagnostics.Contracts;
using System.Text;

namespace VoipManager.Teamspeak3.Communication
{
    using VoipManager.Communication;

    public abstract class Teamspeak3Response : IResponse
    {
        #region IResponse

        public Byte[] Raw
        {
            get { return Encoding.Default.GetBytes(RawText); }
        }
        public String RawText { get; private set; }

        #endregion IResponse



        /// <summary>
        /// Parses the raw response into logical sections and groups.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"/>
        protected Teamspeak3Response(String rawText)
        {
            Utilities.Require<ArgumentNullException>(rawText != null, "raw");

            RawText = rawText;
        }
    }
}
