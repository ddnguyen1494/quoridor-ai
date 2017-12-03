using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Assets.Scripts
{
    internal struct ValueAndAction
    {
        public ValueAndAction(int val, ActionFunction act)
        {
            value = val;
            action = act;
        }
        public int value;
        public ActionFunction action;
    }


    class AlphaBeta
    {
		private static ValueAndAction MaxValue(Agent agent, Node node, ref int alpha, ref int beta, int depth)
        {
            if (agent.CutOff(depth))
            {
                node.Value = agent.Evaluate(node);
				if (node.Value != 0)
                return new ValueAndAction(node.Value, node.Move);
            }
            int tempVal = int.MinValue;
            ActionFunction tempAction = new ActionFunction();
            if (node.Children.Count == 0)
                agent.GenerateSuccessors(node);
			foreach (Node child in node.Children)
            {
                //if (agent.IsTimeUp())
                //    break;
                agent.board.ExecuteFunction(child.Move);
                var retValAction = MinValue(agent, child, ref alpha, ref beta, depth + 1);
                agent.board.ExecuteFunction(child.Undo);
                if (retValAction.value > tempVal)
                {
                    tempVal = retValAction.value;
                    tempAction = child.Move;
                    if (retValAction.value >= beta)
                    {
                        retValAction.action = child.Move;
                        return retValAction;
                    }
                    alpha = Math.Max(alpha, retValAction.value);
                }
            }
            return new ValueAndAction(tempVal, tempAction);

        }

		private static ValueAndAction MinValue(Agent agent, Node node, ref int alpha, ref int beta, int depth)
        {
            if (agent.CutOff(depth))
            {
                node.Value = agent.Evaluate(node);
				if (node.Value != 0)
                return new ValueAndAction(node.Value, node.Move);
            }
            int tempVal = int.MaxValue;
            ActionFunction tempAction = new ActionFunction();
            if (node.Children.Count == 0)
                agent.GenerateSuccessors(node);
            foreach (Node child in node.Children)
            {
                //if (agent.IsTimeUp())
                //    break;
                agent.board.ExecuteFunction(child.Move);
                var retValAction = MaxValue(agent, child, ref alpha, ref beta, depth + 1);
                agent.board.ExecuteFunction(child.Undo);
                if (retValAction.value < tempVal)
                {
                    tempVal = retValAction.value;
                    tempAction = child.Move;
                    if (retValAction.value <= alpha)
                    {
                        retValAction.action = child.Move;
                        return retValAction;
                    }
                    beta = Math.Min(beta, retValAction.value);
                }
            }
            return new ValueAndAction(tempVal, tempAction);
        }

        public static ActionFunction Search(Agent agent)
        {
            int alpha = int.MinValue;
            int beta = int.MaxValue;
			return MaxValue(agent, agent.root, ref alpha, ref beta, 0).action;
        }
    }
}
