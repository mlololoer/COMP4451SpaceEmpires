using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
public enum HexMapLayer {BACKGROUND_LAYER,PLANET_LAYER,ASTEROID_LAYER,RESOURCE_LAYER,SHIP_LAYER,OVERLAY_LAYER};

//Manages the map where all hexes are contained in.
public class HexMap : MonoBehaviour
{

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

    public Sprite SELECTION_FOCUS_SPRITE;
    public Sprite SELECTION_MOVEMENT_SPRITE;

    //Stores all background hexes generated, by coordinates
    GameObject[,] HexArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores all asteroids by coordinates
    GameObject[,] AsteroidArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores all resources by coordinates
    GameObject[,] ResourceArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores all ships by coordinates
    GameObject[,] ShipArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores all planets by coordinates
    GameObject[,] PlanetArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores FOW tiles.
    //INDEPENDENT OF EACH PLAYER!!!
    //INDEPENDENT OF EACH PLAYER!!!
    //INDEPENDENT OF EACH PLAYER!!!
    public GameObject[,] FOWArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];

    void Start() {
        GenerateMap(0.05f, 0.2f);
        InitializeShip(new CubicHex(2, 2));
        InitializeShip(new CubicHex(0, -7));
    }

    //default hex dimensions: 	radius of 0.5 Unity in-world units 
    public void GenerateMap(float resourceDensity, float asteroidDensity) {
    	
    	//the very CENTER hex is always the SUN.
    	InitializeHex(new CubicHex(0,0), HexMapLayer.BACKGROUND_LAYER, SUN_SPRITE);

    	//hex ring with radius ringRadius
    	for (int ringRadius = 1; ringRadius <= MAP_RADIUS; ++ringRadius) {

    		//get each corner hex that is ringRadius hexes away from the center
    		foreach (CubicDirection dir in Enum.GetValues(typeof(CubicDirection))) {
                Vector3Int basePos = CubicHex.DirectionBases[(int)dir]*ringRadius;
                CubicHex baseDir = new CubicHex(basePos.x, basePos.y);

    			//iterate through the line from the corner hex to the next corner hex
    			for (int stepLen = 0; stepLen < ringRadius; ++stepLen) {
	    			//Decisions for what the hex's texture should be

	    			//For the hexes immediately surrounding the SUN, they MUST be empty (for the dyson sphere).
    				if (ringRadius == 1) {
    					InitializeHex(baseDir, HexMapLayer.BACKGROUND_LAYER, BACKGROUND_SPRITE);
    				}

    				//Ring 3 should contain intermittent asteroids.
    				else if (ringRadius == 3) {
    					if (UnityEngine.Random.Range(0.0f, 1.0f) < asteroidDensity) {
    						InitializeAsteroid(baseDir);
    					}
    					InitializeHex(baseDir, HexMapLayer.BACKGROUND_LAYER, BACKGROUND_SPRITE);
    				}

    				//Ring 7 should contain the planets.
    				else if (ringRadius == 7) {
    					//only at northernmost and southernmost points of the ring (i.e. 2 players for now)
    					if ((dir == CubicDirection.NORTH || dir == CubicDirection.SOUTH) && stepLen == 0) {
    						InitializePlanet(baseDir);
    					} else {
    						InitializeHex(baseDir, HexMapLayer.BACKGROUND_LAYER, BACKGROUND_SPRITE);
    					}
    				}

    				//All other rings should be empty (to be filled with resources)
    				else {
    					if (UnityEngine.Random.Range(0.0f, 1.0f) < resourceDensity) {
                            //add specialized resource type later on?
    						InitializeResource(baseDir, RESOURCE_SPRITE);
    					}
    					InitializeHex(baseDir, HexMapLayer.BACKGROUND_LAYER, BACKGROUND_SPRITE);
    					
    					
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

    public void rotateRing(int ringNum, HexMapLayer layer) {
		//temporarily store the very last hex to be swapped
        CubicHex finalHexCoords = new CubicHex(CubicHex.DirectionBases[(int)CubicDirection.NORTH]*ringNum);
        finalHexCoords = finalHexCoords.Adjacent(CubicDirection.SOUTHWEST);

		GameObject replacementHex = GetHex(finalHexCoords, layer);
        GameObject temporaryHex = null;

		foreach (CubicDirection dir in Enum.GetValues(typeof(CubicDirection))) {
			CubicHex baseDir = new CubicHex(CubicHex.DirectionBases[(int)dir]*ringNum);
			for (int stepLen = 0; stepLen < ringNum; ++stepLen) {

                temporaryHex = MoveHex(baseDir, layer, replacementHex);
                replacementHex = temporaryHex;

                //step towards next hex (wrap around NORTHWEST->NORTH)
                baseDir = baseDir.Adjacent((CubicDirection)(((int)dir + 2) % 6));
			}
		}
    }

    

    public GameObject InitializeHex(CubicHex pos, HexMapLayer layer, Sprite sprite) {
		GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
		newHexTile.name = ("Hex: " + pos.x + ", " + pos.y);
		newHexTile.GetComponent<CubicHexComponent>().Hex = pos;
		newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
		newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)layer;
		newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = sprite;

		SetHex(pos, layer, newHexTile);

        return newHexTile;
    }

    public GameObject InitializeShip(CubicHex pos) {
        if (GetHex(pos, HexMapLayer.SHIP_LAYER) != null) {
            Debug.Log("Ship might already exist at this location: " + pos.x + ", " + pos.y);
        }
		GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
		newHexTile.name = ("Ship: " + pos.x + ", " + pos.y);
		newHexTile.GetComponent<CubicHexComponent>().Hex = pos;
		newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
		newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.SHIP_LAYER;
		newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = SHIP_SPRITE;
        newHexTile.GetComponent<CubicHexComponent>().Info = new ShipInfo("Hi", GameManager.GM.ActiveEmpire, 0);

		SetHex(pos, HexMapLayer.SHIP_LAYER, newHexTile);
        
        return newHexTile;
    }

    public GameObject InitializeAsteroid(CubicHex pos) {
        if (GetHex(pos, HexMapLayer.ASTEROID_LAYER) != null) {
            Debug.Log("Asteroid might already exist at this location: " + pos.x + ", " + pos.y);
        }
        GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
        newHexTile.name = ("Asteroid: " + pos.x + ", " + pos.y);
        newHexTile.GetComponent<CubicHexComponent>().Hex = pos;
        newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
        newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.ASTEROID_LAYER;
        newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = ASTEROID_SPRITE;

        SetHex(pos, HexMapLayer.ASTEROID_LAYER, newHexTile);
        
        return newHexTile;
    }

    public GameObject InitializeResource(CubicHex pos, Sprite sprite) {
        if (GetHex(pos, HexMapLayer.RESOURCE_LAYER) != null) {
            Debug.Log("Resource might already exist at this location: " + pos.x + ", " + pos.y);
        }
        GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
        newHexTile.name = ("Resource: " + pos.x + ", " + pos.y);
        newHexTile.GetComponent<CubicHexComponent>().Hex = pos;
        newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
        newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.RESOURCE_LAYER;
        newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        newHexTile.GetComponent<CubicHexComponent>().Info = new ResourceInfo("Rare Metals", null);

        SetHex(pos, HexMapLayer.RESOURCE_LAYER, newHexTile);
        
        return newHexTile;
    }

    public GameObject InitializePlanet(CubicHex pos) {
        if (GetHex(pos, HexMapLayer.PLANET_LAYER) != null) {
            Debug.Log("Planet might already exist at this location: " + pos.x + ", " + pos.y);
        }
        GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
        newHexTile.name = ("Planet: " + pos.x + ", " + pos.y);
        newHexTile.GetComponent<CubicHexComponent>().Hex = pos;
        newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
        newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.PLANET_LAYER;
        newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = PLANET_SPRITE;
        newHexTile.GetComponent<CubicHexComponent>().Info = new PlanetInfo("Narnia", GameManager.GM.ActiveEmpire);

        SetHex(pos, HexMapLayer.PLANET_LAYER, newHexTile);
        
        return newHexTile;
    }

    public GameObject InitializeOverlayHex(CubicHex pos) {
        GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
        newHexTile.name = ("Overlay hex: " + pos.x + ", " + pos.y);
        newHexTile.GetComponent<CubicHexComponent>().Hex = pos;
        newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
        newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.OVERLAY_LAYER;
        newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = SELECTION_FOCUS_SPRITE;

        return newHexTile;
    }

    public GameObject MoveHex(CubicHex replacementPos, HexMapLayer layer, GameObject replacementHex) {
        GameObject replacedHex = GetHex(replacementPos, layer);
        if (replacementHex != null) {
            replacementHex.GetComponent<CubicHexComponent>().Hex.SetCoords(replacementPos.x, replacementPos.y);
            replacementHex.transform.position = replacementPos.WorldPosition();
        }
        SetHex(replacementPos, layer, replacementHex);
        return replacedHex;
    }    

    GameObject[,] GetHexArray(HexMapLayer layer) {
        switch(layer) {
            case HexMapLayer.BACKGROUND_LAYER:{return HexArray;}
            case HexMapLayer.PLANET_LAYER:  {return PlanetArray;}
            case HexMapLayer.ASTEROID_LAYER:{return AsteroidArray;}
            case HexMapLayer.RESOURCE_LAYER:{return ResourceArray;}
            case HexMapLayer.SHIP_LAYER:{return ShipArray;}
            default: {return null;}
        }
    }

    public GameObject GetHex(CubicHex pos, HexMapLayer layer) {
        return GetHexArray(layer)[MAP_X_OFFSET + pos.x, MAP_Y_OFFSET + pos.y];
    }

    public void SetHex(CubicHex pos, HexMapLayer layer, GameObject hex) {
        GetHexArray(layer)[MAP_X_OFFSET + pos.x, MAP_Y_OFFSET + pos.y] = hex;
    }

    
}
