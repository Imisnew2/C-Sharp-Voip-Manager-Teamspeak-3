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
using System.Text;
using VoipManager.Communication;

namespace VoipManager.Teamspeak3.Communication
{
    public abstract class Teamspeak3Response : IResponse
    {
        // We only want to expose the normalized values for a Teamspeak 3 Response.
        public Teamspeak3Request Request { get; private set; }
        public String            Raw     { get; private set; }

        // If they cast it to a IResponse, they can get the 'raw' values.
        IRequest IResponse.Request { 
            get { return Request; }
            set { Request = (Teamspeak3Request)value; }
        }
        Byte[] IResponse.Raw {
            get { return Encoding.Default.GetBytes(Raw); }
        }


        /// <summary>
        /// Parses the raw response into logical sections and groups.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"/>
        protected Teamspeak3Response(String raw)
        {
            if (raw == null) {
                throw new ArgumentNullException("raw");
            }

            Raw = raw;
        }
    }
}
