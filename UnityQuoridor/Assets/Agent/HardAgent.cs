using System;
using System.Collections.Generic;
using Assets.Utility;

namespace Assets.Scripts
{
	public class HardAgent : MediumAgent
	{
        public HardAgent() : base()
        {
        }

		public HardAgent(int depth) : base(depth)
		{
		}

        public override int Evaluate(Node node)
        {
            int score;
            int opponent_dist;
            int my_dist;
            if (node.AgentSP == int.MinValue && node.OpponentSP == int.MinValue)
            {
                opponent_dist = AStarSearch.FindShortestPathLength(board, (player_num + 1) % 2);
                my_dist = AStarSearch.FindShortestPathLength(board, player_num);
            }
            else
            {
                opponent_dist = node.OpponentSP;
                my_dist = node.AgentSP;
            }
            PlayerInfo[] playerStatus = board.playerStatus;

            score = (opponent_dist - my_dist) + 1 * (playerStatus[player_num].wallsLeft - playerStatus[(player_num + 1) % 2].wallsLeft);
            //score = (opponent_dist - my_dist) + 1 * (playerStatus[(player_num + 1) % 2].wallsLeft - playerStatus[player_num].wallsLeft);
            if (playerStatus[player_num].wallsLeft == 0 && my_dist > opponent_dist)//you waste all your walls and opponent still win.
                score -= 10;
            else if (playerStatus[(player_num + 1) % 2].wallsLeft == 0 && opponent_dist > my_dist) //good. You outplay him
                score += 10;
            if (opponent_dist == 0)
                score = -50;
            if (my_dist == 0)
                score = 50;
            return score;
        }
        public override void GenerateSuccessors (Node node)
		{
			int me = node.Player;
			int opponent = (node.Player + 1) % 2;
			List<Position> my_shortest_path = AStarSearch.FindShortestPathSimplified (board, me);
			List<Position> opponent_shortest_path = AStarSearch.FindShortestPathSimplified (board, opponent);

            if (me == player_num)
            {
                node.AgentSP = my_shortest_path.Count;
                node.OpponentSP = opponent_shortest_path.Count;
                ActionFunction undo = new ActionFunction(Board.UndoMovePawn, me, board.playerStatus[me].x, board.playerStatus[me].y);
                //Always use the shortest path
                if (board.IsPawnMoveLegal(my_shortest_path[0].X, my_shortest_path[0].Y, my_shortest_path[1].X, my_shortest_path[1].Y))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, me, my_shortest_path[1].X, my_shortest_path[1].Y), undo, opponent));
                }
                //There is a player in front of me
                //1. Check if I can jump directly to next position in my shortestpath
                //2. If not possible, generate all moves
                else if (board.IsPawnMoveLegal(my_shortest_path[0].X, my_shortest_path[0].Y, my_shortest_path[2].X, my_shortest_path[2].Y))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, me, my_shortest_path[2].X, my_shortest_path[2].Y), undo, opponent));
                }
                else
                {
                    GeneratePossibleMoves(me, ref node);
                }
            }
            else
            {
                node.OpponentSP = my_shortest_path.Count - 1;
                node.AgentSP = opponent_shortest_path.Count - 1;
                GeneratePossibleMoves(me, ref node);
            }

            //Lets try to block the other player along their shortest path
            ActionFunction undoWall;
            if (board.playerStatus[me].wallsLeft == 0)
                return;
            for (int i = 0; i < opponent_shortest_path.Count - 2; i++)
            { // - 2 because we can't place walls after they reach goal
                Position current = opponent_shortest_path[i];
                Position next = opponent_shortest_path[i + 1];
                int xDiff = next.X - current.X;
                int yDiff = next.Y - current.Y;
                if (xDiff == -1)
                { //Move up
                    if (board.CheckWallH(current.X - 1, current.Y - 1))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceHorizontalWall, me, current.X - 1, current.Y - 1);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceHorizontalWall, me, current.X - 1, current.Y - 1), undoWall, opponent));
                    }
                    if (board.CheckWallH(current.X - 1, current.Y))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceHorizontalWall, me, current.X - 1, current.Y);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceHorizontalWall, me, current.X - 1, current.Y), undoWall, opponent));
                    }
                    if (board.CheckWallV(current.X - 1, current.Y - 1))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceVerticalWall, me, current.X - 1, current.Y - 1);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceVerticalWall, me, current.X - 1, current.Y - 1), undoWall, opponent));
                    }
                    if (board.CheckWallV(current.X - 1, current.Y))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceVerticalWall, me, current.X - 1, current.Y);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceVerticalWall, me, current.X - 1, current.Y), undoWall, opponent));
                    }
                }

                else if (xDiff == 1)//Move down
                {
                    if (board.CheckWallH(current.X, current.Y))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceHorizontalWall, me, current.X, current.Y);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceHorizontalWall, me, current.X, current.Y), undoWall, opponent));
                    }
                    if (board.CheckWallH(current.X, current.Y - 1))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceHorizontalWall, me, current.X, current.Y - 1);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceHorizontalWall, me, current.X, current.Y - 1), undoWall, opponent));
                    }
                    if (board.CheckWallV(current.X, current.Y))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceVerticalWall, me, current.X, current.Y);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceVerticalWall, me, current.X, current.Y), undoWall, opponent));
                    }
                    if (board.CheckWallV(current.X, current.Y - 1))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceVerticalWall, me, current.X, current.Y - 1);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceVerticalWall, me, current.X, current.Y - 1), undoWall, opponent));
                    }
                }
                else if (yDiff == -1)//Move left
                {
                    if (board.CheckWallV(current.X - 1, current.Y - 1))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceVerticalWall, me, current.X - 1, current.Y - 1);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceVerticalWall, me, current.X - 1, current.Y - 1), undoWall, opponent));
                    }
                    if (board.CheckWallV(current.X, current.Y - 1))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceVerticalWall, me, current.X, current.Y - 1);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceVerticalWall, me, current.X, current.Y - 1), undoWall, opponent));
                    }
                    if (board.CheckWallH(current.X - 1, current.Y - 1))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceHorizontalWall, me, current.X - 1, current.Y - 1);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceHorizontalWall, me, current.X - 1, current.Y - 1), undoWall, opponent));
                    }
                    if (board.CheckWallH(current.X, current.Y - 1))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceHorizontalWall, me, current.X, current.Y - 1);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceHorizontalWall, me, current.X, current.Y - 1), undoWall, opponent));
                    }
                }
                else if (yDiff == 1)//Move right
                {
                    if (board.CheckWallV(current.X - 1, current.Y))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceVerticalWall, me, current.X - 1, current.Y);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceVerticalWall, me, current.X - 1, current.Y), undoWall, opponent));
                    }
                    if (board.CheckWallV(current.X, current.Y))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceVerticalWall, me, current.X, current.Y);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceVerticalWall, me, current.X, current.Y), undoWall, opponent));
                    }
                    if (board.CheckWallH(current.X - 1, current.Y))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceHorizontalWall, me, current.X - 1, current.Y);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceHorizontalWall, me, current.X - 1, current.Y), undoWall, opponent));
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

