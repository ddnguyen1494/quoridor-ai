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
        private int MAX_DEPTH = 12;

        static int current_depth = 0;

        //return Agent's decision
        public ActionFunction NextMove(Board state)
        {
			UnityEngine.Debug.Log("Start of ActionFunction in Agent Class!"); //delete once fnc is complete
            ActionFunction bestAction = new ActionFunction();
            root = new Node(state, 0, new ActionFunction(), 1);
            Stopwatch sw = new Stopwatch();
            try
            {
				sw.Start();
                do
                {
                    current_depth++;
                    
                    bestAction = AlphaBeta.Search(root);
				} while (sw.ElapsedMilliseconds / 1000 < 10 && current_depth < MAX_DEPTH); // 10 seconds to think
				sw.Stop();
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
            PlayerInfo[] playerStatus = node.State.playerStatus;
            int currentPlayer = node.Player;
            int opponent_dist = AStarSearch.FindShortestPathLength(node, PLAYER1);
            int my_dist = AStarSearch.FindShortestPathLength(node, PLAYER2);
            if (opponent_dist == 0)
                return -1000;
            if (my_dist == 0)
                return +1000;
            int score = opponent_dist - my_dist + (playerStatus[PLAYER2].wallsLeft - playerStatus[PLAYER1].wallsLeft);
            return score;
            //if (playerStatus[(currentPlayer + 1) % 1].GetDistanceToGoal() != 0)  //opponent is not at winning state
            //{
            //    return 81 * playerStatus[currentPlayer].GetDistanceToGoal() / playerStatus[(currentPlayer + 1) % 1].GetDistanceToGoal();
            //}
            //if (currentPlayer == 0) //opponent wins (they want the max score)
            //    return int.MaxValue;
            //return int.MinValue;    //AI wins (lowest score)
        }

        public static bool CutOff(Board board, int depth)
        {
            if (board.playerStatus[PLAYER1].CheckWin() ||
                board.playerStatus[PLAYER2].CheckWin() ||
                depth == 1)
                return true;
            return false;
        }
        
        public static void GenerateSuccessors(Node node)
        {
            //Player 1 prioritizes moving up -> left or right -> Wall -> Down (tentative Move Ordering)
            PlayerInfo[] playerStatus = node.State.playerStatus;
            gameSquareInfo[,] boardStatus = node.State.boardStatus;
            WallPeg[,] wallPegStatus = node.State.wallPegStatus;
            int p = node.Player;
            int nextPlayer = (p + 1) % 2;
            Board newBoard = new Board(node.State);
            #region Player 1's Successors
            if (node.Player == PLAYER1)  //first check in all if statemtents is the players position in relation to moving
            {
                //Move up
                if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y))
                {
                    Board.MoveUp(newBoard, p, 1, 1);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveUp, p, 1, 1), nextPlayer));
                }
                else
                {
                    // going up jumping over 1 player directly
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 2, playerStatus[p].y))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveUp(newBoard, p, 2, 1);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveUp, p, 1, 2), nextPlayer));
                    }
                    // going Up by 1 and Right by 1
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y + 1))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveUp(newBoard, p, 1, 1, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveUp, p, 1, 1, true), nextPlayer));
                    }
                    //going Left by 1 AND Up by 1
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y - 1))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveLeft(newBoard, p, 1, 2, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveLeft, p, 1, 2, true), nextPlayer));
                    }
                }

                //Move Left
                if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y - 1))
                {
                    newBoard = new Board(node.State);
                    Board.MoveLeft(newBoard, p, 1, 2);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveLeft, p, 1, 2), nextPlayer));
                }
                // going Left jumping over 1 player directly
                else if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y - 2))
                {
                    newBoard = new Board(node.State);
                    Board.MoveLeft(newBoard, p, 2, 2);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveLeft, p, 2, 2), nextPlayer));
                }

                //Move Right
                if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y + 1))
                {
                    newBoard = new Board(node.State);
                    Board.MoveRight(newBoard, p, 1, 0);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveRight, p, 1, 0), nextPlayer));
                }
                //going Right by jumping over 1 player directly
                else if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y + 2))
                {
                    newBoard = new Board(node.State);
                    Board.MoveRight(newBoard, p, 2, 0);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveRight, p, 2, 0), nextPlayer));
                }

                //Move Down
                // going down by 1 (checks on location and for walls)
                if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 1, playerStatus[p].y))
                {
                    newBoard = new Board(node.State);
                    Board.MoveDown(newBoard, p, 1, 3);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveDown, p, 1, 3), nextPlayer));
                }
                else
                {
                    // going down jumping over 1 player directly
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 2, playerStatus[p].y))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveDown(newBoard, p, 2, 3);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveDown, p, 2, 3), nextPlayer));
                    }
                    // going Down by 1 AND to the Left by 1
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 1, playerStatus[p].y - 1))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveDown(newBoard, p, 1, 3, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveDown, p, 1, 3, true), nextPlayer));
                    }
                    // going Right by 1 AND Down by 1
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y + 1))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveRight(newBoard, p, 1, 0, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveRight, p, 1, 0, true), nextPlayer));
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
                if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 1, playerStatus[p].y))
                {
                    newBoard = new Board(node.State);
                    Board.MoveDown(newBoard, p, 1, 3);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveDown, p, 1, 3), nextPlayer));
                }
                else
                {
                    // going down jumping over 1 player directly
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 2, playerStatus[p].y))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveDown(newBoard, p, 2, 3);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveDown, p, 2, 3), nextPlayer));
                    }
                    // going Down by 1 AND to the Left by 1
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 1, playerStatus[p].y -1))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveDown(newBoard, p, 1, 3, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveDown, p, 1, 3, true), nextPlayer));
                    }
                    // going Right by 1 AND Down by 1
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x + 1, playerStatus[p].y + 1))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveRight(newBoard, p, 1, 0, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveRight, p, 1, 0, true), nextPlayer));
                    }
                }


                //Move Left
                //   if (playerStatus[p].y - 1 >= 0)
                //  {
                if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y -1))
                {
                    newBoard = new Board(node.State);
                    Board.MoveLeft(newBoard, p, 1, 2);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveLeft, p, 1, 2), nextPlayer));
                }
                // going Left jumping over 1 player directly
                else if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y - 2))
                {
                    newBoard = new Board(node.State);
                    Board.MoveLeft(newBoard, p, 2, 2);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveLeft, p, 2, 2), nextPlayer));
                }
                //}
                //Move Right
                if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y + 1))
                {
                    newBoard = new Board(node.State);
                    Board.MoveRight(newBoard, p, 1, 0);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveRight, p, 1, 0), nextPlayer));
                }
                //going Right jumping over 1 player directly
                else if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x, playerStatus[p].y + 2))
                {
                    newBoard = new Board(node.State);
                    Board.MoveRight(newBoard, p, 2, 0);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveRight, p, 2, 0), nextPlayer));
                }

                //Move up
                if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y))
                {
                    Board.MoveUp(newBoard, p, 1, 1);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveUp, p, 1, 1), nextPlayer));
                }
                else
                {
                    // going up jumping over 1 player directly
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 2, playerStatus[p].y))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveUp(newBoard, p, 1, 2);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveUp, p, 1, 2), nextPlayer));
                    }
                    // going Up by 1 and Right by 1
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y + 1))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveUp(newBoard, p, 1, 1, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveUp, p, 1, 1, true), nextPlayer));
                    }
                    //going Left by 1 AND Up by 1
                    if (node.State.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, playerStatus[p].x - 1, playerStatus[p].y -1))
                    {
                        newBoard = new Board(node.State);
                        Board.MoveLeft(newBoard, p, 1, 2, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(Board.MoveLeft, p, 1, 2, true), nextPlayer));
                    }
                }
            }
            #endregion

            #region Wall Generation (for both players) taking too long
            //for (int x = 0; x < Board.BOARD_SIZE - 1; x++)
            //{
            //    for (int y = 0; y < Board.BOARD_SIZE - 1; y++)
            //    {
            //        //Horizontal Wall
            //        if (playerStatus[p].wallsLeft > 0 && node.State.CheckWallH(x, y))
            //        {
            //            newBoard = new Board(node.State);
            //            newBoard.PlaceWallH(x, y);
            //            newBoard.playerStatus[p].wallsLeft--;
            //            node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.PlaceWallH, x, y, -1, true), nextPlayer));
            //        }

            //        //Vertical Wall
            //        if (playerStatus[p].wallsLeft > 0 && node.State.CheckWallV(x, y))
            //        {
            //            newBoard = new Board(node.State);
            //            newBoard.PlaceWallV(x, y);
            //            newBoard.playerStatus[p].wallsLeft--;
            //            node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.PlaceWallV, x, y, -1 ,false), nextPlayer));
            //        }

            //    }
            //}
            #endregion
        }
    }
}
