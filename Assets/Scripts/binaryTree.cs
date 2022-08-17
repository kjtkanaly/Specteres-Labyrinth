public class Solution {
    
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
    
    public string LongestCommonPrefix(string[] strs) {
     
        int levels = 2;
        
        node Parent = createTree(0, levels, null);
        
        printTree(Parent);
        
        return "";
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
        Console.WriteLine(Node.level);
        
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