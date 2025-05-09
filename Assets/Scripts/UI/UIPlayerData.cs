using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class UIPlayerData : MonoBehaviour
{
	private const string IS_CURRENT_PLAYER = "IsCurrentPlayer";
	[SerializeField] private Animator Animator;
	[SerializeField] private TextMeshProUGUI TextCounter;
	
	public void SetIsCurrentPlayer(bool aIsCurrentPlayer)
	{
		Animator.SetBool(IS_CURRENT_PLAYER, aIsCurrentPlayer);
	}

	public void SetPoints(int aPoints)
	{
		TextCounter.text = aPoints.ToString();
	}
}