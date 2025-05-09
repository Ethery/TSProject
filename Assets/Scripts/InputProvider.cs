using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour
{
	private Vector2 m_MousePositionOnScreen;
	private HoverHandler m_HoveredObject;

	public Vector2 MousePositionOnScreen => m_MousePositionOnScreen;
	public HoverHandler HoveredObject =>  m_HoveredObject;
	
	private void OnMousePosition(InputValue aValue)
	{
		m_MousePositionOnScreen = aValue.Get<Vector2>();

		if (Camera.main)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit outHit, 100))
			{
				if (outHit.transform.TryGetComponent(out HoverHandler outHoverHandler))
				{
					if (m_HoveredObject != outHoverHandler)
					{
						if (m_HoveredObject)
						{
							m_HoveredObject.OnUnhovered();
						}
						m_HoveredObject = outHoverHandler;
						m_HoveredObject.OnHovered();
						return;
					}
				}
			}
		}

		if (m_HoveredObject)
		{
			m_HoveredObject.OnUnhovered();
			m_HoveredObject = null;
		}

	}

	private void OnClick(InputValue _)
	{
		if (Camera.main)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit outHit, 100))
			{
				if(outHit.transform.TryGetComponent(out ClickHandler outClickHandler))
				{
					outClickHandler.OnClicked();
				}
			}
		}
	}
}
