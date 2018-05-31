using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurforgeBreadthFirstSearch {

	public int[] edgeTo;
	public int[] distTo;
	public int s;
	
	public void BFS(SurforgeAdjacencyList<int> G, int s) {
		var queue = new Queue<int>();
		queue.Enqueue(s);
		distTo[s] = 0;
		
		while (queue.Count != 0)
		{
			int v = queue.Dequeue();
			
			foreach (var w in G.FindNeighbours(v))
			{
				if (distTo[w] == -1)
				{
					queue.Enqueue(w);
					distTo[w] = distTo[v] + 1;
					edgeTo[w] = v;
				}
			}
		}
	}
	
	public void BreadthFirstSearch(SurforgeAdjacencyList<int> G, int s) {
		edgeTo = new int[G._vertexList.Count];
		distTo = new int[G._vertexList.Count];
		
		for (int i = 0; i < G._vertexList.Count; i++)
		{
			distTo[i] = -1;
			edgeTo[i] = -1;
		}
		
		this.s = s;
		
		BFS(G, s);
	}
}
