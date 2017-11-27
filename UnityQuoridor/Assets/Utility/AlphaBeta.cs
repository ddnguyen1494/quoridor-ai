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
        private static ValueAndAction MaxValue(Node node, ref int alpha, ref int beta, int depth)
        {
            if (Agent.CutOff(node.State, depth))
            {
                node.Value = Agent.Evaluate(node);
                return new ValueAndAction(node.Value, node.Action);
            }
            int tempVal = int.MinValue;
            ActionFunction tempAction = new ActionFunction();
            Agent.GenerateSuccessors(node);
            foreach (Node child in node.Children)
            {
                var retValAction = MinValue(child, ref alpha, ref beta, depth + 1);
                if(retValAction.value > tempVal)
                {
                    tempVal = retValAction.value;
                    tempAction = retValAction.action;
                    if (retValAction.value >= beta)
                        return retValAction;
                    alpha = Math.Max(alpha, retValAction.value);
                }
            }
            return new ValueAndAction(tempVal, tempAction);

        }

        private static ValueAndAction MinValue(Node node, ref int alpha, ref int beta, int depth)
        {
            if (Agent.CutOff(node.State, depth))
            {
                node.Value = Agent.Evaluate(node);
                return new ValueAndAction(node.Value, node.Action);
            }
            int tempVal = int.MaxValue;
            ActionFunction tempAction = new ActionFunction();
            Agent.GenerateSuccessors(node);
            foreach (Node child in node.Children)
            {
                var retValAction = MaxValue(child, ref alpha, ref beta, depth + 1);
                if (retValAction.value < tempVal)
                {
                    tempVal = retValAction.value;
                    tempAction = retValAction.action;
                    if (retValAction.value <= alpha)
                        return retValAction;
                    beta = Math.Min(beta, retValAction.value);
                }
            }
            return new ValueAndAction(tempVal, tempAction);
        }

        public static ActionFunction Search(Node root)
        {
            int alpha = int.MinValue;
            int beta = int.MaxValue;
            return MaxValue(root, ref alpha, ref beta, 0).action;
        }
    }
}
