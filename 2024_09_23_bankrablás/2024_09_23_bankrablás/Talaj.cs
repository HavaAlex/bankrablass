using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2024_09_23_bankrablás
{
    public class Talaj : VarosElem
    {
        public Talaj() 
        {
            can_go_in = true;
        }
        public override void utik(Seriff s)
        {
            Console.WriteLine("kakaa");
        }
        public override string ToString()
        {
            return "o";
        }
    }
}
