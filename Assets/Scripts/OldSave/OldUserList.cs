using System.Collections.Generic;
using System.IO;
using System.Text;
using NBTUtility;
using UnityEngine;

namespace MVZ2.OldSaves
{
    public class OldUserList
    {
        public string[] usernames;

        public static OldUserList ReadStream(Stream stream)
        {
            var usernames = new string[8];

            stream.Seek(1, SeekOrigin.Begin);
            while (stream.Position < stream.Length)
            {
                int pos = (int)stream.Position;

                int userNo = stream.ReadByte();
                int nameLength = stream.ReadByte();
                string name = ReadString(stream, nameLength, Encoding.UTF8);

                usernames[userNo] = name;
            }
            return new OldUserList() { usernames = usernames };
        }
        private static string ReadString(Stream fileStream, int length, Encoding encoding)
        {
            byte[] nameBytes = new byte[length];
            fileStream.Read(nameBytes, 0, length);
            return encoding.GetString(nameBytes);
        }
    }
}
