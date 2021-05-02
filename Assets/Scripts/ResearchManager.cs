using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchManager : MonoBehaviour
{
	//have 'active research' to be checked by gamemanager later
	public Text researchText;
	Color unavailableColor = new Color(0.5f, 0.5f, 0.5f, 1f);
	Color researcheableColor = new Color(1f, 1f, 1f, 1f);
	Color researchingColor = new Color(1f, 1f, 0f, 1f);
	Color researchedColor = new Color(0f, 1f, 0f, 1f);
	//public Button RB_Heat_0;
	public Button RB_Heat_1;
	public Button RB_Heat_2;
	//public Button RB_Type_0;
	public Button RB_Type_1;
	public Button RB_Type_2;
	//public Button RB_Limits_0;
	public Button RB_Limits_1;
	public Button RB_Limits_2;
	public Button RB_attackBonus;
	public Button RB_healBonus;

	//_offset
	public void PressedRB_Heat_1() {
		int researchType = 0;
		int researchIdx	 = 1;
		GameManager.GM.GetActiveEmpire().researchProgress.SetResearch(researchType, researchIdx);
		UpdateColors();
	}
	//_offset
	public void PressedRB_Heat_2() {
		int researchType = 0;
		int researchIdx	 = 2;
		GameManager.GM.GetActiveEmpire().researchProgress.SetResearch(researchType, researchIdx);
		UpdateColors();
	}
	//_offset
	public void PressedRB_Type_1() {
		int researchType = 1;
		int researchIdx	 = 1;
		GameManager.GM.GetActiveEmpire().researchProgress.SetResearch(researchType, researchIdx);
		UpdateColors();
	}
	//_offset
	public void PressedRB_Type_2() {
		int researchType = 1;
		int researchIdx	 = 2;
		GameManager.GM.GetActiveEmpire().researchProgress.SetResearch(researchType, researchIdx);
		UpdateColors();
	}
	//_offset
	public void PressedRB_Limits_1() {
		int researchType = 2;
		int researchIdx	 = 1;
		GameManager.GM.GetActiveEmpire().researchProgress.SetResearch(researchType, researchIdx);
		UpdateColors();
	}
	//_offset
	public void PressedRB_Limits_2() {
		int researchType = 2;
		int researchIdx	 = 2;
		GameManager.GM.GetActiveEmpire().researchProgress.SetResearch(researchType, researchIdx);
		UpdateColors();
	}
	//_offset
	public void PressedRB_attackBonus() {
		int researchType = 3;
		int researchIdx	 = 1;
		GameManager.GM.GetActiveEmpire().researchProgress.SetResearch(researchType, researchIdx);
		UpdateColors();
	}
	//_offset
	public void PressedRB_healBonus() {
		int researchType = 4;
		int researchIdx	 = 1;
		GameManager.GM.GetActiveEmpire().researchProgress.SetResearch(researchType, researchIdx);
		UpdateColors();
	}

	void UpdateColor(Button button, int researchType, int researchIdx) {
		Image img = button.GetComponent<Image>();
		if (GameManager.GM.GetActiveEmpire().researchProgress.researching && GameManager.GM.GetActiveEmpire().researchProgress.researchingType == researchType && GameManager.GM.GetActiveEmpire().researchProgress.researchingIdx == researchIdx) {
			img.color = researchingColor;
		} else if (GameManager.GM.GetActiveEmpire().researchProgress.CompletedResearch(researchType, researchIdx)) {
			img.color = researchedColor;
		} else if (GameManager.GM.GetActiveEmpire().researchProgress.CanResearch(researchType, researchIdx)) {
			img.color = researcheableColor;
		} else {
			img.color = unavailableColor;
		}
	}

	public void UpdateColors() {
		UpdateColor(RB_Heat_1,0,1);
		UpdateColor(RB_Heat_2,0,2);
		UpdateColor(RB_Type_1,1,1);
		UpdateColor(RB_Type_2,1,2);
		UpdateColor(RB_Limits_1,2,1);
		UpdateColor(RB_Limits_2,2,2);
		UpdateColor(RB_attackBonus,3,1);
		UpdateColor(RB_healBonus,4,1);
	}
	
/*
	public List<ResearchButton> researchButtonList;

	public void Subscribe(ResearchButton button) {
		if (researchButtonList == null) {
			researchButtonList = new List<ResearchButton>();
		}

		researchButtonList.Add(button);
	}
*/
/*

	public void OnTabClicked(ResearchButton button) {
		switch(button.name) {
			case "ResearchButton_1_1": {
				if (GameManager.GM.researchProgress.ResearchPossible(0)) return;
				break;
			}
			case "ResearchButton_1_2": {
				if (GameManager.GM.researchProgress.ResearchPossible(1)) return;
				break;
			}
			case "ResearchButton_1_3": {
				if (GameManager.GM.researchProgress.ResearchPossible(2)) return;
				break;
			}
			case "ResearchButton_2_1": {
				if (GameManager.GM.researchProgress.ResearchPossible(3)) return;
				break;
			}
			case "ResearchButton_2_2": {
				if (GameManager.GM.researchProgress.ResearchPossible(4)) return;
				break;
			}
			case "ResearchButton_2_3": {
				if (GameManager.GM.researchProgress.ResearchPossible(5)) return;
				break;
			}
		}
		ClearResearchButtons();
		Image img = button.GetComponent<Image>();
		img.color = new Color(0f, 1f, 0f, 1f);

	}
*/
	/*
	public void ClearResearchButtons() {
		foreach(ResearchButton button in researchButtonList) {
			Image img = button.GetComponent<Image>();
			img.color = new Color(1f, 1f, 1f, 1f);
		}
	}
	*/
}
