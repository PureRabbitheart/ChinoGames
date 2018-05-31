﻿using System.Collections;
using System.Collections.Generic;
using System;

public class SurforgeAdjacencyList<K>
{
	public List<List<K>> _vertexList = new List<List<K>>();
	private Dictionary<K, List<K>> _vertexDict = new Dictionary<K, List<K>>();
	
	public SurforgeAdjacencyList(K rootVertexKey)
	{
		AddVertex(rootVertexKey);
	}
	
	private List<K> AddVertex(K key)
	{
		List<K> vertex = new List<K>();
		_vertexList.Add(vertex);
		_vertexDict.Add(key, vertex);
		
		return vertex; 
	}
	
	public void AddEdge(K startKey, K endKey)
	{   
		//no check for startKey endKey equal

		List<K> startVertex = _vertexDict.ContainsKey(startKey) ? _vertexDict[startKey] : null;
		List<K> endVertex = _vertexDict.ContainsKey(endKey) ? _vertexDict[endKey] : null;
		
		if (startVertex == null)
			startVertex = AddVertex(startKey);
		
		if (endVertex == null)
			endVertex = AddVertex(endKey);


		if (!startVertex.Contains(endKey)) {
			startVertex.Add(endKey);
		}
		if (!endVertex.Contains(startKey)) {
			endVertex.Add(startKey);
		}
	}
	
	public void RemoveVertex(K key)
	{
		List<K> vertex = _vertexDict[key];
		
		//First remove the edges / adjacency entries
		int vertexNumAdjacent = vertex.Count;
		for (int i = 0; i < vertexNumAdjacent; i++)
		{  
			K neighbourVertexKey = vertex[i];
			RemoveEdge(key, neighbourVertexKey);
		}
		
		//Lastly remove the vertex / adj. list
		_vertexList.Remove(vertex);
		_vertexDict.Remove(key);
	}
	
	public void RemoveEdge(K startKey, K endKey)
	{
		((List<K>)_vertexDict[startKey]).Remove(endKey);
		((List<K>)_vertexDict[endKey]).Remove(startKey);
	}
	
	public bool Contains(K key)
	{
		return _vertexDict.ContainsKey(key);
	}
	
	public int VertexDegree(K key)
	{
		return _vertexDict[key].Count;
	}

	public List<K> FindNeighbours(K key)
	{
		return _vertexDict[key];
	}





}