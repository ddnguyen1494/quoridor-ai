using System;
using System.Collections.Generic;

namespace BreadthSearch
{
	class Node<T>
	{
		public T contents;
		public Node<T> previous;
		public List<Node<T>> children = new List<Node<T>>();
	};

	class Graph<T>
	{
		//The list of all nodes in the graph, the root is allNodes[0]
		public List<Node<T>> allNodes = new List<Node<T>>();

		//Clears the tree and sets the root of the graph to a node with the given T as its content
		public int addRoot(T newItem)
		{
			allNodes.Clear();
			Node<T> root = new Node<T>();
			root.contents = newItem;
			allNodes.Add(root);
			return 0;
		}

		//Sets the next child of allNodes[parentNum] to a new node with newItem as its content
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
		public List<Node<T>> BFS(T goal, bool print)
		{
			foreach (Node<T> n in allNodes)
			{
				n.previous = null; //prevents the result of a previous search from interfering
			}

			if (print)
			{
				Console.WriteLine("Doing breadth first search, looking for item " + goal.ToString());
			}

			Node<T> x;
			Node<T> c;
			Node<T>[] open = new Node<T>[allNodes.Count];
			Node<T>[] closed = new Node<T>[allNodes.Count];
			int openTop = 0;
			int openEnd = 1;
			int closedTop = 0;
			open[0] = allNodes[0];
			while (openTop < allNodes.Count)
			{
				x = open[openTop];
				openTop++;
				if (x.contents.Equals(goal))
				{
					var output = getPath(x);
					if (print)
					{
						Console.WriteLine(x.contents);
						Console.Write("Open: [");
						for (int a = openTop; a < openEnd - 1; a++)
						{
							Console.Write(open[a].contents.ToString() + ", ");
						}
						if (openEnd < open.Length - 1)
						{
							Console.Write(open[openEnd - 1].contents.ToString());
						}
						Console.Write("]\nClosed: [");
						for (int b = 0; b < closedTop - 1; b++)
						{
							Console.Write(closed[b].contents.ToString() + ", ");
						}
						if (closedTop > 0)
						{
							Console.Write(closed[closedTop - 1].contents.ToString());
						}
						Console.WriteLine("]\n");
						Console.WriteLine(goal.ToString() + " found.");
						Console.Write("Path: ");
						for (int i = 0; i < output.Count - 1; i++)
						{
							Console.Write(output[i].contents + ", ");
						}
						Console.WriteLine(output[output.Count - 1].contents);
					}
					
					return output;
				}
				int j = 0;
				for (int i = 0; i < x.children.Count; i++)
				{
					c = x.children[i];
					if (c.previous == null)
					{
						c.previous = x;
						open[openEnd + j] = c;
						j++;
					}
				}
				openEnd += j;
				closed[closedTop] = x;
				closedTop++;
				if (print)
				{
					Console.WriteLine(x.contents);
					Console.Write("Open: [");
					for (int a = openTop; a < openEnd - 1; a++)
					{
						Console.Write(open[a].contents.ToString() + ", ");
					}
					if (openTop <= open.Length - 1)
					{
						Console.Write(open[openEnd - 1].contents.ToString());
					}
					Console.Write("]\nClosed: [");
					for (int b = 0; b < closedTop - 1; b++)
					{
						Console.Write(closed[b].contents.ToString() + ", ");
					}
					if (closedTop > 0)
					{
						Console.Write(closed[closedTop - 1].contents.ToString());
					}
					Console.WriteLine("]\n");
				}
			}
			openEnd++;
			if (print)
			{
				Console.WriteLine("Did not find item " + goal.ToString());
			}
			return null;
		}

		//starting at end, generates a path from the root to end
		public List<Node<T>> getPath(Node<T> end)
		{
			List<Node<T>> result = new List<Node<T>>();
			Node<T> n = end;
			while (n != null)
			{
				result.Add(n);
				n = n.previous;
			}
			result.Reverse();
			return result;
		}


		//Prints the list of nodes and children
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
	};

	class Program
	{
		//Loads the graph from page 101 in the text book
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
			page101.BFS('U', true);
			Console.Write("Press enter to exit.");
			Console.ReadLine();
			return 0;
		}
	}
}

