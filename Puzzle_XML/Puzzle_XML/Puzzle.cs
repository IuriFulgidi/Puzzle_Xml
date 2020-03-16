using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle_XML
{
    public class Puzzle
    {
        public string Name { get; set; }
        public string NStates { get; set; }
        public bool Solved { get; set; }
        public int NFaces { get; set; }
        public override string ToString()
        {
            if (this.Solved)
                return $"a {Name} that's solved";
            else
                return $"a {Name} that's not solved";
        }
    }
}
