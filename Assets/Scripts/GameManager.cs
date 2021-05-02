using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	void Start() {
		Empires.Add(new Empire("John"));
		Empires.Add(new Empire("AIPlayer"));
		hexMap = GameObject.Find("HexMap").GetComponent<HexMap>();
		//FOWManager.FM.hexMap = hexMap;
		//FOWManager.FM.initializeFOW(new CubicHex(0,0));
	}

	

	
	/*	On a player's turn, they are free to:
			select and move/modify ships
				when a ship is moved, if it moves past an enemy ship too close, a fight scene can be triggered
			select and modify planets
			conduct research
	*/
    public void ProcessTurn() {
    	//process spaceship actions in all spaceships
    	//calculate research researchProgress
    	foreach(Empire empire in Empires) {
    		empire.researchProgress.ProcessTurn();
    	}
    	hexMap.rotateRing(3, HexMapLayer.ASTEROID_LAYER);
    	//SelectionManager.SM.ProcessMovement();
    	empireIndex++;
    }

    public Empire GetActiveEmpire() {
		return Empires[empireIndex];
	}


    //Singleton function
    void Awake() {
		if(GM != null) {GameObject.Destroy(GM);}
		else {GM = this;}
		DontDestroyOnLoad(this);
	}
}

