using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsCounter : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI Player1TextCounter;
	[SerializeField] private TextMeshProUGUI Player2TextCounter;


	private void Update()
	{
		Player1TextCounter.text = GameManager.Instance.Game.PlayerPoints[0].ToString();
		Player2TextCounter.text = GameManager.Instance.Game.PlayerPoints[1].ToString();
	}
}
