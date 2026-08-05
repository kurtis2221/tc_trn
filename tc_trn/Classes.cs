using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace tc_trn
{
    class Constants
    {
        public static byte[] BYTES_VECTOR3_ZERO = new byte[12];
        public static byte[] BYTES_UINT_ZERO = new byte[4];
        public static byte[] BYTES_100 = { 0x00, 0x00, 0xC8, 0x42 };
        public static byte[] BYTE_0 = { 0x00 };
        public static byte[] BYTE_1 = { 0x01 };
    }

    class HotKey
    {
        public Keys mod;
        public Keys key;
        public bool active;
        public Action cheat;
        public string text;
        //
        public bool fly;
        public bool toggle;

        public HotKey(Keys mod, Keys key, Action cheat, string text)
        {
            this.key = key;
            this.mod = mod;
            this.cheat = cheat;
            this.text = text;
        }
    }

    struct Vector3
    {
        public float x, y, z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    class Teleport
    {
        public string name;
        public Vector3 pos;
    }
}
