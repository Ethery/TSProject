using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ClickHandler : MonoBehaviour
{
	[SerializeField]
	private UnityEvent OnObjectClicked;

	public void OnClicked()
	{
		OnObjectClicked?.Invoke();
	}

}
