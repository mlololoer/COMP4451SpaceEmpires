using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ShipType {FRIGATE, DESTROYER, CRUISER};
public class Unit : MonoBehaviour
{

	public string unitName;
	public ShipType unitClass;
	public bool hlth_upgrade;
	public bool dmg_upgrade;
	public int damage;
	public int maxHP;
	public int currentHP;

	public Unit(string unitName, ShipType unitClass, bool hlth_upgrade, bool dmg_upgrade, int damage, int maxHP, int currentHP) {
		this.unitName = unitName;
		this.unitClass = unitClass;
		this.hlth_upgrade = hlth_upgrade;
		this.dmg_upgrade = dmg_upgrade;
		this.damage = damage;
		this.maxHP = maxHP;
		this.currentHP = currentHP;
	}

	public bool TakeDamage(int dmg)
	{
		currentHP -= dmg;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

}

public class PureUnit
{

	public string unitName;
	public ShipType unitClass;
	public bool hlth_upgrade;
	public bool dmg_upgrade;
	public int damage;
	public int maxHP;
	public int currentHP;

	public PureUnit(string unitName, ShipType unitClass, bool hlth_upgrade, bool dmg_upgrade, int damage, int maxHP, int currentHP) {
		this.unitName = unitName;
		this.unitClass = unitClass;
		this.hlth_upgrade = hlth_upgrade;
		this.dmg_upgrade = dmg_upgrade;
		this.damage = damage;
		this.maxHP = maxHP;
		this.currentHP = currentHP;
	}
}
