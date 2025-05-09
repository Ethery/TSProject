using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StoneObject : MonoBehaviour
{
	[SerializeField] private VisualDataSet DataSet;

	[SerializeField] private Animator Animator;

	public bool IsFromThePool;

	private Game.Stone m_Stone;
	private Renderer[] m_Renderers;

	private static StoneObject _stoneToSwap;
	
	#region Animator Parameters
	
	private const string HiddenAnimParam = "Hidden";
	private const string SelectedAnimParam = "Selected";
	private const string HoveredAnimParam = "Hovered";

	#endregion
	
	public Game.Stone Stone
	{
		get => m_Stone;
		set
		{
			m_Stone = value;
		}
	}

	private void Start()
	{
		m_Renderers = GetComponentsInChildren<Renderer>();
	}

	private void Update()
	{
		if (Stone != null)
		{
			ApplyType(Stone.Value);
			if (Animator != null)
			{
				Animator.SetBool(HiddenAnimParam, Stone.Hidden);
				Animator.SetBool(SelectedAnimParam, _stoneToSwap == this);
			}
		}
	}

	private void ApplyType(Game.EStone aStoneType)
	{
		foreach (Renderer renderer in m_Renderers)
		{
			renderer.material.mainTexture = DataSet.Textures[aStoneType];
		}
	}

	public void OnHovered()
	{
		if (GameManager.Instance.Game.CurrentRunningAction.HasValue
		    && GameManager.Instance.Game.CurrentRunningAction != Game.EGameAction.Boast)
		{
			Animator.SetBool(HoveredAnimParam, true);
		}
	}

	public void OnUnHovered()
	{
		Animator.SetBool(HoveredAnimParam, false);
	}

	public void OnClicked()
	{
		if (GameManager.Instance.Game.CurrentRunningAction.HasValue)
		{
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
							GameManager.Instance.Game.FlipStone(Stone.Value);
						}
						break;
					}
				case Game.EGameAction.SwapStones:
					{
						if (!this.IsFromThePool && GameManager.Instance.Game.Line.Length >= 2)
						{
							if (_stoneToSwap == null)
							{
								_stoneToSwap = this;
								Debug.Log($"Setting swap stone to {_stoneToSwap.Stone.Value}");
							}
							else
							{
								Debug.Log($"Swapping {_stoneToSwap.Stone.Value} to {this.Stone.Value}");
								GameManager.Instance.Game.SwapStones(_stoneToSwap.Stone.Value, this.Stone.Value);
								_stoneToSwap = null;
							}
						}
						break;
					}
				case Game.EGameAction.WatchStone:
					if (this.Stone.Hidden)
						StartCoroutine(WatchCoroutine());
					break;
				case Game.EGameAction.Defy:
					GameManager.Instance.Selector.AskForSelection(Game.ALL,Guess);
					break;
				case Game.EGameAction.Boast:
					if (GameManager.Instance.IsBoasting && GameManager.Instance.BoastingStone.HasValue)
					{
						GameManager.Instance.Game.ShowStone(Stone.Value);
						if (GameManager.Instance.BoastingStone == Stone.Value)
						{
							GameManager.Instance.BoastedStones.Add(Stone.Value);
							GameManager.Instance.AskBoast();
						}
						else
						{
							GameManager.Instance.Game.AddPointsToPlayer((GameManager.Instance.Game.CurrentPlayer+1)%GameManager.Instance.Game.NumberOfPlayers,3);
						}
					}
					break;
			}
		}
	}

	private void Guess(Game.EStone aStone)
	{
		GameManager.Instance.Game.Defy(GameManager.Instance.Game.CurrentPlayer, this.Stone, aStone);
		GameManager.Instance.Selector.gameObject.SetActive(false);
	}

	public IEnumerator WatchCoroutine()
	{
		GameManager.Instance.IsWatching = true;
		GameManager.Instance.Game.FlipStone(Stone.Value);
		yield return new WaitForSeconds(5f);
		GameManager.Instance.Game.FlipStone(Stone.Value);
		GameManager.Instance.Game.WatchStone(Stone.Value);
		GameManager.Instance.IsWatching = false;
	}
}
