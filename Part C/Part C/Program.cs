using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public bool Delete(T k) {
        bool result;
        //Check if the key exists
        if (Search(k) == false) {
            //key doesn't exist
            result = false;
        } else {
            //Key exists so we can delete it
            result = DeletePrivate(k, root, null);
        }

        return result;
    }

    //Removes an existing key from the B-Tree
    private bool DeletePrivate(T k, Node<T> currentNode, Node<T> parentNode) {
        //Returns the index where k is located for the keys[] array of the passed node if a match is found, otherwise -1;
        int keyAtCurrentNode(T k, Node<T> currentNode) {
            int index;
            ////Determine if the key is located at the current node
            for (int i = 0; i < currentNode.NumKeys; i++) {
                //Console.Write("\n Comparing " + k + " against " + currentNode.Keys[i]);
                if (k.CompareTo(currentNode.Keys[i]) == 0) {
                    //Console.WriteLine("\n" + k + " found at current node at index " + i);
                    index = i;
                    return index;
                }
                //Console.Write(". no match");
            }
            //Did not find k at the current node
            index = -1;
            return index;
        }


        Node<T> parent = parentNode;
        Node<T> current = currentNode;

        //Debugging
        Console.WriteLine("\nNext deletePrivate() iteration:");
        Console.Write("Parent: ");
        if (parent == null) {
            Console.WriteLine("parent null");
        } else {
            parent.PrintNode();
        }
        Console.Write("\nCurrent: ");
        current.PrintNode();
        Console.WriteLine("");


        //Check if the key is at the current node
        int keyIndex = keyAtCurrentNode(k, current);
        if (keyIndex == -1) {
            Console.WriteLine("\n" + k + " not at the current node");

            //Determine if the next node we will go down to has enough keys, new current must have AT LEAST t keys
            int nextIndex = determineIndex(k, current);
            Node<T> nextCurrent = current.Children[nextIndex];

            int minKeys = t;
            if (nextCurrent.NumKeys >= minKeys) {
                Console.WriteLine("Child has enough keys");

                //descend
                DeletePrivate(k, nextCurrent, current);
            } 
            else {
                Console.WriteLine("Child needs more keys");

                // borrows a key from a sibling (if possible) or 
                if (1 == 2) {

                }
                //merges with an adjacent (node is just before or after the current node) node where the t-1 keys of each node plus one
                //from the parent node yield a single node with 2t - 1 keys. Note: The parent node is guaranteed to have an extra key(Why?)
                else {
                    Console.WriteLine("current will take index " + nextIndex + " to get to next current");
                    Node<T> mergedNode = new Node<T> ();

                    //Merge two sibling nodes together that have t - 1 keys and a key from the current node. Then update the 2,3,4-tree references.
                    (Node<T>, Node<T>) Merge (bool right, Node<T> sibling, Node<T> next, Node<T> current, Node<T> parent) {
                        //Determine which key to take from current (could be 1, 2, or 3 potential keys for a 2,3,4-tree)
                        //I don't like how unclear this is but I can't find a better way right now
                        int currentIndex;
                        int pathTaken = determineIndex(next.Keys[0], current);
                        Console.WriteLine("Path/index taken to get to next from current " + pathTaken);
                        //If we took the left most key from current
                        if (pathTaken == 0) {
                            currentIndex = 0;
                        }
                        //If we took the right most key from current
                        else if (pathTaken == ((2 * t) -1)) {
                            currentIndex = t;
                        } 
                        else {
                            //Determine where the sibling is (it changes what key we will take from current)
                            //Current Node:
                            //index        0       1        2
                            //Keys:        a       b        c
                            //            /    |        |    \    
                            //           (-----)        (------)
                            //                (----------)     
                            //Paths:     0     1         2       3
                            //e.g. If siblingPath(0) < nextpath(1), then the sibling index will have the correct index value we want. 
                            //     a would be in the middle, which is correct.
                            int siblingPathTaken = determineIndex(sibling.Keys[0], current);
                            if (siblingPathTaken < pathTaken) {
                                //the sibling index will be the key we take
                                currentIndex = siblingPathTaken;
                            } 
                            else {
                                //else the next index will be the key we take
                                currentIndex = pathTaken;
                            }
                        }
          

                        Node<T> mergedNode = new Node<T>();
                        mergedNode.NumKeys = ((2 * t) - 1);
                        //Create new nodes
                        if (right == true) {
                            //Right sibling is being merged
                            mergedNode.Keys[0] = next.Keys[0];
                            mergedNode.Keys[1] = current.Keys[currentIndex];
                            mergedNode.Keys[2] = sibling.Keys[0];

                            mergedNode.Children[0] = next.Children[0];
                            mergedNode.Children[1] = next.Children[1];
                            mergedNode.Children[2] = sibling.Children[0];
                            mergedNode.Children[3] = sibling.Children[1];
                        } 
                        else {
                            //Left sibling is being merged
                            mergedNode.Keys[0] = sibling.Keys[0];
                            mergedNode.Keys[1] = current.Keys[currentIndex];
                            mergedNode.Keys[2] = next.Keys[0];

                            mergedNode.Children[0] = sibling.Children[0];
                            mergedNode.Children[1] = sibling.Children[1];
                            mergedNode.Children[2] = next.Children[0];
                            mergedNode.Children[3] = next.Children[1];
                        }

                        Console.WriteLine("New merged node and children: ");
                        mergedNode.PrintNode();
                        Console.WriteLine("");
                        if (mergedNode.Children[0] != null) {
                            mergedNode.Children[0].PrintNode();
                            Console.WriteLine("");
                        } else {
                            Console.WriteLine("merged node has no children");
                        }
                        if (mergedNode.Children[1] != null) {
                            mergedNode.Children[1].PrintNode();
                            Console.WriteLine("");
                        }
                        if (mergedNode.Children[2] != null) {
                            mergedNode.Children[2].PrintNode();
                            Console.WriteLine("");
                        }
                        if (mergedNode.Children[3] != null) {
                            mergedNode.Children[3].PrintNode();
                            Console.WriteLine("");
                        }

                        //If the parent only has one key then the entire root node will be merged into the new merged node and the height will decrease by 1
                        if (parent == null && current.NumKeys == 1) {
                            current = null;
                        } 
                        else {
                            // Assuming currentIndex is the position of the key to remove.
                            // Shift keys to the left to fill the gap created by the removed key.
                            for (int i = currentIndex; i < current.NumKeys - 1; i++) {
                                current.Keys[i] = current.Keys[i + 1];
                            }
                            // If the node is not a leaf, shift the child pointers as well.
                            if (!current.IsLeaf) {
                                // Note: There is one more child than there are keys, so loop one extra time.
                                for (int i = currentIndex + 1; i < current.NumKeys; i++) {
                                    current.Children[i] = current.Children[i + 1];
                                }
                            }

                            // Set the last key and last child (if not a leaf) to default values to clean up references.
                            current.Keys[current.NumKeys - 1] = default(T);
                            if (!current.IsLeaf) {
                                current.Children[current.NumKeys] = null; // Assuming null is appropriate for your child links.
                            }

                            // Decrease the number of keys in the node.
                            current.NumKeys--;

                            current.Children[currentIndex] = mergedNode;
                            Console.WriteLine("New current after shift: ");
                            current.PrintNode();
                        } 

                        return (current, mergedNode);
                    }


                    //Check left adjacent
                    if (nextIndex > 0) {
                        //Console.WriteLine("Left adjacent sibling exists");
                        //check if index - 1 is mergable
                        if (current.Children[nextIndex - 1].NumKeys == (t - 1)) {
                            //Console.WriteLine("Left adjacent sibling can merge");

                            //Merge nodes together
                            (current, mergedNode) = Merge(false, current.Children[nextIndex - 1], nextCurrent, current, parent);
                        }
                    }
                    //Check right adjacent
                    if (nextIndex < ((2 * t) - 1)) {
                        //Console.WriteLine("Right adjacent sibling exists");
                        //check if index + 1 is mergable
                        if (current.Children[nextIndex + 1].NumKeys == (t - 1)) {
                            //Console.WriteLine("right adjacent sibling can merge");

                            //Merge nodes together
                            (current, mergedNode) = Merge(true, current.Children[nextIndex + 1], nextCurrent, current, parent);
                        }
                    }


                    if (current == null) {
                        //Root was merged
                        current = mergedNode;
                        root = current; //Update Root
                    }

                    Console.Write("Current after merge: ");
                    current.PrintNode();
                    Console.WriteLine("");
                    PrintByLevels();

                    DeletePrivate(k, current, parent);
                }


            }
        } 
        else { 
            Console.WriteLine("key: \'" + k +"\' at the current node at Keys[" + keyIndex + "]");

            //Check if the key is a leaf node
            if (current.IsLeaf) {
                Console.WriteLine("At leaf");
                //We can simply remove it from keys[]
                current.Keys[keyIndex] = default(T);

            } else {
                Console.WriteLine("Not at leaf, time for some recursive stuff");
                //if the child node q that precedes k has t keys, then recursively delete the 
                //predecessor k’ of k in the subtree rooted at q and replace k with k’.
                    

                //if the child node r that succeeds k has t keys, then recursively delete the
                //successor k’ of k in the subtree rooted at r and replace k with k’.
                    

                //otherwise, merge q and r with k from the parent to yield a single node s
                //with 2t - 1 keys.Recursively delete k from the subtree s.

            }
        }    

        

        return false;
    }

    //returns true if key k is found; false otherwise(4 marks). DONE
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

        Console.WriteLine("\n-----------------------------------------");
        Console.WriteLine("Testing delete()");
        //myBTree.Delete('d');
        //myBTree.Delete('d');
        //myBTree.Delete('b');
        myBTree.Delete('a');

        Console.WriteLine("\nAfter delete()");
        myBTree.PrintByLevels();
        myBTree.Print();

    }
}

