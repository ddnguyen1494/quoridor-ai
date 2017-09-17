using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoridorConsole
{
    class Program
    {
        static int Main(string[] args)
        {
            Board b = new Board();
            b.addPlayer(8, 0);
            b.addPlayer(8, 16);
            b.startGame();
            //cout << b.ToString() << '\n';
            while (b.winner == -1)
            {
                b.playerTurn();
            }
            //b.getMoves(0);
            //cout << b.movesString();
            //b.movePlayer(0, 8, 2);
            //cout << "Player " << 1 + b.winner << " won.\nPress enter to finish.";
            Console.WriteLine( "Player " + (1 + b.winner) + " won.\nPress enter to finish. ");
            Console.ReadLine();
            Console.ReadLine();
            return 0;
        }
    }
}
