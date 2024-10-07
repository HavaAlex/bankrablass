using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2024_09_23_bankrablás
{
    public class Aranyrog : VarosElem
    {
        
        public Aranyrog() 
        {
            can_go_in = true;
        }
        public override void utik(Seriff s)
        {
            Console.WriteLine("kakaa");
        }
        public override string ToString()
        {
            return "A";
        }
    }
}
