using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Assets.Scripts
{
    class Agent
    {
        const int PLAYER1 = 0;
        const int PLAYER2 = 1;
        //private Graph gameTree;
        Node currentState;
        private static int _ply_depth;
        
        public Agent(int ply_depth)
        {
            _ply_depth = ply_depth;
        }
        
        //return Agent's decision
        public BitArray NextMove(BitArray state)
        {
            currentState = new Node(state, _ply_depth);
            return AlphaBeta.Search(currentState);
        }

        public static int Evaluate(BitArray state)
        {
            PlayerInfo[] players;
            int currentPlayer;
            List<WallPeg> wallPegList;
            StateDecoder.DecodeState(state, out players, out currentPlayer, out wallPegList);
            if (players[(currentPlayer + 1) % 1].GetDistanceToGoal() != 0)  //opponent is not at winning state
            {
                return 81 * players[0].GetDistanceToGoal() / players[1].GetDistanceToGoal();
            }
            else
            {
                return int.MaxValue;
            }
        }

        public static bool CutOff(Node node, int depth)
        {
            PlayerInfo[] players = StateDecoder.DecodePlayerInfo(node.State);
            if (players[PLAYER1].CheckWin() ||
                players[PLAYER2].CheckWin() ||
                depth == _ply_depth)
                return true;
            return false;
        }

        public static void GenerateSuccessors(Node node)
        {
            PlayerInfo[] players;
            int currentPlayer;
            List<WallPeg> wallPegList;
            StateDecoder.DecodeState(node.State, out players, out currentPlayer, out wallPegList);

            //TODO: Generate all legal children of this Node




            return;
        }
    }
}
