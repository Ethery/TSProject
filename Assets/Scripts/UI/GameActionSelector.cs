using System;
using UnityEngine;
using UnityEngine.UI;

public class GameActionSelector : MonoBehaviour
{
	[SerializeField]
	private Game.EGameAction Action;

	private void Update()
	{
		Toggle toggle = GetComponent<Toggle>();
		toggle.SetIsOnWithoutNotify(GameManager.Instance.Game.CurrentRunningAction == Action);
		toggle.interactable = CanBePlayed();
	}

	public bool CanBePlayed()
	{
		if (GameManager.Instance.IsBoasting || GameManager.Instance.IsWatching) 
			return false;
		
		switch (Action)
		{
			case Game.EGameAction.PlaceStoneBefore:
			case Game.EGameAction.PlaceStoneAfter:
			case Game.EGameAction.HideStone:
				return true;
			case Game.EGameAction.SwapStones:
				int lineCount = 0;
				foreach (Game.Stone stone in GameManager.Instance.Game.Line)
				{
					if (stone != null && stone.Value != Game.EStone.None)
						lineCount++;
				}
				return lineCount >= 2;
			case Game.EGameAction.WatchStone:
				foreach(Game.Stone stone in GameManager.Instance.Game.Line)
				{
					if (stone != null && stone.Hidden)
						return true;
				}
				return false;
			case Game.EGameAction.Defy:
				foreach(Game.Stone stone in GameManager.Instance.Game.Line)
				{
					if (stone != null && stone.Hidden)
						return true;
				}
				return false;
			case Game.EGameAction.Boast:
				return true;
			default:
				return false;
		}
	}

	public void OnButtonClick(bool aIsOn)
	{
		if (GameManager.Instance.IsBoasting)
			return;
		
		if (aIsOn)
		{
			GameManager.Instance.Game.CurrentRunningAction = Action;
		}
		else
		{
			if(GameManager.Instance.Game.CurrentRunningAction.HasValue && GameManager.Instance.Game.CurrentRunningAction == Action)
			{
				GameManager.Instance.Game.CurrentRunningAction = null;
			}
		}

		switch (GameManager.Instance.Game.CurrentRunningAction)
		{
			case Game.EGameAction.Boast:
				GameManager.Instance.ConfrontBoast();
				break;
		}
	}
}
