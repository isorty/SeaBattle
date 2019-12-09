using System;

namespace Battleships.Core
{
    public interface ICell
    {
        Boolean State { get; set; }

        Boolean Explored { get; set; }

        int I { get; set; }
        int J { get; set; }
    }
}
