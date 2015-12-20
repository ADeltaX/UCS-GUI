using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class Message
    {
        private static readonly List<string> m_vChatFilterList = new List<string>();
        private byte[] m_vData;
        private int m_vLength;
        private ushort m_vMessageVersion;
        private ushort m_vType;

        public Message()
        {
        }

        public Message(Client c)
        {
            Client = c;
            m_vType = 0;
            m_vLength = -1;
            m_vMessageVersion = 0;
            m_vData = null;
        }

        public Message(Client c, Message m) //Clone
        {
            m_vType = m.GetMessageType();
            m_vLength = m.GetLength();
            m_vData = new byte[m.GetLength()];
            Array.Copy(m.GetData(), m_vData, m.GetLength());
            m_vMessageVersion = m.GetMessageVersion();
            Client = c;
        }

        public Message(Client c, BinaryReader br)
        {
            Client = c;
            m_vType = br.ReadUInt16WithEndian();
            var tempLength = br.ReadBytes(3);
            m_vLength = (0x00 << 24) | (tempLength[0] << 16) | (tempLength[1] << 8) | tempLength[2];
            m_vMessageVersion = br.ReadUInt16WithEndian();
            m_vData = br.ReadBytes(m_vLength);
        }

        public int Broadcasting { get; set; }

        public Client Client { get; set; }

        public static string FilterString(string str)
        {
            var filter = GetChatFilterList();
            var sb = new StringBuilder();
            var filterIsUsed = false;
            foreach (var s in filter)
            {
                if (str.ToLower().Contains(s.ToLower()))
                {
                    sb.Clear();
                    filterIsUsed = true;
                    var replacment = "";
                    for (var i = 0; i < s.Length; i++)
                        replacment += "*";

                    var parts = str.ToLower().Split(new[] {s.ToLower()}, StringSplitOptions.None);

                    sb.Append(str.Substring(0, parts[0].Length));

                    for (var i = 1; i < parts.Length; i++)
                    {
                        sb.Append(replacment);
                        sb.Append(str.Substring(sb.Length, parts[i].Length));
                    }
                    str = sb.ToString();
                }
            }
            if (filterIsUsed)
            {
                return sb.ToString();
            }
            return str;
        }

        public static List<string> GetChatFilterList()
        {
            if (m_vChatFilterList.Count == 0)
            {
                var fileName = ConfigurationManager.AppSettings["filterFilePath"];
                var lines = File.ReadAllLines(fileName);
                m_vChatFilterList.AddRange(lines);
            }
            return m_vChatFilterList;
        }

        public static void ReloadChatFilterList()
        {
            m_vChatFilterList.Clear();

            var fileName = ConfigurationManager.AppSettings["filterFilePath"];
            var lines = File.ReadAllLines(fileName);
            m_vChatFilterList.AddRange(lines);
        }

        public virtual void Decode()
        {
        }

        public virtual void Encode()
        {
        }

        public byte[] GetData()
        {
            return m_vData;
        }

        public int GetLength()
        {
            return m_vLength;
        }

        public ushort GetMessageType()
        {
            return m_vType;
        }

        public ushort GetMessageVersion()
        {
            return m_vMessageVersion;
        }

        public byte[] GetRawData()
        {
            var encodedMessage = new List<byte>();

            encodedMessage.AddRange(BitConverter.GetBytes(m_vType).Reverse());
            encodedMessage.AddRange(BitConverter.GetBytes(m_vLength).Reverse().Skip(1));
            encodedMessage.AddRange(BitConverter.GetBytes(m_vMessageVersion).Reverse());
            encodedMessage.AddRange(m_vData);

            return encodedMessage.ToArray();
        }

        public virtual void Process(Level level)
        {
        }

        public void SetData(byte[] data)
        {
            m_vData = data;
            m_vLength = data.Length;
        }

        public void SetMessageType(ushort type)
        {
            m_vType = type;
        }

        public void SetMessageVersion(ushort v)
        {
            m_vMessageVersion = v;
        }

        public string ToHexString()
        {
            var hex = BitConverter.ToString(m_vData);
            return hex.Replace("-", " ");
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(m_vData, 0, m_vLength);
        }
    }
}