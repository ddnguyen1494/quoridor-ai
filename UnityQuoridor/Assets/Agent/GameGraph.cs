using System.Collections.Generic;
using System.Collections;

namespace Assets.Scripts
{
    public struct ActionFunction
    {
        public Board.Action function;
        public int player;
        public int x;
        public int y;

        public ActionFunction(Board.Action fnc, int player, int x, int y)
        {
            this.function = fnc;
            this.player = player;
            this.x = x;
            this.y = y;
        }

		public override string ToString ()
		{
			return string.Format("Move: {0}, X: {1}, Y: {2}", 
				function.Method.Name, x, y);
		}
    }
    internal class Node
    {
        public int Value { get; set; }
        public ActionFunction Move { get; set; }
        public ActionFunction Undo { get; set; }
        public int Player { get; set; }
        public List<Node> Children { get; set; }

        public Node(ActionFunction move, ActionFunction undoMove, int player)
        {
            Move = move;
            Undo = undoMove;
            Player = player;
            Children = new List<Node>();
        }

        public Node(int player)
        {
            Player = player;
            Children = new List<Node>();
        }

		public override string ToString ()
		{
			return Move.ToString ();
		}
    }

}
