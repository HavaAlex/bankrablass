using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2024_09_23_bankrablás
{
    public class Seriff : VarosElem
    {
        public int sebzes;
        public Seriff(int hp, int sebzes, int aranyrogokszama, int x, int y) 
        {
            this.hp = hp;
            this.sebzes = sebzes;
            this.aranyrogokszama = aranyrogokszama;
            this.x = x;
            this.y = y;
        }
        public override void utik(Seriff s)
        {
            Console.WriteLine("kakaa");
        }
        public override string ToString()
        {
            return Convert.ToString(aranyrogokszama);
        }
    }
}
