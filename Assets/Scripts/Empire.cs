using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empire
{
	public string empireName;
	public List<GameObject> empireShips;
	public List<GameObject> empirePlanets;
	public ResearchProgress researchProgress;
	public int ownedResources;
	public GameObject[,] FOWArray;

	public Empire(string empireName) {
		this.empireName = empireName;
		this.empireShips = new List<GameObject>();
		this.empirePlanets = new List<GameObject>();
		this.researchProgress = new ResearchProgress();
		ownedResources = 0;
	}

}
