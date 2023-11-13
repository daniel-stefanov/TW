using System;
using System.Collections.Generic;

namespace EnergyProblems
{
    public class Node
    {
        public string Id { get; set; }
        public List<Node> NextNodes { get; set; } = new List<Node>();
        public double Energy { get; set; }
    }

    public class EnergyNetwork
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
    }

    public class Solution
    {
        // Implement logic here
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Graph 1
            EnergyNetwork graph1 = new EnergyNetwork
            {
                Nodes = new List<Node>
            {
                new Node { Id = "G1", Energy = 20 },
                new Node { Id = "I1" },
                new Node { Id = "C1" }
            }
            };
            graph1.Nodes[0].NextNodes.Add(graph1.Nodes[1]); // G1 -> I1
            graph1.Nodes[1].NextNodes.Add(graph1.Nodes[2]); // I1 -> C1

            // Graph 2
            EnergyNetwork graph2 = new EnergyNetwork
            {
                Nodes = new List<Node>
            {
                new Node { Id = "G1", Energy = 10 },
                new Node { Id = "G2", Energy = 30 },
                new Node { Id = "I1" },
                new Node { Id = "C1" },
                new Node { Id = "C2" }
            }
            };
            graph2.Nodes[0].NextNodes.Add(graph2.Nodes[2]); // G1 -> I1
            graph2.Nodes[1].NextNodes.Add(graph2.Nodes[2]); // G2 -> I1
            graph2.Nodes[2].NextNodes.Add(graph2.Nodes[3]); // I1 -> C1
            graph2.Nodes[2].NextNodes.Add(graph2.Nodes[4]); // I1 -> C2

            // Graph 3
            EnergyNetwork graph3 = new EnergyNetwork
            {
                Nodes = new List<Node>
            {
                new Node { Id = "G1", Energy = 50 },
                new Node { Id = "I1" },
                new Node { Id = "I2" },
                new Node { Id = "C1" }
            }
            };
            graph3.Nodes[0].NextNodes.Add(graph3.Nodes[1]); // G1 -> I1
            graph3.Nodes[1].NextNodes.Add(graph3.Nodes[2]); // I1 -> I2
            graph3.Nodes[2].NextNodes.Add(graph3.Nodes[3]); // I2 -> C1
        }
    }
}
