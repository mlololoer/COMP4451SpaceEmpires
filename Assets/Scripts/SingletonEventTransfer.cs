using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonEventTransfer : MonoBehaviour
{
    public void UIManager_OpenResearchModal() {
    	UIManager.UIM.OpenResearchModal();
    }

    public void UIManager_CloseResearchModal() {
    	UIManager.UIM.CloseResearchModal();
    }

    public void UIManager_BuildAction() {
    	UIManager.UIM.BuildAction();
    }
    
    public void UIManager_AttackAction() {
        UIManager.UIM.AttackAction();
    }

    public void UIManager_Test() {
        UIManager.UIM.Test();
    }

    public void GameManager_ProcessTurn() {
    	GameManager.GM.ProcessTurn();
    }
}
