using UnityEngine;
using System.Collections.Generic;
//Contains game-specific information on a certain hex

public class Constants {
	//Research turn costs
	public static int TURNS_FOR_RESEARCH0 = 2;
	public static int TURNS_FOR_RESEARCH1 = 3;
	public static int TURNS_FOR_RESEARCH2 = 5;
	//Research resource costs
	public static int RESOURCES_FOR_RESEARCH0 = 5;
	public static int RESOURCES_FOR_RESEARCH1 = 10;
	public static int RESOURCES_FOR_RESEARCH2 = 20;
	//Resource outputs
	public static int RESOURCES_PER_RESOURCETILE = 10;
	public static int RESOURCES_PER_PLANET = 50;
	//Resource costs
	public static int FRIGATE_COST = 10;
	public static int DESTROYER_COST = 30;
	public static int CRUISER_COST = 50;
	public static int DYSON_COST = 50;
	//Initial health
	public static int PLANET_INITIAL_HEALTH = 250;
	public static int RESOURCE_INITIAL_HEALTH = 100;
	public static int DYSON_INITIAL_HEALTH = 100;
}
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
	public ShipType shipType {get;}
	int attackPower = 10;
	public int remainingMoves {get; set;}
	public ShipInfo(string name, Empire parentEmpire, GameObject parentGO, ShipType shipType) : base(name, parentEmpire, parentGO){
		this.health = 100;
		this.shipType = shipType;
		resetBeforeTurn();
	}

	public void resetBeforeTurn() {
		remainingMoves = 3-((int)shipType);
	}
	public HexMapLayer attackable() {
		Debug.Log("start attackable");
		CubicHexComponent thisCHC = this.ParentGameObject.GetComponent<CubicHexComponent>();
		Debug.Log("thisCHC:"+thisCHC);
		GameObject tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.RESOURCE_LAYER);
		Debug.Log("tileBelow:"+tileBelow);
		if (tileBelow != null && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != ParentEmpire && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != null) {
			Debug.Log("returntrue");
			return HexMapLayer.RESOURCE_LAYER;
		}
		tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.PLANET_LAYER);
		if (tileBelow != null && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != ParentEmpire && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != null) {
			return HexMapLayer.PLANET_LAYER;
		}
		return HexMapLayer.BACKGROUND_LAYER;
	}
	public HexMapLayer buildable() {
		Debug.Log("start buildable");
		CubicHexComponent thisCHC = this.ParentGameObject.GetComponent<CubicHexComponent>();
		Debug.Log("thisCHC:"+thisCHC);
		GameObject tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.RESOURCE_LAYER);
		Debug.Log("tileBelow:"+tileBelow);
		if (tileBelow != null && (tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire == null)) {
			Debug.Log("Tile below ship's empire is:"+tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire);
			return HexMapLayer.RESOURCE_LAYER;
		}
		tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.PLANET_LAYER);
		if (tileBelow != null && (tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire == null)) {
			Debug.Log("Tile below ship's empire is:"+tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire);
			return HexMapLayer.PLANET_LAYER;
		}
		if (this.ParentEmpire.CanSpendResources(Constants.DYSON_COST) && GameManager.GM.hexMap.CanPlaceDyson(thisCHC.Hex)) {
			Debug.Log("Can place a dyson tile");
			return HexMapLayer.DYSON_LAYER;
		}
		return HexMapLayer.BACKGROUND_LAYER;
	}
	public void attackTile() {
		Debug.Log("startattacktile");
		CubicHexComponent thisCHC = this.ParentGameObject.GetComponent<CubicHexComponent>();
		GameObject tileBelow = GameManager.GM.hexMap.GetHex(thisCHC.Hex, HexMapLayer.RESOURCE_LAYER);
		Debug.Log("tileBelow:"+tileBelow);
		if (tileBelow != null && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != ParentEmpire && tileBelow.GetComponent<CubicHexComponent>().Info.ParentEmpire != null) {
			Debug.Log("inside resource tile");
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
		if (GameManager.GM.hexMap.CanPlaceDyson(thisCHC.Hex)) {
			Debug.Log("Building Dyson");
			ParentEmpire.SpendResources(Constants.DYSON_COST);
			GameManager.GM.hexMap.PlaceDyson(thisCHC.Hex);
		}
	}


}

public class PlanetInfo : HexInfo {
	bool hasSpawnedShip = false;
	public PlanetInfo(string name, Empire parentEmpire, GameObject parentGO) : base(name, parentEmpire, parentGO){
		this.health = Constants.PLANET_INITIAL_HEALTH;
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
		this.health = Constants.PLANET_INITIAL_HEALTH;
	}
	public bool canSpawnShip(ShipType shipType) {
		switch(shipType) {
			case (ShipType.FRIGATE): {
				if (ParentEmpire.researchProgress.GetShipTypesLevel() < 0 || !ParentEmpire.CanSpendResources(Constants.FRIGATE_COST)) {
					return false;
				} break;
			}
			case (ShipType.DESTROYER): {
				if (ParentEmpire.researchProgress.GetShipTypesLevel() < 1 || !ParentEmpire.CanSpendResources(Constants.DESTROYER_COST)) {
					return false;
				} break;
			}
			case (ShipType.CRUISER): {
				if (ParentEmpire.researchProgress.GetShipTypesLevel() < 2 || !ParentEmpire.CanSpendResources(Constants.CRUISER_COST)) {
					return false;
				} break;
			}
		}
		return (!hasSpawnedShip) && (!(GameManager.GM.hexMap.Collides(this.ParentGameObject.GetComponent<CubicHexComponent>().Hex)));
	}
	public void spawnShip(ShipType shipType) {
		ShipInfo info = new ShipInfo(ParentEmpire.empireName + " Ship", ParentEmpire, null, shipType);
		GameObject newShip = GameManager.GM.hexMap.PlaceShipAtPlanet(this.ParentGameObject, info);
		if (newShip != null) {
			hasSpawnedShip = true;
			ParentEmpire.AddShip(newShip);
			switch(shipType) {
				case (ShipType.FRIGATE): {ParentEmpire.SpendResources(Constants.FRIGATE_COST);break;}
				case (ShipType.DESTROYER): {ParentEmpire.SpendResources(Constants.DESTROYER_COST);break;}
				case (ShipType.CRUISER): {ParentEmpire.SpendResources(Constants.CRUISER_COST);break;}
			}
		} else {
			Debug.Log("Ship cannot be spawned!");
		}
	}
}

public class ResourceInfo : HexInfo {
	public ResourceInfo(string name, Empire parentEmpire, GameObject parentGO) : base(name, parentEmpire, parentGO){
		this.health = Constants.RESOURCE_INITIAL_HEALTH;
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
		this.health = Constants.RESOURCE_INITIAL_HEALTH;
	}
}