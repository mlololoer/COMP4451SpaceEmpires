using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empire
{
	public string empireName;
	public List<GameObject> empireShips = new List<GameObject>();
	public List<GameObject> empirePlanets = new List<GameObject>();
	public List<GameObject> empireResources = new List<GameObject>();
	public List<GameObject> empireDysons = new List<GameObject>();
	public ResearchProgress researchProgress {get;}
	public int ownedResources {get; private set;} = 0;
	public GameObject[,] FOWArray;


	public Empire(string empireName) {
		ownedResources = 0;
		this.empireName = empireName;
		this.empireShips = new List<GameObject>();
		this.empirePlanets = new List<GameObject>();
		this.empireResources = new List<GameObject>();
		this.empireDysons = new List<GameObject>();
		this.researchProgress = new ResearchProgress(this);

	}

	public void NewTurn() {

		researchProgress.ProcessTurn();
		//check whether research on health/damage is done

		foreach(GameObject resource in empireResources) {
			ownedResources += Constants.RESOURCES_PER_RESOURCETILE;
		}
		for (int i = 0; i < empireShips.Count; ++i) {
			//restore health
    		ShipInfo info = (ShipInfo)empireShips[i].GetComponent<CubicHexComponent>().Info;
    		info.resetBeforeTurn();
    		info.shipUnit.hlth_upgrade = researchProgress.GetHealBonus();
    		info.shipUnit.dmg_upgrade = researchProgress.GetAttackBonus();
    	}
    	for (int i = 0; i < empirePlanets.Count; ++i) {
    		//restore health
    		ownedResources += Constants.RESOURCES_PER_PLANET;
    		PlanetInfo info = (PlanetInfo)empirePlanets[i].GetComponent<CubicHexComponent>().Info;
    		info.resetBeforeTurn();
    	}
	}
	public bool CanSpendResources(int amount) {
		return ((ownedResources - amount) >= 0);
	}
	public bool SpendResources(int amount) {
		if (CanSpendResources(amount)) {
			ownedResources -= amount;
			return true;
			
		} else {
			Debug.Log("NEGATIVE BALANCE");
			return false;
		}
	}

	public void AddShip(GameObject ship) {this.empireShips.Add(ship);}
	public void RemoveShip(GameObject ship) {this.empireShips.Remove(ship);}
	public void AddResource(GameObject resource) {this.empireResources.Add(resource);}
	public void RemoveResource(GameObject resource) {this.empireResources.Remove(resource);}
	public void AddPlanet(GameObject planet) {this.empirePlanets.Add(planet);}
	public void RemovePlanet(GameObject planet) {this.empirePlanets.Remove(planet);}
}
