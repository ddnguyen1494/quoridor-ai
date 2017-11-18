using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace Assets.Scripts
{
    internal class Node
    {
        public BitArray State { get; set; }
        public List<Node> Children { get; set; }
        public int Value { get; set; }

        public Node(BitArray state, int value)
        {
            State = new BitArray(state);
            Value = value;
            Children = new List<Node>();
        }
    }

}
