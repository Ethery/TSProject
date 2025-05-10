using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
	[SerializeField] private int NumberOfDefaultPlayers = 2;

	[SerializeField] private StoneObject StonePrefab;

	[SerializeField] private Transform[] LinePositions;
	[SerializeField] private Transform[] PoolPositions;
	
	[SerializeField] public SelectorWheel StoneSelector;
	[SerializeField] public BoastActionSelector BoastSelector;

	[NonSerialized] public bool IsWatching;
	
	#region Boasting
	
	[NonSerialized] public bool IsBoasting = false;
	[NonSerialized] public Game.EStone? BoastingStone;

	[NonSerialized] public List<Game.EStone> BoastedStones;
	
	#endregion
	
	private Game m_Game;
	
	private Dictionary<int, StoneObject> m_LineStoneInstances;
	private Dictionary<int, StoneObject> m_PoolStoneInstances;

	private List<StoneObject> m_UnusedStoneInstances;
	
	public Game Game => m_Game;

	private void OnDrawGizmos()
	{
		foreach (Transform linePosition in LinePositions)
		{
			Gizmos.DrawMesh(StonePrefab.GetComponentInChildren<MeshFilter>().sharedMesh, linePosition.position);
		}

		foreach (Transform linePosition in PoolPositions)
		{
			Gizmos.DrawMesh(StonePrefab.GetComponentInChildren<MeshFilter>().sharedMesh, linePosition.position);
		}
	}

	public void Start()
	{
		m_Game = new Game(NumberOfDefaultPlayers);
		m_LineStoneInstances = new Dictionary<int, StoneObject>(7);
		for(int i = 0; i < m_Game.Line.Length; i++)
		{
			if (m_Game.GetStoneInLineAt(i) != Game.EStone.None)
			{
				Transform linePos = LinePositions[i];
				StoneObject stoneInstance = Instantiate(StonePrefab, linePos);
				stoneInstance.Stone = m_Game.Line[i];
				m_LineStoneInstances.Add(i, stoneInstance);
			}
		}

		m_PoolStoneInstances = new Dictionary<int, StoneObject>(7);
		for(int i = 0; i < m_Game.Pool.Length; i++)
		{
			if (m_Game.GetStoneInPoolAt(i) != Game.EStone.None)
			{
				Transform poolPos = PoolPositions[i];
				StoneObject stoneInstance = Instantiate(StonePrefab, poolPos);
				stoneInstance.Stone = m_Game.Pool[i];
				stoneInstance.IsFromThePool = true;
				m_PoolStoneInstances.Add(i, stoneInstance);
			}
		}

		m_UnusedStoneInstances = new List<StoneObject>(4);
		for(int i = 0; i < 4; i++)
		{
			m_UnusedStoneInstances.Add(Instantiate(StonePrefab, transform));
			m_UnusedStoneInstances[i].gameObject.SetActive(false);
		}
		StoneSelector.gameObject.SetActive(false);
		BoastSelector.gameObject.SetActive(false);
	}

	public void Update()
	{
		if (m_Game != null)
		{
			for (int i = 0; i < m_Game.Line.Length; i++)
			{
				if (m_Game.GetStoneInLineAt(i) == Game.EStone.None && m_LineStoneInstances.ContainsKey(i))
				{
					m_UnusedStoneInstances.Add(m_LineStoneInstances[i]);
					m_LineStoneInstances[i].gameObject.SetActive(false);
					m_LineStoneInstances[i].transform.SetParent(transform, false);
					m_LineStoneInstances.Remove(i);
				}
				else if (m_Game.GetStoneInLineAt(i) != Game.EStone.None && !m_LineStoneInstances.ContainsKey(i))
				{
					m_LineStoneInstances.Add(i, m_UnusedStoneInstances[0]);
					m_UnusedStoneInstances.RemoveAt(0);
					m_LineStoneInstances[i].gameObject.SetActive(true);
					m_LineStoneInstances[i].transform.SetParent(LinePositions[i], false);
					m_LineStoneInstances[i].Stone = m_Game.Line[i];
				}
				else if (m_Game.GetStoneInLineAt(i) != Game.EStone.None && m_LineStoneInstances.ContainsKey(i))
				{
					m_LineStoneInstances[i].Stone = m_Game.Line[i];
					m_LineStoneInstances[i].IsFromThePool = false;
				}
			}

			for (int i = 0; i < m_Game.Pool.Length; i++)
			{
				if (m_Game.GetStoneInPoolAt(i) == Game.EStone.None && m_PoolStoneInstances.ContainsKey(i))
				{
					m_UnusedStoneInstances.Add(m_PoolStoneInstances[i]);
					m_PoolStoneInstances[i].gameObject.SetActive(false);
					m_PoolStoneInstances[i].transform.SetParent(transform, false);
					m_PoolStoneInstances.Remove(i);
				}
				else if (m_Game.GetStoneInPoolAt(i) != Game.EStone.None && !m_PoolStoneInstances.ContainsKey(i))
				{
					m_PoolStoneInstances.Add(i, m_UnusedStoneInstances[0]);
					m_UnusedStoneInstances.RemoveAt(0);
					m_PoolStoneInstances[i].gameObject.SetActive(true);
					m_PoolStoneInstances[i].transform.SetParent(PoolPositions[i], false);
					m_PoolStoneInstances[i].Stone = m_Game.Pool[i];
				}
				else if(m_Game.GetStoneInPoolAt(i) != Game.EStone.None && m_PoolStoneInstances.ContainsKey(i))
				{
					m_PoolStoneInstances[i].Stone = m_Game.Pool[i];
					m_PoolStoneInstances[i].IsFromThePool = true;
				}
			}
		}
	}

	#region Boasting Action
	
	public void ConfrontBoast(bool isFirstTime = true)
	{
		m_Game.GoToNextPlayer(false);
		BoastSelector.SelectAction(isFirstTime);
		IsBoasting = true;
	}
	
	public void AskBoast()
	{
		BoastedStones = new List<Game.EStone>();
		BoastingStone = null;
		List<Game.EStone> selection =  new List<Game.EStone>();
		foreach (Game.Stone stone in m_Game.Line)
		{
			if (stone != null && !BoastedStones.Contains(stone.Value) && stone.Hidden)
			{
				selection.Add(stone.Value);
			}
		}

		if (selection.Count <= 0)
		{
			m_Game.AddPointsToPlayer(m_Game.CurrentPlayer, 3);
			IsBoasting = false;
			BoastingStone = null;
			BoastedStones = null;
			return;
		}
		
		StoneSelector.AskForSelection(selection.ToArray(), aShuffle:true, DoNextBoast);
	}

	public void DoNextBoast(Game.EStone lastAnswer)
	{
		BoastingStone = lastAnswer;
		StoneSelector.gameObject.SetActive(false);
	}
	
	#endregion
}