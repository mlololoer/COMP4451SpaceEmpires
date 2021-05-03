using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

	public Text nameText;
	public Text levelText;
	public Slider hpSlider;

	public void SetHUD(Unit unit)
	{
		nameText.text = unit.unitName;
		switch(unit.unitClass) {
			case (ShipType.FRIGATE): {
				levelText.text =  "Frigate";
				break;
			}
			case (ShipType.DESTROYER): {
				levelText.text =  "Destroyer";
				break;
			}
			case (ShipType.CRUISER): {
				levelText.text =  "Cruiser";
				break;
			}
		}
		
		hpSlider.maxValue = unit.maxHP;
		hpSlider.value = unit.currentHP;
	}

	public void SetHP(int hp)
	{
		hpSlider.value = hp;
	}

}
