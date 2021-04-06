using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchManager : MonoBehaviour
{
	//have 'active research' to be checked by gamemanager later
	public Text researchText;

	public List<ResearchButton> researchButtonList;

	public void Subscribe(ResearchButton button) {
		if (researchButtonList == null) {
			researchButtonList = new List<ResearchButton>();
		}

		researchButtonList.Add(button);
	}

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

	public void ClearResearchButtons() {
		foreach(ResearchButton button in researchButtonList) {
			Image img = button.GetComponent<Image>();
			img.color = new Color(1f, 1f, 1f, 1f);
		}
	}
}
