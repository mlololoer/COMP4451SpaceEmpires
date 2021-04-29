using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles turns for every player
public class GameManager : MonoBehaviour
{
	public static GameManager GM;

	//Player's info
	public List<Empire> Empires = new List<Empire>();

	public Empire ActiveEmpire;
	//Hexmap with all units, planets, tiles, etc
	public HexMap hexMap;

	void Start() {
		Empires.Add(new Empire("John"));
		Empires.Add(new Empire("AIPlayer"));
		hexMap = GameObject.Find("HexMap").GetComponent<HexMap>();
		//FOWManager.FM.hexMap = hexMap;
		//FOWManager.FM.initializeFOW(new CubicHex(0,0));
	}

    public void ProcessTurn() {
    	//process spaceship actions in all spaceships
    	//calculate research progress
    	foreach(Empire empire in Empires) {
    		empire.researchProgress.ProcessTurn();
    	}
    	hexMap.rotateRing(3, HexMapLayer.ASTEROID_LAYER);
    	//SelectionManager.SM.ProcessMovement();
    }

    //Singleton function
    void Awake() {
		if(GM != null) {GameObject.Destroy(GM);}
		else {GM = this;}
		DontDestroyOnLoad(this);
	}
}

