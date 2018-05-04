using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Web;

namespace Task8
{
    class Child
    {
        public Node node;
        public Child next = null;

        public Child(Node node)
        {
            this.node = node;
        }

        public void addChild(Node node)
        {
            if (this.next == null)
                this.next = new Child(node);
            else
                this.next.addChild(node);
        }
        public int Count()
        {
            int k = 0;
            Child temp = this;
            while (temp != null)
            {
                temp = temp.next;
                k++;
            }
            return k;
        }
    }

    class Node
    {
        public int data;
        public Child children = null;
        public List<int> available = new List<int>();

        public Node(int data,List<int> nodes)
        {
            this.data = data;
            available = nodes;
        }

        public void addChild(Node node)
        {
            if (this.children == null)
                this.children = new Child(node);
            else
                this.children.addChild(node);
        }
    }
    class Edge
    {
        public int First, Second;
        public Edge(int a=-1,int b=-1)
        {
            First = a;
            Second = b;
        }

    }
    class Program
    {
        static Random rng = new Random();
        static int[,] Generator()
        {
            List<string> First = new List<string>();
            int V = rng.Next(3, 12);
            int E = rng.Next(0, V * (V - 1) / 2 + 1);
            int[,] matr = new int[V, E];
            for (int i = 0; i < E; i++)
            {
                int a, b;
                do
                {
                    a = rng.Next(0, V);
                    do
                    {
                        b = rng.Next(0, V);
                    } while (b == a);
                } while (First.Contains(a+" "+b)|| First.Contains(b + " " + a));
                First.Add(a + " " + b);
                matr[a, i] = 1;
                matr[b, i] = 1;
            }
            return matr;
        }
        static int[,] ToAdjacency(int[,] Incidence)
        {
            int[,] Adjacent = new int[Incidence.GetLength(0), Incidence.GetLength(0)];
            for (int i=0; i < Incidence.GetLength(0); i++)
            {
                for (int j=0; j < Incidence.GetLength(1); j++)
                {
                    if (Incidence[i, j] == 1)
                    {
                        for (int k= 0; k < Incidence.GetLength(0); k++)
                        {
                            if (k != i && Incidence[k, j] == 1)
                            {
                                Adjacent[i, k] = 1;
                            }
                        }
                    }
                }
            }
            return Adjacent;
        }
        static void Show(int[,] matr)
        {
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                for (int j = 0; j < matr.GetLength(1); j++)
                {
                    Console.Write(matr[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        static Node MakeTree(Node tree,int[,]matr,int maxI,List<int> nodes)
        {
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                List<int> newnodes = new List<int>();
                if ((matr[maxI, i] == 1 || (i == maxI))&&nodes.Contains(i))
                {
                    for (int j = 0; j < matr.GetLength(0); j++)
                        if (matr[i, j] == 0 && nodes.Contains(j)&&i!=j)//
                            newnodes.Add(j);

                    tree.addChild(new Node(i, newnodes));
                    if (newnodes.Count > 0)
                    {
                        Node newtree = tree.children.node;//
                        Child leaf = tree.children;
                        while (leaf!=null)
                        {
                            newtree = leaf.node;
                            leaf = leaf.next;
                        }
                        leaf = tree.children;
                        if (newtree!=null) newtree = MakeTree(newtree, matr, newnodes[0], newnodes);
                       /* while (newtree != null)
                        {
                            //if (newtree.children==null||newtree.children.Count()<newnodes.Count)
                            newtree = MakeTree(newtree, matr, newnodes[0], newnodes);
                            if (leaf.next == null)
                                break;
                            else
                            {
                                leaf = leaf.next;
                                newtree = leaf.node;
                            }

                        }*/
                    }
                }
            }
            return tree;
        }
        static void Main(string[] args)
        {
            int[,] matr = Generator();
            Show(matr);
            Console.ReadLine();
            /*Node tree = new Node("");
            tree.addChild(new Node("2"));
            tree.addChild(new Node("6"));
            Node tree1 = tree.children.node;
            tree.children.node.addChild(new Node("1"));
            tree1.addChild(new Node("5"));*/
            Console.WriteLine();
            int[,] adj=ToAdjacency(matr);
            Show(adj);
            //
            int[] count = new int[matr.GetLength(0)];
            for (int i=0;i< matr.GetLength(0); i++)
            {
                for (int j=0;j< matr.GetLength(0); j++)
                {
                    if (adj[i, j] == 1) count[i]++;
                }
            }
            int maxV=count[0];
            int maxI = 0;
            List<int> V = new List<int> { 0 };
            for (int i = 1; i < count.Length; i++)
            {
                V.Add(i);
                if (maxV < count[i])
                {
                    maxV = count[i];
                    maxI = i;
                }
            }
            int[,] matrix = new int[7,7];
            matrix[0, 1] = 1;
            matrix[0, 2] = 1;
            matrix[1, 0] = 1;
            matrix[1, 2] = 1;
            matrix[2, 3] = 1;
            matrix[2, 0] = 1;
            matrix[2, 1] = 1;
            matrix[3, 2] = 1;
            matrix[3, 5] = 1;
            matrix[4, 6] = 1;
            matrix[4, 5] = 1;
            matrix[5, 3] = 1;
            matrix[5, 4] = 1;
            matrix[5, 6] = 1;
            matrix[6, 4] = 1;
            matrix[6, 5] = 1;
            Node tree = new Node(-1, V);
            tree = MakeTree(tree, matrix, 0, V);
            //
            Node tree1 = tree;
            Console.Read();
        }
    }
}
