using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class Game
{
	public sealed class Stone
	{
		public EStone Value = 0;
		public bool Hidden = false;

		public Stone(EStone aValue, bool aHidden)
		{
			Value = aValue;
			Hidden = aHidden;
		}
	}

	public enum EStone
	{
		NONE = 0,
		Crown = 1,
		Flag,
		Hammer,
		Knight,
		Scales,
		Shield,
		Sword,
	}

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

	public Stone[] Line = new Stone[7];
	public Stone[] Pool = new Stone[7];
	public int[] PlayerPoints = new int [2]{ 0, 0 };

	public EGameAction? CurrentRunningAction = null;

	public Game()
	{
		Line = new Stone[7];
		Pool = new Stone[7];
		Array stones = Enum.GetValues(typeof(EStone));

		for (int i = 1; i < stones.Length; i++)
		{
			Pool[i-1] = new Stone((EStone)stones.GetValue(i),false);
			Line[i-1] = null;
		}

		int stoneIdToPlace = UnityEngine.Random.Range(1, 8);

		Line[3] = new Stone((EStone)stoneIdToPlace, false);

		RemoveStoneFromPool((EStone)stoneIdToPlace);

		PlayerPoints = new int[2] { 0, 0 };
	}

	public EStone GetStoneInLineAt(int id)
	{
		if (Line[id] == null)
			return EStone.NONE;
		return Line[id].Value;
	}
	public EStone GetStoneInPoolAt(int id)
	{
		if (Pool[id] == null)
			return EStone.NONE;
		return Pool[id].Value;
	}

	#region Actions

	/// <summary>
	/// Will place a stone on the line before or after the stones already on it.
	/// </summary>
	/// <param name="stoneToPlace">stonetype to place</param>
	/// <param name="before">will the stone be placed Before(true) or After(false) the other stones.</param>
	public void PlaceStone(EStone stoneToPlace,bool before)
	{
		if (before)
		{
			//if the first index is not empty we move it up to free the space.
			if (Line[0] != null)
			{
				MoveStoneInLineNoLoss(0, up: true);
			}

		}
		else
		{
			//if the last index is not empty we move it down to free the space.
			if (Line[Line.Length - 1] != null)
			{
				MoveStoneInLineNoLoss(Line.Length - 1, up: false);
			}
		}

		int i = before ? 0 : Line.Length - 1;
		int end = before ? Line.Length : -1;
		int step = before ? 1 : -1;

		for (; i != end; i += step)
		{		
			//looping either way to find the first "not empty slot" in the line
			if (Line[i] != null)
			{
				//and assigning the previous one the new value.
				Line[i - step] = new Stone(stoneToPlace, false);
				Debug.Log($"Placed {stoneToPlace} {(before ? "before" : "after")} the line");
				RemoveStoneFromPool(stoneToPlace);
				return;
			}
		}


		Debug.LogError($"Couldn't place {stoneToPlace} {(before ? "before" : "after")} the line");
	}

	public void HideStone(int stoneToHide)
	{
		Assert.IsTrue(0 <= stoneToHide && stoneToHide <= Line.Length - 1);
		Assert.IsTrue(Line[stoneToHide] != null);
		
		Line[stoneToHide].Hidden = !Line[stoneToHide].Hidden;
		Debug.Log($"{(Line[stoneToHide].Hidden ? "Hidden" : "Shown")} {Line[stoneToHide].Value}");
	}

	public void HideStone(EStone stoneToHide)
	{
		for(int i = 0;i <Line.Length;i++)
		{
			if (Line[i] != null)
			{
				if (Line[i].Value == stoneToHide)
				{
					HideStone(i);
				}
			}
		}
	}

	public void SwapStones(int indexFrom, int indexTo)
	{
		Stone stoneTo = Line[indexTo];
		Line[indexTo] = Line[indexFrom];
		Line[indexFrom] = stoneTo;
	}
	
	public void SwapStones(EStone stoneFromValue, EStone stoneToValue)
	{
		int fromIndex = GetStoneIndexInLine(stoneFromValue);
		int toIndex = GetStoneIndexInLine(stoneToValue);
		SwapStones(fromIndex,toIndex);
	}

	public Stone WatchStone(int index)
	{
		Assert.IsTrue(0 > index && index > Line.Length - 1);
		Assert.IsTrue(Line[index] != null);

		return Line[index];
	}

	public Stone WatchStone(EStone stoneToSee)
	{
		for (int i = 0; i < Line.Length; i++)
		{
			if (Line[i] != null)
			{
				if (Line[i].Value == stoneToSee)
				{
					return Line[i];
				}
			}
		}
		return null;
	}

	public void Defy(int aAskingPlayerId, int aAnsweringPlayerId,Stone stoneToGuess, EStone aGuess )
	{
		if (stoneToGuess.Value == aGuess)
		{
			AddPointsToPlayer(aAnsweringPlayerId);
		}
		else
		{
			AddPointsToPlayer(aAskingPlayerId);
		}
	}

	/// <summary>
	/// Check a Boast guess.
	/// </summary>
	/// <param name="aBoastingPlayerId"></param>
	/// <param name="aGuess">list of the stones ordered.</param>
	/// <returns>True if the guess is correct. False otherwise.</returns>
	public bool CheckBoast(int aBoastingPlayerId, EStone[] aGuess)
	{
		for(int i = 0;i< Line.Length;i++)
		{
			if (Line[i] == null && aGuess[i] == EStone.NONE)
			{
				continue;
			}
			else if (Line[i] != null && aGuess[i] != EStone.NONE)
			{
				if (aGuess[i] != Line[i].Value)
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

	#region Helpers

	private int GetStoneIndexInLine(EStone stone)
	{
		for (int i = 0; i < Line.Length; i++)
		{
			if (Line[i] != null && Line[i].Value == stone)
			{
				return i;
			}
		}
		return -1;
	}

	private int GetStoneIndexInPool(EStone stone)
	{
		for (int i = 0; i < Pool.Length; i++)
		{
			if (Pool[i] != null && Pool[i].Value == stone)
			{
				return i;
			}
		}
		return -1;
	}

	public void RemoveStoneFromPool(EStone stone)
	{
		for(int i = 0; i < Pool.Length;i++)
		{
			if (Pool[i] != null && Pool[i].Value == stone)
			{
				Pool[i] = null;
			}
		}
	}

	public void AddPointsToPlayer(int aPlayerIndex,int aPointsToAdd = 1)
	{
		PlayerPoints[aPlayerIndex] += aPointsToAdd;
	}

	/// <summary>
	/// move a stone from source index and either up or down the line.
	/// </summary>
	/// <param name="sourceIndex"></param>
	/// <param name="up"></param>
	private void MoveStoneInLineNoLoss(int sourceIndex, bool up)
	{
		int targetIndex = sourceIndex + (up ? 1:-1);

		//If this happens the line is not big enough OR already filled up. (or something went wrong earlier)
		Assert.IsFalse(sourceIndex > Line.Length, $"Can't move stone from index {sourceIndex} to {targetIndex}");

		//if target is occupied we move in the same direction the next stone.
		if (Line[targetIndex] != null)
		{
			Debug.Log($"Move stone from {sourceIndex} to  {targetIndex}");
			//Not checking newTargetIndex since it will be checked at the beggining of the next call to MoveStoneInLineNoLoss.
			MoveStoneInLineNoLoss(targetIndex, up);
		}

		//From there the line[targetIndex] have been moved so we can move properly.
		Stone stone = Line[sourceIndex];

		Line[targetIndex] = stone;
		Line[sourceIndex] = null;
	}

	#endregion
}
