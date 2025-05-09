using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WheelButton : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] private Image Image;
	[SerializeField] private VisualDataSet VisualDataSet;
	
	private Game.EStone m_Stone;

	public Game.EStone Stone
	{
		get => m_Stone;
		set
		{
			m_Stone = value;
			if (Image)
			{
				Image.sprite = VisualDataSet.Sprites[value];
				this.name = value.ToString();
			}
		}
	}

	private Action<Game.EStone> m_Callback;
	
	private RectTransform RectTransform => GetComponent<RectTransform>();

	public void OnPointerClick(PointerEventData aEventData)
	{
		m_Callback.Invoke(Stone);
	}

	public void SetCallback(Action<Game.EStone> aCallback)
	{
		m_Callback = aCallback;
	}

	public void SetSize(Vector2 aButtonsSizeRatio)
	{
		RectTransform.sizeDelta = aButtonsSizeRatio;
	}
}
