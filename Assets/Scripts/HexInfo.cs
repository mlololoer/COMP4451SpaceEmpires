public class HexInfo
{
    public string name;
    public int health;

    public HexInfo(string name) {
    	this.name = name;
    }
}

public class ShipInfo : HexInfo {
	int upgradeLevel;
	public ShipInfo(string name, int upgradeLevel) : base(name){
		this.health = 100;
		this.upgradeLevel = upgradeLevel;
	}
}

public class PlanetInfo : HexInfo {

	public PlanetInfo(string name) : base(name){
		this.health = 200;
	}
}