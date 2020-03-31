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
        public string Shape { get; set; }
        public string Solved { get; set; }
        public int NFaces { get; set; }
        public int Id { get; set; }
        public override string ToString()
        {
            if (this.Solved=="yes")
                return $"a {Name} that's solved";
            else
                return $"a {Name} that's not solved";
        }
    }
}
