using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Assets.Scripts
{
    class Agent
    {
        private Graph gameTree;
        private int _look_ahead;
        //Pre generate game trees with specified depth
        Agent(BitArray startState, int look_ahead)
        {
            _look_ahead = look_ahead;
            gameTree = new Graph(startState, look_ahead);
        }
        
        //return Agent's decision
        public BitArray NextMove(BitArray state)
        {
            gameTree = new Graph(state, _look_ahead);
            Minimax.Search(gameTree);
            return null;
        }

    }
}
