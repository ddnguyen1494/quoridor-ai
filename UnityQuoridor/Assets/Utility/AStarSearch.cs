using System;
using System.Collections.Generic;
using Assets.Scripts;
using Priority_Queue;

namespace Assets.Utility
{
	public struct Position
    {
        public int X;
        public int Y;
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    internal class QueueNode
    {
        public Position Pos { get; set; }
        public int PathCost { get; set; }
		public QueueNode Parent { get; set; }

        public QueueNode(int x, int y, int cost)
        {
            Pos = new Position(x, y);
            PathCost = cost;
			Parent = null;
        }

		public QueueNode(Position pos, int cost) : this(pos.X, pos.Y, cost)
        {
        }

		public QueueNode(Position pos, int cost, QueueNode parent) : this (pos, cost)
		{
			Parent = parent;
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
		public static int FindShortestPathLength(Board board, int player, bool simplified = true)
        {
//            SimplePriorityQueue<QueueNode> open = new SimplePriorityQueue<QueueNode>();
//
//            bool[,] visited = new bool[Board.BOARD_SIZE, Board.BOARD_SIZE];
//            GOAL = board.playerStatus[player].goalX;
//            QueueNode root = new QueueNode(board.playerStatus[player].x, board.playerStatus[player].y, 0);
//            open.Enqueue(root, F(root));
//
//            while (open.Count != 0)
//            {
//                QueueNode current = open.Dequeue();
//                if (current.Pos.X == GOAL)
//                    return current.PathCost;
//                int x = current.Pos.X;
//                int y = current.Pos.Y;
//                visited[x, y] = true;
//                foreach(Position pos in new List<Position> { new Position(x+1,y), new Position(x - 1, y), new Position (x, y+ 1), new Position(x, y -1)})
//                {
//                    QueueNode temp = new QueueNode(pos, current.PathCost + 1);
//                    if ((pos.X >= 0 && pos.X < Board.BOARD_SIZE) &&
//                        (pos.Y >= 0 && pos.Y < Board.BOARD_SIZE) &&
//                        visited[pos.X, pos.Y] == false &&
//                        !open.Contains(temp) &&
//                        board.IsPawnMoveLegalSimplified(x, y, pos.X, pos.Y))
//                        open.Enqueue(temp, F(temp));
//                }
//            } 
//            return -1;
			List<Position> shortestPath;
			if (simplified)
				shortestPath = FindShortestPathSimplified (board, player);
			else
				shortestPath = FindShortestPath (board, player);
			if (shortestPath == null)
				return -1;
			return shortestPath.Count - 1;
        }

		public static List<Position> FindShortestPathSimplified(Board board, int player)
		{
			SimplePriorityQueue<QueueNode> open = new SimplePriorityQueue<QueueNode>();

			bool[,] visited = new bool[Board.BOARD_SIZE, Board.BOARD_SIZE];
			GOAL = board.playerStatus[player].goalX;
			QueueNode root = new QueueNode(board.playerStatus[player].x, board.playerStatus[player].y, 0);
			open.Enqueue(root, F(root));

			while (open.Count != 0)
			{
				QueueNode current = open.Dequeue();
				if (current.Pos.X == GOAL) {
					List<Position> path = new List<Position> ();
					while (current != null) {
						path.Add (current.Pos);
						current = current.Parent;
					}
                    path.Reverse();
					return path;
				}
				int x = current.Pos.X;
				int y = current.Pos.Y;
				visited[x, y] = true;
				foreach(Position pos in new List<Position> { new Position(x+1,y), new Position(x - 1, y), new Position (x, y+ 1), new Position(x, y -1)})
				{
					QueueNode temp = new QueueNode(pos, current.PathCost + 1, current);
					if ((pos.X >= 0 && pos.X < Board.BOARD_SIZE) &&
						(pos.Y >= 0 && pos.Y < Board.BOARD_SIZE) &&
						visited[pos.X, pos.Y] == false &&
						!open.Contains(temp) &&
						board.IsPawnMoveLegalSimplified(x, y, pos.X, pos.Y))
						open.Enqueue(temp, F(temp));
				}
			} 
			return null;
		}

		public static List<Position> FindShortestPath(Board board, int player)
		{
			SimplePriorityQueue<QueueNode> open = new SimplePriorityQueue<QueueNode>();

			bool[,] visited = new bool[Board.BOARD_SIZE, Board.BOARD_SIZE];
			GOAL = board.playerStatus[player].goalX;
			QueueNode root = new QueueNode(board.playerStatus[player].x, board.playerStatus[player].y, 0);
			open.Enqueue(root, F(root));

			while (open.Count != 0)
			{
				QueueNode current = open.Dequeue();
				if (current.Pos.X == GOAL) {
					List<Position> path = new List<Position> ();
					while (current != null) {
						path.Add (current.Pos);
						current = current.Parent;
					}
					return path;
				}
				int x = current.Pos.X;
				int y = current.Pos.Y;
				visited[x, y] = true;
				foreach(Position pos in new List<Position> { new Position(x+1,y), new Position(x - 1, y), new Position (x, y+ 1), new Position(x, y -1),
					new Position(x+2,y), new Position(x - 2, y), new Position (x, y+ 2), new Position(x, y - 2)})
				{
					QueueNode temp = new QueueNode(pos, current.PathCost + 1, current);
					if ((pos.X >= 0 && pos.X < Board.BOARD_SIZE) &&
						(pos.Y >= 0 && pos.Y < Board.BOARD_SIZE) &&
						visited[pos.X, pos.Y] == false &&
						!open.Contains(temp) &&
						board.IsPawnMoveLegal(x, y, pos.X, pos.Y))
						open.Enqueue(temp, F(temp));
				}
			} 
			return null;
		}
    }
}
