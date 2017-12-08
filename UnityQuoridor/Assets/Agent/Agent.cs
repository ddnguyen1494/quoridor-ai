using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using UnityEngine; //just for Debug. Delete once function works properly!!!
using Assets.Utility;

namespace Assets.Scripts
{
    class Agent
    {
        const int PLAYER1 = 0;
        const int PLAYER2 = 1;
        Node root;
		private static int MAX_DEPTH = 1;
        static int current_depth = 0;
        static Board board;
		static int player_num;

        //return Agent's decision
		public ActionFunction NextMove(Board state, int p)
        {
            ActionFunction bestAction = new ActionFunction();
			player_num = p;
            root = new Node(p);
            board = state; 
            Stopwatch sw = new Stopwatch();
            try
            {
				sw.Start();
                do
                {
                    current_depth++;
                    bestAction = AlphaBeta.Search(board, root);
				} while (sw.ElapsedMilliseconds / 1000 < 5 && current_depth < MAX_DEPTH); 
				sw.Stop();
                sw.Reset();
                current_depth = 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return bestAction;
        }

        public static int Evaluate(Node node)
        {
            PlayerInfo[] playerStatus = board.playerStatus;
            int me = player_num;
			int opponent = (player_num + 1) % 2;
            int opponent_dist = AStarSearch.FindShortestPathLength(board, opponent);
            int my_dist = AStarSearch.FindShortestPathLength(board, me);
            if (opponent_dist == 0)
                return -50;
            if (my_dist == 0)
                return +50;
            int score = opponent_dist - my_dist + (playerStatus[opponent].wallsLeft - playerStatus[me].wallsLeft);

            return score;
        }

        public static bool CutOff(int depth)
        {
            if (board.playerStatus[PLAYER1].CheckWin() ||
                board.playerStatus[PLAYER2].CheckWin() ||
                depth == MAX_DEPTH ||
                depth == current_depth)
                return true;
            return false;
        }
        
        public static void GenerateSuccessors(Node node)
        {
            //Player 1 prioritizes moving up -> left or right -> Wall -> Down (tentative Move Ordering)
            PlayerInfo[] playerStatus = board.playerStatus;
            int p = node.Player;
            int nextPlayer = (p + 1) % 2;
            ActionFunction undo = new ActionFunction(Board.UndoMovePawn, p, playerStatus[p].x, playerStatus[p].y);
            #region Player 1's Successors
            if (node.Player == PLAYER1)  //first check in all if statemtents is the players position in relation to moving
            {
                //Move up
                if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x - 1, playerStatus[p].y), undo, nextPlayer));
                }
                else
                {
                    // going up jumping over 1 player directly
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 2, playerStatus[p].y))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x - 2, playerStatus[p].y), undo, nextPlayer));
                    }
                    // going Up by 1 and Right by 1
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y + 1))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x - 1, playerStatus[p].y + 1), undo, nextPlayer));
                    }
                    //going Left by 1 AND Up by 1
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y - 1))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x - 1, playerStatus[p].y - 1), undo, nextPlayer));
                    }
                }

                //Move Left
                if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y - 1))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x, playerStatus[p].y - 1), undo, nextPlayer));
                }
                // going Left jumping over 1 player directly
                else if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y - 2))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x, playerStatus[p].y - 2), undo, nextPlayer));
                }

                //Move Right
                if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y + 1))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x, playerStatus[p].y + 1), undo, nextPlayer));
                }
                //going Right by jumping over 1 player directly
                else if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y + 2))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x, playerStatus[p].y + 2), undo, nextPlayer));
                }

                //Move Down
                // going down by 1 (checks on location and for walls)
                if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 1, playerStatus[p].y))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x + 1, playerStatus[p].y), undo, nextPlayer));
                }
                else
                {
                    // going down jumping over 1 player directly
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 2, playerStatus[p].y))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x + 2, playerStatus[p].y), undo, nextPlayer));
                    }
                    // going Down by 1 AND to the Left by 1
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 1, playerStatus[p].y - 1))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x + 1, playerStatus[p].y - 1), undo, nextPlayer));
                    }
                    // going Right by 1 AND Down by 1
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y + 1))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x - 1, playerStatus[p].y + 1), undo, nextPlayer));
                    }
                }
            }
            #endregion
            #region Player 2's Successors
            else
            {
                //Player 2 prioritizes moving down -> left or right -> Wall -> Up (tentative Move Ordering)
                //Move Down
                // going down by 1 (checks on location and for walls)
                if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 1, playerStatus[p].y))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x + 1, playerStatus[p].y), undo, nextPlayer));
                }
                else
                {
                    // going down jumping over 1 player directly
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 2, playerStatus[p].y))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x + 2, playerStatus[p].y), undo, nextPlayer));
                    }
                    // going Down by 1 AND to the Left by 1
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 1, playerStatus[p].y - 1))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x + 1, playerStatus[p].y - 1), undo, nextPlayer));
                    }
                    // going Right by 1 AND Down by 1
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 1, playerStatus[p].y + 1))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x + 1, playerStatus[p].y + 1), undo, nextPlayer));
                    }
                }


                //Move Left
                //   if (playerStatus[p].y - 1 >= 0)
                //  {
                if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y - 1))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x, playerStatus[p].y - 1), undo, nextPlayer));
                }
                // going Left jumping over 1 player directly
                else if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y - 2))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x, playerStatus[p].y - 2), undo, nextPlayer));
                }
                //}
                //Move Right
                if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y + 1))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x, playerStatus[p].y + 1), undo, nextPlayer));
                }
                //going Right jumping over 1 player directly
                else if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y + 2))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x, playerStatus[p].y + 2), undo, nextPlayer));
                }

                //Move up
                if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y))
                {
                    node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x - 1, playerStatus[p].y), undo, nextPlayer));
                }
                else
                {
                    // going up jumping over 1 player directly
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 2, playerStatus[p].y))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x - 2, playerStatus[p].y), undo, nextPlayer));
                    }
                    // going Up by 1 and Right by 1
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y + 1))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x - 1, playerStatus[p].y + 1), undo, nextPlayer));
                    }
                    //going Left by 1 AND Up by 1
                    if (board.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y - 1))
                    {
                        node.Children.Add(new Node(new ActionFunction(Board.MovePawn, p, playerStatus[p].x - 1, playerStatus[p].y - 1), undo, nextPlayer));
                    }
                }
            }
            #endregion

            #region Wall Generation (for both players) taking too long
            if (playerStatus[p].wallsLeft == 0) // player has no wall left
                return;
            ActionFunction undoWall;
            for (int x = 0; x < Board.BOARD_SIZE - 1; x++)
            {
                for (int y = 0; y < Board.BOARD_SIZE - 1; y++)
                {
                    //Horizontal Wall
                    if (board.CheckWallH(x, y))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceHorizontalWall, p, x, y);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceHorizontalWall, p, x, y), undoWall, nextPlayer));
                    }
                    //Vertical Wall
                    if (board.CheckWallV(x, y))
                    {
                        undoWall = new ActionFunction(Board.UndoPlaceVerticalWall, p, x, y);
                        node.Children.Add(new Node(new ActionFunction(Board.PlaceVerticalWall, p, x, y), undoWall, nextPlayer));
                    }

                }
            }
            #endregion
        }
    }
}
