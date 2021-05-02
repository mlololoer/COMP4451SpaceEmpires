using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empire
{
	public string empireName;
	public List<GameObject> empireShips = new List<GameObject>();
	public List<GameObject> empirePlanets = new List<GameObject>();
	public ResearchProgress researchProgress;
	public int ownedResources;
	public int shipTier = 1;
	public GameObject[,] FOWArray;

	public Empire(string empireName) {
		this.empireName = empireName;
		this.empireShips = new List<GameObject>();
		this.empirePlanets = new List<GameObject>();
		this.researchProgress = new ResearchProgress();
		ownedResources = 0;
	}

	public void SpawnShip(GameObject planet) {
		ShipInfo info = new ShipInfo(empireName + " Ship", this, shipTier);
		GameObject newShip = GameManager.GM.hexMap.PlaceShipAtPlanet(planet, info);
		if (newShip != null) {
			empireShips.Add(newShip);
		} else {
			Debug.Log("Ship cannot be spawned!");
		}
	}
}
