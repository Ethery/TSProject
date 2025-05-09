using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsCounter : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI Player1TextCounter;
	[SerializeField] private TextMeshProUGUI Player2TextCounter;

	private Game m_Game;

	private void Start()
	{
		m_Game = GameManager.Instance.Game;
	}

	private void Update()
	{
		Player1TextCounter.text = m_Game.GetPointsOfPlayer(0).ToString();
		Player2TextCounter.text = m_Game.GetPointsOfPlayer(1).ToString();
	}
}
