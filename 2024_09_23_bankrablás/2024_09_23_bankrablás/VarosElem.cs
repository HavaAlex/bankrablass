using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2024_09_23_bankrablás
{
    public abstract class VarosElem
    {
        public Random r;
        public int hp;
        public int aranyrogokszama;
        public int x;
        public int y;
        public bool can_go_in;
        public abstract void utik(Seriff s);
        public override string ToString()
        {
            return "valamibugos tesó";
        }
    }
}
