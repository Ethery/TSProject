using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoastActionSelector : MonoBehaviour
{
    [SerializeField] private Button DontCareButton;
    
    private bool m_IsFirstTime = false;
    
    public void SelectAction(bool aIsFirstTime = true)
    {
        m_IsFirstTime = aIsFirstTime;
        gameObject.SetActive(true);
        DontCareButton.interactable = m_IsFirstTime;
    }
    
    public void Believe()
    {
        GameManager.Instance.Game.AddPointsToPlayer(GameManager.Instance.Game.NextPlayer, 1);
        GameManager.Instance.IsBoasting = false;
        GameManager.Instance.Game.GoToNextPlayer();
    }

    public void DontBelieve()
    {
        GameManager.Instance.AskBoast();
    }

    public void DontCare()
    {
        if(m_IsFirstTime)
            GameManager.Instance.ConfrontBoast();
    }
}