using System;
using System.Text;
using System.Net;
using System.Management;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace sroprot
{
    public sealed class Utility
    {
        static SHA1 m_ShaProvider = new SHA1CryptoServiceProvider();

        public static string GetSha1Hash(string original_str)
        {
            string result = string.Empty;
            byte[] str_buf = ASCIIEncoding.ASCII.GetBytes(original_str);

            byte[] output = m_ShaProvider.ComputeHash(str_buf);

            for(int i = 0; i < output.Length; i++)
            {
                result += string.Format("{0:x2}", output[i]);
            }
            return result;
        }

        //меиод для генерации пароль, пока пусть тут так как хз куда его приткнуть
        public static string HashPassword(string uname, string password)
        {
            return Utility.GetSha1Hash(uname + password + Global.Sha1PasswordSalt);
        }

        public static string HexDump(byte[] buffer)
        {
            return HexDump(buffer, 0, buffer.Length);
        }

        public static string HexDump(byte[] buffer, int offset, int count)
        {
            const int bytesPerLine = 16;
            StringBuilder output = new StringBuilder();
            StringBuilder ascii_output = new StringBuilder();
            int length = count;
            if (length % bytesPerLine != 0)
            {
                length += bytesPerLine - length % bytesPerLine;
            }
            for (int x = 0; x <= length; ++x)
            {
                if (x % bytesPerLine == 0)
                {
                    if (x > 0)
                    {
                        output.AppendFormat("  {0}{1}", ascii_output.ToString(), Environment.NewLine);
                        ascii_output.Clear();
                    }
                    if (x != length)
                    {
                        output.AppendFormat("{0:d10}   ", x);
                    }
                }
                if (x < count)
                {
                    output.AppendFormat("{0:X2} ", buffer[offset + x]);
                    char ch = (char)buffer[offset + x];
                    if (!Char.IsControl(ch))
                    {
                        ascii_output.AppendFormat("{0}", ch);
                    }
                    else
                    {
                        ascii_output.Append(".");
                    }
                }
                else
                {
                    output.Append("   ");
                    ascii_output.Append(".");
                }
            }
            return output.ToString();
        }

        public static string GetRemoteEpString(Socket sock)
        {
            string result = string.Empty;

            result = ((IPEndPoint)(sock.RemoteEndPoint)).Address.ToString();
            return result;
        }

        public static double BytesToMegabytes(ulong bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        public static string GetHardwareId()
        {
            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {
                id = mo["ProcessorId"].ToString();
                break;
            }
            return id;
        }
    }
}
