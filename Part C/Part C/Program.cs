using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;


class TwoThreeFourTree<T> where T : IComparable<T> {
    private int t;
    private Node<T> root;

    //Specialized node for the 2,3,4 tree
    public class Node<T> where T : IComparable<T> {
        public int NumKeys { get; set; } = 0; // Number of keys currently in node
        public T[] Keys { get; private set; } = new T[3]; // Array to hold the keys
        public Node<T>[] Children { get; private set; } = new Node<T>[4]; // Children references
        public bool IsLeaf => Children[0] == null; // Check if the node is a leaf

        // Method to print keys in this node
        public void PrintNode() {
            Console.Write("Keys in current node: ");
            for (int i = 0; i < NumKeys; i++) {
                Console.Write(Keys[i] + " ");
            }
        }
    }

    //Initializes an empty 2-3-4 tree. (2 marks)
    public TwoThreeFourTree() {
        this.t = 2;
        root = null;
    }

    //determine which path to take based on the key `k`
    private Node<T> moveDown(T k, Node<T> currentNode) {
        // After splitting, we need to determine which path to take
        /*Console.WriteLine("key 0 " + currentNode.Keys[0]);
        Console.WriteLine("key 1 " + currentNode.Keys[1]);
        Console.WriteLine("key 2 " + currentNode.Keys[2]);
        Console.WriteLine("num keys " + currentNode.NumKeys);*/

        // After splitting, we need to 
        // Iterate through all keys in the currentNode to find the correct path
        for (int i = 0; i <= currentNode.NumKeys; i++) {
            if (i == currentNode.NumKeys) {
                //Node goes down the right most path
                currentNode = currentNode.Children[i];
                //Console.WriteLine($"Moving to child at index {i}");

                break;
            } else if (k.CompareTo(currentNode.Keys[i]) > 0 && (i < currentNode.NumKeys)) {
                // If k is greater than the current key, check the next key (if any)
                //Console.WriteLine(k + " greater than " + currentNode.Keys[i]);
            } else {
                // Found the correct range for k, break the loop
                //Console.WriteLine(k + " found");

                // Move to the correct child
                currentNode = currentNode.Children[i];
                //Console.WriteLine($"Moving to child at index {i}");

                break;
            }
        }
        return currentNode;
    }


    //Returns true if key k is successfully inserted; false otherwise. (6 marks)
    public bool Insert(T k) {
        bool result = true;
        Console.WriteLine("\n-------------------------------------------\nKey to insert: " + k);
        //First item in the tree
        if (root == null) {
            root = new Node<T>();
            root.Keys[0] = k;
            root.NumKeys = 1;
        } else { //not the first item in the tree
            if (Search(k)) {
                //No duplicates
                Console.WriteLine("No duplicates allowed.");
                return false;
            }

            Node<T> parentNode = null;
            Node<T> currentNode = root;

            //Loop until the node is inserted or a duplicate is found
            while (currentNode != null) {
                //Console.WriteLine("Currently at node: ");
                //currentNode.PrintNode();
                //Console.ReadLine();
                if (currentNode.NumKeys == 3) {
                    //Console.WriteLine("\nSPlit node");
                    //Need to split a 3 node
                    SplitNode(parentNode, currentNode);

                    //After splitting, update our current position to the parent node
                    currentNode = parentNode;
                    if (currentNode == null) {
                        //we just split a root node, which created a new parent
                        currentNode = root;
                        //Console.WriteLine("root");
                    }

                    currentNode = moveDown(k, currentNode);                    
                } else {
                    if (currentNode.IsLeaf) {
                        //Console.WriteLine("insert into non full node");
                        InsertNonFullNode(currentNode, k);
                    }

                    parentNode = currentNode;
                    // Move down the tree
                    //Console.WriteLine("move down tree");
                    //currentNode = GetNextChild(currentNode, k); //
                    currentNode = moveDown(k, currentNode);
                }
            }
        }
        return result;
    }


    //Splits a 3 node
    private void SplitNode(Node<T> parent, Node<T> nodeToSplit) {
        T middleValue = nodeToSplit.Keys[1];

        Node<T> newLeft = new Node<T>();
        newLeft.Keys[0] = nodeToSplit.Keys[0];
        newLeft.NumKeys = 1;
        newLeft.Children[0] = nodeToSplit.Children[0];
        newLeft.Children[1] = nodeToSplit.Children[1];
        //newLeft.PrintNode();

        Node<T> newRight = new Node<T>();
        newRight.Keys[0] = nodeToSplit.Keys[2];
        newRight.NumKeys = 1;
        newRight.Children[0] = nodeToSplit.Children[2];
        newRight.Children[1] = nodeToSplit.Children[3];
        //newRight.PrintNode();

        //Case 1: the split happens at the root node, root has no parent, make a new one
        if (parent == null) {
            //Console.WriteLine("\nnode is the root");
            root = new Node<T>();
            root.Keys[0] = middleValue;
            root.NumKeys = 1;
            root.Children[0] = newLeft;
            root.Children[1] = newRight;
        } else {
            // Insert middleValue into parent
            /*Console.WriteLine("\nParent exists for split");
            Console.WriteLine("\nparent " + parent.Keys[0] + parent.Keys[1] + parent.Keys[2]);
            Console.WriteLine("\nmiddle value " + middleValue);
            Console.WriteLine("\nnew left ");
            newLeft.PrintNode();
            Console.WriteLine("\nnew right ");
            newRight.PrintNode();*/

            //Move up and add the middle value to the parent
            //Iterate through all keys
            for (int i = 0; i <= parent.NumKeys; i++) {
                //Console.WriteLine("\nComparing " + middleValue + " to " + parent.Keys[i]);
                if (middleValue.CompareTo(parent.Keys[i]) > 0 && (i < parent.NumKeys)) {
                    // If k is greater than the current key, check the next key (if any)
                    //Console.WriteLine(middleValue + " greater than " + parent.Keys[i]);
                } else {
                    // Found the correct range for middle value, break the loop, insert here
                    //Console.WriteLine(middleValue + " found, should be inserted at parent key index " + i);
                    parent.Keys[i] = middleValue;
                    parent.NumKeys++;

                    //connect the new left and right trees to the parent
                    parent.Children[i] = newLeft;
                    parent.Children[i + 1] = newRight;
                    break;
                }
            }
        }
    }


    //Insert a node into the tree at a node that is not full (doesn't already have 3 keys in it)
    private void InsertNonFullNode(Node<T> node, T item) {
        int i = node.NumKeys - 1;
        if (node.IsLeaf) {
            // Find the location to insert the new key
            while (i >= 0 && item.CompareTo(node.Keys[i]) < 0) {
                node.Keys[i + 1] = node.Keys[i];
                i--;
            }
            node.Keys[i + 1] = item;
            node.NumKeys++;
        } else {
            // If it's not a leaf, find the child which might get a new key
            while (i >= 0 && item.CompareTo(node.Keys[i]) < 0) {
                i--;
            }
            i++;
            Node<T> temp = node.Children[i];
            if (temp.NumKeys == 3) {
                SplitNode(node, temp);
                if (item.CompareTo(node.Keys[i]) > 0) {
                    i++;
                }
            }
            InsertNonFullNode(node.Children[i], item);
        }
    }


    //returns true if key k is successfully deleted; false otherwise. (10 marks)
    bool Delete(T k) {
        return false;
    }


    //returns true if key k is found; false otherwise(4 marks).
    public bool Search(T k) {
        //Console.WriteLine(root.Keys[0]);
        bool result = SearchPrivate(k, root);

        /*if (result == true) {
            Console.WriteLine("\nFound " + k);
        } else {
            Console.WriteLine("\n" + k + " not found");
        }*/

        return result;
    }


    //returns true if key k is found; false otherwise
    private bool SearchPrivate(T k, Node<T> node) {
        //Console.WriteLine("Looking for: " + k);
        //Console.Write("Current node: ");
        //node.PrintNode();

        Node<T> currentNode = node;
        bool result = false;
        bool searching = true;
        while (searching) {
            ////Determine if the key is located at the current node
            for (int i = 0; i < currentNode.NumKeys; i++) {
                //Console.Write("\n Comparing " + k + " against " + currentNode.Keys[i]);
                if (k.CompareTo(currentNode.Keys[i]) == 0) {
                    //Console.WriteLine("\n" + k + " found at current node at index " + i);
                    result = true;
                    return result;
                }
                //Console.Write(". no match");
            }

            //Did not find the key at the current node
            //Is this a leaf node
            if (currentNode.IsLeaf) {
                //Searched through the tree and did not find it
                result = false;
                break;
            } else {
                //Can still search more of the tree
                //Determine which path to take
                int index = determineIndex(k, currentNode);

                //Go down the path
                currentNode = currentNode.Children[index];
            }
        }
        return result;
    }


    //Compares the key to the current node and determines what index it should take
    private int determineIndex(T k, Node<T> currentNode) {
        int index = -1;
        //Loop through all of the keys
        for (int i = 0; i <= currentNode.NumKeys; i++) {
            if (i == currentNode.NumKeys) {
                //Node goes down the right most path
                index = currentNode.NumKeys;
                break;

            } else if (k.CompareTo(currentNode.Keys[i]) > 0) {
                // If k is greater than the current key, check the next key (if any)
                //Console.WriteLine(k + " greater than " + currentNode.Keys[i]);
            } else {
                // Found the correct path to take
                index = i;
                break;
            }
        }
        return index;
    }


    //which builds and returns the equivalent red-black tree. For this assignment, the red-black tree is represented as an instance of the class BSTforRBTree. The code for
    //BSTforRBTree is found on Blackboard under Assignments. Remember to remove the class Program
    //before using in your code. (8 marks)
    //public BSTforRBTree<T> Convert() { 
    //
    //} 

    //which prints out the keys of the 2-3-4 tree in order. (4 marks)
    // Print method that initiates the in-order traversal from the root
    public void Print() {
        PrintInOrder(root);
        Console.WriteLine(); // For formatting
    }

    // Recursive method to print the tree in-order
    private void PrintInOrder(Node<T> node) {
        if (node == null) return;

        // Since a node can have multiple keys and children, iterate through them
        // Note: node.NumKeys is the number of keys, and there's always one more child than the number of keys
        for (int i = 0; i < node.NumKeys; i++) {
            // Print left child of the key
            PrintInOrder(node.Children[i]);
            // Print the key itself
            Console.Write(node.Keys[i] + " ");
        }
        // After printing the last key, print the rightmost child
        PrintInOrder(node.Children[node.NumKeys]);
    }

    // Method to print the tree by levels using breadth first search
    public void PrintByLevels() {
        if (root == null) {
            Console.WriteLine("The tree is empty.");
            return;
        }

        Queue<Node<T>> queue = new Queue<Node<T>>();
        queue.Enqueue(root);

        while (queue.Count > 0) {
            int levelSize = queue.Count;
            Console.Write("Level: " + levelSize);
            while (levelSize > 0) {
                //Console.WriteLine(queue.Dequeue().Keys[0]);
                Node<T> current = queue.Dequeue();
                //current.PrintNode();

                // Print current node's items
                Console.Write("[");
                foreach (var item in current.Keys) {
                    Console.Write(item + " ");
                }

                // Add child nodes of the current node to the queue
                foreach (var child in current.Children) {
                    if (child != null) {
                        queue.Enqueue(child);
                    }
                }

                Console.Write("] "); // Separator between nodes at the same level
                levelSize--;
            }

            Console.WriteLine(); // Move to the next level
        }
    }
}

public class Program {
    static void Main(string[] args) {
        TwoThreeFourTree<char> myBTree = new TwoThreeFourTree<char>();

        myBTree.PrintByLevels();

        myBTree.Insert('a');        //Test inserting valid chars into the tree
        myBTree.PrintByLevels();

        myBTree.Insert('b');
        myBTree.PrintByLevels();

        myBTree.Insert('c');
        myBTree.PrintByLevels();

        myBTree.Insert('d');        //Test inserting into a full root node
        myBTree.PrintByLevels();
        myBTree.Insert('e');
        myBTree.PrintByLevels();
        myBTree.Insert('i');
        myBTree.PrintByLevels();
        myBTree.Insert('x');
        myBTree.PrintByLevels();
        myBTree.Insert('y');        //test spliting a leaf node
        myBTree.PrintByLevels();
        myBTree.Insert('z');
        myBTree.PrintByLevels();
        myBTree.Insert('f');
        myBTree.PrintByLevels();

        myBTree.Insert('f');    //test duplicate value

        Console.WriteLine("Inorder traversal: ");
        myBTree.Print();    //test inorder traversal


        Console.WriteLine("\nSearch: ");
        myBTree.Search('b');    //test existing values
        myBTree.Search('a');
        myBTree.Search('z');
        myBTree.Search('d');

        myBTree.Search('p');    //test non-existing value

        
    }
}

