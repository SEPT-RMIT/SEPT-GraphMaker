﻿#undef DEBUG // #define DEBUG to print debug messages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphMaker
{
    public class Station
    {
        public string Name { get; set; }
        public Dictionary<int, string> Times { get; set; }
        //public GoogleGeoCodeResponse Geo { get; set; }
        public Station(string name) { this.Name = name;  this.Times = new Dictionary<int, string>(); }
    }

    public class GraphNode
    {
        public string name { get; set; } // the name of the node
        public List<string> lines; // what lines is this station part of (ie a train line and a bus route)
        public Dictionary<string, Dictionary<string, Station>> adjacency_list; // <line, <next/prev, station>>

        public GraphNode(Station station, string line, Station next, Station prev)
        {
            name = station.Name; // set the name of the node to the name of the station
            lines = new List<string>(); // this list is kinda redundant, but you can see what lines exist in the graph if you access it
            lines.Add(line); // add this line/route to the list of lines/routes that this station is part of
            adjacency_list = new Dictionary<string, Dictionary<string, Station>>(); // <line, <next/prev, station>>
            Dictionary<string, Station> adj = new Dictionary<string, Station>(); // <next = Station, prev = Station>
            adj.Add("next", next); // "next" = Station next
            adj.Add("prev", prev); // "prev" = Station prev
            adjacency_list.Add(line, adj);
        }
    }
    
    public class Graph
    {
        List<GraphNode> graph; // the nodes of the graph, each node is connected by a reference to the next

        public Graph()
        {
            graph = new List<GraphNode>(); // initalise the list
        }

        public void AddLineToGraph(List<Station> line, string line_name)
        {
            for (int i = 0; i < line.Count; i++) // for each string
            {
                bool node_exists = false;
                Station prev = null, next = null;
                if (i != 0) // not the first node, a previous station exists
                {
                    prev = line[i - 1]; // prev = the previous station in the list
                }
                if (i != (line.Count - 1)) // not the last node, a next station exists
                {
                    next = line[i + 1]; // next = the next station in the list
                }

                // if the node already exists in the graph, we want to add this line/route
                // and these next/prev references to it's adjacency_list rather than
                // add a duplicate node

                foreach(GraphNode n in graph)
                {
                    if (n.name == line[i].Name) // if a node exists with this name
                    {
                        Dictionary<string, Station> adj = new Dictionary<string, Station>(); // new sub-Dict
                        adj.Add("next", next); // set next reference
                        adj.Add("prev", prev); // set prev reference
                        n.adjacency_list.Add(line_name, adj); // add sub-Dict to main-Dict
                        node_exists = true; // set a flag
                    }
                }

                // ADD A NEW NODE, if we ammended an existing node, we skip this part
                if (node_exists == false)
                {
                    GraphNode node = new GraphNode(line[i], line_name, next, prev); // create a new node
                    graph.Add(node); // add it to the graph
                }
            }
        }

        // order of print not necessarily correct, ammended nodes get rearranged in the list
        public void PrintGraph(string line)
        {
            foreach (GraphNode node in graph)
            {
                if (node.adjacency_list.ContainsKey(line)) // if the node is on the specified line/route
                {
                    Console.Write("{0} ", node.name); // print it's name
                    Dictionary<string, Station> adj = node.adjacency_list[line]; // grab the next/prev dict
                    foreach (KeyValuePair<string, Station> pair in adj)
                    {
                        
                        Console.Write("{0} ", pair.Key); // print "next" / "prev"
                        if (pair.Value != null) // first/last station have no reference for prev/next respectively
                        {
                            Console.Write("{0} ", pair.Value.Name); // print the next/prev
                        }
                        else
                        {
                            Console.Write("NONE "); // else print NONE
                        }

                        if (pair.Key == "prev") // if this is the "prev" reference
                        {
                            Console.WriteLine(); // give us a newline
                        }
                    }
                }
            }
        }
    }

    public class GraphMaker
    {
        static void Main()
        {
            Graph graph = new Graph();
            List<Station> FrankstonLine = new List<Station>();
            List<Station> DandenongLine = new List<Station>();

            FrankstonLine.Add(new Station("Highett"));
            FrankstonLine.Add(new Station("Moorabbin"));
            FrankstonLine.Add(new Station("Patterson"));
            FrankstonLine.Add(new Station("Bentleigh"));
            FrankstonLine.Add(new Station("McKinnon"));
            FrankstonLine.Add(new Station("Ormond"));
            FrankstonLine.Add(new Station("Glenhuntly"));
            FrankstonLine.Add(new Station("Caulfield"));

            graph.AddLineToGraph(FrankstonLine, "Frankston Line");

            DandenongLine.Add(new Station("Huntingdale"));
            DandenongLine.Add(new Station("Oakleigh"));
            DandenongLine.Add(new Station("Hughesdale"));
            DandenongLine.Add(new Station("Murrumbeena"));
            DandenongLine.Add(new Station("Carnegie"));
            DandenongLine.Add(new Station("Caulfield"));

            graph.AddLineToGraph(DandenongLine, "Dandenong Line");

            graph.PrintGraph("Frankston Line");
            Console.WriteLine();
            graph.PrintGraph("Dandenong Line");

            Console.ReadKey();
        }
    }
}