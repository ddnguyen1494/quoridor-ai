using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //to restart scene

public class GameManager : MonoBehaviour {

	const int maxPlayers = 4;
	const int boardSize = 9;
	const int numPlayers = 2;

	public Camera cam;

	public GameObject[] players;
	private PlayerInfo[] playerStatus = new PlayerInfo[numPlayers];

	public GameObject[,] board = new GameObject[boardSize, boardSize];
	private gameSquareInfo[,] boardStatus = new gameSquareInfo[boardSize , boardSize];

	public GameObject[,] pegs = new GameObject[boardSize-1,boardSize-1]; 
	private WallPeg[,] wallPegStatus = new WallPeg[boardSize-1,boardSize-1];

	public GameObject GamePiece1;
	public GameObject GamePiece2;
	public GameObject PegPiece;
	public GameObject wallH;
	public GameObject wallV;
	public GameObject gmPanel;

	public float startDelay = 1f;
	private WaitForSeconds m_StartWait;
	private WaitForSeconds m_EndWait;

	int totalWalls = 20;
	bool gameOver = false;
	int[,] accessible = new int[boardSize,boardSize];

	public Text playersTurnText;
	public Text wallsRemainText;
	public Text WinnerText;

	Vector3 newPosition;

	Ray ray;
	RaycastHit hit;

	// Use this for initialization
	void Start () {
		gmPanel.SetActive (false);
		MakeBoard (); // make references to board
		SpawnPlayers (); // make references to players

		m_StartWait = new WaitForSeconds (startDelay);
		StartCoroutine (GameLoop());
	}
		
	void Update()
	{
		// At any point, hitting the ESC key will quit the Application
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}
		
	void MakeBoard()
	{
		bool switchpiece = true;
		Vector3 currentPos = new Vector3 (1f,0f,0f);
		Vector3 pegPos = new Vector3 (2f,-1f,-0.75f);
		Quaternion pegRot = Quaternion.Euler(90,0,0);

		for (int i = 0; i < 9; i++) {
			for (int j = 0; j < 9; j++) {
				//pegs
				if(i < 8 && j < 8){
					pegs[i,j] = Instantiate(PegPiece, pegPos, pegRot);
					wallPegStatus [i, j] = pegs [i, j].GetComponent<WallPeg> ();
					wallPegStatus [i, j].x = i;
					wallPegStatus [i, j].y = j;
				}
				//board
				if (switchpiece) {
					
					board [i, j] = Instantiate (GamePiece1, currentPos, Quaternion.identity);
					boardStatus [i,j] = board [i,j].GetComponent<gameSquareInfo> ();
					boardStatus [i,j].location = currentPos;
					boardStatus [i, j].x = i;
					boardStatus [i, j].y = j;
				} 
				else {
					
					board [i, j] = Instantiate (GamePiece2, currentPos, Quaternion.identity);
					boardStatus [i,j] = board [i,j].GetComponent<gameSquareInfo> ();
					boardStatus [i,j].location = currentPos;
					boardStatus [i, j].x = i;
					boardStatus [i, j].y = j;
				}
				switchpiece = !switchpiece;
				currentPos += new Vector3 (2f,0,0);
				pegPos += new Vector3 (2f,0,0);
			}
			currentPos += new Vector3 (-18f,-2f,0);
			pegPos += new Vector3 (-18f,-2f,0);
		}
	}

	void SpawnPlayers()
	{
		for (int i = 0; i < numPlayers; i++)
		{
			playerStatus[i] = players[i].GetComponent<PlayerInfo> (); // Reference to playerInfo script
			playerStatus[i].body = Instantiate (players[i], playerStatus[i].spawnPoint, Quaternion.identity) as GameObject;
			playerStatus [i] = playerStatus [i].body.GetComponent<PlayerInfo> ();
			playerStatus[i].body = players[i].gameObject;
			playerStatus[i].id = i + 1;
		}

		//For 2 players set up start information this way
		if (numPlayers == 2) {
			playerStatus[0].transform.position = playerStatus [0].spawnPoint;
			playerStatus[0].x = 8;
			playerStatus[0].y = 4;
			playerStatus[0].goalX = 0;
			playerStatus[0].goalY = -1;
			boardStatus[8, 4].isOpen = false;

			playerStatus[1].transform.position = playerStatus [1].spawnPoint;
			playerStatus[1].x = 0;
			playerStatus[1].y = 4;
			playerStatus[1].goalX = 8;
			playerStatus[1].goalY = -1;
			boardStatus[0, 4].isOpen = false;

		}
	}

	private IEnumerator GameLoop()
	{
		yield return StartCoroutine( GamePrep ());
		yield return StartCoroutine (PlayGame ());
	}

	// Split amount of walls between players
	private IEnumerator GamePrep()
	{
		int wallAmt = totalWalls / numPlayers;
		for (int i = 0; i < numPlayers; i++)
		{
			playerStatus [i].wallsLeft = wallAmt;
			wallsRemainText.text += "Player " + (i + 1) + "'s Walls: " + playerStatus [i].wallsLeft + "\n";
		}
		yield return m_StartWait;
	}

	private IEnumerator PlayGame()
	{
		while (!gameOver) {
			Debug.Log("Player 1's turn Begin.");
			yield return StartCoroutine (PlayersTurn (1)); //player1's turn
			Debug.Log("Player 1's turn has ended.");
			// Check to see if player 1 has won
			if (playerStatus[0].CheckWin())
			{
				Debug.Log("Player 1 wins.");
				gameOver = true;
				GameOver (1);
				break;
			}

			Debug.Log("Player 2's turn Begin.");
			yield return StartCoroutine (PlayersTurn (2)); //player2's turn
			Debug.Log("Player 2's turn has ended.");
			// Check to see if player 2 has won
			if (playerStatus[1].CheckWin())
			{
				Debug.Log("Player 2 wins.");
				gameOver = true;
				GameOver (2);
				break;
			}
		} //end while loop

		yield return null;
	}

	private IEnumerator PlayersTurn(int playerNum)
	{
		int p = playerNum - 1;
		//move or choose wall
		playerStatus [p].currentTurn = true;
		playersTurnText.text = "Player " + playerNum + "'s Turn!";
		while(playerStatus[playerNum-1].currentTurn)
		{
			if (Input.GetMouseButtonUp(0)) {
				GameObject tempObj;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast (ray, out hit))
				{
					if(hit.transform.tag == "Board")
					{
						gameSquareInfo tempBoard;
						int xDiff;
						int yDiff;
						tempObj = hit.collider.gameObject;
						tempBoard = tempObj.GetComponent<gameSquareInfo>();

						xDiff = tempBoard.x - playerStatus [p].x;
						yDiff = tempBoard.y - playerStatus [p].y;

						//no matter what, chosen board must be available
						if(tempBoard.isOpen)
						{
						// DOWN-----------------------------------------------------------
							// going down by 1 (checks on location and for walls)
							if (xDiff == 1 && yDiff == 0 &&
								!boardStatus [playerStatus [p].x, playerStatus [p].y].hasBotWall) {
								MoveDown (p, 1);
								playerStatus [p].currentTurn = false;
							}
							// going down jumping over 1 player directly
							else if (xDiff == 2 && yDiff == 0 &&
								!boardStatus [playerStatus [p].x, playerStatus [p].y].hasBotWall &&
								!boardStatus [playerStatus [p].x+1, playerStatus [p].y].isOpen &&
								!boardStatus [playerStatus [p].x+1, playerStatus [p].y].hasBotWall) {
								MoveDown (p, 2);
								playerStatus [p].currentTurn = false;
							}
							// going Down by 1 AND to the Left by 1
							else if(xDiff == 1 && yDiff == -1 &&
								((!boardStatus [playerStatus [p].x, playerStatus [p].y].hasBotWall && //going down then left
									boardStatus [playerStatus [p].x+1, playerStatus [p].y].hasBotWall && 
									!boardStatus [playerStatus [p].x+1, playerStatus [p].y].isOpen) 
									||
									(!boardStatus [playerStatus [p].x, playerStatus [p].y-1].hasRightWall && //going left then down
										boardStatus [playerStatus [p].x, playerStatus [p].y-2].hasRightWall &&
										!boardStatus [playerStatus [p].x, playerStatus [p].y-1].isOpen)) ){
								MoveDown (p,1,true);
								playerStatus [p].currentTurn = false;
							}

						// UP-----------------------------------------------------------------------
							// going up by 1 (checks on location and for walls)
							else if (xDiff == -1 && yDiff == 0 &&
							 !boardStatus [playerStatus [p].x-1, playerStatus [p].y].hasBotWall) {
								MoveUp (p, 1);
								playerStatus [p].currentTurn = false;
							}
							// going up jumping over 1 player directly
							else if (xDiff == -2 && yDiff == 0 &&
							 !boardStatus [playerStatus [p].x-1, playerStatus [p].y].hasBotWall &&
							 !boardStatus [playerStatus [p].x-1, playerStatus [p].y].isOpen &&
							 !boardStatus [playerStatus [p].x-2, playerStatus [p].y].hasBotWall) {
								MoveUp (p, 2);
								playerStatus [p].currentTurn = false;
							}
							// going Up by 1 AND Right by 1
							else if(xDiff == -1 && yDiff == 1 &&
								((!boardStatus [playerStatus [p].x-1, playerStatus [p].y].hasBotWall && //going up then Right
								   boardStatus [playerStatus [p].x-2, playerStatus [p].y].hasBotWall &&
								  !boardStatus [playerStatus [p].x-1, playerStatus [p].y].isOpen)
								  ||
								 (!boardStatus [playerStatus [p].x, playerStatus [p].y].hasRightWall && //going Right then Up
								   boardStatus [playerStatus [p].x, playerStatus [p].y+1].hasRightWall &&
								  !boardStatus [playerStatus [p].x, playerStatus [p].y+1].isOpen))  ){
								MoveUp (p, 1, true);
								playerStatus [p].currentTurn = false;

							}

						// LEFT-----------------------------------------------------------------------
							// going Left by 1 (checks on location and for walls)
							else if(xDiff == 0 && yDiff == -1 &&
							 !boardStatus [playerStatus [p].x, playerStatus [p].y-1].hasRightWall) {
								MoveLeft (p,1);
								playerStatus [p].currentTurn = false;
							}
							// going Left jumping over 1 player directly
							else if(xDiff == 0 && yDiff == -2 &&
							 !boardStatus [playerStatus [p].x, playerStatus [p].y-1].hasRightWall &&
							 !boardStatus [playerStatus [p].x, playerStatus [p].y-1].isOpen &&
							 !boardStatus [playerStatus [p].x, playerStatus [p].y-2].hasRightWall) {
								MoveLeft (p,2);
								playerStatus [p].currentTurn = false;
							}
							//going Left by 1 AND Up by 1
							else if(xDiff == -1 && yDiff == -1 &&
								((!boardStatus [playerStatus [p].x, playerStatus [p].y-1].hasRightWall && //going left then up
									boardStatus [playerStatus [p].x, playerStatus [p].y-2].hasRightWall &&
									!boardStatus [playerStatus [p].x, playerStatus [p].y-1].isOpen)
									||
									(!boardStatus [playerStatus [p].x-1, playerStatus [p].y].hasBotWall && //going up then left
										boardStatus [playerStatus [p].x-2, playerStatus [p].y].hasBotWall &&
										!boardStatus [playerStatus [p].x-1, playerStatus [p].y].isOpen))  ){
								MoveLeft (p, 1, true);
								playerStatus [p].currentTurn = false;

							}

						// RIGHT-----------------------------------------------------------------------
							// going Right by 1 (checks on location and for walls)
							else if(xDiff == 0 && yDiff == 1 &&
							 !boardStatus [playerStatus [p].x, playerStatus [p].y].hasRightWall) {
								MoveRight (p,1);
								playerStatus [p].currentTurn = false;
							}
							else if(xDiff == 0 && yDiff == 2 &&
							 !boardStatus [playerStatus [p].x, playerStatus [p].y].hasRightWall &&
							 !boardStatus [playerStatus [p].x, playerStatus [p].y+1].isOpen &&
							 !boardStatus [playerStatus [p].x, playerStatus [p].y+1].hasRightWall) {
								MoveRight (p,2);
								playerStatus [p].currentTurn = false;
							}
							// going Right by 1 AND Down by 1
							else if(xDiff == 1 && yDiff == 1 &&
							 ((!boardStatus [playerStatus [p].x, playerStatus [p].y].hasRightWall && //going Right then Down
								boardStatus [playerStatus [p].x, playerStatus [p].y+1].hasRightWall && 
							   !boardStatus [playerStatus [p].x, playerStatus [p].y+1].isOpen) 
							   ||
							  (!boardStatus [playerStatus [p].x, playerStatus [p].y].hasBotWall && //going Down then Right
							 	boardStatus [playerStatus [p].x+1, playerStatus [p].y].hasBotWall &&
							   !boardStatus [playerStatus [p].x+1, playerStatus [p].y].isOpen)) ){
									MoveRight (p,1,true);
									playerStatus [p].currentTurn = false;
							}
						}

					} //end of "Board" tag
					//PLACING HORIZONTAL WALLS via Left-Click--------------------------------------------------
					if(hit.transform.tag == "Peg")
					{
						WallPeg tempPeg;
						tempObj = hit.collider.gameObject;
						tempPeg = tempObj.GetComponent<WallPeg>();

						//if wall can be placed, place it and end turn
						if (CheckWallH (tempPeg.x, tempPeg.y)) {
							playerStatus [p].currentTurn = false;
							playerStatus [p].wallsLeft--;
							UpdateWallRemTxt ();
						}
					}
				}
			}//end of Left Click if-statement

			//PLACING VERTICAL WALLS via Right-Click---------------------------------------------------
			if (Input.GetMouseButtonUp (1)) {
				if (Physics.Raycast (ray, out hit)) {
					GameObject tempObj;
					ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					if (hit.transform.tag == "Peg") {
						WallPeg tempPeg;
						tempObj = hit.collider.gameObject;
						tempPeg = tempObj.GetComponent<WallPeg> ();

						//if wall can be placed, place it and end turn
						if (CheckWallV (tempPeg.x, tempPeg.y)) {
							playerStatus [p].currentTurn = false;
							playerStatus [p].wallsLeft--;
							UpdateWallRemTxt ();
						}
					}
				}
			}//end of Right Click if statement


			yield return null;
		}// end of current turn while loop
	}
		
	//if turn jumping is false it will move either 1 space or 2(jumping directly)
	//if turn jumping is true it will move down and left
	void MoveDown(int p, int movement, bool turnjumping=false) 
	{
		if (!turnjumping) {
			playerStatus [p].transform.position = boardStatus [playerStatus [p].x + movement, playerStatus [p].y].transform.position + new Vector3 (0, 0, -1f);
			boardStatus [playerStatus [p].x, playerStatus [p].y].isOpen = true;
			boardStatus [playerStatus [p].x + movement, playerStatus [p].y].isOpen = false;
			playerStatus [p].x += movement; 
		} 
		else {
			playerStatus [p].transform.position = boardStatus [playerStatus [p].x + movement, playerStatus [p].y - movement].transform.position + new Vector3 (0, 0, -1f);
			boardStatus [playerStatus [p].x, playerStatus [p].y].isOpen = true;
			boardStatus [playerStatus [p].x + movement, playerStatus [p].y - movement].isOpen = false;
			playerStatus [p].x += movement; 
			playerStatus [p].y -= movement;
		}
	}

	//if turn jumping is false it will move either 1 space or 2(jumping directly)
	//if turn jumping is true it will move up and right
	void MoveUp(int p, int movement, bool turnjumping=false)
	{
		if (!turnjumping) {
			playerStatus [p].transform.position = boardStatus [playerStatus [p].x - movement, playerStatus [p].y].transform.position + new Vector3 (0, 0, -1f);
			boardStatus [playerStatus [p].x, playerStatus [p].y].isOpen = true;
			boardStatus [playerStatus [p].x - movement, playerStatus [p].y].isOpen = false;
			playerStatus [p].x -= movement;
		}
		else {
			playerStatus [p].transform.position = boardStatus [playerStatus [p].x - movement, playerStatus [p].y + movement].transform.position + new Vector3 (0, 0, -1f);
			boardStatus [playerStatus [p].x, playerStatus [p].y].isOpen = true;
			boardStatus [playerStatus [p].x - movement, playerStatus [p].y + movement].isOpen = false;
			playerStatus [p].x -= movement; 
			playerStatus [p].y += movement;
		}
	}

	//if turn jumping is false it will move either 1 space or 2(jumping directly)
	//if turn jumping is true it will move left and up
	void MoveLeft(int p, int movement, bool turnjumping=false)
	{
		if (!turnjumping) {
			playerStatus [p].transform.position = boardStatus [playerStatus [p].x, playerStatus [p].y - movement].transform.position + new Vector3 (0, 0, -1f);
			boardStatus [playerStatus [p].x, playerStatus [p].y].isOpen = true;
			boardStatus [playerStatus [p].x, playerStatus [p].y - movement].isOpen = false;
			playerStatus [p].y -= movement;
		}
		else {
			playerStatus [p].transform.position = boardStatus [playerStatus [p].x - movement, playerStatus [p].y - movement].transform.position + new Vector3 (0, 0, -1f);
			boardStatus [playerStatus [p].x, playerStatus [p].y].isOpen = true;
			boardStatus [playerStatus [p].x - movement, playerStatus [p].y - movement].isOpen = false;
			playerStatus [p].x -= movement; 
			playerStatus [p].y -= movement;
		}
	}

	//if turn jumping is false it will move either 1 space or 2(jumping directly)
	//if turn jumping is true it will move right and down
	void MoveRight(int p, int movement, bool turnjumping=false)
	{
		if (!turnjumping) {
			playerStatus [p].transform.position = boardStatus [playerStatus [p].x, playerStatus [p].y + movement].transform.position + new Vector3 (0, 0, -1f);
			boardStatus [playerStatus [p].x, playerStatus [p].y].isOpen = true;
			boardStatus [playerStatus [p].x, playerStatus [p].y + movement].isOpen = false;
			playerStatus [p].y += movement;
		}
		else {
			playerStatus [p].transform.position = boardStatus [playerStatus [p].x + movement, playerStatus [p].y + movement].transform.position + new Vector3 (0, 0, -1f);
			boardStatus [playerStatus [p].x, playerStatus [p].y].isOpen = true;
			boardStatus [playerStatus [p].x + movement, playerStatus [p].y + movement].isOpen = false;
			playerStatus [p].x += movement; 
			playerStatus [p].y += movement;
		}
	}
		
	bool CheckWallH(int xPos, int yPos)
	{
		if (!boardStatus [xPos, yPos].hasBotWall && 
			!boardStatus [xPos, yPos + 1].hasBotWall && 
			 wallPegStatus [xPos, yPos].isOpen) {

			//Checking if winnable
			// Set as if there is a wall and check if can still be won
			wallPegStatus [xPos, yPos].isOpen = false; //dont need?
			boardStatus [xPos, yPos].hasBotWall = true;
			boardStatus [xPos, yPos+1].hasBotWall = true;

			//if still winnable place the wall, else revert back to no wall
			if (CheckWinnable (0) && CheckWinnable(1)) {
				PlaceWallH (xPos, yPos);
				return true;
			} else {
				wallPegStatus [xPos, yPos].isOpen = true; //dont need?
				boardStatus [xPos, yPos].hasBotWall = false;
				boardStatus [xPos, yPos+1].hasBotWall = false;
				Debug.Log ("CANT PLACE H WALL CHEATER!!!");
				return false;
			}

		} else{
			Debug.Log ("CANT PLACE H WALL CHEATER!!!");
			return false;
		}

	}

	bool CheckWallV(int xPos, int yPos)
	{
		if (!boardStatus [xPos, yPos].hasRightWall &&
		    !boardStatus [xPos + 1, yPos].hasRightWall &&
		    wallPegStatus [xPos, yPos].isOpen) {

			//Checking if winnable
			// Set as if there is a wall and check if can still be won
			wallPegStatus [xPos, yPos].isOpen = false; //dont need?
			boardStatus [xPos, yPos].hasRightWall = true;
			boardStatus [xPos+1, yPos].hasRightWall = true;

			//if still winnable place the wall, else revert back to no wall
			if (CheckWinnable (0) && CheckWinnable(1)) {
				PlaceWallV (xPos, yPos);
				return true;
			} else {
				wallPegStatus [xPos, yPos].isOpen = true; //dont need?
				boardStatus [xPos, yPos].hasRightWall = false;
				boardStatus [xPos+1, yPos].hasRightWall = false;
				Debug.Log ("CANT PLACE V WALL CHEATER!!!");
				return false;
			}
		} else {
			Debug.Log ("CANT PLACE V WALL CHEATER!!!");
			return false;
		}

	}

	void PlaceWallH(int xPos, int yPos)
	{
		Instantiate (wallH, wallPegStatus[xPos,yPos].transform.position, Quaternion.identity);
		wallPegStatus [xPos, yPos].isOpen = false;
		boardStatus [xPos, yPos].hasBotWall = true;
		boardStatus [xPos, yPos+1].hasBotWall = true;
	}

	void PlaceWallV(int xPos, int yPos)
	{
		Quaternion wallRot = Quaternion.Euler(0,0,90);

		Instantiate (wallV, wallPegStatus[xPos,yPos].transform.position, wallRot);
		wallPegStatus [xPos, yPos].isOpen = false;
		boardStatus [xPos, yPos].hasRightWall = true;
		boardStatus [xPos+1, yPos].hasRightWall = true;
	}

	void UpdateWallRemTxt()
	{
		wallsRemainText.text = "Remaining Walls:\n\n";
		for (int i = 0; i < numPlayers; i++)
		{
			wallsRemainText.text += "Player " + (i + 1) + "'s Walls: " + playerStatus [i].wallsLeft + "\n";
		}
	}

	bool CheckWinnable(int id)
	{
		bool possible = false;
		//gets the possible routes for current player
		getAccessible(playerStatus[id].x, playerStatus[id].y);
		//checks to see which player goal is looked at
		//if any goal state doesn't have access of 1000 then its possible
		if (playerStatus[id].goalX >= 0)
		{
			
			for (int i = 0; i < boardSize; i++)
			{
				if (accessible[playerStatus[id].goalX,i] != 1000)
				{
					possible = true;
					break;
				}
			}
			return possible;
		}
		if (playerStatus[id].goalY >= 0)
		{
			
			for (int i = 0; i < boardSize; i++)
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

	void getAccessible(int x, int y, int num, int direction)
	{
		if (accessible[x, y] > num)
		{
			accessible[x, y] = num;
			if (direction != 1 && x != 8 && CanMoveDown(x,y))
			{	
				getAccessible(x+1, y, num + 1, 3);
			}
			if (direction != 2 && y !=8 && CanMoveRight(x, y))
			{
				getAccessible(x, y+1, num + 1, 0);
			}
			if (direction != 3 && x != 0 && CanMoveUp(x, y))
			{
				getAccessible(x-1, y, num + 1, 1);
			}
			if (direction != 0 && y !=0 && CanMoveLeft(x, y))
			{
				getAccessible(x, y-1, num + 1, 2);
			}
		}
	}

	void getAccessible(int x, int y)
	{
		for (int i = 0; i < boardSize; i++)
		{
			for (int j = 0; j < boardSize; j++)
			{
				accessible[i, j] = 1000;
			}
		}
		getAccessible(x, y, 0, -1);
	}



	//used for checking if Accessible for winning (NOT USED FOR ACTUAL PLAYER MOVING)
	bool CanMoveDown(int posX, int posY) //test
	{
		// going down by 1
		if (!boardStatus [posX, posY].hasBotWall)
			return true;
		else
			return false;
	}

	//used for checking if Accessible for winning (NOT USED FOR ACTUAL PLAYER MOVING)
	bool CanMoveUp(int posX, int posY)
	{
		//going up by 1
		if (!boardStatus [posX - 1, posY].hasBotWall)
			return true;
		else
			return false;
	}

	//used for checking if Accessible for winning (NOT USED FOR ACTUAL PLAYER MOVING)
	bool CanMoveRight(int posX, int posY) //test
	{
		//going Right by 1
		if (!boardStatus [posX, posY].hasRightWall)
			return true;
		else
			return false;
	}

	//used for checking if Accessible for winning (NOT USED FOR ACTUAL PLAYER MOVING)
	bool CanMoveLeft(int posX, int posY) //test
	{
		//going Left by 1
		if (!boardStatus [posX, posY-1].hasRightWall)
			return true;
		else
			return false;
	}


	void GameOver(int playerNum)
	{
		gmPanel.SetActive (true);
		WinnerText.text = "Player " + (playerNum) + " is the Winner!";

	}

	public void RestartScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
} // end
