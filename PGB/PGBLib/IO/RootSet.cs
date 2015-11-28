using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.IO
{
    struct RootSet
    {
        public string Root { get; }
        public string DestinationRoot { get; }

        public RootSet(string root, string destinationRoot)
        {
            Root = root;
            DestinationRoot = destinationRoot;
        }

        public RootSet(string root)
        {
            Root = root;
            DestinationRoot = "";
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(DestinationRoot))
            {
                return Root + " --> " + DestinationRoot;
            }
            else
            {
                return Root;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is RootSet)
            {
                RootSet objRoot = (RootSet)obj;
                if (objRoot.Root == Root && objRoot.DestinationRoot == DestinationRoot)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool Equals(RootSet set)
        {
            if (set.DestinationRoot == DestinationRoot && set.Root == Root)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 47;
                // Suitable nullity checks etc, of course :)
                hash = hash * 97 + Root.GetHashCode();
                hash = hash * 97 + DestinationRoot.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(RootSet a, RootSet b)
        {
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }

            return a.DestinationRoot == b.DestinationRoot && a.Root == b.Root;
        }

        public static bool operator !=(RootSet a, RootSet b)
        {
            return !(a == b);
        }
    }
}
