using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBTUtility;

namespace MVZ2.OldSaves
{
    internal static class NBTHelper
    {
        public static bool LoadBool(this NBTData nbt, string key, bool defaultValue)
        {
            return nbt.TryGetValue(key, out var data) ? ((byte)data > 0) : defaultValue;
        }
        public static byte LoadByte(this NBTData nbt, string key, byte defaultValue)
        {
            return nbt.TryGetValue(key, out var data) ? (byte)data : defaultValue;
        }
        public static short LoadShort(this NBTData nbt, string key, short defaultValue)
        {
            return nbt.TryGetValue(key, out var data) ? (short)data : defaultValue;
        }
        public static int LoadInt(this NBTData nbt, string key, int defaultValue)
        {
            return nbt.TryGetValue(key, out var data) ? (int)data : defaultValue;
        }
        public static long LoadLong(this NBTData nbt, string key, long defaultValue)
        {
            return nbt.TryGetValue(key, out var data) ? (long)data : defaultValue;
        }
        public static float LoadFloat(this NBTData nbt, string key, float defaultValue)
        {
            return nbt.TryGetValue(key, out var data) ? (float)data : defaultValue;
        }
        public static double LoadDouble(this NBTData nbt, string key, double defaultValue)
        {
            return nbt.TryGetValue(key, out var data) ? (double)data : defaultValue;
        }
        public static string LoadString(this NBTData nbt, string key, string defaultValue)
        {
            return nbt.TryGetValue(key, out var data) ? (string)data : defaultValue;
        }
        public static byte[] LoadByteArray(this NBTData nbt, string key, byte[] defaultValue)
        {
            return nbt.TryGetValue(key, out var data) ? (byte[])data : defaultValue;
        }
    }
}
