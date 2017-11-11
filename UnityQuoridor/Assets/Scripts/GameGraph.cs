using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace Assets.Scripts
{
    //This is a tuple with 2 nodes
    class Path
    {
        public Node previous;
        public Node current;

        public Path(Node first)
        {
            previous = null;
            current = first;
        }

        public Path(Node parent, Node child)
        {
            previous = parent;
            current = child;
        }

        //public override string ToString()
        //{
        //    string result = "(";
        //    if (previous == null)
        //    {
        //        result += "NULL";
        //    }
        //    else
        //    {
        //        result += previous.contents.ToString();
        //    }
        //    result += ", " + current.contents.ToString() + ")";
        //    return result;
        //}
    }
    internal class Node
    {
        public BitArray State { get; set; }
        public List<Node> Children { get; }
        public int Depth { get; set; }
        public int Utility { get; set; }

        public Node(BitArray state, int depth)
        {
            State = new BitArray(state);
            Depth = depth;
            Children = new List<Node>();
        }
    }

    //Implements a rooted graph as a list of nodes, supports functions to add items,
    //do a breadth first search, and print the tree's structure
    class Graph
    {
        public List<Node> allNodes; //The list of all nodes in the graph, the root is allNodes[0]
        private int max_depth;
        public Graph(BitArray rootState, int depth)
        {
            allNodes = new List<Node>();
            AddRoot(rootState);
            max_depth = depth;
            GenerateSuccessors(0 , 1);
        }
        
        //Clears the tree and sets the root of the graph to a node with the given T as its content
        //Returns the index of the added node (always 0 for the root)
        public int AddRoot(BitArray state)
        {
            allNodes.Clear();
            Node root = new Node(state, 0);
            allNodes.Add(root);
            return 0;
        }

        //Recursive functions to generate the game tree to the specified depth
        public void GenerateSuccessors(int parentNum, int depth)
        {
            if (allNodes[parentNum].Depth == max_depth)
                return;
            List<BitArray> possbileMoves = AgentHelper.GetAllPossibleMoves(allNodes[parentNum].State);
            foreach (BitArray state in possbileMoves)
            {
                int index = Add(parentNum, state, depth);
                GenerateSuccessors(index, depth + 1);
            }
        }

        //Sets the next child of allNodes[parentNum] to a new node with newItem as its content
        //Returns the index of the added node
        public int Add(int parentNum, BitArray state, int depth)
        {
            Node current = new Node(state, depth);
            allNodes[parentNum].Children.Add(current);
            allNodes.Add(current);
            return allNodes.Count - 1;
        }

        //Sets the next child of allNodes[parent] to allNodes[existing]
        public void AddExisting(int parent, int existing)
        {
            allNodes[parent].Children.Add(allNodes[existing]);
        }

        ////Searches for the node with goal as its contents, if print is true, prints output messages with x, open, and closed
        ////This search always starts at the root (allNodes[0])
        //public Node[] BFS(T goal, bool print)
        //{
        //    List<Path> goalPath = new List<Path>();
        //    Path startVertex = new Path(allNodes[0]);
        //    if (print)
        //    {
        //        Console.WriteLine("Doing breadth first search, looking for item " + goal.ToString());
        //    }
        //    Node x;
        //    Node c;
        //    Path[] open = new Path[allNodes.Count];
        //    Path[] closed = new Path[allNodes.Count];
        //    int openTop = 0; //The index of the first item still in the open set, the current item to be examined
        //    int openEnd = 1; //The index of the most recently added item to the open set
        //    int closedTop = 0; //How many items have been added to closed
        //    open[0] = startVertex;
        //    if (print)
        //    {
        //        Console.WriteLine("Open: " + pathArrayToString(open, openTop, openEnd - 1));
        //        Console.WriteLine("Closed: " + pathArrayToString(closed, 0, closedTop));
        //    }
        //    while (openTop < allNodes.Count)
        //    {
        //        x = open[openTop].current;
        //        closed[closedTop] = open[openTop];

        //        if (x.contents.Equals(goal))
        //        {
        //            List<Node> output = getPath(open[openTop], closed);
        //            if (print)
        //            {
        //                Console.WriteLine(x.contents);
        //                Console.WriteLine(goal.ToString() + " found.");
        //                Console.Write("Path: ");
        //                for (int i = 0; i < output.Count - 1; i++)
        //                {
        //                    Console.Write(output[i].contents + ", ");
        //                }
        //                Console.WriteLine(output[output.Count - 1].contents);
        //            }

        //            return output.ToArray();
        //        }
        //        openTop++;
        //        closedTop++;
        //        int j = 0;
        //        for (int i = 0; i < x.children.Count; i++)
        //        {
        //            c = x.children[i];
        //            bool add = true;
        //            //what the inner 2 loops do is make sure that the child is not on the open or closed lists
        //            for (int k = 0; k < openEnd; k++)
        //            {
        //                if (open[k].current == c)
        //                {
        //                    add = false;
        //                    break;
        //                }
        //            }
        //            if (add)
        //            {
        //                for (int k = 0; k < closedTop; k++)
        //                {
        //                    if (closed[k].current == c)
        //                    {
        //                        add = false;
        //                        break;
        //                    }
        //                }
        //                if (add)
        //                {
        //                    open[openEnd + j] = new Path(x, c);
        //                    j++;
        //                }
        //            }
        //        }
        //        openEnd += j;
        //        if (print)
        //        {
        //            Console.WriteLine(x.contents);
        //            Console.WriteLine("Open: " + pathArrayToString(open, openTop, openEnd - 1));
        //            Console.WriteLine("Closed: " + pathArrayToString(closed, 0, closedTop - 1));
        //        }
        //    }
        //    openTop++;
        //    if (print)
        //    {
        //        Console.WriteLine("Open: " + pathArrayToString(open, openTop, openEnd));
        //        Console.WriteLine("Closed: " + pathArrayToString(closed, 0, closedTop - 1));
        //        Console.WriteLine("Did not find item " + goal.ToString());
        //    }
        //    return null;
        //}

        ////Returns a string with the contents of arr from index start to index end inclusive
        //public static string arrayToString(object[] arr, int start, int end)
        //{
        //    string result = "[";
        //    for (int a = start; a < end; a++)
        //    {
        //        result += arr[a].ToString() + ", ";
        //    }
        //    if (end < arr.Length && arr[end] != null)
        //    {
        //        result += arr[end].ToString();
        //    }
        //    return result + "]";
        //}

        ////Returns a string with current.contents of each entry in arr from index start to index end inclusive
        //public static string pathArrayToString(Path[] arr, int start, int end)
        //{
        //    string result = "[";
        //    for (int a = start; a < end; a++)
        //    {
        //        result += arr[a].current.contents.ToString() + ", ";
        //    }
        //    if (end < arr.Length && arr[end] != null)
        //    {
        //        result += arr[end].current.contents.ToString();
        //    }
        //    return result + "]";
        //}


        //starting at end, generates a path from the root to end
        public List<Node> GetPath(Path end, Path[] closed)
        {
            List<Node> result = new List<Node>();
            Path currentPart = end;
            while (currentPart.current != null)
            {
                result.Add(currentPart.current);
                if (currentPart.previous == null)
                {
                    break;
                }
                bool found = false;
                for (int i = 0; i < closed.Length; i++)
                {
                    if (closed[i].current == currentPart.previous)
                    {
                        currentPart = closed[i];
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    break;
                }
            }
            result.Reverse();
            return result;
        }

        //Prints the list of nodes and children
        //Format- node contents: child 1 contents, child 2 contents, ... child n contents,
        //public void print()
        //{
        //    foreach (Node n in allNodes)
        //    {
        //        Console.Write(n.contents.ToString() + ":");
        //        foreach (Node o in n.children)
        //        {
        //            Console.Write(o.contents.ToString() + ", ");
        //        }
        //        Console.WriteLine();
        //    }
        //}
    }

}
