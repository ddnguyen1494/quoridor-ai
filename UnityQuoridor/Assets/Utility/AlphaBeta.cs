using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

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
        private static ValueAndAction MaxValue(Board board, Node node, ref int alpha, ref int beta, int depth)
        {
            if (Agent.CutOff(depth))
            {
                node.Value = Agent.Evaluate(node);
				if (node.Value != 0)
					UnityEngine.Debug.Log(node.ToString () + "at depth: " + depth + " has value = " + node.Value); 
                return new ValueAndAction(node.Value, node.Move);
            }
            int tempVal = int.MinValue;
            ActionFunction tempAction = new ActionFunction();
            Agent.GenerateSuccessors(node);
            foreach (Node child in node.Children)
            {
                board.ExecuteFunction(child.Move);
                var retValAction = MinValue(board, child, ref alpha, ref beta, depth + 1);
                board.ExecuteFunction(child.Undo);
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

        private static ValueAndAction MinValue(Board board, Node node, ref int alpha, ref int beta, int depth)
        {
            if (Agent.CutOff(depth))
            {
                node.Value = Agent.Evaluate(node);
				if (node.Value != 0)
					UnityEngine.Debug.Log(node.ToString () + " at depth= " + depth + " has value = " + node.Value); 
                return new ValueAndAction(node.Value, node.Move);
            }
            int tempVal = int.MaxValue;
            ActionFunction tempAction = new ActionFunction();
            Agent.GenerateSuccessors(node);
            foreach (Node child in node.Children)
            {
                board.ExecuteFunction(child.Move);
                var retValAction = MaxValue(board, child, ref alpha, ref beta, depth + 1);
                board.ExecuteFunction(child.Undo);
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

        public static ActionFunction Search(Board board, Node root)
        {
            int alpha = int.MinValue;
            int beta = int.MaxValue;
            return MaxValue(board, root, ref alpha, ref beta, 0).action;
        }
    }
}
