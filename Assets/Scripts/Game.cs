using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Game
{
	public sealed class Stone
	{
		public readonly EStone Value;
		public bool Hidden;

		public Stone(EStone aValue, bool aHidden)
		{
			Value = aValue;
			Hidden = aHidden;
		}
	}

	public enum EStone
	{
		None = -1,
		Crown = 0,
		Flag = 1,
		Hammer = 2,
		Knight = 3,
		Scales = 4,
		Shield = 5,
		Sword = 6,
		Count = 7,
	}

	public static readonly EStone[] ALL = new EStone[]{ EStone.Crown, EStone.Flag, EStone.Hammer, EStone.Knight, EStone.Scales, EStone.Shield, EStone.Sword };

	public enum EGameAction
	{
		PlaceStoneBefore,
		PlaceStoneAfter,
		HideStone,
		SwapStones,
		WatchStone,
		Defy,
		Boast,
	}

	#region Running Game datas

	private readonly Stone[] m_Line;
	private readonly Stone[] m_Pool;
	private readonly int[] m_PlayerPoints;
	
	private int m_CurrentPlayer = 0;
	private readonly int m_NumberOfPlayers;

	public EGameAction? CurrentRunningAction = null;
	public int NumberOfPlayers => m_NumberOfPlayers;
	public int CurrentPlayer => m_CurrentPlayer;
	public int NextPlayer => (m_CurrentPlayer + 1) % m_NumberOfPlayers;
	public Stone[] Line => m_Line;
	public Stone[] Pool => m_Pool;
	
	#endregion
	
	public Game(int aNumberOfPlayers)
	{
		m_NumberOfPlayers = aNumberOfPlayers;
		m_Line = new Stone[(int)EStone.Count];
		m_Pool = new Stone[(int)EStone.Count];

		int stoneIdToPlace = Random.Range(0, (int)EStone.Count);
		
		for (int i = 0; i < (int)EStone.Count; i++)
		{
			m_Line[i] = null;
			if (stoneIdToPlace != i)
			{
				m_Pool[i] = new Stone((EStone)i, false);
			}
		}
		
		m_Line[3] = new Stone((EStone)stoneIdToPlace, false);

		m_PlayerPoints = new int[m_NumberOfPlayers];

		Debug.Log(ToString());
	}
	
	#region Actions
	public void GoToNextPlayer()
	{
		CurrentRunningAction = null;
		m_CurrentPlayer = NextPlayer;
	}
	
	/// <summary>
	/// Will place a stone on the line before or after the stones already on it.
	/// </summary>
	/// <param name="aStoneToPlace">stonetype to place</param>
	/// <param name="aBefore">will the stone be placed Before(true) or After(false) the other stones.</param>
	public void PlaceStone(EStone aStoneToPlace,bool aBefore)
	{
		if (aBefore)
		{
			//if the first index is not empty we move it up to free the space.
			if (m_Line[0] != null)
			{
				MoveStoneInLineNoLoss(0, aUp: true);
			}

		}
		else
		{
			//if the last index is not empty we move it down to free the space.
			if (m_Line[^1] != null)
			{
				MoveStoneInLineNoLoss(m_Line.Length - 1, aUp: false);
			}
		}

		int i = aBefore ? 0 : m_Line.Length - 1;
		int end = aBefore ? m_Line.Length : -1;
		int step = aBefore ? 1 : -1;

		for (; i != end; i += step)
		{		
			//looping either way to find the first "not empty slot" in the line
			if (m_Line[i] != null)
			{
				//and assigning the previous one the new value.
				m_Line[i - step] = new Stone(aStoneToPlace, false);
				Debug.Log($"Placed {aStoneToPlace} {(aBefore ? "before" : "after")} the line");
				RemoveStoneFromPool(aStoneToPlace);
				
				GoToNextPlayer();
				return;
			}
		}


		Debug.LogError($"Couldn't place {aStoneToPlace} {(aBefore ? "before" : "after")} the line");
	}

	public void ShowStone(EStone aStoneToShow)
	{
		for(int i = 0;i <m_Line.Length;i++)
		{
			if (m_Line[i] == null)
				continue;
			
			if (m_Line[i].Value == aStoneToShow)
			{
				m_Line[i].Hidden = false;
			}
		}
	}
	
	public void HideStone(EStone aStoneToHide)
	{
		for(int i = 0;i <m_Line.Length;i++)
		{
			if (m_Line[i] == null)
				continue;
			
			if (m_Line[i].Value == aStoneToHide)
			{
				m_Line[i].Hidden = true;
			}
		}
	}
	
	public void FlipStone(int aStoneToHide)
	{
		Assert.IsTrue(0 <= aStoneToHide && aStoneToHide <= m_Line.Length - 1);
		Assert.IsTrue(m_Line[aStoneToHide] != null);
		
		m_Line[aStoneToHide].Hidden = !m_Line[aStoneToHide].Hidden;
		Debug.Log($"{(m_Line[aStoneToHide].Hidden ? "Hidden" : "Shown")} {m_Line[aStoneToHide].Value}");
		
		GoToNextPlayer();
	}

	public void FlipStone(EStone aStoneToHide)
	{
		for(int i = 0;i <m_Line.Length;i++)
		{
			if (m_Line[i] == null)
				continue;
			
			if (m_Line[i].Value == aStoneToHide)
			{
				FlipStone(i);
			}
		}
	}

	public void SwapStones(int aIndexFrom, int aIndexTo)
	{
		// ReSharper disable once SwapViaDeconstruction for Clarity
		Stone stoneTo = m_Line[aIndexTo];
		m_Line[aIndexTo] = m_Line[aIndexFrom];
		m_Line[aIndexFrom] = stoneTo;
		
		GoToNextPlayer();
	}
	
	public void SwapStones(EStone aStoneFromValue, EStone aStoneToValue)
	{
		int fromIndex = GetStoneIndexInLine(aStoneFromValue);
		int toIndex = GetStoneIndexInLine(aStoneToValue);
		SwapStones(fromIndex,toIndex);
	}

	public Stone WatchStone(EStone aStoneToSee)
	{
		for (int i = 0; i < m_Line.Length; i++)
		{
			if (m_Line[i] != null)
			{
				if (m_Line[i].Value == aStoneToSee)
				{
					GoToNextPlayer();
					return m_Line[i];
				}
			}
		}
		return null;
	}

	public void Defy(int aAskingPlayerId,Stone aStoneToGuess, EStone aGuess )
	{
		if (aStoneToGuess.Value == aGuess)
		{
			AddPointsToPlayer((aAskingPlayerId+1)%NumberOfPlayers);
		}
		else
		{
			AddPointsToPlayer(aAskingPlayerId);
		}
		GoToNextPlayer();
	}

	/// <summary>
	/// Check a BeginBoast guess.
	/// </summary>
	/// <param name="aBoastingPlayerId"></param>
	/// <param name="aGuess">list of the stones ordered.</param>
	/// <returns>True if the guess is correct. False otherwise.</returns>
	public bool CheckBoast(int aBoastingPlayerId, EStone[] aGuess)
	{
		for(int i = 0;i< m_Line.Length;i++)
		{
			if (m_Line[i] != null && aGuess[i] != EStone.None)
			{
				if (aGuess[i] != m_Line[i].Value)
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		return true;
	}

	#endregion

	#region public Helpers

	public void AddPointsToPlayer(int aPlayerIndex,int aPointsToAdd = 1)
	{
		m_PlayerPoints[aPlayerIndex] += aPointsToAdd;
	}

	public int GetPointsOfPlayer(int aPlayerId)
	{
		if (aPlayerId >= 0 && aPlayerId < m_PlayerPoints.Length)
		{
			return m_PlayerPoints[aPlayerId];
		}
		Debug.LogError($"Player {aPlayerId} is not in the players list");
		return -1;
	}
	
	public EStone GetStoneInLineAt(int aID)
	{
		if (m_Line[aID] == null)
			return EStone.None;
		return m_Line[aID].Value;
	}
	
	public EStone GetStoneInPoolAt(int aID)
	{
		if (m_Pool[aID] == null)
			return EStone.None;
		return m_Pool[aID].Value;
	}

	#endregion
	
	#region private Helpers
	
	private int GetStoneIndexInLine(EStone aStone)
	{
		for (int i = 0; i < m_Line.Length; i++)
		{
			if (m_Line[i] != null && m_Line[i].Value == aStone)
			{
				return i;
			}
		}
		return -1;
	}

	private int GetStoneIndexInPool(EStone aStone)
	{
		for (int i = 0; i < m_Pool.Length; i++)
		{
			if (m_Pool[i] != null && m_Pool[i].Value == aStone)
			{
				return i;
			}
		}
		return -1;
	}

	private void RemoveStoneFromPool(EStone aStone)
	{
		for(int i = 0; i < m_Pool.Length;i++)
		{
			if (m_Pool[i] != null && m_Pool[i].Value == aStone)
			{
				m_Pool[i] = null;
			}
		}
	}

	/// <summary>
	/// move a stone from source index and either up or down the line.
	/// </summary>
	/// <param name="aSourceIndex"></param>
	/// <param name="aUp"></param>
	private void MoveStoneInLineNoLoss(int aSourceIndex, bool aUp)
	{
		int targetIndex = aSourceIndex + (aUp ? 1:-1);

		//If this happens the line is not big enough OR already filled up. (or something went wrong earlier)
		Assert.IsFalse(aSourceIndex > m_Line.Length, $"Can't move stone from index {aSourceIndex} to {targetIndex}");

		//if target is occupied we move in the same direction the next stone.
		if (m_Line[targetIndex] != null)
		{
			Debug.Log($"Move stone from {aSourceIndex} to  {targetIndex}");
			//Not checking newTargetIndex since it will be checked at the beggining of the next call to MoveStoneInLineNoLoss.
			MoveStoneInLineNoLoss(targetIndex, aUp);
		}

		//From there the line[targetIndex] have been moved so we can move properly.
		Stone stone = m_Line[aSourceIndex];

		m_Line[targetIndex] = stone;
		m_Line[aSourceIndex] = null;
	}

	#endregion

	public override string ToString()
	{
		string printInitResult = "Line:\n";
		foreach (Stone stone in m_Line)
		{
			if (stone != null)
			{
				printInitResult += $"\t{stone.Value}\n";
			}
			else
			{
				printInitResult += "\tNULL\n";
			}
		}
		printInitResult += "Pool:\n";
		foreach (Stone stone in m_Pool)
		{
			if (stone != null)
			{
				printInitResult += $"\t{stone.Value}\n";
			}
			else
			{
				printInitResult += "\tNULL\n";
			}
		}
		return printInitResult;
	}
}
