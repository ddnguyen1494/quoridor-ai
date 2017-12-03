using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //to restart scene
using Assets.Scripts;


public class GameManager : MonoBehaviour
{

    const int maxPlayers = 4;
    const int boardSize = 9;
    const int minPlayers = 2;
    int numPlayers = 2;

    public Camera cam;

    public GameObject[] players;
    private PlayerInfo[] playerStatus;

    public GameObject[,] board = new GameObject[boardSize, boardSize];
    private gameSquareInfo[,] boardStatus;

    public GameObject[,] pegs = new GameObject[boardSize - 1, boardSize - 1];
    private WallPeg[,] wallPegStatus;

    public GameObject GamePiece1;
    public GameObject GamePiece2;
    public GameObject PegPiece;
    public GameObject wallH;
    public GameObject wallV;
    public GameObject gmPanel;

    public float startDelay = 1f;
    public float aiDelay = 10f;
    private WaitForSeconds m_StartWait;

    int totalWalls = 20;
    bool gameOver = false;
    int[,] accessible;

    public Text playersTurnText;
    public Text wallsRemainText;
    public Text WinnerText;
    public Text MessageText;

    //Vector3 newPosition;

    Ray ray;
    RaycastHit hit;

    Assets.Scripts.Board MainBoard;

    private Assets.Scripts.Agent MyAgent;
    // Use this for initialization
    void Start()
    {
        numPlayers = MainMenu.playerTotal;
        //if(numPlayers == 2) //2 players
        //	playerStatus = new PlayerInfo[minPlayers];
        //else //4 players
        //	playerStatus = new PlayerInfo[maxPlayers];

        //Creating the board and get reference to it's member variables for now
        MainBoard = new Board(numPlayers);
        playerStatus = MainBoard.playerStatus;
        Debug.Assert(playerStatus != null);
        wallPegStatus = MainBoard.wallPegStatus;
        Debug.Assert(wallPegStatus != null);
        boardStatus = MainBoard.boardStatus;
        Debug.Assert(boardStatus != null);
        accessible = MainBoard.accessible;
        Debug.Assert(accessible != null);

        gmPanel.SetActive(false);
        MessageText.text = "";
        MakeBoard(); // make references to board
        SpawnPlayers(); // make references to players (and Ai's)

        m_StartWait = new WaitForSeconds(startDelay);
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        // At any point, hitting the ESC key will quit the Application
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void MakeBoard()
    {
        bool switchpiece = true;
        Vector3 currentPos = new Vector3(1f, 0f, 0f);
        Vector3 pegPos = new Vector3(2f, -1f, -0.75f);
        Quaternion pegRot = Quaternion.Euler(90, 0, 0);

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                //pegs
                if (i < 8 && j < 8)
                {
                    pegs[i, j] = Instantiate(PegPiece, pegPos, pegRot);
                    wallPegStatus[i, j] = pegs[i, j].GetComponent<WallPeg>();
                    wallPegStatus[i, j].x = i;
                    wallPegStatus[i, j].y = j;
                }
                //board
                if (switchpiece)
                {

                    board[i, j] = Instantiate(GamePiece1, currentPos, Quaternion.identity);
                    boardStatus[i, j] = board[i, j].GetComponent<gameSquareInfo>();
                    boardStatus[i, j].location = currentPos;
                    boardStatus[i, j].x = i;
                    boardStatus[i, j].y = j;
                }
                else
                {

                    board[i, j] = Instantiate(GamePiece2, currentPos, Quaternion.identity);
                    boardStatus[i, j] = board[i, j].GetComponent<gameSquareInfo>();
                    boardStatus[i, j].location = currentPos;
                    boardStatus[i, j].x = i;
                    boardStatus[i, j].y = j;
                }
                switchpiece = !switchpiece;
                currentPos += new Vector3(2f, 0, 0);
                pegPos += new Vector3(2f, 0, 0);
            }
            currentPos += new Vector3(-18f, -2f, 0);
            pegPos += new Vector3(-18f, -2f, 0);
        }
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            playerStatus[i] = players[i].GetComponent<PlayerInfo>(); // Reference to playerInfo script
            playerStatus[i].body = Instantiate(players[i], playerStatus[i].spawnPoint, Quaternion.identity) as GameObject;
            playerStatus[i] = playerStatus[i].body.GetComponent<PlayerInfo>();
            playerStatus[i].body = players[i].gameObject;
            playerStatus[i].id = i + 1;
        }

        // For 2 players set up start information this way
        if (numPlayers == 2)
        {
            playerStatus[0].transform.position = playerStatus[0].spawnPoint;
            playerStatus[0].x = 8;
            playerStatus[0].y = 4;
            playerStatus[0].goalX = 0;
            playerStatus[0].goalY = -1;
            boardStatus[8, 4].isOpen = false;
            if (MainMenu.playerSettings == 2) // If EvE was selected make 1st player a Bot
            {
                playerStatus[0].isAi = true;
                MyAgent = new Assets.Scripts.Agent();
            }

            playerStatus[1].transform.position = playerStatus[1].spawnPoint;
            playerStatus[1].x = 0;
            playerStatus[1].y = 4;
            playerStatus[1].goalX = 8;
            playerStatus[1].goalY = -1;
            boardStatus[0, 4].isOpen = false;
            if (MainMenu.playerSettings == 1 || MainMenu.playerSettings == 2) // If PvE or EvE was selected make 2nd player a Bot
            {
                playerStatus[1].isAi = true;
                MyAgent = new Assets.Scripts.Agent();
            }
        }
        // For 4 players set up start information this way
        else if (numPlayers == 4)
        {
            playerStatus[0].transform.position = playerStatus[0].spawnPoint;
            playerStatus[0].x = 8;
            playerStatus[0].y = 4;
            playerStatus[0].goalX = 0;
            playerStatus[0].goalY = -1;
            boardStatus[8, 4].isOpen = false;

            playerStatus[1].transform.position = playerStatus[1].spawnPoint;
            playerStatus[1].x = 0;
            playerStatus[1].y = 4;
            playerStatus[1].goalX = 8;
            playerStatus[1].goalY = -1;
            boardStatus[0, 4].isOpen = false;

            playerStatus[2].transform.position = playerStatus[2].spawnPoint;
            playerStatus[2].x = 4;
            playerStatus[2].y = 8;
            playerStatus[2].goalX = -1;
            playerStatus[2].goalY = 0;
            boardStatus[4, 8].isOpen = false;

            playerStatus[3].transform.position = playerStatus[3].spawnPoint;
            playerStatus[3].x = 4;
            playerStatus[3].y = 0;
            playerStatus[3].goalX = -1;
            playerStatus[3].goalY = 8;
            boardStatus[4, 0].isOpen = false;
        }

    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(GamePrep());
        yield return StartCoroutine(PlayGame());
    }

    // Split amount of walls between players
    private IEnumerator GamePrep()
    {
        int wallAmt = totalWalls / numPlayers;
        for (int i = 0; i < numPlayers; i++)
        {
            playerStatus[i].wallsLeft = wallAmt;
            wallsRemainText.text += "Player " + (i + 1) + "'s Walls: " + playerStatus[i].wallsLeft + "\n";
        }
        yield return null;
    }

    private IEnumerator PlayGame()
    {
        int turnOrder = 0;

        while (!gameOver)
        {
//            Debug.Log("Player " + (turnOrder + 1) + "'s turn Begin.");
            if (!playerStatus[turnOrder].isAi)
                yield return StartCoroutine(PlayersTurn(turnOrder)); // Human Players Turn
            else
                yield return StartCoroutine(AITurn(turnOrder)); // AI Players Turn
//            Debug.Log("Player " + (turnOrder + 1) + "'s turn has ended.");
            // Check to see if player has won
            if (playerStatus[turnOrder].CheckWin())
            {
//                Debug.Log("Player " + (turnOrder + 1) + "wins.");
                gameOver = true;
                GameOver(turnOrder + 1);
                break;
            }
            if (++turnOrder == numPlayers)
                turnOrder = 0;
        } //end while loop

        yield return null;
    }
    private IEnumerator AITurn(int playerNum) /// AI Controls
	{
        MessageText.text = "";
        //move or choose wall
        playerStatus[playerNum].currentTurn = true;
        playersTurnText.text = "Player " + (playerNum + 1) + "'s Turn!";
        Assets.Scripts.ActionFunction action = MyAgent.NextMove(MainBoard, playerNum); // <- currently has an issue
        if (action.function == null)
        {
            Debug.LogError("Agent action is null. Something is wrong. Agent just skip his move");
            yield break;
        }
        if (action.function.Method.Name == "MovePawn")
            RenderPawnPosition(action.player, action.x, action.y);
        else if (action.function.Method.Name == "PlaceHorizontalWall")
            RenderWall(action.x, action.y, true);
        else if (action.function.Method.Name == "PlaceVerticalWall")
            RenderWall(action.x, action.y, false);
        else
            Debug.LogError("Agent returning a non-supported action");
        MainBoard.ExecuteFunction(action);
        UpdateWallRemTxt(); //Update UI anyway because it doesn't matter
        yield return null;
    }

    private IEnumerator PlayersTurn(int p) // Player Controls
    {
        MessageText.text = "";
        //int p = playerNum - 1;
        //move or choose wall
        playerStatus[p].currentTurn = true;
        playersTurnText.text = "Player " + (p + 1) + "'s Turn!";
        while (playerStatus[p].currentTurn)
        {
            // Passing Turn
            if (Input.GetKeyUp(KeyCode.X))
            {
                playersTurnText.text = "Player " + (p + 1) + " Passes!";
                playerStatus[p].currentTurn = false;
                yield return m_StartWait;
//                Debug.Log("Player " + (p + 1) + "Passes.");
            }

			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			bool collisionFound = Physics.Raycast(ray, out hit);

			// Moving Handler
			if (Input.GetMouseButtonUp(0))
            {
                GameObject tempObj;
                if (collisionFound)
                {
                    if (hit.transform.tag == "Board")
                    {
                        gameSquareInfo tempSquare;
                        int xDiff;
                        int yDiff;
                        tempObj = hit.collider.gameObject;
                        tempSquare = tempObj.GetComponent<gameSquareInfo>();

                        xDiff = tempSquare.x - playerStatus[p].x;
                        yDiff = tempSquare.y - playerStatus[p].y;

                        //no matter what, chosen board must be available
                        if (tempSquare.isOpen)
                        {
                            // DOWN-----------------------------------------------------------
                            // going down by 1 (checks on location and for walls)
                            if (xDiff == 1 && yDiff == 0 &&
                                MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;
                            }
                            // going down jumping over 1 player directly
                            else if (xDiff == 2 && yDiff == 0 &&
								MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;
                            }
                            // going Down by 1 AND to the Left by 1
                            else if (xDiff == 1 && yDiff == -1 &&
                                MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;
                            }

                            // UP-----------------------------------------------------------------------
                            // going up by 1 (checks on location and for walls)
                            else if (xDiff == -1 && yDiff == 0 &&
                             MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;
                            }
                            // going up jumping over 1 player directly
                            else if (xDiff == -2 && yDiff == 0 &&
                             MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;
                            }
                            // going Up by 1 AND Right by 1
                            else if (xDiff == -1 && yDiff == 1 &&
                                MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;

                            }

                            // LEFT-----------------------------------------------------------------------
                            // going Left by 1 (checks on location and for walls)
                            else if (xDiff == 0 && yDiff == -1 &&
                             MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;
                            }
                            // going Left jumping over 1 player directly
                            else if (xDiff == 0 && yDiff == -2 &&
								MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;
                            }
                            //going Left by 1 AND Up by 1
                            else if (xDiff == -1 && yDiff == -1 &&
                                MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;

                            }

                            // RIGHT-----------------------------------------------------------------------
                            // going Right by 1 (checks on location and for walls)
                            else if (xDiff == 0 && yDiff == 1 &&
                             MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;
                            }
                            else if (xDiff == 0 && yDiff == 2 &&
                             MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;
                            }
                            // going Right by 1 AND Down by 1
                            else if (xDiff == 1 && yDiff == 1 &&
                             MainBoard.IsPawnMoveLegal(playerStatus[p].x, playerStatus[p].y, tempSquare.x, tempSquare.y))
                            {
                                RenderPawnPosition(p, tempSquare.x, tempSquare.y);
                                Board.MovePawn(MainBoard, p, tempSquare.x, tempSquare.y);
                                playerStatus[p].currentTurn = false;
                            }
                        }

                    } //end of "Board" tag
                      //PLACING HORIZONTAL WALLS via Left-Click--------------------------------------------------
                    if (hit.transform.tag == "Peg")
                    {
                        WallPeg tempPeg;
                        tempObj = hit.collider.gameObject;
                        tempPeg = tempObj.GetComponent<WallPeg>();

                        //if wall can be placed, place it and end turn
                        if (playerStatus[p].wallsLeft > 0 && MainBoard.CheckWallH(tempPeg.x, tempPeg.y))
                        {
                            RenderWall(tempPeg.x, tempPeg.y, true);
                            Board.PlaceHorizontalWall(MainBoard, p, tempPeg.x, tempPeg.y);
                            playerStatus[p].currentTurn = false;
                            UpdateWallRemTxt();
                        }
                    }
                }
            }//end of Left Click if-statement

            //PLACING VERTICAL WALLS via Right-Click---------------------------------------------------
            if (Input.GetMouseButtonUp(1))
            {
                if (collisionFound)
                {
                    GameObject tempObj;
                    if (hit.transform.tag == "Peg")
                    {
                        WallPeg tempPeg;
                        tempObj = hit.collider.gameObject;
                        tempPeg = tempObj.GetComponent<WallPeg>();

                        //if wall can be placed, place it and end turn
                        if (playerStatus[p].wallsLeft > 0 && MainBoard.CheckWallV(tempPeg.x, tempPeg.y))
                        {
                            RenderWall(tempPeg.x, tempPeg.y, false);
                            Board.PlaceVerticalWall(MainBoard, p, tempPeg.x, tempPeg.y);
                            playerStatus[p].currentTurn = false;
                            UpdateWallRemTxt();
                        }
                    }
                }
            }//end of Right Click if statement


            yield return null;
        }// end of current turn while loop
    }

    void GameOver(int playerNum)
    {
        gmPanel.SetActive(true);
        WinnerText.text = "Player " + (playerNum) + " is the Winner!";
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }

    public void RenderWall(int xPos, int yPos, bool isHorizontal)
    {
        if (isHorizontal)
            Instantiate(wallH, wallPegStatus[xPos, yPos].transform.position, Quaternion.identity);
        else
        {
            Quaternion wallRot = Quaternion.Euler(0, 0, 90);
            Instantiate(wallV, wallPegStatus[xPos, yPos].transform.position, wallRot);
        }
    }


    public void RenderPawnPosition(int player, int xPos, int yPos)
    {
        playerStatus[player].transform.position = boardStatus[xPos, yPos].transform.position + new Vector3(0, 0, -1f);
    }

    void UpdateWallRemTxt()
    {
        wallsRemainText.text = "Remaining Walls:\n\n";
        for (int i = 0; i < numPlayers; i++)
        {
            wallsRemainText.text += "Player " + (i + 1) + "'s Walls: " + playerStatus[i].wallsLeft + "\n";
        }
    }
} // end
