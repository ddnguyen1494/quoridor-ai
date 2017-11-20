using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

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
            ActionFunction bestAction = new ActionFunction();
            root = new Node(state, 0, new ActionFunction(), 1);
            Stopwatch sw = new Stopwatch();
            try
            {
                do
                {
                    current_depth++;
                    sw.Start();
                    bestAction = AlphaBeta.Search(root);
                } while (sw.ElapsedMilliseconds / 1000 < 10 && current_depth < MAX_DEPTH); // 10 seconds to think
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
            //var wallPegStatus = node.State.wallPegStatus;
            //StateDecoder.DecodeState(state, out players, out currentPlayer, out wallPegList);
            if (playerStatus[(currentPlayer + 1) % 1].GetDistanceToGoal() != 0)  //opponent is not at winning state
            {
                return 81 * playerStatus[currentPlayer].GetDistanceToGoal() / playerStatus[(currentPlayer + 1) % 1].GetDistanceToGoal();
            }
            if (currentPlayer == 0) //opponent wins (they want the max score)
                return int.MaxValue;
            return int.MinValue;    //AI wins (lowest score)
        }

        public static bool CutOff(Board board, int depth)
        {
            if (board.playerStatus[PLAYER1].CheckWin() ||
                board.playerStatus[PLAYER2].CheckWin() ||
                depth == current_depth)
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
            int nextPlayer = (p + 1) % 1;
            Board newBoard = new Board(node.State);
            #region Player 1's Successors
            if (node.Player == PLAYER1)
            {
                //Move up
                if (!boardStatus[playerStatus[p].x - 1, playerStatus[p].y].hasBotWall)
                {
                    newBoard.MoveUp(p, 1, 1);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveUp, p, 1, 1), nextPlayer));
                }
                else
                {
                    // going up jumping over 1 player directly
                    if (!boardStatus[playerStatus[p].x - 1, playerStatus[p].y].hasBotWall &&
                     !boardStatus[playerStatus[p].x - 1, playerStatus[p].y].isOpen &&
                     !boardStatus[playerStatus[p].x - 2, playerStatus[p].y].hasBotWall)
                    {
                        newBoard = new Board(node.State);
                        newBoard.MoveUp(p, 2, 1);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveUp, p, 1, 2), nextPlayer));
                    }
                    // going Up by 1 and Right by 1
                    if (((!boardStatus[playerStatus[p].x - 1, playerStatus[p].y].hasBotWall && //going up then Right
                           boardStatus[playerStatus[p].x - 2, playerStatus[p].y].hasBotWall &&
                          !boardStatus[playerStatus[p].x - 1, playerStatus[p].y].isOpen)
                          ||
                         (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasRightWall && //going Right then Up
                           boardStatus[playerStatus[p].x, playerStatus[p].y + 1].hasRightWall &&
                          !boardStatus[playerStatus[p].x, playerStatus[p].y + 1].isOpen)))
                    {
                        newBoard = new Board(node.State);
                        newBoard.MoveUp(p, 1, 1, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveUp, p, 1, 1, true), nextPlayer));
                    }
                    //going Left by 1 AND Up by 1
                    if (((!boardStatus[playerStatus[p].x, playerStatus[p].y - 1].hasRightWall && //going left then up
                            boardStatus[playerStatus[p].x, playerStatus[p].y - 2].hasRightWall &&
                            !boardStatus[playerStatus[p].x, playerStatus[p].y - 1].isOpen)
                            ||
                            (!boardStatus[playerStatus[p].x - 1, playerStatus[p].y].hasBotWall && //going up then left
                                boardStatus[playerStatus[p].x - 2, playerStatus[p].y].hasBotWall &&
                                !boardStatus[playerStatus[p].x - 1, playerStatus[p].y].isOpen)))
                    {
                        newBoard = new Board(node.State);
                        newBoard.MoveLeft(p, 1, 2, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveLeft, p, 1, 2, true), nextPlayer));
                    }
                }

                //Move Left
                if (!boardStatus[playerStatus[p].x, playerStatus[p].y - 1].hasRightWall)
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveLeft(p, 1, 2);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveLeft, p, 1, 2), nextPlayer));
                }
                // going Left jumping over 1 player directly
                else if (!boardStatus[playerStatus[p].x, playerStatus[p].y - 1].hasRightWall &&
                 !boardStatus[playerStatus[p].x, playerStatus[p].y - 1].isOpen &&
                 !boardStatus[playerStatus[p].x, playerStatus[p].y - 2].hasRightWall)
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveLeft(p, 2, 2);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveLeft, p, 2, 2), nextPlayer));
                }

                //Move Right
                if (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasRightWall)
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveRight(p, 1, 0);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveRight, p, 1, 0), nextPlayer));
                }
                else if (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasRightWall &&
                 !boardStatus[playerStatus[p].x, playerStatus[p].y + 1].isOpen &&
                 !boardStatus[playerStatus[p].x, playerStatus[p].y + 1].hasRightWall)
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveRight(p, 2, 0);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveRight, p, 2, 0), nextPlayer));
                }

                //Move Down
                // going down by 1 (checks on location and for walls)
                if (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasBotWall)
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveDown(p, 1, 3);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveDown, p, 1, 3), nextPlayer));
                }
                else
                {
                    // going down jumping over 1 player directly
                    if (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasBotWall &&
                        !boardStatus[playerStatus[p].x + 1, playerStatus[p].y].isOpen &&
                        !boardStatus[playerStatus[p].x + 1, playerStatus[p].y].hasBotWall)
                    {
                        newBoard = new Board(node.State);
                        newBoard.MoveDown(p, 2, 3);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveDown, p, 2, 3), nextPlayer));
                    }
                    // going Down by 1 AND to the Left by 1
                    if (
                        ((!boardStatus[playerStatus[p].x, playerStatus[p].y].hasBotWall && //going down then left
                            boardStatus[playerStatus[p].x + 1, playerStatus[p].y].hasBotWall &&
                            !boardStatus[playerStatus[p].x + 1, playerStatus[p].y].isOpen)
                            ||
                            (!boardStatus[playerStatus[p].x, playerStatus[p].y - 1].hasRightWall && //going left then down
                                boardStatus[playerStatus[p].x, playerStatus[p].y - 2].hasRightWall &&
                                !boardStatus[playerStatus[p].x, playerStatus[p].y - 1].isOpen)))
                    {
                        newBoard = new Board(node.State);
                        newBoard.MoveDown(p, 1, 3, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveDown, p, 1, 3, true), nextPlayer));
                    }
                    // going Right by 1 AND Down by 1
                    if (
                     ((!boardStatus[playerStatus[p].x, playerStatus[p].y].hasRightWall && //going Right then Down
                        boardStatus[playerStatus[p].x, playerStatus[p].y + 1].hasRightWall &&
                       !boardStatus[playerStatus[p].x, playerStatus[p].y + 1].isOpen)
                       ||
                      (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasBotWall && //going Down then Right
                        boardStatus[playerStatus[p].x + 1, playerStatus[p].y].hasBotWall &&
                       !boardStatus[playerStatus[p].x + 1, playerStatus[p].y].isOpen)))
                    {
                        newBoard = new Board(node.State);
                        newBoard.MoveRight(p, 1, 0, true);
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveRight, p, 1, 0, true), nextPlayer));
                    }
                }
            }
            #endregion
            #region Player 2's Successors
            //Player 2 prioritizes moving down -> left or right -> Wall -> Up (tentative Move Ordering)
            //Move Down
            // going down by 1 (checks on location and for walls)
            //Move Down
            // going down by 1 (checks on location and for walls)
            if (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasBotWall)
            {
                newBoard = new Board(node.State);
                newBoard.MoveDown(p, 1, 3);
                node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveDown, p, 1, 3), nextPlayer));
            }
            else
            {
                // going down jumping over 1 player directly
                if (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasBotWall &&
                    !boardStatus[playerStatus[p].x + 1, playerStatus[p].y].isOpen &&
                    !boardStatus[playerStatus[p].x + 1, playerStatus[p].y].hasBotWall)
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveDown(p, 2, 3);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveDown, p, 2, 3), nextPlayer));
                }
                // going Down by 1 AND to the Left by 1
                if (
                    ((!boardStatus[playerStatus[p].x, playerStatus[p].y].hasBotWall && //going down then left
                        boardStatus[playerStatus[p].x + 1, playerStatus[p].y].hasBotWall &&
                        !boardStatus[playerStatus[p].x + 1, playerStatus[p].y].isOpen)
                        ||
                        (!boardStatus[playerStatus[p].x, playerStatus[p].y - 1].hasRightWall && //going left then down
                            boardStatus[playerStatus[p].x, playerStatus[p].y - 2].hasRightWall &&
                            !boardStatus[playerStatus[p].x, playerStatus[p].y - 1].isOpen)))
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveDown(p, 1, 3, true);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveDown, p, 1, 3, true), nextPlayer));
                }
                // going Right by 1 AND Down by 1
                if (
                 ((!boardStatus[playerStatus[p].x, playerStatus[p].y].hasRightWall && //going Right then Down
                    boardStatus[playerStatus[p].x, playerStatus[p].y + 1].hasRightWall &&
                   !boardStatus[playerStatus[p].x, playerStatus[p].y + 1].isOpen)
                   ||
                  (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasBotWall && //going Down then Right
                    boardStatus[playerStatus[p].x + 1, playerStatus[p].y].hasBotWall &&
                   !boardStatus[playerStatus[p].x + 1, playerStatus[p].y].isOpen)))
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveRight(p, 1, 0, true);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveRight, p, 1, 0, true), nextPlayer));
                }
            }


            //Move Left
            if (playerStatus[p].y - 1 >= 0)
            {
                if (!boardStatus[playerStatus[p].x, playerStatus[p].y - 1].hasRightWall)
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveLeft(p, 1, 2);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveLeft, p, 1, 2), nextPlayer));
                }
                // going Left jumping over 1 player directly
                else if (!boardStatus[playerStatus[p].x, playerStatus[p].y - 1].hasRightWall &&
                 !boardStatus[playerStatus[p].x, playerStatus[p].y - 1].isOpen &&
                 !boardStatus[playerStatus[p].x, playerStatus[p].y - 2].hasRightWall)
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveLeft(p, 2, 2);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveLeft, p, 2, 2), nextPlayer));
                }
            }
            //Move Right
            if (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasRightWall)
            {
                newBoard = new Board(node.State);
                newBoard.MoveRight(p, 1, 0);
                node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveRight, p, 1, 0), nextPlayer));
            }
            else if (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasRightWall &&
             !boardStatus[playerStatus[p].x, playerStatus[p].y + 1].isOpen &&
             !boardStatus[playerStatus[p].x, playerStatus[p].y + 1].hasRightWall)
            {
                newBoard = new Board(node.State);
                newBoard.MoveRight(p, 2, 0);
                node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveRight, p, 2, 0), nextPlayer));
            }

            //Move up
            if (!boardStatus[playerStatus[p].x - 1, playerStatus[p].y].hasBotWall)
            {
                newBoard.MoveUp(p, 1, 1);
                node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveUp, p, 1, 1), nextPlayer));
            }
            else
            {
                // going up jumping over 1 player directly
                if (!boardStatus[playerStatus[p].x - 1, playerStatus[p].y].hasBotWall &&
                 !boardStatus[playerStatus[p].x - 1, playerStatus[p].y].isOpen &&
                 !boardStatus[playerStatus[p].x - 2, playerStatus[p].y].hasBotWall)
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveUp(p, 1, 2);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveUp, p, 1, 2), nextPlayer));
                }
                // going Up by 1 and Right by 1
                if (((!boardStatus[playerStatus[p].x - 1, playerStatus[p].y].hasBotWall && //going up then Right
                       boardStatus[playerStatus[p].x - 2, playerStatus[p].y].hasBotWall &&
                      !boardStatus[playerStatus[p].x - 1, playerStatus[p].y].isOpen)
                      ||
                     (!boardStatus[playerStatus[p].x, playerStatus[p].y].hasRightWall && //going Right then Up
                       boardStatus[playerStatus[p].x, playerStatus[p].y + 1].hasRightWall &&
                      !boardStatus[playerStatus[p].x, playerStatus[p].y + 1].isOpen)))
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveUp(p, 1, 1, true);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveUp, p, 1, 1, true), nextPlayer));
                }
                //going Left by 1 AND Up by 1
                if (((!boardStatus[playerStatus[p].x, playerStatus[p].y - 1].hasRightWall && //going left then up
                        boardStatus[playerStatus[p].x, playerStatus[p].y - 2].hasRightWall &&
                        !boardStatus[playerStatus[p].x, playerStatus[p].y - 1].isOpen)
                        ||
                        (!boardStatus[playerStatus[p].x - 1, playerStatus[p].y].hasBotWall && //going up then left
                            boardStatus[playerStatus[p].x - 2, playerStatus[p].y].hasBotWall &&
                            !boardStatus[playerStatus[p].x - 1, playerStatus[p].y].isOpen)))
                {
                    newBoard = new Board(node.State);
                    newBoard.MoveLeft(p, 1, 2, true);
                    node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.MoveLeft, p, 1, 2, true), nextPlayer));
                }
            }
            #endregion

            #region Wall Generation (for both players)
            for (int x = 0; x < Board.BOARD_SIZE - 1; x++)
            {
                for (int y = 0; y < Board.BOARD_SIZE - 1; y++)
                {
                    //Horizontal Wall
                    if (playerStatus[p].wallsLeft > 0 && node.State.CheckWallH(x, y))
                    {
                        newBoard = new Board(node.State);
                        newBoard.PlaceWallH(x, y);
                        newBoard.playerStatus[p].wallsLeft--;
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.PlaceWallH, x, y, -1, true), nextPlayer));
                    }

                    //Vertical Wall
                    if (playerStatus[p].wallsLeft > 0 && node.State.CheckWallV(x, y))
                    {
                        newBoard = new Board(node.State);
                        newBoard.PlaceWallV(x, y);
                        newBoard.playerStatus[p].wallsLeft--;
                        node.Children.Add(new Node(newBoard, 0, new ActionFunction(newBoard.PlaceWallV, x, y, -1 ,false), nextPlayer));
                    }

                }
            }
            #endregion
        }
    }
}
