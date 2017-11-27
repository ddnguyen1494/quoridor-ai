using System;
using System.Collections.Generic;

namespace BreadthSearch
{
	//This is a tuple with 2 nodes
	class Path<T>
	{
		public Node<T> previous;
		public Node<T> current;

		public Path(Node<T> first)
		{
			previous = null;
			current = first;
		}

		public Path(Node<T> parent, Node<T> child)
		{
			previous = parent;
			current = child;
		}

		public override string ToString()
		{
			string result = "(";
			if (previous == null)
			{
				result += "NULL";
			}
			else
			{
				result += previous.contents.ToString();
			}
			result += ", " + current.contents.ToString() + ")";
			return result;
		}
	}

	//Holds the contents and a list of children
	class Node<T>
	{
		public T contents;
		public List<Node<T>> children = new List<Node<T>>();
	}

	//Implements a rooted graph as a list of nodes, supports functions to add items,
	//do a breadth first search, and print the tree's structure
	class Graph<T>
	{
		//The list of all nodes in the graph, the root is allNodes[0]
		public List<Node<T>> allNodes = new List<Node<T>>();

		//Clears the tree and sets the root of the graph to a node with the given T as its content
		//Returns the index of the added node (always 0 for the root)
		public int addRoot(T newItem)
		{
			allNodes.Clear();
			Node<T> root = new Node<T>();
			root.contents = newItem;
			allNodes.Add(root);
			return 0;
		}

		//Sets the next child of allNodes[parentNum] to a new node with newItem as its content
		//Returns the index of the added node
		public int add(int parentNum, T newItem)
		{
			Node<T> current = new Node<T>();
			current.contents = newItem;
			allNodes[parentNum].children.Add(current);
			allNodes.Add(current);
			return allNodes.Count - 1;
		}

		//Sets the next child of allNodes[parent] to allNodes[existing]
		public void addExisting(int parent, int existing)
		{
			allNodes[parent].children.Add(allNodes[existing]);
		}

		//Searches for the node with goal as its contents, if print is true, prints output messages with x, open, and closed
		//This search always starts at the root (allNodes[0])
		public Node<T>[] BFS(T goal, bool print)
		{
			List<Path<T>> goalPath = new List<Path<T>>();
			Path<T> startVertex = new Path<T>(allNodes[0]);
			if (print)
			{
				Console.WriteLine("Doing breadth first search, looking for item " + goal.ToString());
			}
			Node<T> x;
			Node<T> c;
			Path<T>[] open = new Path<T>[allNodes.Count];
			Path<T>[] closed = new Path<T>[allNodes.Count];
			int openTop = 0; //The index of the first item still in the open set, the current item to be examined
			int openEnd = 1; //The index of the most recently added item to the open set
			int closedTop = 0; //How many items have been added to closed
			open[0] = startVertex;
			if (print)
			{
				Console.WriteLine("Open: " + pathArrayToString(open, openTop, openEnd - 1));
				Console.WriteLine("Closed: " + pathArrayToString(closed, 0, closedTop));
			}
			while (openTop < allNodes.Count)
			{
				x = open[openTop].current;
				closed[closedTop] = open[openTop];

				if (x.contents.Equals(goal))
				{
					List<Node<T>> output = getPath(open[openTop], closed);
					if (print)
					{
						Console.WriteLine(x.contents);
						Console.WriteLine(goal.ToString() + " found.");
						Console.Write("Path: ");
						for (int i = 0; i < output.Count - 1; i++)
						{
							Console.Write(output[i].contents + ", ");
						}
						Console.WriteLine(output[output.Count - 1].contents);
					}

					return output.ToArray();
				}
				openTop++;
				closedTop++;
				int j = 0;
				for (int i = 0; i < x.children.Count; i++)
				{
					c = x.children[i];
					bool add = true;
					//what the inner 2 loops do is make sure that the child is not on the open or closed lists
					for (int k = 0; k < openEnd; k++)
					{
						if (open[k].current == c)
						{
							add = false;
							break;
						}
					}
					if (add)
					{
						for (int k = 0; k < closedTop; k++)
						{
							if (closed[k].current == c)
							{
								add = false;
								break;
							}
						}
						if (add)
						{
							open[openEnd + j] = new Path<T>(x, c);
							j++;
						}
					}
				}
				openEnd += j;
				if (print)
				{
					Console.WriteLine(x.contents);
					Console.WriteLine("Open: " + pathArrayToString(open, openTop, openEnd - 1));
					Console.WriteLine("Closed: " + pathArrayToString(closed, 0, closedTop - 1));
				}
			}
			openTop++;
			if (print)
			{
				Console.WriteLine("Open: " + pathArrayToString(open, openTop, openEnd));
				Console.WriteLine("Closed: " + pathArrayToString(closed, 0, closedTop - 1));
				Console.WriteLine("Did not find item " + goal.ToString());
			}
			return null;
		}

		//Returns a string with the contents of arr from index start to index end inclusive
		public static string arrayToString(object[] arr, int start, int end)
		{
			string result = "[";
			for (int a = start; a < end; a++)
			{
				result += arr[a].ToString() + ", ";
			}
			if (end < arr.Length && arr[end] != null)
			{
				result += arr[end].ToString();
			}
			return result + "]";
		}

		//Returns a string with current.contents of each entry in arr from index start to index end inclusive
		public static string pathArrayToString(Path<T>[] arr, int start, int end)
		{
			string result = "[";
			for (int a = start; a < end; a++)
			{
				result += arr[a].current.contents.ToString() + ", ";
			}
			if (end < arr.Length && arr[end] != null)
			{
				result += arr[end].current.contents.ToString();
			}
			return result + "]";
		}


		//starting at end, generates a path from the root to end
		public List<Node<T>> getPath(Path<T> end, Path<T>[] closed)
		{
			List<Node<T>> result = new List<Node<T>>();
			Path<T> currentPart = end;
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
		public void print()
		{
			foreach (Node<T> n in allNodes)
			{
				Console.Write(n.contents.ToString() + ":");
				foreach (Node<T> o in n.children)
				{
					Console.Write(o.contents.ToString() + ", ");
				}
				Console.WriteLine();
			}
		}
	}

	class Program
	{
		//Loads the graph from page 101 in the text book
		//The char is the letter used to identify each node
		static Graph<char> loadPage101()
		{
			Graph<char> result = new Graph<char>();
			int A;
			int B;
			int C;
			int D;
			int E;
			int F;
			int G;
			int H;
			int I;
			int J;
			int K;
			int L;
			int P;
			A = result.addRoot('A');
			B = result.add(A, 'B');
			C = result.add(A, 'C');
			D = result.add(A, 'D');
			E = result.add(B, 'E');
			F = result.add(B, 'F');
			G = result.add(C, 'G');
			H = result.add(C, 'H');
			I = result.add(D, 'I');
			J = result.add(D, 'J');
			K = result.add(E, 'K');
			L = result.add(E, 'L');
			result.addExisting(F, L);
			result.add(F, 'M');
			result.add(G, 'M');
			result.add(H, 'O');
			P = result.add(H, 'P');
			result.addExisting(I, P);
			result.add(I, 'Q');
			result.add(J, 'R');
			result.add(K, 'S');
			result.add(L, 'T');
			result.add(P, 'U');
			return result;
		}

		//Does a breadth first search on item U from the graph in page 101
		static int Main(string[] args)
		{
			Graph<char> page101 = loadPage101();
			page101.print();
			Console.Write("Press enter to see a breadth first search of 'U'.");
			Console.ReadLine();
			page101.BFS('U', true);
			Console.Write("Press enter to exit.");
			Console.ReadLine();
			return 0;
		}
	}
}