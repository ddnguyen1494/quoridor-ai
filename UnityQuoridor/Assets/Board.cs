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

        public Board(Board other) : this (other._numPlayers)
        {
            accessible = (int[,]) other.accessible.Clone();
            for (int i = 0; i < _numPlayers; i++)
            {
                playerStatus[i] = new PlayerInfo(other.playerStatus[i]);
            }
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    if (i < BOARD_SIZE -1 && j < BOARD_SIZE - 1)
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

        //public void SetWallPegStatus(int i, int j, WallPeg peg)
        //{
        //    wallPegStatus[i, j] = peg;
        //    wallPegStatus[i, j].x = i;
        //    wallPegStatus[i, j].y = j;
        //}

        //public void SetBoardStatus(int i, int j, gameSquareInfo info, Vector3 currentPos)
        //{
        //    boardStatus[i, j] = info;
        //    boardStatus[i, j].location = currentPos;
        //    boardStatus[i, j].x = i;
        //    boardStatus[i, j].y = j;
        //}

        //public void SetPlayerStatus(int i, PlayerInfo info, GameObject body)
        //{
        //    playerStatus[i] = info; // Reference to playerInfo script
        //    playerStatus[i].body = body;
        //    playerStatus[i] = playerStatus[i].body.GetComponent<PlayerInfo>();
        //    playerStatus[i].body = body;
        //    playerStatus[i].id = i + 1;
        //}
        //public void SetPlayerStartInformation(int index, Vector3 spawnPoint, int x , int y, int goalX , int goalY. bool isOpen)
        //{
        //    playerStatus[index].transform.position = playerStatus[0].spawnPoint;
        //    playerStatus[index].x = 8;
        //    playerStatus[index].y = 4;
        //    playerStatus[index].goalX = 0;
        //    playerStatus[index].goalY = -1;
        //    boardStatus[8, 4].isOpen = false;
        //}
    }
}
