## Project Title

QUORIDOR AI

## Description

[Quoridor](https://en.wikipedia.org/wiki/Quoridor) is a board game with 81 squares (9x9). The objective of the game is to get your pawn from the starting position to the other end of the board first. Players on their turn may choose to move one space, horizontally or vertically, or they may place a wall on the board that blocks 2 square spaces on the board. Walls cannot be placed so they block the only route a player has to reach the other side. There are many different strategies to the game which can be interesting to watch or figure out. 

## Getting Started

#### Prerequisites

* Visual Studio
* Unity v2017.2.0f3

#### How to run executable:

.exe file can be found in the UnityQuoridor->Builds Folder

#### How to run Project in Unity:

* Open up Unity and select "Open" at top right.
* Choose whole folder [UnityQuoridor] containing the Quoridor project.
* Project should then open in Unity and can be manipulated.
* To start playing, open Scene1 by going to File->
* Scripts can be found in the UnityQuoridor/Assets

## Built With

* [Unity](https://unity3d.com/learn/tutorials/s/2d-game-creation)
* [High-Speed-Priority-Queue-for-C-Sharp](https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp)

## Functional Description

* GameManager.cs handles the GUI of the game. It listens for user's click event and determine the move he/she wants to make. It's primary purpose is to render the board object and facilitate player's turn.

* Board.cs contains the Board class represents the board using the 3 dependent classes: gameSquareInfo, PlayerStatus, and WallPeg. The Board class also contains methods that check the legality of each move. Finally, it contains static methods that Agent can use to carry out its decision (e.g. MovePawn, PlaceHorizontalWall, PlaceVerticalWall, etc.)

* AlphaBeta.cs contains the static class AlphaBeta which implements Minimax algorithm with alpha beta pruning.

* AStarSearch.cs contains the static class FindShortestPathLength that accepts the Board object and an int representing which player's turn. It uses Best-first search with heuristics calculated using the difference between the player's position and it's respective goal.

* Agent.cs represents the AI agent. It contains four important methods: NextMove, Generate Successors, Cutoff, and Evaluate. When it's the AI's turn, the GameManager will calls the Agent's NextMove method and pass in the current state of the board. The Agent will then start the watch (Iterative Deepening Search) and uses Minimax search to determine the next move. The Evaluate function is called by the Minimax class to evaluate the values of each node.

## Authors

* Daniel Domingo
* David Brown
* Charles Bucher
* Daniel Nguyen

## License

This project is licensed under the MIT License - see the LICENSE.md file for details

