using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class HoverHandler : MonoBehaviour
{
	[SerializeField]
	private UnityEvent OnObjectHovered;
	[SerializeField]
	private UnityEvent OnObjectUnhovered;

	public void OnHovered()
	{
		OnObjectHovered?.Invoke();
	}
	public void OnUnhovered()
	{
		OnObjectUnhovered?.Invoke();
	}

}
