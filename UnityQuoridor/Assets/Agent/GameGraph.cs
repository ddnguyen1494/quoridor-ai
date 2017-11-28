using System.Collections.Generic;
using System.Collections;

namespace Assets.Scripts
{
    public struct ActionFunction
    {
        public Board.Action function;
        public int param1;
        public int param2;
        public int param3;
        public bool param4;
        public ActionFunction(Board.Action fnc, int param1, int param2, int param3 = -1, bool param4 = false)
        {
            this.function = fnc;
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
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
    }

}
