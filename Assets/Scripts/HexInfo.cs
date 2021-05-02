using UnityEngine;
using System.Collections.Generic;
//Contains game-specific information on a certain hex
public class HexInfo
{
    public string name;
    public int health;
    public Empire ParentEmpire;

    public HexInfo(string name, Empire parentEmpire) {
    	this.name = name;
    	this.ParentEmpire = parentEmpire;
    }
}

public class ShipInfo : HexInfo {
	Queue<CubicHex> queuedPath;
	int tier;
	public int remainingMoves {get;} = 2;
	public ShipInfo(string name, Empire parentEmpire, int tier) : base(name, parentEmpire){
		this.health = 100;
		this.tier = tier;
	}

}

public class PlanetInfo : HexInfo {
	public PlanetInfo(string name, Empire parentEmpire) : base(name, parentEmpire){
		this.health = 200;
	}

}

public class ResourceInfo : HexInfo {
	
	public ResourceInfo(string name, Empire parentEmpire) : base(name, parentEmpire){
		this.health = 200;
	}
}