using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinomialHeap
{
    public class BinomialNode<T>
    {
        public T Item                        { get; set; }
        public int Degree                    { get; set; }
        public BinomialNode<T> LeftMostChild { get; set; }
        public BinomialNode<T> RightSibling  { get; set; }

        // Constructor

        public BinomialNode (T item)
        {
            Item = item;
            Degree = 0;
            LeftMostChild = null;
            RightSibling = null;
        }
    }

    //--------------------------------------------------------------------------------------

    // Common interface for all non-linear data structures

    public interface IContainer<T>
    {
        void MakeEmpty();  // Reset an instance to empty
        bool Empty();      // Test if an instance is empty
        int Size();        // Return the number of items in an instance
    }

    //--------------------------------------------------------------------------------------

    public interface IBinomialHeap<T> : IContainer<T> where T : IComparable
    {
        void Add(T item);               // Add an item to a binomial heap
        void Remove();                  // Remove the item with the highest priority
        T Front();                      // Return the item with the highest priority
        void Merge(BinomialHeap<T> H);  // Merge H with the current binomial heap
    }

    //--------------------------------------------------------------------------------------

    // Binomial Heap
    // Implementation:  Leftmost-child, right-sibling

    public class BinomialHeap<T> : IBinomialHeap<T> where T : IComparable
    {
        private BinomialNode<T> head;  // Head of the root list
        private int size;              // Size of the binomial heap

        // Contructor
        // Time complexity:  O(1)

        public BinomialHeap()
        {
            head = new BinomialNode<T>(default(T));   // Header node
            size = 0;
        }

        // Add
        // Inserts an item into the binomial heap
        // Time complexity:  O(log n)

        public void Add(T item)
        {
            BinomialHeap<T> H = new BinomialHeap<T>();

            H.head.RightSibling = new BinomialNode<T>(item);
            Merge(H);
            size++;
        }

        // FindHighest
        // Returns the preceding reference to the highest priority item
        // Assumption:  Root list contains at least one binomial tree
        // Time complexity:  O(log n)

        private BinomialNode<T> FindHighest()
        {
            BinomialNode<T> p = head, q = head;
            T highest;

            // Set highest to the first item
            highest = p.RightSibling.Item;

            // Traverse the root list
            // Find the root/item with the highest priority
            while (p.RightSibling.RightSibling != null)
            {
                p = p.RightSibling;
                if (p.RightSibling.Item.CompareTo(highest) > 0)
                {
                    highest = p.RightSibling.Item;
                    q = p;
                }
            }
            return q;
        }

        // Remove
        // Removes the item with the highest priority from the binomial heap
        // Time complexity:  O(log n)

        public void Remove()
        {
            if (!Empty())
            {
                BinomialHeap<T> H = new BinomialHeap<T>();
                BinomialNode<T> p, q;

                // Get the reference to the preceding node with the highest priority
                q = FindHighest();

                // Remove binomial tree p from root list
                p = q.RightSibling;
                q.RightSibling = q.RightSibling.RightSibling;

                // Add binomial subtrees of p in reverse order into H
                p = p.LeftMostChild;
                while (p != null)
                {
                    q = p.RightSibling;

                    // Splice p into H as the first binomial tree
                    p.RightSibling = H.head.RightSibling;
                    H.head.RightSibling = p;

                    p = q;
                }
                size--;
                Merge(H);
            }
        }

        // Degrees
        // Prints the degree for each binomial tree in the root list
        // Time complexity:  O(log n)

        public void Degrees()
        {
            BinomialNode<T> p = head.RightSibling;

            while (p != null)
            {
                Console.WriteLine(p.Degree);
                p = p.RightSibling;
            }
        }

        // Front
        // Returns the item with the highest priority
        // Time complexity:  O(log n)

        public T Front()
        {
            BinomialNode<T> p = head.RightSibling;

            if (!Empty())
            {
                // Get the reference to the preceding node with the highest priority
                p = FindHighest();
                return p.RightSibling.Item;
            }
            else
                return default(T);
        }

        // Union
        // Takes the union (without consolidation) of the given binomial heap H and the current heap
        // Note:  The given heap H is set to empty
        // Time complexity:  O(log n)

        private void Union(BinomialHeap<T> H)
        {
            BinomialNode<T> prev, curr, Hcurr, temp;

            prev = head;                   // Current binomial heap
            curr = head.RightSibling;
            Hcurr = H.head.RightSibling;   // Given binomial heap

            while (curr != null && Hcurr != null)
            {
                if (curr.Degree <= Hcurr.Degree)
                {
                    // Move prev and curr along the current root list
                    curr = curr.RightSibling;
                    prev = prev.RightSibling;
                }
                else
                {
                    temp = Hcurr;
                    Hcurr = Hcurr.RightSibling;

                    // Splice temp between prev and curr
                    temp.RightSibling = curr;
                    prev = prev.RightSibling = temp;
                }
            }

            // Append remainder of the given heap
            if (curr == null)
                prev.RightSibling = Hcurr;

            size += H.size;

            H.MakeEmpty();
        }

        // BinomialLink
        // Makes child the leftmost child of root
        // Time complexity:  O(1)

        private void BinomialLink(BinomialNode<T> child, BinomialNode<T> root)
        {
            child.RightSibling = root.LeftMostChild;
            root.LeftMostChild = child;
            root.Degree++;
        }

        // Consolidate
        // Consolidates (combines) binomial trees with the same degree
        // Cases from pp 408 and 412 of CLR (Edition 1)
        // Time complexity:  O(log n)

        private void Consolidate()
        {
            BinomialNode<T> prev, curr, next;

            if (Empty())
                return;

            prev = head;
            curr = head.RightSibling;
            next = curr.RightSibling;

            while (next != null)
            {
                // Cases 1 and 2 
                if ((curr.Degree != next.Degree) ||
                    (next.RightSibling != null && next.RightSibling.Degree == curr.Degree))
                {
                    prev = curr;
                    curr = next;
                }
                // Case 3
                else if (curr.Item.CompareTo(next.Item) >= 0)
                {
                    curr.RightSibling = next.RightSibling;
                    BinomialLink(next, curr);
                }
                // Case 4
                else
                {
                    prev.RightSibling = next;
                    BinomialLink(curr, next);
                    curr = next;
                }
                next = curr.RightSibling;
            }
        }

        // Merge
        // Merges the given binomial heap into the current heap
        // Time complexity:  O(log n)

        public void Merge(BinomialHeap<T> H)
        {
            Union(H);
            Consolidate();
        }

        // MakeEmpty
        // Creates an empty binomial heap
        // Time complexity:  O(1)

        public void MakeEmpty()
        {
            head.RightSibling = null;
            size = 0;
        }

        // Empty
        // Returns true is the binomial heap is empty; false otherwise
        // Time complexity:  O(1)

        public bool Empty()
        {
            return size == 0;
        }

        // Size
        // Returns the number of items in the binomial heap
        // Time complexity:  O(1)

        public int Size()
        {
            return size;
        }
    }

    //--------------------------------------------------------------------------------------

    // Used by class BinomailHeap<T>
    // Implements IComparable and overrides ToString (from Object)

    public class PriorityClass : IComparable
    {
        private int priorityValue;
        private char letter;

        public PriorityClass(int priority, char letter)
        {
            this.letter = letter;
            priorityValue = priority;
        }

        public int CompareTo(Object obj)
        {
            PriorityClass other = (PriorityClass)obj;   // Explicit cast
            return priorityValue - other.priorityValue;  // High values have higher priority
        }

        public override string ToString()
        {
            return letter.ToString() + " with priority " + priorityValue;
        }
    }

    //--------------------------------------------------------------------------------------

    // Test for above classes

    public class Test
    {
        public static void Main(string[] args)
        {
            int i;
            Random r = new Random();

            BinomialHeap<PriorityClass> BH = new BinomialHeap<PriorityClass>();

            for (i=0; i<20; i++)
            { 
                BH.Add(new PriorityClass(r.Next(50), (char)('a')));
            }

            Console.WriteLine(BH.Size());
            BH.Degrees();

            while (!BH.Empty())
            {
                Console.WriteLine(BH.Front().ToString());
                BH.Remove();
                BH.Degrees();
                Console.ReadLine();
            }  
            Console.ReadLine();
        }
    }
}

