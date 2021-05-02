using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Linq;
public enum HexMapLayer {BACKGROUND_LAYER,PLANET_LAYER,ASTEROID_LAYER,RESOURCE_LAYER,DYSON_LAYER,SHIP_LAYER,HIGHLIGHT_LAYER,OVERLAY_LAYER};

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
    public Sprite DYSON_SPRITE;

    public Sprite SHIP_SPRITE;

    public Sprite SELECTION_FOCUS_SPRITE;
    public Sprite SELECTION_HIGHLIGHT_SPRITE;

    //Stores all background hexes generated, by coordinates
    GameObject[,] HexArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores all asteroids by coordinates
    HashSet<GameObject> AsteroidSet = new HashSet<GameObject>();
    GameObject[,] AsteroidArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores all resources by coordinates
    //Keeps track of all ships
    HashSet<GameObject> ShipSet = new HashSet<GameObject>();
    //Keeps track of all dyson tiles
    GameObject[,] DysonArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    GameObject[,] ResourceArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores all planets by coordinates
    GameObject[,] PlanetArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];

    GameObject[,] HighlightArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];
    //Stores FOW tiles.
    //INDEPENDENT OF EACH PLAYER!!!
    //INDEPENDENT OF EACH PLAYER!!!
    //INDEPENDENT OF EACH PLAYER!!!
    public GameObject[,] FOWArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];

    //For UI
    GameObject[,] HighlightOverlayArray = new GameObject[MAX_MAP_WIDTH, MAX_MAP_HEIGHT];

    GameObject SelectionOverlayHex = null;

    //default hex dimensions: 	radius of 0.5 Unity in-world units 
    public void GenerateMap(float resourceDensity, float asteroidDensity, List<Empire> empires) {
    	
    	//the very CENTER hex is always the SUN (an asteroid, since ships cannot collide with it)
    	InitializeHex(new CubicHex(0,0), HexMapLayer.ASTEROID_LAYER, SUN_SPRITE);

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
    				else if (ringRadius == 2) {
    					if (UnityEngine.Random.Range(0.0f, 1.0f) < asteroidDensity) {
    						InitializeAsteroid(baseDir);
    					}
    					InitializeHex(baseDir, HexMapLayer.BACKGROUND_LAYER, BACKGROUND_SPRITE);
    				} else if (ringRadius == 4) {
                        if ((dir == CubicDirection.NORTHEAST || dir == CubicDirection.SOUTHWEST) && stepLen == 0) {
                            InitializePlanet(baseDir, null);
                        } else {
                            InitializeHex(baseDir, HexMapLayer.BACKGROUND_LAYER, BACKGROUND_SPRITE);
                        }
                    }
    				//Ring 7 should contain the planets.
    				else if (ringRadius == 7) {
    					//only at northernmost and southernmost points of the ring (i.e. 2 players for now)
    					if ((dir == CubicDirection.NORTH || dir == CubicDirection.SOUTH) && stepLen == 0) {
    						if (dir == CubicDirection.NORTH) {
                                
                                empires[1].AddPlanet(InitializePlanet(baseDir, empires[1]));
                                //add it to empire array
                            } else {
                                empires[0].AddPlanet(InitializePlanet(baseDir, empires[0]));
                            }
                            
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
        //Debug.Log(GetHex(finalHexCoords, HexMapLayer.ASTEROID_LAYER));
        finalHexCoords = finalHexCoords.Adjacent(CubicDirection.SOUTHWEST);

		GameObject replacementHex = GetHex(finalHexCoords, layer);
        GameObject temporaryHex = null;

		foreach (CubicDirection dir in Enum.GetValues(typeof(CubicDirection))) {
			CubicHex baseDir = new CubicHex(CubicHex.DirectionBases[(int)dir]*ringNum);
			for (int stepLen = 0; stepLen < ringNum; ++stepLen) {

                temporaryHex = ReplaceHex(baseDir, layer, replacementHex);
                replacementHex = temporaryHex;

                //step towards next hex (wrap around NORTHWEST->NORTH)
                baseDir = baseDir.Adjacent((CubicDirection)(((int)dir + 2) % 6));
			}
		}
    }

    public bool Collides(CubicHex hex) {
        foreach (GameObject ship in ShipSet) {
            if (ship.GetComponent<CubicHexComponent>().Hex.GetCoords() == hex.GetCoords()) {
                Debug.Log("SHIP COLLISION");
                return true;
            }
        }
        foreach (GameObject asteroid in AsteroidSet) {
            if (asteroid.GetComponent<CubicHexComponent>().Hex.GetCoords() == hex.GetCoords()) {
                Debug.Log("ASTEROID COLLISION");
                return true;
            }
        }
        if (DysonArray[MAP_X_OFFSET + hex.x, MAP_Y_OFFSET + hex.y] != null) {
            Debug.Log("DYSON COLLISION");
            return true;
        }
        return false;
    }
    public bool CanPlaceDyson(CubicHex hex) {
        CubicHex center = new CubicHex(0,0);
        foreach (CubicDirection dir in Enum.GetValues(typeof(CubicDirection))) {
            if (hex.GetCoords() == center.Adjacent(dir).GetCoords()) {
                if (GetHex(hex, HexMapLayer.DYSON_LAYER) != null) {
                    return true;
                }
            }
        }
        return false;
    }
    public GameObject PlaceDyson(CubicHex hex) {
        if (CanPlaceDyson(hex)) {
            return InitializeDyson(hex);
        } else {
            Debug.Log("Can't place dyson here");
            return null;
        }
    }

    GameObject InitializeDyson(CubicHex pos) {
        GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
        newHexTile.name = ("Dyson Sphere Component");
        newHexTile.GetComponent<CubicHexComponent>().Hex = new CubicHex(pos);
        newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
        newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.DYSON_LAYER;
        newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = DYSON_SPRITE;
        newHexTile.GetComponent<CubicHexComponent>().Info = null;
        
        SetHex(pos,HexMapLayer.DYSON_LAYER,newHexTile);
        return newHexTile;
    }

    public bool DysonComplete() {
        CubicHex center = new CubicHex(0,0);
        foreach (CubicDirection dir in Enum.GetValues(typeof(CubicDirection))) {
            if (GetHex(center.Adjacent(dir), HexMapLayer.DYSON_LAYER) == null) {
                return false;
            }
        }
        return false;
    }

    public GameObject InitializeHex(CubicHex pos, HexMapLayer layer, Sprite sprite) {
		GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
		newHexTile.name = ("Hex: " + pos.x + ", " + pos.y);
		newHexTile.GetComponent<CubicHexComponent>().Hex = pos;
		newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
		newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)layer;
		newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        if (layer == HexMapLayer.HIGHLIGHT_LAYER) {
            HighlightList.Add(newHexTile);
        } else {
            SetHex(pos, layer, newHexTile);
        }

        return newHexTile;
    }

    public GameObject PlaceShipAtPlanet(GameObject planet, ShipInfo info) {
        if (planet == null || info == null || planet.GetComponent<CubicHexComponent>() == null || planet.GetComponent<SortingGroup>() == null) {
            Debug.Log("Spawn ship at invalid planet");
            return null;
        }
        return InitializeShip(planet.GetComponent<CubicHexComponent>().Hex, info);
    }

    public GameObject InitializeShip(CubicHex pos, ShipInfo info) {
		GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
		newHexTile.name = ("Ship");
		newHexTile.GetComponent<CubicHexComponent>().Hex = new CubicHex(pos);
		newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
		newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.SHIP_LAYER;
		newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = SHIP_SPRITE;
        info.ParentGameObject = newHexTile;
        newHexTile.GetComponent<CubicHexComponent>().Info = info;
        ShipSet.Add(newHexTile);
        return newHexTile;
    }

    public GameObject InitializeAsteroid(CubicHex pos) {
        if (GetHex(pos, HexMapLayer.ASTEROID_LAYER) != null) {
            Debug.Log("Asteroid might already exist at this location: " + pos.x + ", " + pos.y);
        }
        GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
        newHexTile.name = ("Asteroid");
        newHexTile.GetComponent<CubicHexComponent>().Hex = pos;
        newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
        newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.ASTEROID_LAYER;
        newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = ASTEROID_SPRITE;

        SetHex(pos, HexMapLayer.ASTEROID_LAYER, newHexTile);
        AsteroidSet.Add(newHexTile);
        
        return newHexTile;
    }

    public GameObject InitializeResource(CubicHex pos, Sprite sprite) {
        if (GetHex(pos, HexMapLayer.RESOURCE_LAYER) != null) {
            Debug.Log("Resource might already exist at this location: " + pos.x + ", " + pos.y);
        }
        GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
        newHexTile.name = ("Resource");
        newHexTile.GetComponent<CubicHexComponent>().Hex = pos;
        newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
        newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.RESOURCE_LAYER;
        newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        newHexTile.GetComponent<CubicHexComponent>().Info = new ResourceInfo("Resource", null, newHexTile);

        SetHex(pos, HexMapLayer.RESOURCE_LAYER, newHexTile);
        
        return newHexTile;
    }

    public GameObject InitializePlanet(CubicHex pos, Empire empire) {
        if (GetHex(pos, HexMapLayer.PLANET_LAYER) != null) {
            Debug.Log("Planet might already exist at this location: " + pos.x + ", " + pos.y);
        }
        GameObject newHexTile = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
        newHexTile.name = ("Planet");
        newHexTile.GetComponent<CubicHexComponent>().Hex = pos;
        newHexTile.GetComponent<CubicHexComponent>().ParentHexMap = this;
        newHexTile.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.PLANET_LAYER;
        newHexTile.GetComponentInChildren<SpriteRenderer>().sprite = PLANET_SPRITE;
        newHexTile.GetComponent<CubicHexComponent>().Info = new PlanetInfo("New Planet", empire, newHexTile);

        SetHex(pos, HexMapLayer.PLANET_LAYER, newHexTile);
        
        return newHexTile;
    }

    public void SetSelectionHex(CubicHex pos) {
        if (SelectionOverlayHex != null)
        {
            ClearSelectionHex();
        }
        SelectionOverlayHex = Instantiate(HexTilePrefab, pos.WorldPosition(), Quaternion.identity, this.transform);
        SelectionOverlayHex.name = ("Overlay hex: " + pos.x + ", " + pos.y);
        SelectionOverlayHex.GetComponent<CubicHexComponent>().Hex = pos;
        SelectionOverlayHex.GetComponent<CubicHexComponent>().ParentHexMap = this;
        SelectionOverlayHex.GetComponentInChildren<SortingGroup>().sortingOrder = (int)HexMapLayer.OVERLAY_LAYER;
        SelectionOverlayHex.GetComponentInChildren<SpriteRenderer>().sprite = SELECTION_FOCUS_SPRITE;

    }

    public void ClearSelectionHex() {
        if (SelectionOverlayHex != null) {
            GameObject.Destroy(SelectionOverlayHex);
            SelectionOverlayHex = null;
        }
    }

    public GameObject GetSelectionHex() {
        return SelectionOverlayHex;
    }

    public GameObject ReplaceHex(CubicHex replacementPos, HexMapLayer layer, GameObject replacementHex) {
        GameObject replacedHex = GetHex(replacementPos, layer);
        /*if (replacementHex != null) {
            replacementHex.GetComponent<CubicHexComponent>().Hex.SetCoords(replacementPos.x, replacementPos.y);
            replacementHex.transform.position = replacementPos.WorldPosition();
        }*/
        SetHex(replacementPos, layer, replacementHex);
        return replacedHex;
    }    

    public void MoveShip(GameObject ship, CubicHex to) {

        Debug.Log("To "+to);
        if (ship != null) {
            ship.GetComponent<CubicHexComponent>().Hex.SetCoords(to.x, to.y);
            ship.transform.position = to.WorldPosition();

        } else {
            Debug.Log("ship was null");
        }
    }

    GameObject[,] GetHexArray(HexMapLayer layer) {
        switch(layer) {
            case HexMapLayer.BACKGROUND_LAYER:{return HexArray;}
            case HexMapLayer.PLANET_LAYER:  {return PlanetArray;}
            case HexMapLayer.ASTEROID_LAYER:{return AsteroidArray;}
            case HexMapLayer.RESOURCE_LAYER:{return ResourceArray;}
            case HexMapLayer.DYSON_LAYER:{return DysonArray;}
            case HexMapLayer.HIGHLIGHT_LAYER:{return HighlightArray;}
            default: {Debug.Log("Invalid layer!"); return null;}
        }
    }

    public GameObject GetHex(CubicHex pos, HexMapLayer layer) {
        return GetHexArray(layer)[MAP_X_OFFSET + pos.x, MAP_Y_OFFSET + pos.y];
    }

    void SetHex(CubicHex pos, HexMapLayer layer, GameObject hex) {
        
        GetHexArray(layer)[MAP_X_OFFSET + pos.x, MAP_Y_OFFSET + pos.y] = hex;
        if (hex != null) {
            hex.GetComponent<CubicHexComponent>().Hex.SetCoords(pos.x, pos.y);
            hex.transform.position = pos.WorldPosition();
        } else {
            //Debug.Log("SetHex with null");
        }
    }
    

    List<GameObject> HighlightList;
    int[,] highlightDistances = new int[MAX_MAP_WIDTH,MAX_MAP_HEIGHT];
    //No need to keep all the highlight hexes in a separate 2D array because we never need to refer to their locations. Just need to 
    public void SetHighlightHexes(List<CubicHex> hexes)
    {
        if (HighlightList != null) {
            ClearHighlightHexes();
        }
        HighlightList = new List<GameObject>();
        foreach (CubicHex hex in hexes)
        {
            InitializeHex(hex, HexMapLayer.HIGHLIGHT_LAYER, SELECTION_HIGHLIGHT_SPRITE);
        }
    }

    public void ClearHighlightHexes() {
        if (HighlightList != null) {
            for (int i = 0; i < HighlightList.Count; ++i) {
                GameObject.Destroy(HighlightList[i]);
            }
            HighlightList = null;
        }
    }

    public bool HexIsInHighlightHexes(CubicHex hex) {
        if (HighlightList != null) {
            foreach(GameObject go in HighlightList) {
                if (go != null && go.GetComponent<CubicHexComponent>() != null) {
                    if (go.GetComponent<CubicHexComponent>().Hex.GetCoords() == hex.GetCoords()) return true;
                }
            }
        }
        return false;
    }


    //for finding moveable tiles from a ship. since ships will probably not move more than 4 tiles in a turn, we simple use BFS.
    public List<CubicHex> GetHexesFromDist(CubicHex pos, int maxDist) {
        if (maxDist > MAX_MAP_WIDTH/2 || maxDist > MAX_MAP_HEIGHT/2) {
            Debug.Log("Reached a hex outside of the map!");
            return null;
        }
        HashSet<CubicHex> reachable = new HashSet<CubicHex>();
        //initially we don't know the distances of each hex, so we set them to infinity
        for (int i = 0; i < MAX_MAP_WIDTH; ++i){
            for (int j = 0; j < MAX_MAP_HEIGHT; ++j){
                highlightDistances[i,j] = Int32.MaxValue;
            }
        }
        //center hex has distance of 0
        highlightDistances[MAP_X_OFFSET + pos.x, MAP_Y_OFFSET + pos.y] = 0;

        Queue<CubicHex> queue = new Queue<CubicHex>();

        queue.Enqueue(pos);
        while (queue.Count > 0) {
            CubicHex hex = queue.Dequeue();
            int hexDist = highlightDistances[MAP_X_OFFSET + hex.x, MAP_Y_OFFSET + hex.y];
            if (hex != pos) {
                reachable.Add(hex);
            }
            foreach (CubicDirection dir in Enum.GetValues(typeof(CubicDirection)))
            {
                CubicHex adj = hex.Adjacent(dir);
                if (Collides(adj)) {
                    continue;
                }
                int adjDist = highlightDistances[MAP_X_OFFSET + adj.x, MAP_Y_OFFSET + adj.y];
                if (hexDist + 1 < adjDist && hexDist + 1 <= maxDist) {
                    queue.Enqueue(adj);
                    highlightDistances[MAP_X_OFFSET + adj.x, MAP_Y_OFFSET + adj.y] = hexDist + 1;
                }
            }
        }
        return reachable.ToList();
    }

    public int GetDistToHex(CubicHex dest) {
        return highlightDistances[MAP_X_OFFSET + dest.x, MAP_Y_OFFSET + dest.y];
    }
}
