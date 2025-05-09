using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIPlayerDataContainer : MonoBehaviour
{
	[SerializeField] private UIPlayerData PlayerCounterPrefab;
	[SerializeField] private List<UIPlayerData> PlayerCounters;

	
	private Game m_Game;

	private void Start()
	{
		m_Game = GameManager.Instance.Game;
		for (int i = 0; i < m_Game.NumberOfPlayers; i++)
		{
			PlayerCounters.Add(Instantiate(PlayerCounterPrefab, transform));
		}
	}

	private void Update()
	{
		for(int i = 0;i< PlayerCounters.Count;i ++)
		{
			UIPlayerData counter = PlayerCounters[i];
			counter.SetPoints(m_Game.GetPointsOfPlayer(i));
			counter.SetIsCurrentPlayer(GameManager.Instance.Game.CurrentPlayer == i);
		}
	}
}
