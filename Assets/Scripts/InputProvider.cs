using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour
{
	public Vector2 MousePosition { get; private set; }
	public event Action Clicked;

	public HoverHandler hoveredObject = null;
	private void OnMousePosition(InputValue value)
	{
		MousePosition = value.Get<Vector2>();

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit _hit, 100))
		{
			if (_hit.transform.TryGetComponent(out HoverHandler _hoverHandler))
			{
				if (hoveredObject != _hit.transform.gameObject)
				{
					if (hoveredObject != null)
					{
						hoveredObject.OnUnhovered();
					}
					hoveredObject = _hoverHandler;
					hoveredObject.OnHovered();
					return;
				}
			}
		}
		if (hoveredObject != null)
		{
			hoveredObject.OnUnhovered();
			hoveredObject = null;
		}

	}

	private void OnClick(InputValue _)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit _hit, 100))
		{
			if(_hit.transform.TryGetComponent(out ClickHandler _clickHandler))
			{
				_clickHandler.OnClicked();
			}
		}
	}
}
