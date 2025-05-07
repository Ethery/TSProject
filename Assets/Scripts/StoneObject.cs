using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class StoneObject : MonoBehaviour
{
	[SerializeField]
	private List<Texture> Textures;

	[SerializeField]
	private Animator Animator;

	public bool IsFromThePool;

	public Game.Stone m_stone;

	private static StoneObject stoneToSwap;

	public Game.Stone Stone
	{
		get => m_stone;
		set
		{
			m_stone = value;
		}
	}

	private void Update()
	{
		if (Stone != null)
		{
			ApplyType(Stone.Value);
			if (Animator != null)
			{
				Animator.SetBool("Hidden", Stone.Hidden);
			}
			Animator.SetBool("Selected", stoneToSwap == this);
		}
	}

	private void ApplyType(Game.EStone stoneType)
	{
		foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
		{
			renderer.material.mainTexture = Textures[(int)stoneType];
		}
	}

	public void OnHovered()
	{
		Animator.SetBool("Hovered", true);
	}

	public void OnUnHovered()
	{
		Animator.SetBool("Hovered", false);
	}

	public void OnClicked()
	{
		if (GameManager.Instance.Game.CurrentRunningAction.HasValue)
		{
			bool placeBefore = false;
			switch (GameManager.Instance.Game.CurrentRunningAction)
			{
				case Game.EGameAction.PlaceStoneBefore:
					{
						if (this.IsFromThePool)
						{
							GameManager.Instance.Game.PlaceStone(Stone.Value, true);
						}
						break;
					}
				case Game.EGameAction.PlaceStoneAfter:
					{
						if (this.IsFromThePool)
						{
							GameManager.Instance.Game.PlaceStone(Stone.Value, false);
						}
						break;
					}
				case Game.EGameAction.HideStone:
					{
						if (!this.IsFromThePool)
						{
							GameManager.Instance.Game.HideStone(Stone.Value);
						}
						break;
					}
				case Game.EGameAction.SwapStones:
					{
						if (!this.IsFromThePool && GameManager.Instance.Game.Line.Length >= 2)
						{
							if (stoneToSwap == null)
							{
								stoneToSwap = this;
								Debug.Log($"Setting swap stone to {stoneToSwap.Stone.Value}");
							}
							else
							{
								Debug.Log($"Swapping {stoneToSwap.Stone.Value} to {this.Stone.Value}");
								GameManager.Instance.Game.SwapStones(stoneToSwap.Stone.Value, this.Stone.Value);
								stoneToSwap = null;
							}
						}
						break;
					}
				case Game.EGameAction.WatchStone:
					if (this.Stone.Hidden)
						StartCoroutine(WatchCoroutine());
					break;
				case Game.EGameAction.Defy:
					GameManager.Instance.Selector.AskForSelection(Guess);
					break;
				case Game.EGameAction.Boast:
					break;
			}
		}
	}

	private void Guess(Game.EStone stone)
	{
		GameManager.Instance.Game.Defy(0, 1, this.Stone, stone);
		GameManager.Instance.Selector.gameObject.SetActive(false);
	}

	public IEnumerator WatchCoroutine()
	{
		GameManager.Instance.Game.HideStone(Stone.Value);
		yield return new WaitForSeconds(5f);
		GameManager.Instance.Game.HideStone(Stone.Value);
	}
}
