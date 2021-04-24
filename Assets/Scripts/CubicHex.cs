using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubicDirection {NORTH,NORTHEAST,SOUTHEAST,SOUTH,SOUTHWEST,NORTHWEST};

//Helper class for converting between Unity worldspace and cubic hex coordinate systems
public class CubicHex
{
	public static Vector3Int[] DirectionBases = {new Vector3Int(0,1,-1),new Vector3Int(1,0,-1),new Vector3Int(1,-1,0),new Vector3Int(0,-1,1),new Vector3Int(-1,0,1),new Vector3Int(-1,1,0)};
    
    public CubicHex Adjacent(CubicDirection dir) {
    	return new CubicHex(this.x + DirectionBases[(int)dir].x, this.y + DirectionBases[(int)dir].y);
    }

    //Cubic coordinates
    public int x;
    public int y;
    public int z;
    //radius will always stay the same
    public static float radius = 0.5f;

    public CubicHex(int x, int y) {
    	this.x = x;
    	this.y = y;
    	this.z = -(x + y);
    }

    public CubicHex(Vector3Int coords) {
        this.x = coords.x;
        this.y = coords.y;
        if (coords.z != -(x + y)) {
            Debug.Log("Invalid cubic hex coordinates");
        }
        this.z = -(x + y);
    }

    public Vector3Int GetCoords() {
        return new Vector3Int(x, y, z);
    }

    public void SetCoords(int x, int y) {
        this.x = x;
        this.y = y;
        this.z = -(x + y);
    }


    public static float Width() {
    	return 2 * radius;
    }
    public static float Height() {
    	return (Mathf.Sqrt(3)) * radius;
    }

    public Vector3 WorldPosition() {
    	// horizontally, 1 unit is offset by 3/4 the width
    	// vertically, 1 unit is offset by its height (also depending on its x, whether to 'bump' up or not)
    	return new Vector3(x * 0.75f * Width(), (y + (x / 2f)) * Height(), 0);
    }
    
}
