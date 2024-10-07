using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2024_09_23_bankrablás
{
    public class Whiskey : VarosElem
    {
        public int heal = 50;
        public Whiskey() 
        { 
            can_go_in = false;
        }
        public override void utik(Seriff s)
        {
            Console.WriteLine("kakaa");
        }


        public override string ToString()
        {
            return "W";
        }
    }
}
