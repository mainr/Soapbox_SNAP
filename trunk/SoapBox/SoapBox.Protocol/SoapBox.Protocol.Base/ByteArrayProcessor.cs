#region "SoapBox.Protocol License"
/// <header module="SoapBox.Protocol"> 
/// Copyright (C) 2010 SoapBox Automation, All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Protocol.
///
/// SoapBox Protocol is available under your choice of these licenses:
///  - GPLv3
///  - CDDLv1.0
///
/// GNU General Public License Usage
/// SoapBox Protocol is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// SoapBox Protocol is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with SoapBox Protocol. If not, see <http://www.gnu.org/licenses/>.
/// 
/// Common Development and Distribution License Usage
/// SoapBox Protocol is subject to the CDDL Version 1.0. 
/// You should have received a copy of the CDDL Version 1.0 along
/// with SoapBox Protocol.  If not, see <http://www.sun.com/cddl/cddl.html>.
/// The CDDL is a royalty free, open source, file based license.
/// </header>
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SoapBox.Protocol.Base
{
    internal class ByteArrayProcessor 
    {   
        protected byte[] m_buf;  // Buffer for unprocessed data.
        private byte[] m_inputBuffer = null; //data in process

        public ByteArrayProcessor()    
        {        
            m_buf = new byte[0];    
        }    

        public IEnumerable<string> Process(byte[] newData, int byteCount)    
        {
            m_inputBuffer = new byte[byteCount];
            Buffer.BlockCopy(newData, 0, m_inputBuffer, 0, byteCount);

            byte[] line;
            while ((line = ReadLine()) != null)            
            {
                string part1 = Encoding.ASCII.GetString(m_buf);
                string part2 = Encoding.ASCII.GetString(line);
                string retVal = part1 + part2;
                yield return retVal;
                m_buf = new byte[0];
            }            
            // Store last incomplete line in buffer.            
            m_buf = concat(m_buf, m_inputBuffer);
        }

        private byte[] concat(byte[] first, byte[] second)
        {
            int newLen = first.Length + second.Length;
            byte[] retVal = new byte[newLen];
            int firstLength = first.Length;
            Buffer.BlockCopy(first, 0, retVal, 0, firstLength);
            Buffer.BlockCopy(second, 0, retVal, firstLength, second.Length);
            return retVal;
        }

        /// <summary>
        /// Immitates a stream ReadLine (modifies the input!)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte[] ReadLine()
        {
            int foundNewLine = -1;
            for (int i = 0; i < m_inputBuffer.Length; i++)
            {
                if (m_inputBuffer[i] == 10) //newline character
                {
                    foundNewLine = i;
                    break;
                }
            }
            if (foundNewLine >= 0)
            {
                byte[] returnVal = new byte[foundNewLine];
                byte[] newBuffer = new byte[m_inputBuffer.Length - foundNewLine - 1];
                Buffer.BlockCopy(m_inputBuffer, 0, returnVal, 0, foundNewLine);
                Buffer.BlockCopy(m_inputBuffer, foundNewLine + 1, newBuffer, 0, m_inputBuffer.Length - foundNewLine - 1);
                m_inputBuffer = newBuffer;
                return returnVal;
            }
            else
            {
                return null;
            }
        }

    }
}
