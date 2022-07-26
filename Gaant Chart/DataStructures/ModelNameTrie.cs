using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Structures
{
    public class ModelNameTrie
    {
        private class Node
        {
            public char? val;
            public long id;
            public List<Node> children;

            public Node(char? val, long id)
            {
                children = new List<Node>();
                this.val = val;
                this.id = id;
            }
        }

        private Node head;
        public int count;

        public ModelNameTrie()
        {
            head = new Node(null, -1);
            count = 0;
        }

        public void insert(String name, long id)
        {
            Node node = head;
            for(int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                Boolean found = false;

                foreach(Node child in node.children)
                {
                    if(child.val == c)
                    {
                        node = child;
                        found = true;
                        break;
                    }
                }

                if(!found)
                {
                    Node newNode = new Node(c, -1);
                    node.children.Add(newNode);
                    node = newNode;
                }
            }

            node.id = id;
            count += 1;
        }

        public List<(String, long)> suggest(String search)
        {
            List<(String, long)> result = new List<(String, long)>();
            Node node = head;
            for(int i = 0; i < search.Length; i++)
            {
                char c = search[i];
                Boolean found = false;
                foreach(Node child in node.children)
                {
                   if(child.val == c)
                    {
                        node = child;
                        found = true;
                        break;
                    }
                }

                if(!found)
                {
                    return result;
                }
            }

            String word = search;
            if (word.Length > 0) word = word.Remove(word.Length - 1);

            Stack<Node> stack = new Stack<Node>();
            Stack<Node> visitedLog = new Stack<Node>();

            stack.Push(node);
            visitedLog.Push(node);

            while(stack.Count() > 0)
            {
                node = stack.Pop();
                foreach(Node child in node.children)
                {
                    stack.Push(child);
                    visitedLog.Push(child);
                }

                word += node.val;
                
                if(node.id != -1)
                {
                    result.Add((word, node.id));
                    while(stack.Count() > 0 && stack.Peek() != visitedLog.Peek())
                    {
                        if (word.Length > 0) word = word.Remove(word.Length - 1);
                        visitedLog.Pop();
                    }
                }
            }

            return result;

        }


    }
}
