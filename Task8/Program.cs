using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Web;

namespace Task8
{
    class Child//Потомки дерева
    {
        public Node node;
        public Child next = null;

        public Child(Node node)
        {
            this.node = node;
        }

        public void addChild(Node node)//Добавить потомка к дереву
        {
            if (this.next == null)
                this.next = new Child(node);
            else
                this.next.addChild(node);
        }
        public int Count()//Подсчет кол-ва потомков
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

    class Node//Дерево (не бинарное)
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
    class Program
    {
        const string mistake = "Введено не корректное число";
        static int CheckNumber(out int chislo, int min, int max)
        {
            bool OK;
            do
            {
                OK = Int32.TryParse(Console.ReadLine(), out chislo) && chislo >= min && chislo <= max;
                if (!OK) Console.WriteLine(mistake);
            } while (!OK);
            return chislo;
        }
        static Random rng = new Random();
        static int[,] Generator()//Генератор матрицы
        {
            List<string> First = new List<string>();
            //Допустимое кол-во вершин от 3 до 11 включительно для быстрой проверки результата работы вручную
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
        static int[,] ToAdjacency(int[,] Incidence)//Перевод матрицы инциденций в матрицу смежости
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
        static void Show(int[,] matr)//Вывод матрицы
        {
            if (matr.GetLength(1) == 0)
                Console.WriteLine("Матрица состоит из " + matr.GetLength(0) + " вершин и не содержит ребер");
            else
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
        }
        static Node MakeTree(Node tree,int[,]matr,int maxI,List<int> nodes)
            //Создание дерева, где ветви - пустые подмножества
            //tree - дерево, к которому добавляются потомки
            //matr - матрица смежности
            //maxI - номер вершины с максимальным кол-вом исходящих из нее ребер
            //После первой итерации maxI - первая доступная для использования вершина
            //nodes - список доступных вершин для "вершины" дерева
        {
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                //newnodes - новый список доступных вершин для "вершины" дерева
                List<int> newnodes = new List<int>();
                if ((matr[maxI, i] == 1 || (i == maxI))&&nodes.Contains(i))//Создание "яруса" дерева
                    //В данный ярус входят все доступные для использования вершины, связанные ребрами с вершиной MaxI
                {
                    //Составление нового списка доступных вершин
                    //Все новые вершины должны как минимум входить в старый список, 
                    //а также между вершиной i и данной не должно быть ребра
                    for (int j = 0; j < matr.GetLength(0); j++)
                        if (matr[i, j] == 0 && nodes.Contains(j)&&i!=j)//
                            newnodes.Add(j);
                    //Добавление нового листа в дерево
                    tree.addChild(new Node(i+1, newnodes));
                    //Если новый лист доступных вершин не пуст, то построение из новой вершины дерева нового дерева
                    //Если новый лист доступных вершин пуст, значит больше вершин в данную ветвь добавить нельзя
                    if (newnodes.Count > 0)
                    {
                        //Так как дерево не бинарное, потомков может несколько, следовательно, новое дерево создается в самом последнем потомке
                        //Осуществляется переход к этому дереву и вызов этого же метода для создания дерева
                        Node newtree = tree.children.node;//
                        Child leaf = tree.children;
                        while (leaf!=null)
                        {
                            newtree = leaf.node;
                            leaf = leaf.next;
                        }
                        leaf = tree.children;
                        //newtree - новый созданный потомок
                        //matr - матрица смежности
                        //newnodes[0] - первая доступная для использования в опред. пустом множестве вершина
                        //newnodes - новый список доступных вершин
                        if (newtree!=null) newtree = MakeTree(newtree, matr, newnodes[0], newnodes);
                    }
                }
            }
            return tree;
        }
        static void GoThroughTree(Node tree,ref List<string> list,ref string temp)
        //Проход по дереву для заполнения list ветвями этого дерева
        //list - список всех пустых подмножеств
        //temp - временная строка, создаваемая постепенно и затем записывающаяся в список list
        //temp - строка, представляющая пустое подмножество, заполняющаяся по мере прохождения по дереву
        {
            if (tree.children != null)
            {
                temp +=" "+ tree.children.node.data;
                GoThroughTree(tree.children.node,ref list,ref temp);
                Child leaf = tree.children;
                while (leaf.next != null)
                {
                    leaf = leaf.next;
                    temp += " " + leaf.node.data;
                    GoThroughTree(leaf.node, ref list, ref temp);
                }
                if (temp!="")
                temp = temp.Substring(0, temp.LastIndexOf(' '));
            }
            else
            {
                list.Add(temp);
                temp = temp.Substring(0, temp.LastIndexOf(' '));
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Чтобы сгенерировать матрицу введите 1, чтобы ввести самому 2");
            List<int> V = new List<int> { 0 };
            int[,] adj;
            int option;
            int K;
            CheckNumber(out option, 1, 2);
            if (option == 1)
            {
                //Генерация матрицы
                int[,] matr = Generator();
                //Вывод матрицы
                Show(matr);
                Console.ReadLine();
                Console.WriteLine();
                //Перевод матрицы к матрице смежности
                adj = ToAdjacency(matr);
                //Show(adj);          
                //К - кол-во элементов в пустом подмножестве
                K = rng.Next(2, adj.GetLength(0));
            }
            else
            {
                Console.WriteLine("Введите кол-во вершин в графе: ");
                int vertex;
                CheckNumber(out vertex, 2, 100);
                Console.WriteLine("Введите кол-во ребер в графе: ");
                int edges;
                CheckNumber(out edges, 0, vertex*(vertex-1)/2);
                int[,] matr = new int[vertex, edges];
                for (int i = 0; i < edges; i++)
                {
                    int a, b;
                    Console.WriteLine("Введите номер 1 вершины, которую связывает ребро "+(i+1));
                    CheckNumber(out a, 1, vertex+1);
                    do
                    {
                        Console.WriteLine("Введите номер 2 вершины, которую связывает ребро " + (i + 1));
                        CheckNumber(out b, 1, vertex + 1);
                    } while (b == a);
                    matr[a-1, i] = 1;
                    matr[b-1, i] = 1;
                }
                Show(matr);
                adj = ToAdjacency(matr);
                Console.WriteLine("Введите кол-во вершин в пустом подмножестве: ");
                CheckNumber(out K, 1, vertex);
            }
            //count - кол-во ребер, исходящих из вершины
            int[] count = new int[adj.GetLength(0)];
            for (int i = 0; i < adj.GetLength(0); i++)
            {
                for (int j = 0; j < adj.GetLength(0); j++)
                {
                    if (adj[i, j] == 1) count[i]++;
                }
            }
            //maxV - макс. кол-во ребер, исходящих из вершины
            int maxV = count[0];
            //maxI - номер вершины с макс. кол-вом ребер
            int maxI = 0;
            //Поиск maxI, maxV
            for (int i = 1; i < count.Length; i++)
            {
                V.Add(i);
                if (maxV < count[i])
                {
                    maxV = count[i];
                    maxI = i;
                }
            }
            //V - лист доступных вершин
            Node tree = new Node(-1, V);
            //Создание дерева, в котором ветви - пустые подмножества
            tree = MakeTree(tree, adj, maxI, V);
            List<string> empties = new List<string>();
            //empties - список всех пустых подмножеств
            string t="";
            GoThroughTree(tree, ref empties, ref t);
            string Answer = "";
            //Answer - строка, представляющая пустое подмножество заданной длины K
            for (int i = 0; i < empties.Count&&Answer==""; i++)
            {
                string[] splitted = empties[i].Split(' ');
                if (splitted.Length == K+1)
                    for (int j=0;j< splitted.Length; j++)
                    {
                        Answer += " " + splitted[j];
                    }
            }
            if (Answer != "")
                Console.WriteLine("Пустое подмножество длины " + K + ":" + Answer);
            else
                Console.WriteLine("Пустое подмножество длины " + K + " отсутствует");
            Console.WriteLine("Все подмножества: ");
            Console.WriteLine(@"
Хотите ли вы вывести все пустые подграфы?
Нажмите 1 для вывода их, или любую другую для выхода из программы");
            string st = Console.ReadLine();
            if (st == "1")
            {
                //Подмножества имеют тенденцию повторяться в дереве, следовательно, нужно убрать все дубликаты и вывести только уникальные
                List<string> ToCheck = new List<string>();
                foreach (string str in empties)
                {
                    string stemp = str.Substring(1);
                    string[] temp = stemp.Split(' ');
                    int[] forsort = new int[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        Int32.TryParse(temp[i], out forsort[i]);
                    }
                    Array.Sort(forsort);//Сортировка элементов в множестве вершин пустого подграфа
                    stemp = String.Concat<int>(forsort);
                    if (!ToCheck.Contains(stemp))
                    {
                        ToCheck.Add(stemp);
                        Console.WriteLine(str);
                    }
                }
            }
            //Node tree1 = tree;
            Console.Read();
        }
    }
}
