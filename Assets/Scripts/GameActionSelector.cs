using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
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
					if (stone != null && stone.Value != Game.EStone.NONE)
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
				return true;
			case Game.EGameAction.Boast:
				return true;
			default:
				return false;
		}
	}

	public void OnButtonClick(bool isOn)
	{
		if (isOn)
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
	}
}
