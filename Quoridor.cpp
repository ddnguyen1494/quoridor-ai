// Quoridor.cpp : Defines the entry point for the console application.
// A console based implementation of Quoridor designed by David Brown
//Version 1.2 10/4/2017

#include "stdafx.h"
#include <iostream>
#include <cstdlib>
#include <string>
using namespace std;

//consists of integer x and y coordinate
struct position
{
	int x;
	int y;
};

//represents a player's piece
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

	//creates a player starting at (0, 0)
	player()
	{
		x = 0;
		y = 0;
	}

	//creates a player starting at the given position
	player(int sx, int sy)
	{
		x = sx;
		y = sy;
	}

	//returns true if the player has reached goalX or goalY
	bool checkWin()
	{
		return (x == goalX || y == goalY);
	}

};

//stores the state of the game
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

	//Creates the board with the given number of players and no walls
	board(int pNum)
	{
		for (int j = 0; j < height; j++)
		{
			for (int i = 0; i < width; i++)
			{
				collisions[i][j] = ' ';
			}
		}
		for (int k = 0; k < pNum; k++)
		{
			addPlayer();
		}
		startGame();
	}

	//Translates from a string outputted from stateString() into the board's state
	board(string state)
	{
		for (int j = 0; j < height; j++)
		{
			for (int i = 0; i < width; i++)
			{
				collisions[i][j] = ' ';
			}
		}
		int start = 0;
		int value = 0;
		int comma = 0;
		while (state[comma] != ',')
		{
			comma++;
		}
		while (start < comma)
		{
			value *= 10;
			value += (int)(state[start] - '0');
			start++;
		}
		numPlayers = value;
		start++;
		comma++;
		while (state[comma] != ',')
		{
			comma++;
		}
		value = 0;
		while (start < comma)
		{
			value *= 10;
			value += (int)(state[start] - '0');
			start++;
		}
		currentPlayer = value;
		start++;
		comma++;
		for (int j = 1; j <= 15; j+=2)
		{
			for (int i = 1; i <= 15; i+=2)
			{
				if (state[start] == 'H')
				{
					placeWallH(i - 1, j);
				}
				else if (state[start] == 'V')
				{
					placeWallV(i, j - 1);
				}
				start += 2;
			}
		}
		comma = start;
		int wall = 0;
		int x = 0;
		int y = 0;
		for (int k = 0; k < numPlayers; k++)
		{
			wall = 0;
			x = 0;
			y = 0;
			while (state[comma] != ',')
			{
				comma++;
			}
			while (start < comma)
			{
				wall *= 10;
				wall += (int)(state[start] - '0');
				start++;
			}
			start++;
			comma = start;
			while (state[comma] != ',')
			{
				comma++;
			}
			while (start < comma)
			{
				x *= 10;
				x += (int)(state[start] - '0');
				start++;
			}
			start++;
			comma = start;
			while (state[comma] != ',')
			{
				comma++;
			}
			while (start < comma)
			{
				y *= 10;
				y += (int)(state[start] - '0');
				start++;
			}
			start++;
			comma = start;
			playerList[k] = player(x, y);
			playerList[k].wallsLeft = wall;
			playerList[k].textChar = (char)('a' + k);
			collisions[x][y] = playerList[k].textChar;
		}

	}
	
	//adds a player if there are 3 or fewer players
	void addPlayer()
	{
		if (numPlayers <= 3)
		{
			int stX[4] = { 8, 8, 0, 16 };
			int stY[4] = { 0, 16, 8, 8 };
			int gX[4] = { -1, -1, 16, 0 };
			int gY[4] = { 16, 0, -1, -1 };
			player p(stX[numPlayers], stY[numPlayers]);
			p.goalX = gX[numPlayers];
			p.goalY = gY[numPlayers];
			p.id = numPlayers;
			p.textChar = (char)('a' + p.id);
			playerList[numPlayers] = p;
			numPlayers++;
			collisions[p.x][p.y] = p.textChar;
		}
	}

	//gives all players an equal amount of walls
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

	//returns whether playerList[id] can reach the goal row or column
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

	//Moves playerList[id] to (nx, ny), changing their old position in collisions to ' '
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

	//returns true if from (x, y), it is possible to move to the adjacent square to the right
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

	//returns true if from (x, y), it is possible to move to the adjacent square above it
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

	//returns true if from (x, y), it is possible to move to the adjacent square to the left
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

	//returns true if from (x, y), it is possible to move to the adjacent square below it
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

	//sets accessible[x][y] to num and adjacent accessible squares to num + 1, does not go backwards
	void getAccessible(int x, int y, int num, int direction)
	{
		if (accessible[x][y] > num)
		{
			accessible[x][y] = num;
			if (direction != 2 && canMoveRight(x, y))
			{
				getAccessible(x + 2, y, num + 1, 0);
			}
			if (direction != 3 && canMoveUp(x, y))
			{
				getAccessible(x, y - 2, num + 1, 1);
			}
			if (direction != 0 && canMoveLeft(x, y))
			{
				getAccessible(x - 2, y, num + 1, 2);
			}
			if (direction != 1 && canMoveDown(x, y))
			{
				getAccessible(x, y + 2, num + 1, 3);
			}
		}
	}

	//sets accessible to the shortest distance from (x, y) to all other squares,
	//inaccessible squares are marked with 1000 and the method does not pass through walls
	void getAccessible(int x, int y)
	{
		for (int i = 0; i < 17; i++)
		{
			for (int j = 0; j < 17; j++)
			{
				accessible[i][j] = 1000;
			}
		}
		getAccessible(x, y, 0, -1);
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
					validMoves[6].x = -1;
					validMoves[6].y = -1;
					if (canMoveDown(p.x - 2, p.y))
					{
						validMoves[11].x = p.x - 2; //move 12: 1 square left, 1 square down
						validMoves[11].y = p.y + 2;
					}
					if (canMoveUp(p.x - 2, p.y))
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
				validMoves[3].x = -1;
				validMoves[3].y = -1;
				if (!canMoveDown(p.x, p.y + 2))
				{
					validMoves[7].x = -1;
					validMoves[7].y = -1;
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

	//shifts all in bounds moves in validMoves as far left as possible, leaving out of bound values at the right
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

	//uses keyboard input to let a human player take a turn
	//0-9 moves the piece to the labeled square,
	//x passes, and h and v place horizontal and vertical walls
	void playerTurn()
	{
		char inputChar = 'p';
		getMoves(currentPlayer);
		cout << ToString();
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
		
		string message = "It is player " + to_string(currentPlayer + 1) + "'s turn.\nTo move, type the number of an avaliable move.\nTo pass, press x.\n";
		if (playerList[currentPlayer].wallsLeft > 0)
		{
			message += "You have " + to_string(playerList[currentPlayer].wallsLeft) + " walls left.\nTo place a horizontal wall, press h. To place a vertical wall, press v.\n";
		}
		while ((inputChar > '9' || inputChar < '0') && (inputChar != 'h' && inputChar != 'v' && inputChar != 'x'))
		{
			cout << message;
			cin >> inputChar;
			if (inputChar >= '0' && inputChar <= '9')
			{
				int moveNum = (int)(inputChar - '0');
				if (moveNum >= 0 && moveNum <= 11 && inBounds(validMoves[moveNum].x, validMoves[moveNum].y))
				{
					movePlayer(currentPlayer, validMoves[moveNum].x, validMoves[moveNum].y);
					if (playerList[currentPlayer].checkWin())
					{
						winner = currentPlayer;
					}
				}
				else
				{
					inputChar = 'p';
				}
			}
			else
			{
				if (inputChar == 'x')
				{
					break;
				}
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

		}

		currentPlayer++;
		if (currentPlayer >= numPlayers)
		{
			currentPlayer = 0;
		}
		cout << '\n';
	}

	//Returns a string representation of validMoves with each (x,y) coordinate on a new line
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
			else
			{
				break;
			}
		}
		return result;
	}

	//Returns a string representation of collisions, gridlines are shown when there is no wall
	//and x or y is odd and the whole grid is bordered with = and ||.
	string ToString()
	{
		string result = "//=================\\\\\n";
		int xMod = 0;
		int yMod = 0;
		for (int j = 0; j < height; j++)
		{
			result += "||";
			for (int i = 0; i < width; i++)
			{
				xMod = i % 2;
				yMod = j % 2;
				if (collisions[i][j] == ' ')
				{
					if (xMod == 0)
					{
						if (yMod == 1)
						{
							result += '-';
						}
						else
						{
							result += ' ';
						}
					}
					else
					{
						if (yMod == 0)
						{
							result += '|';
						}
						else
						{
							result += '+';
						}
					}
				}
				else
				{
					result += collisions[i][j];
				}
			}
			result += "||\n";
		}
		result += "\\\\=================//\n";
		return result;
	}

	//Returns the most compact string representing the board's state
	string stateString()
	{
		string result = "";
		result += to_string(numPlayers) +",";
		result += to_string(currentPlayer) + ",";
		for (int j = 1; j <= 15; j += 2)
		{
			for (int i = 1; i <= 15; i += 2)
			{
				result += collisions[i][j];
				result += ",";
			}
		}
		for (int k = 0; k < numPlayers; k++)
		{
			result += to_string(playerList[k].wallsLeft) + ",";
			result += to_string(playerList[k].x) + ",";
			result += to_string(playerList[k].y) + ",";
		}
		return result;
	}

};

//runs the main loop of the game
int main(int argc, char *argv[])
{
	int players = -1;
	if (argc == 1) //player count not provided on the command line, assume 2 players
	{
		players = 2;
	}
	else if (strlen(argv[1]) == 1 && argv[1][0] >= '2' && argv[1][0] <= '4')
	{
		players = (int)(argv[0][0] - '0');
	}
	if (players < 2 || players > 4)
	{
		cout << "Quoridor is for 2 to 4 players. The program can only be run if the provided number of players is in range.\n";
		return -1;
	}
	board b(players);
	while (b.winner == -1) //loop until the game is won
	{
		cout << b.stateString() << '\n';
		b.playerTurn();
	}
	cout << b.ToString();
	cout << "Player "<< 1 + b.winner << " won.\nPress enter to finish.";
	cin.get();
	cin.get();
	return 0;
}