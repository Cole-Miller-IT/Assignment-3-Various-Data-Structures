using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSTforRBTree
{
    public enum Color { RED, BLACK };       // Colors of the red-black tree

    public interface ISearchable<T>
    {
        void Add(T item, Color rb);
        void Print();
    }

    //-------------------------------------------------------------------------

    // Implementation:  BSTforRBTree

    class BSTforRBTree<T> : ISearchable<T> where T : IComparable<T> {

        // Common generic node class for a BSTforRBTree

        public class Node //Changed to public for Convert()
        {
            // Read/write properties

            public T Item;
            public Color RB;
            public Node Left;
            public Node Right;

            public Node(T item, Color rb)
            {
                Item = item;
                RB = rb;
                Left = Right = null;
            }
        }

        private Node root;

        public BSTforRBTree()
        {
            root = null;    // Empty BSTforRBTree
        }

        // Add 
        // Insert an item into a BSTforRBTRee
        // Duplicate items are not inserted
        // Worst case time complexity:  O(log n) 
        // since the maximum depth of a red-black tree is O(log n)

        public void Add(T item, Color rb)
        {
            Node curr;
            bool inserted = false;

            if (root == null)
                root = new Node(item, rb);   // Create a root
            else 
            {
                curr = root;
                while (!inserted)
                {
                    if (item.CompareTo(curr.Item) < 0)
                    {
                        if (curr.Left == null)              // Empty spot
                        {
                            curr.Left = new Node(item, rb);
                            inserted = true;
                        }
                        else
                            curr = curr.Left;               // Move left
                    }
                    else
                        if (item.CompareTo(curr.Item) > 0)
                        {
                            if (curr.Right == null)         // Empty spot
                            {
                                curr.Right = new Node(item, rb);
                                inserted = true;
                            }
                            else
                                curr = curr.Right;          // Move right
                        }
                        else
                            inserted = true;                // Already inserted
                }
            }
        }

        public void Print()
        {
            Print(root, 0);                // Call private, recursive Print
            Console.WriteLine();
        }

        // Print
        // Inorder traversal of the BSTforRBTree
        // Time complexity:  O(n)

        private void Print(Node node, int k)
        {
            string s;
            string t = new string(' ', k);

            if (node != null)
            {
                Print(node.Right, k+4);
                s = node.RB == Color.RED ? "R" : "B" ;
                Console.WriteLine(t + node.Item.ToString() + s);
                Print(node.Left, k+4);
            }
        }
    }

    //-----------------------------------------------------------------------------

    /*public class Program
    {
        static void Main(string[] args)
        {
            Random randomValue = new Random();       // Random number
            Color c;

            BSTforRBTree<int> B = new BSTforRBTree<int>();
            for (int i = 0; i < 20; i++)
            {
                c = i%2 == 0 ? Color.RED : Color.BLACK; 
                B.Add(randomValue.Next(90) + 10, c); // Add random integers with alternating colours
            }
            B.Print();                               // In order

            Console.ReadLine();
        }
    }*/
}
