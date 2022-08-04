using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Structures
{
    public class Trie
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

        public Trie()
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

        public (List<long>, double) findSimilar(string word)
        {
            double highestScore = Double.NegativeInfinity;
            List<long> highestIds = new List<long>();

            Stack<Node> stack = new Stack<Node>();
            Stack<Node> visitLog = new Stack<Node>();
            Stack<int[]> scores = new Stack<int[]>();

            stack.Push(head);
            while(stack.Count > 0)
            {
                Node node = stack.Pop();
                int[] prevScore = (scores.Count > 0) ? scores.Peek() : null;
                int[] score = computeSimilarityScore(word, node.val, prevScore);
                scores.Push(score);


                if(node.id != -1)
                {
                    if(highestScore < score[word.Length])
                    {
                        highestScore = score[word.Length];
                        highestIds.Clear();
                        highestIds.Add(node.id);
                    }
                    else if(highestScore == score[word.Length])
                    {
                        highestIds.Add(node.id);
                    }

                }

                if(node.children.Count == 0)
                {
                    while(stack.Count() > 0 && stack.Peek() != visitLog.Peek())
                    {
                        visitLog.Pop();
                        scores.Pop();
                    }
                }

                foreach(Node child in node.children)
                {
                    stack.Push(child);
                    visitLog.Push(child);
                }
            }

            return (highestIds, highestScore);
        }
        private int[] computeSimilarityScore(string word, char? c, int[] prevScore)
        {
            int MISMATCH = -3;
            int GAP = -2;
            int MATCH = 2;

            int[] score = new int[word.Length + 1];

            if(!c.HasValue)
            {
                score[0] = 0;
                
                for(int i = 1; i < word.Length + 1; i++)
                {
                    score[i] = score[i - 1] + GAP;
                }
            }
            else
            {
                score[0] = prevScore[0] - 2;

                for(int i = 1; i < word.Length + 1; i++)
                {
                    int match = (word[i - 1] != c.Value) ? int.MinValue : prevScore[i - 1] + MATCH;
                    int mismatch = prevScore[i - 1] + MISMATCH;
                    int gapI = score[i - 1] + GAP;
                    int gapJ = prevScore[i] + GAP;

                    score[i] = multiElmMax(match, mismatch, gapI, gapJ);
                }
            }

            return score;
        }
        private int multiElmMax(int a, int b, int c, int d)
        {
            return Math.Max(
                Math.Max(a, b),
                Math.Max(c, d)
              );
        }


    }
}
