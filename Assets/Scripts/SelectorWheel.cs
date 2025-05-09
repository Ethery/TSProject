using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Game;

public class SelectorWheel : MonoBehaviour
{
	/// <summary>
	/// Percentage of this object that the button will take for their own size.
	/// </summary>
	[SerializeField] private float ButtonsSizeRatio = 0.2f;


	[SerializeField] private WheelButton ButtonPrefab;
	
	[FormerlySerializedAs("buttons")]
	[SerializeField] private List<WheelButton> Buttons = new List<WheelButton>();
	
	private RectTransform m_RectTransform;
	
	private float m_DistanceToCenter;
	private float m_MinBetweenHeightAndWidth;
	
	public void AskForSelection(EStone[] aSelection, Action<Game.EStone> aCallback)
	{
		InitSelection(aSelection);
		foreach(WheelButton button in Buttons)
		{
			button.SetCallback(aCallback);
		}
		
		//This will start the animation
		gameObject.SetActive(true);
	}
	
	private void OnDrawGizmos()
	{
		if(!m_RectTransform)
			m_RectTransform = GetComponent<RectTransform>();

		UpdateSizeRelatedVariables();
		Gizmos.DrawWireSphere(transform.position, m_DistanceToCenter);
		for(int i = 0; i < 7; i++)
		{
			Gizmos.DrawWireCube(GetButtonPosition(i,7),GetButtonSize());
		}
	}
	
	private void InitSelection(EStone[] aSelection)
	{
		int i = 0;
		for (; i < Buttons.Count && i < aSelection.Length; i++)
		{
			Buttons[i].Stone = aSelection[i];
		}

		for (; i < aSelection.Length; i++)
		{
			WheelButton button = Instantiate(ButtonPrefab,transform);
			button.Stone = aSelection[i];
			Buttons.Add(button);
		}

		for (int j = Buttons.Count - 1; j >= i; j++)
		{
			Destroy(Buttons[j].gameObject);
			Buttons.RemoveAt(j);
		}
	}
	
	private void Awake()
	{
		m_RectTransform = GetComponent<RectTransform>();
	}

	public void Update()
	{
		UpdateSizeRelatedVariables();
		{
			for(int i = 0; i< Buttons.Count; i++)
			{
				WheelButton button = Buttons[i];
				button.SetSize(GetButtonSize());
				button.transform.position = GetButtonPosition(i,Buttons.Count);
			}
		}
	}

	private bool UpdateSizeRelatedVariables()
	{
		float newMin = Math.Min(m_RectTransform.rect.width,
			m_RectTransform.rect.height);
		if (!Mathf.Approximately(m_MinBetweenHeightAndWidth, newMin))
		{
			m_MinBetweenHeightAndWidth = newMin;
			m_DistanceToCenter = m_MinBetweenHeightAndWidth / 2;
			m_DistanceToCenter *= transform.lossyScale.x;
			return true;
		}
		return false;
	}
	
	private Vector3 GetButtonPosition(int aIndex, int aMaxButtons)
	{
		float angleBetweenButtons = 360f / aMaxButtons;
		return transform.position + (Quaternion.AngleAxis(angleBetweenButtons * aIndex, Vector3.forward) * (Vector3.up*m_DistanceToCenter));
	}
	
	private Vector3 GetButtonSize()
	{
		return Vector2.one * (m_MinBetweenHeightAndWidth * ButtonsSizeRatio);
	}
}
