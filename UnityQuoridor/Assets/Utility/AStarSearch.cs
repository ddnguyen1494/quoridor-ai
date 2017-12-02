using System;
using System.Collections.Generic;
using Assets.Scripts;
using Priority_Queue;

namespace Assets.Utility
{
    internal struct Position
    {
        public int X;
        public int Y;
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    internal struct QueueNode
    {
        public Position Pos { get; set; }
        public int PathCost { get; set; }
        public QueueNode(int x, int y, int cost)
        {
            Pos = new Position(x, y);
            PathCost = cost;
        }
        public QueueNode(Position pos, int cost)
        {
            Pos = pos;
            PathCost = cost;
        }

        public override bool Equals(Object obj)
        {
            QueueNode other = (QueueNode)obj;
            return Pos.X == other.Pos.X && Pos.Y == other.Pos.Y && PathCost == other.PathCost;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    static class AStarSearch
    {
        private static int GOAL;
        private static int F(QueueNode node)
        {
            return node.PathCost + Math.Abs(node.Pos.X - GOAL);
        }
        public static int FindShortestPathLength(Board board, int player)
        {
            SimplePriorityQueue<QueueNode> open = new SimplePriorityQueue<QueueNode>();

            bool[,] visited = new bool[Board.BOARD_SIZE, Board.BOARD_SIZE];
            int GOAL;
            if (player > 1) //4 players
                GOAL = board.playerStatus[player].goalY;
            else
                GOAL = board.playerStatus[player].goalX;
            QueueNode root = new QueueNode(board.playerStatus[player].x, board.playerStatus[player].y, 0);
            open.Enqueue(root, F(root));

            while (open.Count != 0)
            {
                QueueNode current = open.Dequeue();
                if ((player > 1 && current.Pos.Y == GOAL) ||
                    (player <= 1 && current.Pos.X == GOAL))
                    return current.PathCost;
                int x = current.Pos.X;
                int y = current.Pos.Y;
                visited[x, y] = true;
                foreach(Position pos in new List<Position> { new Position(x+1,y), new Position(x - 1, y), new Position (x, y+ 1), new Position(x, y -1)})
                {
                    QueueNode temp = new QueueNode(pos, current.PathCost + 1);
                    if ((pos.X >= 0 && pos.X < Board.BOARD_SIZE) &&
                        (pos.Y >= 0 && pos.Y < Board.BOARD_SIZE) &&
                        visited[pos.X, pos.Y] == false &&
                        !open.Contains(temp) &&
                        board.IsPawnMoveLegalSimplified(x, y, pos.X, pos.Y))
                        open.Enqueue(temp, F(temp));
                }
            } 
            return -1;
        }
    }
}
