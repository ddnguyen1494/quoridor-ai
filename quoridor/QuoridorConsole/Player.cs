using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoridorConsole
{
    class Player
    {
        public int x;
        public int y;
        public int goalX;
        public int goalY;
        public int wallsLeft;
        public int id;
        public char textChar;

        public Player()
        {
            x = 0;
            y = 0;
        }

        public Player(int sx, int sy)
        {
            x = sx;
            y = sy;
        }

        public bool checkWin()
        {
            return (x == goalX || y == goalY);
        }

    }
}
