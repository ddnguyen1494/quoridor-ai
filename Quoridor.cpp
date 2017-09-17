// Quoridor.cpp : Defines the entry point for the console application.
// A console based implementation of Quoridor designed by David Brown
#include "stdafx.h"
#include <iostream>
#include <cstdlib>
#include <string>
using namespace std;
class player
{
public:
	int x;
	int y;
	int goalX;
	int goalY;
	int wallsLeft;
	int id;
	char textChar;
	player()
	{
		x = 0;
		y = 0;
	}

	player(int sx, int sy)
	{
		x = sx;
		y = sy;
	}

	bool checkWin()
	{
		return (x == goalX || y == goalY);
	}

};
struct position
{
	int x;
	int y;
};
class board
{
public:
	const int width = 17;
	const int height = 17;
	char collisions[17][17];
	int accessible[17][17];
	int numPlayers = 0;
	player playerList[4];
	position validMoves[12];
	int currentPlayer = 0;
	int winner = -1;
	board()
	{
		for (int j = 0; j < height; j++)
		{
			for (int i = 0; i < width; i++)
			{
				collisions[i][j] = ' ';
			}
		}
	}
	void addPlayer(int x, int y)
	{
		if (numPlayers <= 2)
		{
			player p(x, y);
			p.id = numPlayers;
			p.textChar = (char)('a' + p.id);
			playerList[p.id] = p;
			numPlayers++;
			collisions[x][y] = p.textChar;
		}
		if (numPlayers == 1)
		{
			playerList[0].x = 8;
			playerList[0].y = 0;
			playerList[0].goalX = -1;
			playerList[0].goalY = 16;
		}
		else if (numPlayers == 2)
		{
			playerList[1].x = 8;
			playerList[1].y = 16;
			playerList[1].goalX = -1;
			playerList[1].goalY = 0;
		}
		else if (numPlayers == 3)
		{
			playerList[2].x = 0;
			playerList[2].y = 8;
			playerList[2].goalX = 16;
			playerList[2].goalY = -1;
		}
		else
		{
			playerList[3].x = 16;
			playerList[3].y = 8;
			playerList[3].goalX = 0;
			playerList[3].goalY = -1;
		}
	}
	void startGame()
	{
		int wallAmount = 20 / numPlayers;
		for (int i = 0; i < numPlayers; i++)
		{
			playerList[i].wallsLeft = wallAmount;
		}
	}
	//places a horizontal wall of length 3 with its left position at (x,y), call canPlaceWallH before using this
	void placeWallH(int x, int y)
	{
		collisions[x][y] = 'H';
		collisions[x + 1][y] = 'H';
		collisions[x + 2][y] = 'H';
	}
	//places a vertical wall of length 3 with its top position at (x,y), call canPlaceWallV before using this
	void placeWallV(int x, int y)
	{
		collisions[x][y] = 'V';
		collisions[x][y + 1] = 'V';
		collisions[x][y + 2] = 'V';
	}
	//returns whether it is possible to place a horizontal wall with its left at position (x,y) and its length 3
	bool canPlaceWallH(int x, int y)
	{
		if (y < 1 || y >= width || y % 2 == 0)
		{
			return false;
		}
		if (x < 0 || x + 2 >= width || x % 2 != 0)
		{
			return false;
		}
		if (collisions[x][y] != ' ' || collisions[x + 1][y] != ' ' || collisions[x + 2][y] != ' ')
		{
			return false;
		}
		return true;
	}

	bool checkWinnable(int id)
	{
		getAccessible(playerList[id].x, playerList[id].y);
		if (playerList[id].goalX >= 0)
		{
			bool possible = false;
			for (int i = 0; i < 17; i += 2)
			{
				if (accessible[playerList[id].goalX][i] != 1000)
				{
					possible = true;
					break;
				}
			}
			return possible;
		}
		else
		{
			bool possible = false;
			for (int i = 0; i < 17; i += 2)
			{
				if (accessible[i][playerList[id].goalY] != 1000)
				{
					possible = true;
					break;
				}
			}
			return possible;
		}
		return false;
	}

	//returns whether it is possible to place a vertical wall with its top at position (x,y) and its length 3
	bool canPlaceWallV(int x, int y)
	{
		if (x < 1 || x >= width || x % 2 == 0)
		{
			return false;
		}
		if (y < 0 || y + 2 >= height || y % 2 != 0)
		{
			return false;
		}
		if (collisions[x][y] != ' ' || collisions[x][y + 1] != ' ' || collisions[x][y + 2] != ' ')
		{
			return false;
		}
		return true;
	}
	void movePlayer(int id, int nx, int ny)
	{
		collisions[playerList[id].x][playerList[id].y] = ' ';
		playerList[id].x = nx;
		playerList[id].y = ny;
		collisions[playerList[id].x][playerList[id].y] = playerList[id].textChar;
	}
	//returns true if (x,y) is in bounds and false if it is out of bounds
	bool inBounds(int x, int y)
	{
		return (x >= 0 && x < width && y >= 0 && y < height);
	}
	bool canMoveRight(int x, int y)
	{
		if (x % 2 == 0 && y % 2 == 0)
		{
			if (inBounds(x + 2, y) && collisions[x + 1][y] == ' ')
			{
				return true;
			}
		}
		return false;
	}
	bool canMoveUp(int x, int y)
	{
		if (x % 2 == 0 && y % 2 == 0)
		{
			if (inBounds(x, y - 2) && collisions[x][y - 1] == ' ')
			{
				return true;
			}
		}
		return false;
	}
	bool canMoveLeft(int x, int y)
	{
		if (x % 2 == 0 && y % 2 == 0)
		{
			if (inBounds(x - 2, y) && collisions[x - 1][y] == ' ')
			{
				return true;
			}
		}
		return false;
	}
	bool canMoveDown(int x, int y)
	{
		if (x % 2 == 0 && y % 2 == 0)
		{
			if (inBounds(x, y + 2) && collisions[x][y + 1] == ' ')
			{
				return true;
			}
		}
		return false;
	}

	void getAccessible(int x, int y, int num)
	{
		if (accessible[x][y] > num)
		{
			accessible[x][y] = num;
			if (canMoveRight(x, y))
			{
				getAccessible(x + 2, y, num + 1);
			}
			if (canMoveUp(x, y))
			{
				getAccessible(x, y - 2, num + 1);
			}
			if (canMoveLeft(x, y))
			{
				getAccessible(x - 2, y, num + 1);
			}
			if (canMoveDown(x, y))
			{
				getAccessible(x, y + 2, num + 1);
			}
		}
	}

	void getAccessible(int x, int y)
	{
		for (int i = 0; i < 17; i++)
		{
			for (int j = 0; j < 17; j++)
			{
				accessible[i][j] = 1000;
			}
		}
		getAccessible(x, y, 0);
	}

	//sets the validMoves list to all moves that are valid to make for the given player
	void getMoves(int id)
	{
		player p = playerList[id];
		validMoves[0].x = p.x + 2; //move 1: 1 square right
		validMoves[0].y = p.y;
		validMoves[1].x = p.x; //move 2: 1 square up
		validMoves[1].y = p.y - 2;
		validMoves[2].x = p.x - 2; //move 3: 1 square left
		validMoves[2].y = p.y;
		validMoves[3].x = p.x; //move 4: 1 square down
		validMoves[3].y = p.y + 2;
		validMoves[4].x = p.x + 4; //move 5: 2 squares right
		validMoves[4].y = p.y;
		validMoves[5].x = p.x; //move 6: 2 squares up
		validMoves[5].y = p.y - 4;
		validMoves[6].x = p.x - 4; //move 7: 2 squares left
		validMoves[6].y = p.y;
		validMoves[7].x = p.x; //move 8: 2 squares down
		validMoves[7].y = p.y + 4;
		validMoves[8].x = -1; //move 9: 1 square right, 1 square down
		validMoves[8].y = -1;
		validMoves[9].x = -1; //move 10: 1 square right, 1 square up
		validMoves[9].y = -1;
		validMoves[10].x = -1; //move 11: 1 square left, 1 square up
		validMoves[10].y = -1;
		validMoves[11].x = -1; //move 12: 1 square left, 1 square down
		validMoves[11].y = -1;
		if (canMoveRight(p.x, p.y))
		{
			if (collisions[p.x + 2][p.y] == ' ')
			{
				validMoves[4].x = -1;
				validMoves[4].y = -1;
			}
			else
			{
				validMoves[0].x = -1;
				validMoves[0].y = -1;
				if (!canMoveRight(p.x + 2, p.y))
				{
					validMoves[4].x = -1;
					validMoves[4].y = -1;
					if (canMoveDown(p.x + 2, p.y))
					{
						validMoves[8].x = p.x + 2; //move 9: 1 square right, 1 square down
						validMoves[8].y = p.y + 2;
					}
					if (canMoveUp(p.x + 2, p.y))
					{
						validMoves[9].x = p.x + 2; //move 10: 1 square right, 1 square up
						validMoves[9].y = p.y - 2;
					}
				}
			}
		}
		else
		{
			validMoves[0].x = -1;
			validMoves[0].y = -1;
			validMoves[4].x = -1;
			validMoves[4].y = -1;
		}
		if (canMoveUp(p.x, p.y))
		{
			if (collisions[p.x][p.y - 2] == ' ')
			{
				validMoves[5].x = -1;
				validMoves[5].y = -1;
			}
			else
			{
				validMoves[1].x = -1;
				validMoves[1].y = -1;
				if (!canMoveUp(p.x, p.y - 2))
				{
					validMoves[5].x = -1;
					validMoves[5].y = -1;
					if (canMoveRight(p.x, p.y - 2))
					{
						validMoves[9].x = p.x + 2; //move 10: 1 square right, 1 square up
						validMoves[9].y = p.y - 2;
					}
					if (canMoveLeft(p.x, p.y - 2))
					{
						validMoves[10].x = p.x - 2; //move 11: 1 square left, 1 square up
						validMoves[10].y = p.y - 2;
					}
				}
			}
		}
		else
		{
			validMoves[1].x = -1;
			validMoves[1].y = -1;
			validMoves[5].x = -1;
			validMoves[5].y = -1;
		}
		if (canMoveLeft(p.x, p.y))
		{
			if (collisions[p.x - 2][p.y] == ' ')
			{
				validMoves[6].x = -1;
				validMoves[6].y = -1;
			}
			else
			{
				validMoves[2].x = -1;
				validMoves[2].y = -1;
				if (!canMoveLeft(p.x - 2, p.y))
				{
					validMoves[4].x = -1;
					validMoves[4].y = -1;
					if (canMoveDown(p.x + 2, p.y))
					{
						validMoves[11].x = p.x - 2; //move 12: 1 square left, 1 square down
						validMoves[11].y = p.y + 2;
					}
					if (canMoveUp(p.x + 2, p.y))
					{
						validMoves[10].x = p.x - 2; //move 11: 1 square left, 1 square up
						validMoves[10].y = p.y - 2;
					}
				}
			}
		}
		else
		{
			validMoves[2].x = -1;
			validMoves[2].y = -1;
			validMoves[6].x = -1;
			validMoves[6].y = -1;
		}
		if (canMoveDown(p.x, p.y))
		{
			if (collisions[p.x][p.y + 2] == ' ')
			{
				validMoves[7].x = -1;
				validMoves[7].y = -1;
			}
			else
			{
				validMoves[0].x = -1;
				validMoves[0].y = -1;
				if (!canMoveDown(p.x, p.y - 2))
				{
					validMoves[5].x = -1;
					validMoves[5].y = -1;
					if (canMoveRight(p.x, p.y + 2))
					{
						validMoves[8].x = p.x + 2; //move 9: 1 square right, 1 square down
						validMoves[8].y = p.y + 2;
					}
					if (canMoveLeft(p.x, p.y + 2))
					{
						validMoves[11].x = p.x - 2; //move 12: 1 square left, 1 square down
						validMoves[11].y = p.y + 2;
					}
				}
			}
		}
		else
		{
			validMoves[3].x = -1;
			validMoves[3].y = -1;
			validMoves[7].x = -1;
			validMoves[7].y = -1;
		}
		compactMoves();
	}

	void compactMoves()
	{
		int max = 0;
		for (int i = 0; i < 12; i++)
		{
			if (inBounds(validMoves[i].x, validMoves[i].y))
			{
				validMoves[max] = validMoves[i];
				collisions[validMoves[max].x][validMoves[max].y] = (char)('0' + max);
				max++;
			}
		}
		while (max < 12)
		{
			validMoves[max].x = -1;
			validMoves[max].y = -1;
			max++;
		}
	}

	void playerTurn()
	{
		char inputChar = 'p';
		getMoves(currentPlayer);
		cout << ToString() << '\n';
		for (int i = 0; i < 12; i++)
		{
			if (inBounds(validMoves[i].x, validMoves[i].y))
			{
				collisions[validMoves[i].x][validMoves[i].y] = ' ';
			}
			else
			{
				break;
			}
		}
		cout << "It is player " << currentPlayer + 1 << "'s turn.\n";
		cout << "To move, type the number of an avaliable move.\n";
		cout << "To pass, press x.\n";
		if (playerList[currentPlayer].wallsLeft > 0)
		{
			cout << "You have " << playerList[currentPlayer].wallsLeft << " walls left.\n";
			cout << "To place a horizontal wall, press h. To place a vertical wall, press v.\n";
		}
		while ((inputChar > '9' || inputChar < '0') && (inputChar != 'h' && inputChar != 'v' && inputChar != 'x'))
		{
			cin >> inputChar;
			if (inputChar > '9' || inputChar < '0')
			{
				if (playerList[currentPlayer].wallsLeft == 0)
				{
					cout << "You are out of walls. Your only choice is to move.\n";
					inputChar = 'p';
				}
				else
				{
					if (inputChar == 'h')
					{
						int x = -1;
						int y = -1;
						while (!canPlaceWallH(x, y))
						{
							x = -1;
							y = -1;
							cout << "Input the left position of the wall. ";
							cin >> x;
							cin >> y;
						}
						placeWallH(x, y);
						bool valid = true;
						bool winnable;
						for (int i = 0; i < numPlayers; i++)
						{
							winnable = checkWinnable(i);
							cout << winnable << '\n';
							if (!winnable)
							{
								valid = false;
								break;
							}
						}
						if (valid)
						{
							playerList[currentPlayer].wallsLeft--;
						}
						else
						{
							cout << "The wall cannot be placed there because it makes the game unwinnable for at least one player.\n";
							collisions[x][y] = ' ';
							collisions[x+1][y] = ' ';
							collisions[x+2][y] = ' ';
							inputChar = 'p';
						}
					}
					else if (inputChar == 'v')
					{
						int x = -1;
						int y = -1;
						while (!canPlaceWallV(x, y))
						{
							cout << "Input the top position of the wall. ";
							cin >> x;
							cin >> y;
						}
						placeWallV(x, y);
						bool valid = true;
						bool winnable;
						for (int i = 0; i < numPlayers; i++)
						{
							winnable = checkWinnable(i);
							//cout << winnable << '\n';
							if (!winnable)
							{
								valid = false;
								break;
							}
						}
						if (valid)
						{
							playerList[currentPlayer].wallsLeft--;
						}
						else
						{
							cout << "The wall cannot be placed there because it makes the game unwinnable for at least one player.\n";
							collisions[x][y] = ' ';
							collisions[x][y + 1] = ' ';
							collisions[x][y + 2] = ' ';
							inputChar = 'p';
						}
					}
				}

			}
			else
			{
				int moveNum = (int)(inputChar - '0');
				if (moveNum >= 0 && moveNum <= 11 && inBounds(validMoves[moveNum].x, validMoves[moveNum].y))
				{
					movePlayer(currentPlayer, validMoves[moveNum].x, validMoves[moveNum].y);
				}
				else
				{
					inputChar = 'p';
				}
			}
		}
		if (playerList[currentPlayer].checkWin())
		{
			winner = currentPlayer;
		}
		currentPlayer++;
		if (currentPlayer >= numPlayers)
		{
			currentPlayer = 0;
		}
		cout << '\n';
	}

	string movesString()
	{
		string result = "Avaliable Moves for player " + to_string(currentPlayer + 1) + ":\n";
		for (int i = 0; i < 12; i++)
		{
			if (inBounds(validMoves[i].x, validMoves[i].y))
			{
				result += to_string(i);
				result += " (" + to_string(validMoves[i].x);
				result += ", " + to_string(validMoves[i].y);
				result += ")\n";
			}
		}
		return result;
	}
	string ToString()
	{
		string result = "";
		for (int j = 0; j < height; j++)
		{
			for (int i = 0; i < width; i++)
			{
				if (collisions[i][j] == ' ')
				{
					if (i % 2 == 0 && j % 2 == 1)
					{
						result += '-';
					}
					else if (i % 2 == 1 && j % 2 == 0)
					{
						result += '|';
					}
					else if (i % 2 == 1 && j % 2 == 1)
					{
						result += '+';
					}
					else
					{
						result += ' ';
					}
				}
				else
				{
					result += collisions[i][j];
				}
			}
			result += '\n';
		}

		return result;
	}

};

int main()
{
	board b;
	b.addPlayer(8, 0);
	b.addPlayer(8, 16);
	b.startGame();
	//cout << b.ToString() << '\n';
	while (b.winner == -1)
	{
		b.playerTurn();
	}
	//b.getMoves(0);
	//cout << b.movesString();
	//b.movePlayer(0, 8, 2);
	cout << "Player "<< 1 + b.winner << " won.\nPress enter to finish.";
	cin.get();
	cin.get();
	return 0;
}