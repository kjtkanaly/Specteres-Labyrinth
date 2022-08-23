using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenTwo : MonoBehaviour
{
    
    public class node {
        
        public node Parent;
        public node Left;
        public node Right;
        public List<node> ConnectedNode;
        
        // Debug Variables
        public int level;
        
        public node(node parentValue = null, int levelValue = 0) {
            
            Parent = parentValue;
            level  = levelValue;
            
            Left = null;
            Right = null;
        }
    }
	
	////////////////////////////////////
	/// Variables and Parameters
	
	private int levels = 2;
	
	private node Parent;
    
    public void Start() { 

        Parent = createTree(0, levels, null);
        
        printTree(Parent);
        
    }
    
    public node createTree(int currentLevel, int levels, node Parent) {
        
        node Node = new node(Parent, currentLevel);
        
        if (currentLevel < levels)
        {
            Node.Left = createTree(currentLevel + 1, levels, Node);
            Node.Right = createTree(currentLevel + 1, levels, Node);
        }
        else 
        {
            Node.Left = null;
            Node.Right = null;
        }
        
        return Node;
    }
    
    // Debug Function
    public void printTree(node Node)
    {
        Debug.Log(Node.level);
        
        if (Node.Left != null)
        {
            printTree(Node.Left);   
        }
        if (Node.Right != null)
        {
            printTree(Node.Right);
        }
    }
}