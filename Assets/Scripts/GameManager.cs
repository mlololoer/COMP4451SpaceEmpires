using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager GM;
	//Player's info
	public ResearchProgress researchProgress;
	public string empireName = "Nebuchadnezzar";
	public SpaceshipTile[] empireSpaceships = {};
	public PlanetTile[] empirePlanets = {};
	//public PlanetTile[] empirePlanets;
	public int ownedResources;

    public void ProcessTurn() {
    	//process production in all planets
    	//process spaceship actions in all spaceships
    	//calculate research progress
    	researchProgress.ProcessTurn();
    }

    void Awake() {
		if(GM != null) {
			GameObject.Destroy(GM);
		} else {
			GM = this;
		}
		researchProgress = new ResearchProgress();
		DontDestroyOnLoad(this);
	}
}

public class ResearchProgress {
	/*	Research Hierarchy (for now)
		Spaceship Heat Shield 1 -> Spaceship Heat Shield 2 -> Spaceship Heat Shield 3	(higher heat shield: reach closer to central star)
		Spaceship Durability 1 -> Spaceship Durability 2 -> Spaceship Durability 3	(higher durability: unlock spaceships with more health and weapons)
		
	*/	
	readonly int[] turnsToCompleteResearch = new int[3] {3,5,7};
	int[] completedResearch =  new int[6];
	int activeResearchIndex;

	public void ProcessTurn() {
		if (completedResearch[activeResearchIndex] >= turnsToCompleteResearch[activeResearchIndex%3]) {
			//a research has copmleted, announce so
		} else {
			completedResearch[activeResearchIndex]++;
		}
	}
	public bool ResearchPossible(int researchIndex) {
		bool incompleteFound = false;
		for (int i = 0; i < researchIndex%3; ++i) {
			if (completedResearch[(researchIndex/3) + i] < turnsToCompleteResearch[i]) incompleteFound = true;
		}
		if (!incompleteFound) {
			activeResearchIndex = researchIndex;
		}
		return incompleteFound;
	}
}