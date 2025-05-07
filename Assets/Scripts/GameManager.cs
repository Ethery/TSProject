using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : Singleton<GameManager>
{
	public StoneObject StonePrefab;
	public Transform[] LinePositions;
	public Transform[] PoolPositions;

	private Dictionary<int, StoneObject> LineStoneInstances;
	private Dictionary<int, StoneObject> PoolStoneInstances;

	private List<StoneObject> UnusedStoneInstances = new List<StoneObject>();

	[SerializeField]
	public SelectorWheel Selector = null;

	[SerializeField]
	public Game Game = null;

	[SerializeField]
	public VisualDatas VisualDatas = null;

	private void OnDrawGizmos()
	{
		for (int i = 0; i < LinePositions.Length; i++)
		{
			Gizmos.DrawMesh(StonePrefab.GetComponentInChildren<MeshFilter>().sharedMesh, LinePositions[i].position);
		}
		for (int i = 0; i < PoolPositions.Length; i++)
		{
			Gizmos.DrawMesh(StonePrefab.GetComponentInChildren<MeshFilter>().sharedMesh, PoolPositions[i].position);
		}
	}

	public void Start()
	{
		Game = new Game();
		LineStoneInstances = new Dictionary<int, StoneObject>(7);
		for(int i = 0; i < Game.Line.Length; i++)
		{
			if (Game.GetStoneInLineAt(i) != Game.EStone.NONE)
			{
				Transform linePos = LinePositions[i];
				StoneObject stoneInstance = Instantiate(StonePrefab, linePos);
				stoneInstance.Stone = Game.Line[i];
				LineStoneInstances.Add(i, stoneInstance);
			}
		}

		PoolStoneInstances = new Dictionary<int, StoneObject>(7);
		for(int i = 0; i < Game.Pool.Length; i++)
		{
			if (Game.GetStoneInPoolAt(i) != Game.EStone.NONE)
			{
				Transform poolPos = PoolPositions[i];
				StoneObject stoneInstance = Instantiate(StonePrefab, poolPos);
				stoneInstance.Stone = Game.Pool[i];
				stoneInstance.IsFromThePool = true;
				PoolStoneInstances.Add(i, stoneInstance);
			}
		}

		for(int i=0;i<4;i++)
		{
			UnusedStoneInstances.Add(Instantiate(StonePrefab, transform));
			UnusedStoneInstances[i].gameObject.SetActive(false);
		}
	}

	public void Update()
	{
		if (Game != null)
		{
			for (int i = 0; i < Game.Line.Length; i++)
			{
				if (Game.GetStoneInLineAt(i) == Game.EStone.NONE && LineStoneInstances.ContainsKey(i))
				{
					UnusedStoneInstances.Add(LineStoneInstances[i]);
					LineStoneInstances[i].gameObject.SetActive(false);
					LineStoneInstances[i].transform.SetParent(transform, false);
					LineStoneInstances.Remove(i);
				}
				else if (Game.GetStoneInLineAt(i) != Game.EStone.NONE && !LineStoneInstances.ContainsKey(i))
				{
					LineStoneInstances.Add(i, UnusedStoneInstances[0]);
					UnusedStoneInstances.RemoveAt(0);
					LineStoneInstances[i].gameObject.SetActive(true);
					LineStoneInstances[i].transform.SetParent(LinePositions[i], false);
					LineStoneInstances[i].Stone = Game.Line[i];
				}
				else if (Game.GetStoneInLineAt(i) != Game.EStone.NONE && LineStoneInstances.ContainsKey(i))
				{
					LineStoneInstances[i].Stone = Game.Line[i];
					LineStoneInstances[i].IsFromThePool = false;
				}
			}

			for (int i = 0; i < Game.Pool.Length; i++)
			{
				if (Game.GetStoneInPoolAt(i) == Game.EStone.NONE && PoolStoneInstances.ContainsKey(i))
				{
					UnusedStoneInstances.Add(PoolStoneInstances[i]);
					PoolStoneInstances[i].gameObject.SetActive(false);
					PoolStoneInstances[i].transform.SetParent(transform, false);
					PoolStoneInstances.Remove(i);
				}
				else if (Game.GetStoneInPoolAt(i) != Game.EStone.NONE && !PoolStoneInstances.ContainsKey(i))
				{
					PoolStoneInstances.Add(i, UnusedStoneInstances[0]);
					UnusedStoneInstances.RemoveAt(0);
					PoolStoneInstances[i].gameObject.SetActive(true);
					PoolStoneInstances[i].transform.SetParent(PoolPositions[i], false);
					PoolStoneInstances[i].Stone = Game.Pool[i];
				}
				else if(Game.GetStoneInPoolAt(i) != Game.EStone.NONE && PoolStoneInstances.ContainsKey(i))
				{
					PoolStoneInstances[i].Stone = Game.Pool[i];
					PoolStoneInstances[i].IsFromThePool = true;
				}
			}
		}
	}
}