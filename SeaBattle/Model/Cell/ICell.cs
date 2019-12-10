using System;

namespace Battleships.Model
{
    public interface ICell
    {
        Boolean State { get; set; }

        Boolean Explored { get; set; }

        int I { get; set; }
        int J { get; set; }
    }
}
