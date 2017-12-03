using System;
using System.Collections.Generic;
using Assets.Utility;

namespace Assets.Scripts
{
	public class HardAgent : MediumAgent
	{
		public HardAgent(int depth) : base(depth)
		{
		}

		public override void GenerateSuccessors (Node node)
		{
			int me = node.Player;
			int opponent = (node.Player + 1) % 2;
			List<Position> my_shortest_path = AStarSearch.FindShortestPathSimplified (board, me);
			List<Position> opponent_shortest_path = AStarSearch.FindShortestPathSimplified (board, opponent);
			ActionFunction undo = new ActionFunction(Board.UndoMovePawn, me, board.playerStatus[me].x, board.playerStatus[me].y);
			//Always use the shortest path
			if (board.IsPawnMoveLegal (my_shortest_path [0].X, my_shortest_path [0].Y, my_shortest_path [1].X, my_shortest_path [1].Y)) {
				node.Children.Add (new Node (new ActionFunction (Board.MovePawn, me, my_shortest_path [1].X, my_shortest_path [1].Y), undo, opponent));
			}
			//There is a player in front of me
			//1. Check if I can jump directly to next position in my shortestpath
			//2. If not possible, I think it's ok to just jump over opponent
			else if (board.IsPawnMoveLegal (my_shortest_path [0].X, my_shortest_path [0].Y, my_shortest_path [2].X, my_shortest_path [2].Y)) 
			{ 
				node.Children.Add (new Node (new ActionFunction (Board.MovePawn, me, my_shortest_path [1].X, my_shortest_path [1].Y), undo, opponent));
			} 
			else 
			{
				//Can I jump over him?
				int xDiff = my_shortest_path [1].X - my_shortest_path [0].X;
				int yDiff = my_shortest_path [1].Y - my_shortest_path [1].Y;
				if (board.IsPawnMoveLegal (my_shortest_path [0].X, my_shortest_path [0].Y, my_shortest_path [0].X + xDiff * 2, my_shortest_path [0].Y + yDiff * 2))
					node.Children.Add (new Node (new ActionFunction (Board.MovePawn, me, my_shortest_path [0].X + xDiff * 2, my_shortest_path [0].Y + yDiff * 2), undo, opponent));
				else
					UnityEngine.Debug.LogError ("Wow. It's actually possible to get here");
			}

			//Lets try to block the other player along their shortest path
			ActionFunction undoWall;
			for (int i = 0; i < opponent_shortest_path.Count - 2; i++) { // - 2 because we can't place walls after they reach goal
				Position current = opponent_shortest_path[i];
				Position next = opponent_shortest_path [i + 1];
				int xDiff = next.X - current.X;
				int yDiff = next.Y - current.Y;
				if (xDiff == 1) { //Move up
					if (board.CheckWallH(current.X, current.Y - 1))
					{
						undoWall = new ActionFunction(Board.UndoPlaceHorizontalWall, me, current.X, current.Y - 1);
						node.Children.Add(new Node(new ActionFunction(Board.PlaceHorizontalWall, me, current.X, current.Y - 1), undoWall, opponent));
					}
					if (board.CheckWallH(current.X, current.Y))
					{
						undoWall = new ActionFunction(Board.UndoPlaceHorizontalWall, me, current.X, current.Y);
						node.Children.Add(new Node(new ActionFunction(Board.PlaceHorizontalWall, me, current.X, current.Y), undoWall, opponent));
					}
				}
					
			}
		}
	}
}

