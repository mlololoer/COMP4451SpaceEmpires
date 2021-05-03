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

	void Start() {
		DontDestroyOnLoad(gameObject);
	}

	void TestStart() {
		Debug.Log("TESTSTART - RUN ONLY ONCE");
		Empires.Add(new Empire("John"));
		Empires.Add(new Empire("AIPlayer"));
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
			return true;
		} else if (hexMap.DysonComplete()) {
			Debug.Log("Dyson sphere winner: " + GetActiveEmpire());
			return true;
		}
		return false;
	}


	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		Debug.Log("Main scene loaded back");
		Debug.Log("Battle finished: " + CrossSceneManager.battleFinished);
		Debug.Log("Battle outcome: " + CrossSceneManager.battleOutcome);
		if (CrossSceneManager.battleFinished) {
			foreach (Transform child in GameManager.GM.hexMap.transform) {
	            child.gameObject.GetComponentInChildren< Renderer >().enabled = true;
	        }
		}
	}

	public void LoadFightScene(PureUnit player, PureUnit ai) {
		foreach (Transform child in GameManager.GM.hexMap.transform) {
            child.gameObject.GetComponentInChildren< Renderer >().enabled = false;
        }
        CrossSceneManager.playerUnit = player;
        CrossSceneManager.aiUnit = ai;
        SceneManager.LoadScene ("FightScene");

	}

    //Singleton function
    void Awake() {
    	SceneManager.sceneLoaded += OnSceneLoaded;
		if(GM != null) {GameObject.Destroy(GM);}
		else {GM = this;}
		DontDestroyOnLoad(this);
		TestStart();
	}
}

