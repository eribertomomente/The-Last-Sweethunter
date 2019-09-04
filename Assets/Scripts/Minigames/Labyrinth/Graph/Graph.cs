
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Graph {

	// holds all edgeds going out from a node
	private Dictionary<Node, List<Edge>> data;

	public Graph() {
		data = new Dictionary<Node, List<Edge>>();
	}

	public void AddEdge(Edge e) {
		AddNode (e.from);
		AddNode (e.to);
		if (!data[e.from].Contains(e))
			data [e.from].Add (e);
	}

	// used only by AddEdge 
	public void AddNode(Node n) {
        if (!data.ContainsKey(n))
            data.Add(n, new List<Edge>());
	}

	// returns the list of edged exiting from a node
	public Edge[] getConnections(Node n) {
		if (!data.ContainsKey (n)) return new Edge[0];
		return data [n].ToArray ();
	}

	public Node[] getNodes() {
		return data.Keys.ToArray ();
	}

    // true if n1 is connected with n2 with a single edge
    public bool areConnected(Node n1, Node n2)
    {
        Edge[] ee = this.getConnections(n1);
        foreach ( Edge e in ee)
        {
            if (e.to.Equals(n2) || e.from.Equals(n2))
            {
                return true;
            }
        }
        // veridico anche nel caso contrario dato che non ho un grafo orientato
        ee = this.getConnections(n2);
        foreach (Edge e in ee)
        {
            if (e.to.Equals(n1) || e.from.Equals(n1))
            {
                return true;
            }
        }
        return false;
    }

}