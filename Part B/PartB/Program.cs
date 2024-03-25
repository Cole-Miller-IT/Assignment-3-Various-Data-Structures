using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BinomialHeap {
    public class BinomialNode<T> {
        public T Item { get; set; }
        public int Degree { get; set; }
        public BinomialNode<T> LeftMostChild { get; set; }
        public BinomialNode<T> RightSibling { get; set; }

        // Constructor

        public BinomialNode(T item) {
            Item = item;
            Degree = 0;
            LeftMostChild = null;
            RightSibling = null;
        }
    }

    //--------------------------------------------------------------------------------------

    // Common interface for all non-linear data structures

    public interface IContainer<T> {
        void MakeEmpty();  // Reset an instance to empty
        bool Empty();      // Test if an instance is empty
        int Size();        // Return the number of items in an instance
    }

    //--------------------------------------------------------------------------------------

    public interface IBinomialHeap<T> : IContainer<T> where T : IComparable {
        void Add(T item);               // Add an item to a binomial heap
        void Remove();                  // Remove the item with the highest priority
        T Front();                      // Return the item with the highest priority
        //void Merge(BinomialHeap<T> H);  // Merge H with the current binomial heap
    }

    //--------------------------------------------------------------------------------------

    // Binomial Heap
    // Implementation:  Leftmost-child, right-sibling

    public class BinomialHeap<T> : IBinomialHeap<T> where T : IComparable {
        private int size;                   // Size of the binomial heap
        private BinomialNode<T>[] B;        //Array that holds all of the smaller binomial heaps B[k]
        private BinomialNode<T> highest;    //Reference to the highest priority node in the heap

        // Contructor
        // Time complexity:  O(1)

        public BinomialHeap() {
            B = new BinomialNode<T>[] {};
            highest = null;
            size = 0;
        }

        ///////////////////NEw methods to implement///////////////////////////
        //Creates a new node and adds a new node to the end of the array B. DONE
        public void Add(T item) {
            //Create a new binomial node
            BinomialNode<T> newNode = new BinomialNode<T>(item);

            // If the heap (B) is empty or the new item is of higher priority than the current highest
            if (highest == null || item.CompareTo(highest.Item) > 0) {
                highest = newNode;
            }

            //Increase array size and add the new node to it at the end of the array
            addArrayItem(ref B, newNode);
            size++;
        }

        //Adds an existing node to the end of the array B
        private void nodeAdd(BinomialNode<T> existingNode) {
            //Increase array size and add the new node to it at the end of the array
            addArrayItem(ref B, existingNode);

            size++;
        }

        // Returns the item with the highest priority DONE
        public T Front() {
            return highest.Item;
        }


        // Removes the item with the highest priority from the binomial heap
        public void Remove() {
            //Remove the node with the highest priority first
            if (highest == null) {
                Console.WriteLine("\nBinomial heap is empty.");
            } 
            else {
                if (highest.LeftMostChild == null) {
                    //Case 1: highest is a single node (Bk = 0)
                    Console.WriteLine("\nsingle node to remove");
                } else {
                    //Case 2: highest node has 1 or more connected nodes (Bk >= 1)
                    //Split higest nodes children into smaller trees, then add them to the array B
                    //if highest is Bk = 3, then highest will be split into Bk = 0, Bk = 1, and Bk = 2. Those subtrees are added to B, then highest is set to null/removed.
                    void splitTree() {
                        BinomialNode<T> current = highest.LeftMostChild;
                        BinomialNode<T> nextCurrent = null;
                        do {
                            //Get a reference first before splitting the tree
                            nextCurrent = current.RightSibling; 

                            //Clip the right portion of the tree before adding to B
                            current.RightSibling = null;
                            nodeAdd(current);

                            Console.WriteLine("Split and added new tree: ");
                            PrintTree(current);

                            //Proceed to the next portion of the tree
                            current = nextCurrent;

                        } while (current != null);
                    }


                    splitTree();
                }

                //Remove the node
                Console.WriteLine("\nRemoved node: ");
                Console.WriteLine(highest.Item);
                void removeNode (BinomialNode<T> highest) {
                    for (int k = 0; k < B.Length; k++) {
                        if (B[k] == highest) {
                            //Found the node to remove
                            Console.WriteLine("Found node");
                            B[k] = null;
                            size--;
                        }
                    }
                }


                removeNode(highest);
            }

            Console.WriteLine("Binomial heap (B) after remove: ");
            Print();

            //Combine all remaining nodes into a compact binomial heap
            Coalesce();

            Console.WriteLine("Binomial heap (B) after coalesce: ");
            Print();
        }

        //Print the binomial heap
        public void Print() {
            Console.WriteLine($"\nBinomial Heap Size: {size}");
            for (int k = 0; k < B.Length; k++) {
                if (B[k] != null) {
                    Console.WriteLine($"\nBinomial Tree");
                    PrintTree(B[k]);
                }
            }
        }


        private void PrintTree(BinomialNode<T> node, string indent = "", bool last = true) {
            // Check if the node is null
            if (node == null) return;

            Console.Write(indent);
            if (last) {
                Console.Write("└─ ");
                indent += "  ";
            } else {
                Console.Write("├─ ");
                indent += "| ";
            }
            Console.WriteLine(node.Item);

            var child = node.LeftMostChild;
            while (child != null) {
                PrintTree(child, indent, child.RightSibling == null);
                child = child.RightSibling;
            }
        }



        //Combine all of the binomial trees so that the binomial heap is returned to having at most one instance of each Bk
        //Bk=0,     Bk=1,       Bk=2,       etc..
        //1 node,   2 nodes,    4 nodes,    etc..
        private void Coalesce() {
            for (int k = 0; k < B.Length; k++) {
                if (B[k] != null) { 
                    
                }
            }
        }

        //Increases the array size by 1 and adds an item to it
        private void addArrayItem(ref BinomialNode<T>[] B, BinomialNode<T> node) {
            Array.Resize(ref B, B.Length + 1);
            B[B.Length - 1] = node; // Adding the new item at the end
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////
        /// </summary>


        public void MakeEmpty() {
            Console.WriteLine("Not implemented");
        }

        // Empty
        // Returns true is the binomial heap is empty; false otherwise
        // Time complexity:  O(1)

        public bool Empty() {
            return size == 0;
        }

        // Size
        // Returns the number of items in the binomial heap
        // Time complexity:  O(1)

        public int Size() {
            return size;
        }
    }

    //--------------------------------------------------------------------------------------

    // Used by class BinomailHeap<T>
    // Implements IComparable and overrides ToString (from Object)

    public class PriorityClass : IComparable {
        private int priorityValue;
        private char letter;

        public PriorityClass(int priority, char letter) {
            this.letter = letter;
            priorityValue = priority;
        }

        public int CompareTo(System.Object obj) {
            PriorityClass other = (PriorityClass)obj;   // Explicit cast
            return priorityValue - other.priorityValue;  // High values have higher priority
        }

        public override string ToString() {
            return letter.ToString() + " with priority " + priorityValue;
        }
    }

    //--------------------------------------------------------------------------------------

    // Test for above classes

    public class Test {
        public static void Main(string[] args) {
            BinomialHeap<PriorityClass> BH = new BinomialHeap<PriorityClass>();

            BH.Add(new PriorityClass(50, (char)('a')));
            BH.Add(new PriorityClass(51, (char)('b')));
            BH.Add(new PriorityClass(53, (char)('c')));
            BH.Add(new PriorityClass(54, (char)('d')));
            BH.Add(new PriorityClass(55, (char)('e')));
            BH.Add(new PriorityClass(51, (char)('b')));

            BH.Print();

            Console.WriteLine("Front (highest priority): " + BH.Front());

            BH.Remove();
            //BH.Remove();
            //BH.Remove();
            //BH.Remove();
            //BH.Remove();
            //BH.Remove();

            //Console.ReadLine();
            /*for (i = 0; i < 20; i++) {
                BH.Add(new PriorityClass(r.Next(50), (char)('a')));
            }

            Console.WriteLine(BH.Size());
            BH.Degrees();

            while (!BH.Empty()) {
                Console.WriteLine(BH.Front().ToString());
                BH.Remove();
                BH.Degrees();
                Console.ReadLine();
            }
            Console.ReadLine();*/


        }
    }
}

