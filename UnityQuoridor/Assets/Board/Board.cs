using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Board
    {
        public delegate bool Action(Board b, int player_wallXPos, int movement_wallYPos, int direction_dummy, bool jump_isHorizontal);

        public bool ExecuteFunction(Scripts.ActionFunction actionFunction)
        {
            Action action = actionFunction.function;
            return action.Invoke(this, actionFunction.param1, actionFunction.param2, actionFunction.param3, actionFunction.param4);
        }

        public const int MAX_PLAYERS = 4;
        public const int MIN_PLAYERS = 2;
        public const int BOARD_SIZE = 9;
        public const int TOTAL_WALLS = 20;

        int _numPlayers;

        public PlayerInfo[] playerStatus; // initialized in Start
        public gameSquareInfo[,] boardStatus;
        public WallPeg[,] wallPegStatus;
        public int[,] accessible;

        private Board()
        {
            boardStatus = new gameSquareInfo[BOARD_SIZE, BOARD_SIZE];
            wallPegStatus = new WallPeg[BOARD_SIZE - 1, BOARD_SIZE - 1];
            accessible = new int[BOARD_SIZE, BOARD_SIZE];
        }

        public Board(int numPlayers) : this()
        {
            _numPlayers = numPlayers;
            playerStatus = new PlayerInfo[numPlayers];
        }

        public Board(Board other) : this(other._numPlayers)
        {
            accessible = (int[,])other.accessible.Clone();
            for (int i = 0; i < _numPlayers; i++)
            {
                playerStatus[i] = new PlayerInfo(other.playerStatus[i]);
            }
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    if (i < BOARD_SIZE - 1 && j < BOARD_SIZE - 1)
                    {
                        wallPegStatus[i, j] = new WallPeg(other.wallPegStatus[i, j]);
                    }
                    boardStatus[i, j] = new gameSquareInfo(other.boardStatus[i, j]);
                }
            }
        }
        public void GetAccessible(int x, int y, int num, int direction)
        {
            if (accessible[x, y] > num)
            {
                accessible[x, y] = num;
                if (direction != 1 && x != 8 && CanMoveDown(x, y))
                {
                    GetAccessible(x + 1, y, num + 1, 3);
                }
                if (direction != 2 && y != 8 && CanMoveRight(x, y))
                {
                    GetAccessible(x, y + 1, num + 1, 0);
                }
                if (direction != 3 && x != 0 && CanMoveUp(x, y))
                {
                    GetAccessible(x - 1, y, num + 1, 1);
                }
                if (direction != 0 && y != 0 && CanMoveLeft(x, y))
                {
                    GetAccessible(x, y - 1, num + 1, 2);
                }
            }
        }

        public void GetAccessible(int x, int y)
        {
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    accessible[i, j] = 1000;
                }
            }
            GetAccessible(x, y, 0, -1);
        }

        #region Logic for checking if a move is legal
        public bool CheckWallH(int xPos, int yPos)
        {
            if (!boardStatus[xPos, yPos].hasBotWall &&
                !boardStatus[xPos, yPos + 1].hasBotWall &&
                 wallPegStatus[xPos, yPos].isOpen)
            {

                //Checking if winnable
                // Set as if there is a wall and check if can still be won
                wallPegStatus[xPos, yPos].isOpen = false; //dont need?
                boardStatus[xPos, yPos].hasBotWall = true;
                boardStatus[xPos, yPos + 1].hasBotWall = true;

                //if still winnable place the wall, else revert back to no wall
                if (CheckWinnable(0) && CheckWinnable(1))
                {
                    //PlaceWallH(xPos, yPos);
                    return true;
                }
                else
                {
                    wallPegStatus[xPos, yPos].isOpen = true; //dont need?
                    boardStatus[xPos, yPos].hasBotWall = false;
                    boardStatus[xPos, yPos + 1].hasBotWall = false;
                    Debug.Log("CANT PLACE H WALL CHEATER!!!");
                    return false;
                }

            }
            else
            {
                //MessageText.text = "Can't Place Horizontal wall there!";
                Debug.Log("CANT PLACE H WALL CHEATER!!!");
                return false;
            }

        }

        public bool CheckWinnable(int id)
        {
            bool possible = false;
            //gets the possible routes for current player
            GetAccessible(playerStatus[id].x, playerStatus[id].y);
            //checks to see which player goal is looked at
            //if any goal state doesn't have access of 1000 then its possible
            if (playerStatus[id].goalX >= 0)
            {

                for (int i = 0; i < BOARD_SIZE; i++)
                {
                    if (accessible[playerStatus[id].goalX, i] != 1000)
                    {
                        possible = true;
                        break;
                    }
                }
                return possible;
            }
            if (playerStatus[id].goalY >= 0)
            {

                for (int i = 0; i < BOARD_SIZE; i++)
                {
                    if (accessible[i, playerStatus[id].goalY] != 1000)
                    {
                        possible = true;
                        break;
                    }
                }
                return possible;
            }
            return false;
        }

        public bool CheckWallV(int xPos, int yPos)
        {
            if (!boardStatus[xPos, yPos].hasRightWall &&
                !boardStatus[xPos + 1, yPos].hasRightWall &&
                wallPegStatus[xPos, yPos].isOpen)
            {

                //Checking if winnable
                // Set as if there is a wall and check if can still be won
                wallPegStatus[xPos, yPos].isOpen = false; //dont need?
                boardStatus[xPos, yPos].hasRightWall = true;
                boardStatus[xPos + 1, yPos].hasRightWall = true;

                //if still winnable place the wall, else revert back to no wall
                if (CheckWinnable(0) && CheckWinnable(1))
                {
                    //PlaceWallV(xPos, yPos);
                    return true;
                }
                else
                {
                    wallPegStatus[xPos, yPos].isOpen = true; //dont need?
                    boardStatus[xPos, yPos].hasRightWall = false;
                    boardStatus[xPos + 1, yPos].hasRightWall = false;
                    //MessageText.text = "Can't Place Vertical wall there!";
                    Debug.Log("CANT PLACE V WALL CHEATER!!!");
                    return false;
                }
            }
            else
            {
                //MessageText.text = "Can't Place Vertical wall there!";
                Debug.Log("CANT PLACE V WALL CHEATER!!!");
                return false;
            }

        }

        #endregion

        #region Functions representing player's decision(moving or placing wall)
        //if turn jumping is false it will move either 1 space or 2(jumping directly)
        //if turn jumping is true it will move down and left
        public static bool MoveDown(Board board, int p, int movement, int direction, bool turnjumping = false)
        {
            if (!turnjumping)
            {
                //board.playerStatus[p].transform.position = board.boardStatus[board.playerStatus[p].x + movement, board.playerStatus[p].y].transform.position + new Vector3(0, 0, -1f);
                board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y].isOpen = true;
                board.boardStatus[board.playerStatus[p].x + movement, board.playerStatus[p].y].isOpen = false;
                board.playerStatus[p].x += movement;
            }
            else
            {
                //board.playerStatus[p].transform.position = board.boardStatus[board.playerStatus[p].x + movement, board.playerStatus[p].y - movement].transform.position + new Vector3(0, 0, -1f);
                board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y].isOpen = true;
                board.boardStatus[board.playerStatus[p].x + movement, board.playerStatus[p].y - movement].isOpen = false;
                board.playerStatus[p].x += movement;
                board.playerStatus[p].y -= movement;
            }
            return false;
        }

        //if turn jumping is false it will move either 1 space or 2(jumping directly)
        //if turn jumping is true it will move up and right
        public static bool MoveUp(Board board, int p, int movement, int direction, bool turnjumping = false)
        {
            if (!turnjumping)
            {
                //board.playerStatus[p].transform.position = board.boardStatus[board.playerStatus[p].x - movement, board.playerStatus[p].y].transform.position + new Vector3(0, 0, -1f);
                board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y].isOpen = true;
                board.boardStatus[board.playerStatus[p].x - movement, board.playerStatus[p].y].isOpen = false;
                board.playerStatus[p].x -= movement;
            }
            else
            {
                //board.playerStatus[p].transform.position = board.boardStatus[board.playerStatus[p].x - movement, board.playerStatus[p].y + movement].transform.position + new Vector3(0, 0, -1f);
                board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y].isOpen = true;
                board.boardStatus[board.playerStatus[p].x - movement, board.playerStatus[p].y + movement].isOpen = false;
                board.playerStatus[p].x -= movement;
                board.playerStatus[p].y += movement;
            }
            return false;
        }

        //if turn jumping is false it will move either 1 space or 2(jumping directly)
        //if turn jumping is true it will move left and up
        public static bool MoveLeft(Board board, int p, int movement, int direction, bool turnjumping = false)
        {
            if (!turnjumping)
            {
                //board.playerStatus[p].transform.position = board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y - movement].transform.position + new Vector3(0, 0, -1f);
                board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y].isOpen = true;
                board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y - movement].isOpen = false;
                board.playerStatus[p].y -= movement;
            }
            else
            {
                //board.playerStatus[p].transform.position = board.boardStatus[board.playerStatus[p].x - movement, board.playerStatus[p].y - movement].transform.position + new Vector3(0, 0, -1f);
                board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y].isOpen = true;
                board.boardStatus[board.playerStatus[p].x - movement, board.playerStatus[p].y - movement].isOpen = false;
                board.playerStatus[p].x -= movement;
                board.playerStatus[p].y -= movement;
            }
            return false;
        }

        //if turn jumping is false it will move either 1 space or 2(jumping directly)
        //if turn jumping is true it will move right and down
        public static bool MoveRight(Board board, int p, int movement, int direction, bool turnjumping = false)
        {
            if (!turnjumping)
            {
                //board.playerStatus[p].transform.position = board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y + movement].transform.position + new Vector3(0, 0, -1f);
                board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y].isOpen = true;
                board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y + movement].isOpen = false;
                board.playerStatus[p].y += movement;
            }
            else
            {
                //board.playerStatus[p].transform.position = board.boardStatus[board.playerStatus[p].x + movement, board.playerStatus[p].y + movement].transform.position + new Vector3(0, 0, -1f);
                board.boardStatus[board.playerStatus[p].x, board.playerStatus[p].y].isOpen = true;
                board.boardStatus[board.playerStatus[p].x + movement, board.playerStatus[p].y + movement].isOpen = false;
                board.playerStatus[p].x += movement;
                board.playerStatus[p].y += movement;
            }
            return false;
        }

        public bool PlaceWallH(int xPos, int yPos, int dummy = -1, bool isHorizontal = true)
        {
            wallPegStatus[xPos, yPos].isOpen = false;
            boardStatus[xPos, yPos].hasBotWall = true;
            boardStatus[xPos, yPos + 1].hasBotWall = true;
            return true;
        }

        public bool PlaceWallV(int xPos, int yPos, int dummy = -1, bool isHorizontal = false)
        {
            wallPegStatus[xPos, yPos].isOpen = false;
            boardStatus[xPos, yPos].hasRightWall = true;
            boardStatus[xPos + 1, yPos].hasRightWall = true;
            return true;
        }
        #endregion

        #region Functions checking if accessible (i.e not blocked by a wall)
        //used for checking if Accessible for winning (NOT USED FOR ACTUAL PLAYER MOVING)
        bool CanMoveDown(int posX, int posY) //test
        {
            // going down by 1
            if (!boardStatus[posX, posY].hasBotWall)
                return true;
            else
                return false;
        }

        //used for checking if Accessible for winning (NOT USED FOR ACTUAL PLAYER MOVING)
        bool CanMoveUp(int posX, int posY)
        {
            //going up by 1
            if (!boardStatus[posX - 1, posY].hasBotWall)
                return true;
            else
                return false;
        }

        //used for checking if Accessible for winning (NOT USED FOR ACTUAL PLAYER MOVING)
        bool CanMoveRight(int posX, int posY) //test
        {
            //going Right by 1
            if (!boardStatus[posX, posY].hasRightWall)
                return true;
            else
                return false;
        }

        //used for checking if Accessible for winning (NOT USED FOR ACTUAL PLAYER MOVING)
        bool CanMoveLeft(int posX, int posY) //test
        {
            //going Left by 1
            if (!boardStatus[posX, posY - 1].hasRightWall)
                return true;
            else
                return false;
        }
        #endregion

        public bool IsPawnMoveLegal(int formerX, int formerY, int newX, int newY)
        {
            int xDiff = newX - formerX;
            int yDiff = newY - formerY;
            // DOWN-----------------------------------------------------------
            // going down by 1 (checks on location and for walls)
            if (xDiff == 1 && yDiff == 0 &&
                formerX < 8 &&
                !boardStatus[formerX, formerY].hasBotWall &&
                boardStatus[formerX + 1, formerY].isOpen)
            {
                return true;
            }
            // going down jumping over 1 player directly
            else if (xDiff == 2 && yDiff == 0 &&
                    formerX < 7 &&
                    !boardStatus[formerX, formerY].hasBotWall &&
                    !boardStatus[formerX + 1, formerY].isOpen &&
                    !boardStatus[formerX + 1, formerY].hasBotWall)
            {
                return true;
            }
            // going Down by 1 AND to the Left by 1
            else if ((xDiff == 1 && yDiff == -1) &&
                    (formerX < 8 && formerY > 0) &&
                    ((formerX != 7 &&
                    !boardStatus[formerX, formerY].hasBotWall &&    //special case of down then left
                     !boardStatus[formerX + 1, formerY].isOpen &&
                    !boardStatus[newX, newY].hasRightWall)
                    ||
                    (formerX != 7 &&                                //going down then left
                    !boardStatus[formerX, formerY].hasBotWall &&
                     boardStatus[formerX + 1, formerY].hasBotWall &&
                     !boardStatus[formerX + 1, formerY].isOpen &&
                    !boardStatus[newX, newY].hasRightWall)
                    ||
                    (formerY == 1 &&
                    !boardStatus[formerX, formerY - 1].hasRightWall &&   //special case of left then down
                    !boardStatus[formerX, formerY - 1].isOpen &&
                   !boardStatus[formerX, formerY - 1].hasBotWall)
                    ||
                  (formerY != 1 &&
                  !boardStatus[formerX, formerY - 1].hasRightWall && //going left then down
                    boardStatus[formerX, formerY - 2].hasRightWall &&
                   !boardStatus[formerX, formerY - 1].isOpen &&
                   !boardStatus[formerX, formerY - 1].hasBotWall)))
            {
                return true;
            }

            // UP-----------------------------------------------------------------------
            // going up by 1 (checks on location and for walls)
            else if (xDiff == -1 && yDiff == 0 &&
                    formerX > 0 &&
                    !boardStatus[formerX - 1, formerY].hasBotWall &&
                    !boardStatus[formerX - 1, formerY].isOpen)
            {
                return true;
            }
            // going up jumping over 1 player directly
            else if (xDiff == -2 && yDiff == 0 &&
                formerX > 1 &&
             !boardStatus[formerX - 1, formerY].hasBotWall &&
             !boardStatus[formerX - 1, formerY].isOpen &&
             !boardStatus[formerX - 2, formerY].hasBotWall)
            {
                return true;
            }
            // going Up by 1 AND Right by 1
            else if (xDiff == -1 && yDiff == 1 &&
                formerX > 0 && formerY < 8 &&
                ((formerX == 1 &&                                //special case of up then right
                !boardStatus[formerX - 1, formerY].hasBotWall &&
                !boardStatus[formerX - 1, formerY].isOpen &&
                !boardStatus[formerX - 1, formerY].hasRightWall)
                ||
                (formerX != 1 &&
                !boardStatus[formerX - 1, formerY].hasBotWall && //going up then Right
                   boardStatus[formerX - 2, formerY].hasBotWall &&
                  !boardStatus[formerX - 1, formerY].isOpen &&
                  !boardStatus[formerX - 1, formerY].hasRightWall)
                  ||
                  (formerY == 7 &&                              //special case of right then up
                  !boardStatus[formerX, formerY].hasRightWall &&
                  !boardStatus[formerX, formerY + 1].isOpen &&
                  !boardStatus[formerX - 1, formerY + 1].hasBotWall
                  )
                  ||
                 (formerY != 7 &&
                 !boardStatus[formerX, formerY].hasRightWall && //going Right then Up
                   boardStatus[formerX, formerY + 1].hasRightWall &&
                  !boardStatus[formerX, formerY + 1].isOpen &&
                   !boardStatus[formerX - 1, formerY + 1].hasBotWall)))
            {
                return true;

            }

            // LEFT-----------------------------------------------------------------------
            // going Left by 1 (checks on location and for walls)
            else if (xDiff == 0 && yDiff == -1 &&
                    formerY > 0 &&
                    !boardStatus[formerX, formerY - 1].hasRightWall &&
                    !boardStatus[formerX, formerY - 1].isOpen)
            {
                return true;
            }
            // going Left jumping over 1 player directly
            else if (xDiff == 0 && yDiff == -2 &&
                     formerY > 1 &&
                     !boardStatus[formerX, formerY - 1].hasRightWall &&
                     !boardStatus[formerX, formerY - 1].isOpen &&
                     !boardStatus[formerX, formerY - 2].hasRightWall)
            {
                return true;
            }
            //going Left by 1 AND Up by 1
            else if (xDiff == -1 && yDiff == -1 &&
                (formerX > 0 && formerY > 0) &&
                ((formerY == 1 &&                                    //special case of left then up
                !boardStatus[formerX, formerY - 1].hasRightWall &&
                !boardStatus[formerX, formerY - 1].isOpen &&
                !boardStatus[newX, newY].hasBotWall)
                ||
                (formerY != 1 &&
                !boardStatus[formerX, formerY - 1].hasRightWall && //going left then up
                boardStatus[formerX, formerY - 2].hasRightWall &&
                !boardStatus[formerX, formerY - 1].isOpen &&
                !boardStatus[newX, newY].hasBotWall)
                ||
                (formerX == 1 &&                                    //special case of up then left
                !boardStatus[formerX - 1, formerY].hasBotWall &&
                !boardStatus[formerX - 1, formerY].isOpen &&
                !boardStatus[newX, newY].hasRightWall)
                ||
                (formerX != 1 &&
                !boardStatus[formerX - 1, formerY].hasBotWall && //going up then left
                boardStatus[formerX - 2, formerY].hasBotWall &&
                !boardStatus[formerX - 1, formerY].isOpen &&
                !boardStatus[newX, newY].hasRightWall)))
            {
                return true;

            }

            // RIGHT-----------------------------------------------------------------------
            // going Right by 1 (checks on location and for walls)
            else if (xDiff == 0 && yDiff == 1 &&
                formerY < 8 &&
                !boardStatus[formerX, formerY].hasRightWall &&
                !boardStatus[formerX, formerY + 1].isOpen)
            {
                return true;
            }
            ////going Right by jumping over 1 player directly
            else if (xDiff == 0 && yDiff == 2 &&
                formerY < 7 &&
                !boardStatus[formerX, formerY].hasRightWall &&
                !boardStatus[formerX, formerY + 1].isOpen &&
                 !boardStatus[formerX, formerY + 1].hasRightWall)
            {
                return true;
            }
            // going Right by 1 AND Down by 1
            else if (xDiff == 1 && yDiff == 1 &&
                (formerX < 8 && formerY < 8) &&
             ((formerY == 7 &&         //special case of right then down
             !boardStatus[formerX, formerY].hasRightWall &&
             !boardStatus[formerX, formerY + 1].isOpen &&
             !boardStatus[formerX, formerY + 1].hasBotWall)
             ||
             (formerY != 7 &&
             !boardStatus[formerX, formerY].hasRightWall && //going Right then Down
                boardStatus[formerX, formerY + 1].hasRightWall &&
               !boardStatus[formerX, formerY + 1].isOpen &&
               !boardStatus[formerX, formerY + 1].hasBotWall)
               ||
               (formerX == 7 &&                             //special case of down then right.
               !boardStatus[formerX, formerY].hasBotWall &&
               !boardStatus[formerX + 1, formerY].isOpen &&
               !boardStatus[formerX + 1, formerY].hasRightWall)
               ||
              (!boardStatus[formerX, formerY].hasBotWall && //going Down then Right
                boardStatus[formerX + 1, formerY].hasBotWall &&
               !boardStatus[formerX + 1, formerY].isOpen &&
               !boardStatus[formerX + 1, formerY].hasRightWall)))
            {
                return true;
            }
            return false;
        }

        //Check if there is a wall blocking the move
        public bool IsPawnMoveLegalSimplified(int formerX, int formerY, int newX, int newY)
        {
            int diffX = newX - formerX;
            int diffY = newY - formerY;
            if (diffX == 1 && diffY == 0)
                return CanMoveDown(formerX, formerY);
            else if (diffX == -1 && diffY == 0)
                return CanMoveUp(formerX, formerY);
            else if (diffX == 0 && diffY == -1)
                return CanMoveLeft(formerX, formerY);
            else if (diffX == 0 && diffY == 1)
                return CanMoveRight(formerX, formerY);
            else
                return false;   //shouldn't get here
        }
    }
}
