#undef DEBUG // #define DEBUG to print debug messages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphMaker
{
    /**
     * GraphNodes for the Graph class. Each node has a reference to the next node, or no reference
     * in the case of the last node in the sequence, it's next reference is "" (null)
     * 
     * This class will need to be modified to work with a graph that is greater than 1 dimension, it
     * currently supports a one dimensional graph - a single linked list.
     */ 
    public class GraphNode
    {
        public string name { get; set; } // the name of the node
        public string next { get; set; } // a reference to the next node

        /**
         * Constructor takes only a node name
         */
        public GraphNode(string name)
        {
            #if(DEBUG)
            Console.WriteLine("creating node({0})", name);
            #endif
            this.name = name; // set name
            this.next = String.Empty; // set next to ""
        }
        /**
         * This constructor takes a node name, and a reference to the next node
         */
        public GraphNode(string node_name, string next_name)
            : this(node_name) // calls default constructor first
        {
            #if (DEBUG)
            Console.WriteLine("creating node({0}, {1})", node_name, next_name);
            #endif
            if (next_name != null && next_name != String.Empty) // if the next_name provided is not null or ""
            {
                this.next = next_name; // set next name
            }
        }

        /**
         * Write the node information to a string "Name: name       Next: next_name"
         */ 
        override public string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("Name: ");
            str.Append(name);
            if (name.Length < 10) // short names need an extra tab to line up with long names
                str.Append("\t");
            str.Append("\tNext: ");
            str.Append(next); // if there's no next, this will append String.Empty
            return str.ToString();
        }
    }
    
    /**
     * The Graph class models a graph structure of connected nodes. At this stage, it's a single linked list
     */ 
    public class Graph
    {
        List<GraphNode> nodes; // the nodes of the graph, each node is connected by a reference to the next

        public Graph()
        {
            nodes = new List<GraphNode>(); // initalise the list
        }

        /**
         * Create a graph from an array of strings. Each string represents a station and except for the last,
         * will contain a reference to the next station. The graph is one-dimensional at this stage (a linked list).
         */ 
        public void CreateGraph(string[] strings) // I used an array here so I can iterate it, can probably use List though.
        {
            for (int i = 0; i < strings.Length; i++) // for each string
            {
                if (i != (strings.Length - 1)) // if it's not the last node in the list
                {
                    nodes.Add(new GraphNode(strings[i], strings[i + 1])); // create a new node with a reference to the next node
                }
                else
                {
                    nodes.Add(new GraphNode(strings[i])); // else create a new node with no next node
                }
            }
        }

        /**
         * Print the name and next for each node in the graph
         */ 
        public void PrintGraph()
        {
            foreach (GraphNode node in nodes)
            {
                Console.WriteLine("{0}", node.ToString());
            }
        }
    }

    /**
     * The main application class, runs a small example
     */ 
    public class GraphMaker
    {
        static void Main()
        {
            Graph graph = new Graph(); // initialise a new graph
            List<String> strings = new List<String>(); // initialise a new list of strings
            strings.Add("Mentone"); // add a bunch of stations to the list
            strings.Add("Cheltenham");
            strings.Add("Highett");
            strings.Add("Moorabbin");
            strings.Add("Patterson");
            strings.Add("Mckinnon");
            graph.CreateGraph(strings.ToArray<string>()); // create the graph with the list (array) of strings as input
            graph.PrintGraph(); // print the graph
            Console.ReadKey(); // wait for input
        } // END MAIN
    }
}