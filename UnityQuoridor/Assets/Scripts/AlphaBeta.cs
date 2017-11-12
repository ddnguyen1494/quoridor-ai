using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace Assets.Scripts
{
    class AlphaBeta
    {
        private static Node MaxValue(Node node, ref int alpha, ref int beta, int depth)
        {
            if (Agent.CutOff(node, depth))
            {
                node.Value = Agent.Evaluate(node.State);
                return node;
            }
            int tempVal = int.MinValue;
            BitArray tempState = null;
            Agent.GenerateSuccessors(node);
            foreach(Node child in node.Children)
            {
                var retNode = MinValue(child, ref alpha, ref beta, depth + 1);
                if(retNode.Value > tempVal)
                {
                    tempVal = retNode.Value;
                    tempState = retNode.State;
                    if (retNode.Value >= beta)
                        return retNode;
                    alpha = Math.Max(alpha, retNode.Value);
                }
            }
            return new Node(tempState, tempVal);

        }

        private static Node MinValue(Node node, ref int alpha, ref int beta, int depth)
        {
            if (Agent.CutOff(node, depth))
            {
                node.Value = Agent.Evaluate(node.State);
                return node;
            }
            int tempVal = int.MaxValue;
            BitArray tempState = null;
            Agent.GenerateSuccessors(node);
            foreach (Node child in node.Children)
            {
                var retNode = MaxValue(child, ref alpha, ref beta, depth + 1);
                if (retNode.Value < tempVal)
                {
                    tempVal = retNode.Value;
                    tempState = retNode.State;
                    if (retNode.Value <= alpha)
                        return retNode;
                    beta = Math.Min(beta, retNode.Value);
                }
            }
            return new Node(tempState, tempVal);
        }

        public static BitArray Search(Node node)
        {
            int alpha = int.MinValue;
            int beta = int.MaxValue;
            Node nextNode = MinValue(node, ref alpha, ref beta, 0);
            return nextNode.State;
        }
    }
}
