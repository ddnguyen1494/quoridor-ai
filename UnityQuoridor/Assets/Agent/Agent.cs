using System;

namespace Assets.Scripts
{
	public abstract class Agent
	{
		public const int PLAYER1 = 0;
		public const int PLAYER2 = 1;
		public Node root;
		public int MAX_DEPTH = 12;
		public int current_depth;
		public Board board;
		public int player_num;

        public Agent()
        {

        }
		public Agent(int depth)
		{
            MAX_DEPTH = depth;
        }

		public abstract ActionFunction NextMove(Board board, int player);

		public abstract void GenerateSuccessors (Node node);

		public abstract int Evaluate (Node node);

		public abstract bool CutOff (int depth);

        public abstract bool IsTimeUp();
	}
}

