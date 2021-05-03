using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Handles turns for every player
public class GameManager : MonoBehaviour
{
	public static GameManager GM;

	//Player's info
	public List<Empire> Empires = new List<Empire>();

	int empireIndex;
	//Hexmap with all units, planets, tiles, etc
	public HexMap hexMap;
	int turn = 0;

	void SetupEverything() {
		Debug.Log("RUN ONLY ONCE");
		Empires.Add(new Empire(CrossSceneManager.EmpireName));
		Empires.Add(new Empire("AI Player"));
		hexMap = GameObject.Find("HexMap").GetComponent<HexMap>();
		hexMap.GenerateMap(0.05f, 0.2f, Empires);
	}

	

	
	/*	On a player's turn, they are free to:
			select and move/modify ships
				when a ship is moved, if it moves past an enemy ship too close, a fight scene can be triggered
			select and modify planets
			conduct research
	*/
    public void ProcessTurn() {
    	//BEFORE a player starts their turn, process all research/building stuff.
    	if (WinningCondition()) {
    		return;
    	}
    	++turn;
    	empireIndex = (empireIndex+1)%Empires.Count;
    	//do not process turns if it is the very first turn for each player
    	if (turn >= Empires.Count) {
    		GetActiveEmpire().NewTurn();
    	}
    	
    	//process spaceship actions in all spaceships
    	//calculate research researchProgress
    	//foreach(Empire empire in Empires) {
    	//	empire.researchProgress.ProcessTurn();
    	//}
    	hexMap.rotateRing(2, HexMapLayer.ASTEROID_LAYER);
    	SelectionManager.SM.ClearAllSelections();
    	
    }

    public Empire GetActiveEmpire() {
		return Empires[empireIndex];
	}

	bool WinningCondition() {
		Empire remainingEmpire = null;
		int empiresWithPlanets = 0;
		foreach (Empire empire in Empires) {
			if (empire.empirePlanets.Count != 0) {
				++empiresWithPlanets;
			} else {
				remainingEmpire = empire;
			}
		}
		if (empiresWithPlanets == 1) {
			Debug.Log("Last standing winner: " + remainingEmpire.empireName);
			CrossSceneManager.humanWon = remainingEmpire == Empires[0];
			SceneManager.LoadScene ("Gameover");
			return true;
		} else if (hexMap.DysonComplete()) {
			Debug.Log("Dyson sphere winner: " + GetActiveEmpire());
			CrossSceneManager.humanWon = GetActiveEmpire() == Empires[0];
			SceneManager.LoadScene ("Gameover");
			return true;
		}
		
		return false;
	}
	
	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		Debug.Log("Main scene loaded back");
		Debug.Log("Battle finished: " + CrossSceneManager.battleFinished);
		Debug.Log("Battle outcome: " + CrossSceneManager.battleOutcome);
		if (CrossSceneManager.battleFinished) {
	        CrossSceneManager.battleFinished = false;
			ShowAll();
			if (playerTemp == null || aiTemp == null) {
				Debug.Log("Two ships are null");
			}
	        if (CrossSceneManager.battleOutcome) {
	        	//Human wins
	        	((ShipInfo)playerTemp.GetComponent<CubicHexComponent>().Info).shipUnit = CrossSceneManager.playerUnit;
	        	((ShipInfo)aiTemp.GetComponent<CubicHexComponent>().Info).destroy();
	        } else {
	        	((ShipInfo)aiTemp.GetComponent<CubicHexComponent>().Info).shipUnit = CrossSceneManager.aiUnit;
	        	((ShipInfo)playerTemp.GetComponent<CubicHexComponent>().Info).destroy();
	        }
	        Debug.Log("AFTER FIGHT HEALTH:" + ((ShipInfo)playerTemp.GetComponent<CubicHexComponent>().Info).shipUnit.currentHP);
	        playerTemp = null;
	        	aiTemp = null;
		}
	}
	GameObject playerTemp = null;
	GameObject aiTemp = null;
	public void LoadFightScene(GameObject player, GameObject ai) {
		HideAll();
		Debug.Log("BEFORE FIGHT HEALTH:" + ((ShipInfo)player.GetComponent<CubicHexComponent>().Info).shipUnit.currentHP);
        CrossSceneManager.playerUnit = ((ShipInfo)player.GetComponent<CubicHexComponent>().Info).shipUnit;
        CrossSceneManager.aiUnit = ((ShipInfo)ai.GetComponent<CubicHexComponent>().Info).shipUnit;
        playerTemp = player;
        aiTemp = ai;

        SceneManager.LoadScene ("FightScene");
	}

	public void HideAll() {
		foreach (Transform child in GameManager.GM.hexMap.transform) {
            child.gameObject.GetComponentInChildren< Renderer >().enabled = false;
        }
        GameManager.GM.GetComponentInChildren<Canvas>().enabled = false;
	}
	void ShowAll() {
		foreach (Transform child in GameManager.GM.hexMap.transform) {
            child.gameObject.GetComponentInChildren< Renderer >().enabled = true;
        }
        GameManager.GM.GetComponentInChildren<Canvas>().enabled = true;
	}


    //Singleton function
	static bool alrun = false;
    void Awake() {
    	if (alrun) {
    		if (this == GM) {return;}
    		else {GameObject.Destroy(this.gameObject); return;}
    	}
		if(GM != null) {GameObject.Destroy(GM);}
		else {GM = this;}
		DontDestroyOnLoad(this.gameObject);
		alrun = true;

		SceneManager.sceneLoaded += OnSceneLoaded;
		SetupEverything();
	}

}

