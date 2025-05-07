using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WheelButton : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] Image Image;

	private Game.EStone m_stone;

	public Game.EStone Stone
	{
		get => m_stone;
		set
		{
			m_stone = value;
			Image.sprite = GameManager.Instance.VisualDatas.GetSpriteFor(value);
		}
	}

	Action<Game.EStone> m_callback;

	public void OnPointerClick(PointerEventData eventData)
	{
		m_callback.Invoke(Stone);
	}

	public void SetCallback(Action<Game.EStone> callback)
	{
		m_callback = callback;
	}
}
