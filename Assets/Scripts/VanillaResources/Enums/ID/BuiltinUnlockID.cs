using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class BuiltinUnlockID
    {
        public static readonly NamespaceID trigger = Get("trigger");
        public static readonly NamespaceID starshard = Get("starshard");
        private static NamespaceID Get(string path)
        {
            return new NamespaceID(Builtin.spaceName, path);
        }
    }
}
