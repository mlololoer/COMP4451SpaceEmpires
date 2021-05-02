using UnityEngine;
using System.Collections.Generic;
//Contains game-specific information on a certain hex
public class HexInfo
{
	public GameObject ParentGameObject;
    public string name;
    public int health;
    public Empire ParentEmpire;

    public HexInfo(string name, Empire parentEmpire, GameObject parentGO) {
    	this.name = name;
    	this.ParentEmpire = parentEmpire;
    	this.ParentGameObject = parentGO;
    }
}
public enum ShipType {FRIGATE, DESTROYER, CRUISER};
/*public class ShipDetails {
	string unitName;
	ShipType unitClass;
	bool hlth_upgrade;
	bool dmg_upgrade;
	int damage;
	int maxHP;
	int currentHP;
	int heal;
}*/

public class ShipInfo : HexInfo {
	Queue<CubicHex> queuedPath;
	int tier;
	int attackPower = 10;
	public int remainingMoves {get; set;}
	public ShipInfo(string name, Empire parentEmpire, GameObject parentGO, int tier) : base(name, parentEmpire, parentGO){
		this.health = 100;
		this.tier = tier;
		resetBeforeTurn();
	}

	public void resetBeforeTurn() {
		remainingMoves = 3-tier;
	}
	public bool attackable() {
		Debug.Log("start attackable");
		CubicHexComponent thisCHC = this.ParentGameObject.GetComponent<CubicHexComponent>();
		Debug.Log("thisCHC:"+thisCHC);
		GameObject tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.RESOURCE_LAYER);
		Debug.Log("tileBelow:"+tileBelow);
		if (tileBelow != null && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != ParentEmpire && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != null) {
			Debug.Log("returntrue");
			return true;
		}
		tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.PLANET_LAYER);
		if (tileBelow != null && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != ParentEmpire && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != null) {
			return true;
		}
		return false;
	}
	public bool buildable() {
		Debug.Log("start buildable");
		CubicHexComponent thisCHC = this.ParentGameObject.GetComponent<CubicHexComponent>();
		Debug.Log("thisCHC:"+thisCHC);
		GameObject tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.RESOURCE_LAYER);
		Debug.Log("tileBelow:"+tileBelow);
		if (tileBelow != null && (tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire == null)) {
			Debug.Log("Tile below ship's empire is:"+tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire);
			return true;
		}
		tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.PLANET_LAYER);
		if (tileBelow != null && (tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire == null)) {
			Debug.Log("Tile below ship's empire is:"+tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire);
			return true;
		}
		return false;
	}
	public void attackTile() {
		Debug.Log("startattacktile");
		CubicHexComponent thisCHC = this.ParentGameObject.GetComponent<CubicHexComponent>();
		GameObject tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.RESOURCE_LAYER);
		Debug.Log("tileBelow:"+tileBelow);
		if (tileBelow != null && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != ParentEmpire && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != null) {
			Debug.Log("inside research tile");
			((ResourceInfo)tileBelow.GetComponent<CubicHexComponent>().Info).inflictAttack(attackPower);
		}
		tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.PLANET_LAYER);
		if (tileBelow != null && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != ParentEmpire && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != null) {
			Debug.Log("Tile below ship's empire is:"+tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire);
			PlanetInfo planetToAttack = ((PlanetInfo)tileBelow.GetComponent<CubicHexComponent>().Info);
			planetToAttack.inflictAttack(attackPower);
		}
	}
	public void buildOnTile() {
		Debug.Log("start buildontile");
		CubicHexComponent thisCHC = this.ParentGameObject.GetComponent<CubicHexComponent>();
		Debug.Log("thisCHC:"+thisCHC);
		GameObject tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.RESOURCE_LAYER);
		Debug.Log("tileBelow:"+tileBelow);
		if (tileBelow != null && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire == null) {
			Debug.Log("in tilebelowbuild");
			ResourceInfo build = ((ResourceInfo)tileBelow.GetComponent<CubicHexComponent>().Info);
			build.buildStation(this.ParentEmpire);
		}
		tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.PLANET_LAYER);
		if (tileBelow != null && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire == null) {
			PlanetInfo build = ((PlanetInfo)tileBelow.GetComponent<CubicHexComponent>().Info);
			build.buildCity(this.ParentEmpire);
		}
	}


}

public class PlanetInfo : HexInfo {
	bool hasSpawnedShip = false;
	public PlanetInfo(string name, Empire parentEmpire, GameObject parentGO) : base(name, parentEmpire, parentGO){
		this.health = 250;
	}
	public void resetBeforeTurn() {
		this.hasSpawnedShip = false;
	}
	public void inflictAttack(int damage) {
		if (this.health - damage <= 0) {
			//destroy self
			this.ParentEmpire.RemovePlanet(this.ParentGameObject);
			this.ParentEmpire = null;
			this.health = 0;
		} else {
			this.health -= damage;
		}
	}
	public void buildCity(Empire owner) {
		this.ParentEmpire = owner;
		Debug.Log("Planet now owned by "+this.ParentEmpire);
		this.ParentEmpire.AddPlanet(this.ParentGameObject);
		this.health = 250;
	}
	public bool canSpawnShip() {
		return (!hasSpawnedShip) && (!(GameManager.GM.hexMap.Collides(this.ParentGameObject.GetComponent<CubicHexComponent>().Hex)));
	}
	public void spawnShip() {
		ShipInfo info = new ShipInfo(ParentEmpire.empireName + " Ship", ParentEmpire, null, ParentEmpire.shipTier);
		GameObject newShip = GameManager.GM.hexMap.PlaceShipAtPlanet(this.ParentGameObject, info);
		if (newShip != null) {
			hasSpawnedShip = true;
			ParentEmpire.AddShip(newShip);
		} else {
			Debug.Log("Ship cannot be spawned!");
		}
	}
}

public class ResourceInfo : HexInfo {
	public ResourceInfo(string name, Empire parentEmpire, GameObject parentGO) : base(name, parentEmpire, parentGO){
		this.health = 0;
	}
	public void inflictAttack(int damage) {
		Debug.Log("damaged");
		if (this.health - damage <= 0) {
			//Resources never disappear, they only lose an owner
			this.ParentEmpire.RemoveResource(this.ParentGameObject);
			this.ParentEmpire = null;
			this.health = 0;
		} else {
			this.health -= damage;
		}
	}
	public void buildStation(Empire owner) {
		this.ParentEmpire = owner;
		Debug.Log("Resource now owned by "+this.ParentEmpire);
		this.ParentEmpire.AddResource(this.ParentGameObject);
		this.health = 100;
	}
}