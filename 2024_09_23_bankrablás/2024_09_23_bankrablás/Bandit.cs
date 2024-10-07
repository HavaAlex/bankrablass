using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2024_09_23_bankrablás
{
    public class Bandit : VarosElem
    {
        Random r = new Random();

        public Bandit(int hp, int aranyrogokszama, int x, int y)
        {
            this.hp = hp;  
            this.aranyrogokszama = aranyrogokszama;
            this.x = x;
            this.y = y;
            can_go_in = false;
        }
        public override void utik(Seriff s)
        {
            int sebzes = r.Next(4, 16);
            //Console.WriteLine("box");
            s.hp -= sebzes;
        }
        public override string ToString() 
        {
            return Convert.ToString(aranyrogokszama);
        }
    }
}
