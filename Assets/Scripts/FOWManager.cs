using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FOWManager : MonoBehaviour
{
	public static FOWManager FM;

	public HexMap hexMap;

	public void ClearFOW(CubicHexComponent hexComponent) {
		foreach (CubicDirection dir in Enum.GetValues(typeof(CubicDirection))) {
			Vector3Int adjacent = CubicHex.DirectionBases[(int)dir] + hexComponent.Hex.GetCoords();

			GameObject.Destroy(hexComponent.ParentHexMap.FOWArray[30+adjacent.x, 15+adjacent.y]);
		}
		int centerX = hexComponent.Hex.GetCoords().x;
		int centerY = hexComponent.Hex.GetCoords().y;
		GameObject.Destroy(hexComponent.ParentHexMap.FOWArray[30+centerX, 15+centerY]);
	}

	public void initializeFOW(CubicHex start) {
		
	}

    //Singleton implementation
	void Awake() {
		if(FM != null)
			GameObject.Destroy(FM);
		else
			FM = this;
		DontDestroyOnLoad(this);
	}
}
