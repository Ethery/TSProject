using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SelectorWheel : MonoBehaviour
{
	[SerializeField]
	private float DistanceToCenter;

	public List<WheelButton> buttons;

	private void Start()
	{
		buttons = new List<WheelButton>();
	}

	public void Update()
	{
		buttons.Clear();
		buttons.AddRange(GetComponentsInChildren<WheelButton>());
		float angle = 360 / buttons.Count;
		for(int i = 0; i< buttons.Count; i++)
		{
			WheelButton button = buttons[i];
			button.Stone = (Game.EStone)i + 1;
			button.transform.position = transform.position + (Quaternion.AngleAxis(angle*i, Vector3.forward) * (Vector3.up*DistanceToCenter));
		}
	}

	public void AskForSelection(Action<Game.EStone> callback)
	{
		gameObject.SetActive(true);
		foreach(WheelButton button in buttons)
		{
			button.SetCallback(callback);
		}
	}
}
