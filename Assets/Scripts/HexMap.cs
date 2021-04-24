using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

public class HexMap : MonoBehaviour
{
	public const int BACKGROUND_SORT_ORDER = 0;
	//is smaller than...
	public const int PLANET_SORT_ORDER = 1;
	//is smaller than...
	public const int SHIP_SORT_ORDER = 2;

	const int MAX_MAP_WIDTH = 30;
	const int MAX_MAP_HEIGHT = 30;

	const int MAP_X_OFFSET = MAX_MAP_WIDTH/2;
	const int MAP_Y_OFFSET = MAX_MAP_HEIGHT/2;

    int MAP_RADIUS = 10;

	//prefab hex's radius (not needed, defined in CubicHex anyway)
	const float HEX_RADIUS = 0.5f;

	public GameObject HexTilePrefab;

    public Sprite BACKGROUND_SPRITE;
    public Sprite PLANET_SPRITE;
    public Sprite ASTEROID_SPRITE;
    public Sprite SUN_SPRITE;
    public Sprite RESOURCE_SPRITE;

    public Sprite SHIP_SPRITE;

    //Stores all background hexes generated, by coordinates
    GameObject[,] HexArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores all ships by coordinates
    GameObject[,] ShipArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores all planets by coordinates
    GameObject[,] PlanetArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Each player's FOW may be different, so we don't include an array for it here.

    void Start() {
        generateMap(0.05f, 0.2f);
        initializeShip(2, 3);
    }

    //default hex dimensions: 	radius of 0.5 Unity in-world units 
    public void generateMap(float resourceDensity, float asteroidDensity) {
    	
    	//the very CENTER hex is always the SUN.
    	initializeHex(0, 0, BACKGROUND_SORT_ORDER, SUN_SPRITE);

    	//hex ring with radius ringRadius
    	for (int ringRadius = 1; ringRadius <= MAP_RADIUS; ++ringRadius) {

    		//get each corner hex that is ringRadius hexes away from the center
    		foreach (CubicDirection dir in Enum.GetValues(typeof(CubicDirection))) {
                Vector3Int basePos = CubicHex.DirectionBases[(int)dir]*ringRadius;
                CubicHex baseDir = new CubicHex(basePos.x, basePos.y);

    			//iterate through the line from the corner hex to the next corner hex
    			for (int stepLen = 0; stepLen < ringRadius; ++stepLen) {
	    			//Decisions for what the hex's texture should be

	    			//For the hexes immediately surrounding the SUN, they MUST be empty.
    				if (ringRadius == 1) {
    					initializeHex(baseDir.x, baseDir.y, BACKGROUND_SORT_ORDER, BACKGROUND_SPRITE);
    				}

    				//Ring 3 should contain intermittent asteroids.
    				else if (ringRadius == 3) {
    					if (UnityEngine.Random.Range(0.0f, 1.0f) < asteroidDensity) {
    						initializeHex(baseDir.x, baseDir.y, BACKGROUND_SORT_ORDER, ASTEROID_SPRITE);
    					} else {
    						initializeHex(baseDir.x, baseDir.y, BACKGROUND_SORT_ORDER, BACKGROUND_SPRITE);
    					}
    				}

    				//Ring 7 should contain the planets.
    				else if (ringRadius == 7) {
    					//only at northernmost and southernmost points of the ring (i.e. 2 players for now)
    					if ((dir == CubicDirection.NORTH || dir == CubicDirection.SOUTH) && stepLen == 0) {
    						initializePlanet(baseDir.x, baseDir.y);
    					} else {
    						initializeHex(baseDir.x, baseDir.y, BACKGROUND_SORT_ORDER, BACKGROUND_SPRITE);
    					}
    				}

    				//All other rings should be empty (to be filled with resources)
    				else {
    					if (UnityEngine.Random.Range(0.0f, 1.0f) < resourceDensity) {
    						initializeHex(baseDir.x, baseDir.y, BACKGROUND_SORT_ORDER, RESOURCE_SPRITE);
    					} else {
    						initializeHex(baseDir.x, baseDir.y, BACKGROUND_SORT_ORDER, BACKGROUND_SPRITE);
    					}
    					
    				}

    				//step towards next hex (wrap around NORTHWEST->NORTH)
    				baseDir = baseDir.Adjacent((CubicDirection)(((int)dir + 2) % 6));
    			}
    		}
    	}
    }

    /*
              X
            !   O
          O       !
        X           X
        !           O
        O           !
        X           X
          !       O
            O   !
              X
    For each subsection of the hex ring, we first replace the very first hex with previous.
    Then we successively shift the hexes over.
    Store the very last hex of the subsection into the same temporary variable, to replace on the next subsection.
    */

    public void rotateRing(int ringNum) {
		//temporarily store the very last hex to be swapped
        CubicHex finalHexCoords = new CubicHex(CubicHex.DirectionBases[(int)CubicDirection.NORTH]*ringNum);
        finalHexCoords = finalHexCoords.Adjacent(CubicDirection.SOUTHWEST);

		GameObject replacementHex = HexArray[MAP_X_OFFSET + finalHexCoords.x, MAP_Y_OFFSET + finalHexCoords.y];
        GameObject temporaryHex = null;

		foreach (CubicDirection dir in Enum.GetValues(typeof(CubicDirection))) {
			CubicHex baseDir = new CubicHex(CubicHex.DirectionBases[(int)dir]*ringNum);
			
			for (int stepLen = 0; stepLen < ringNum; ++stepLen) {
                temporaryHex = HexArray[MAP_X_OFFSET + baseDir.x, MAP_Y_OFFSET + baseDir.y];
                replacementHex.GetComponentInChildren<CubicHexComponent>().Hex.SetCoords(baseDir.x, baseDir.y);
                replacementHex.transform.position = baseDir.WorldPosition();
				HexArray[MAP_X_OFFSET + baseDir.x, MAP_Y_OFFSET + baseDir.y] = replacementHex;
                replacementHex = temporaryHex;

                //step towards next hex (wrap around NORTHWEST->NORTH)
                baseDir = baseDir.Adjacent((CubicDirection)(((int)dir + 2) % 6));
			}
		}
    }

    public GameObject initializeHex(int x, int y, int sortOrder, Sprite sprite) {
    	CubicHex newCubicHex = new CubicHex(x, y);
		GameObject newHexTile = Instantiate(HexTilePrefab, newCubicHex.WorldPosition(), Quaternion.identity, this.transform);
		newHexTile.name = ("Hex: " + x + ", " + y);
		newHexTile.GetComponentInChildren<CubicHexComponent>().Hex = newCubicHex;
		newHexTile.GetComponentInChildren<CubicHexComponent>().ParentHexMap = this;
		newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = sortOrder;
		newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = sprite;

		HexArray[MAP_X_OFFSET + x, MAP_Y_OFFSET + y] = newHexTile;

        return newHexTile;
    }

    public GameObject initializeShip(int x, int y) {
        if (ShipArray[MAP_X_OFFSET + x, MAP_Y_OFFSET + y] != null) {
            Debug.Log("Ship might already exist at this location: " + x + ", " + y);
        }
    	CubicHex newCubicHex = new CubicHex(x, y);
		GameObject newHexTile = Instantiate(HexTilePrefab, newCubicHex.WorldPosition(), Quaternion.identity, this.transform);
		newHexTile.name = ("Ship: " + x + ", " + y);
		newHexTile.GetComponentInChildren<CubicHexComponent>().Hex = newCubicHex;
		newHexTile.GetComponentInChildren<CubicHexComponent>().ParentHexMap = this;
		newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = SHIP_SORT_ORDER;
		newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = SHIP_SPRITE;
        newHexTile.GetComponentInChildren<CubicHexComponent>().Info = new ShipInfo("Hi",0);

		ShipArray[MAP_X_OFFSET + x, MAP_Y_OFFSET + y] = newHexTile;
        
        return newHexTile;
    }

    public GameObject initializePlanet(int x, int y) {
        if (PlanetArray[MAP_X_OFFSET + x, MAP_Y_OFFSET + y] != null) {
            Debug.Log("Planet might already exist at this location: " + x + ", " + y);
        }
        CubicHex newCubicHex = new CubicHex(x, y);
        GameObject newHexTile = Instantiate(HexTilePrefab, newCubicHex.WorldPosition(), Quaternion.identity, this.transform);
        newHexTile.name = ("Planet: " + x + ", " + y);
        newHexTile.GetComponentInChildren<CubicHexComponent>().Hex = newCubicHex;
        newHexTile.GetComponentInChildren<CubicHexComponent>().ParentHexMap = this;
        newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = PLANET_SORT_ORDER;
        newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = PLANET_SPRITE;
        newHexTile.GetComponentInChildren<CubicHexComponent>().Info = new PlanetInfo("Narnia");

        PlanetArray[MAP_X_OFFSET + x, MAP_Y_OFFSET + y] = newHexTile;
        
        return newHexTile;
    }
}
