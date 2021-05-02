using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empire
{
	public string empireName;
	public List<GameObject> empireShips = new List<GameObject>();
	public List<GameObject> empirePlanets = new List<GameObject>();
	public List<GameObject> empireResources = new List<GameObject>();
	public ResearchProgress researchProgress;
	public int ownedResources = 0;
	public int shipTier = 0;
	public GameObject[,] FOWArray;

	public Empire(string empireName) {
		this.empireName = empireName;
		this.empireShips = new List<GameObject>();
		this.empirePlanets = new List<GameObject>();
		this.empireResources = new List<GameObject>();
		this.researchProgress = new ResearchProgress();
		ownedResources = 0;
	}

	/*public void SpawnShip(GameObject planet) {
		ShipInfo info = new ShipInfo(empireName + " Ship", this, null, shipTier);
		GameObject newShip = GameManager.GM.hexMap.PlaceShipAtPlanet(planet, info);
		if (newShip != null) {
			empireShips.Add(newShip);
		} else {
			Debug.Log("Ship cannot be spawned!");
		}
	}*/


	public void NewTurn() {
		foreach(GameObject resource in empireResources) {
			ownedResources += 10;
		}
		for (int i = 0; i < empireShips.Count; ++i) {
    		ShipInfo info = (ShipInfo)empireShips[i].GetComponent<CubicHexComponent>().Info;
    		info.resetBeforeTurn();
    	}
    	for (int i = 0; i < empirePlanets.Count; ++i) {
    		PlanetInfo info = (PlanetInfo)empirePlanets[i].GetComponent<CubicHexComponent>().Info;
    		Debug.Log("Resetting planet...");
    		info.resetBeforeTurn();
    	}
	}

	public void AddShip(GameObject ship) {Debug.Log("SHIP ADDDE: "+ship);this.empireShips.Add(ship);}
	public void RemoveShip(GameObject ship) {this.empireShips.Remove(ship);}
	public void AddResource(GameObject resource) {this.empireResources.Add(resource);}
	public void RemoveResource(GameObject resource) {this.empireResources.Remove(resource);}
	public void AddPlanet(GameObject planet) {this.empirePlanets.Add(planet);}
	public void RemovePlanet(GameObject planet) {this.empirePlanets.Remove(planet);}
}
