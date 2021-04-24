using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;

public class SelectionManager : MonoBehaviour
{
	public static SelectionManager SM;

	[SerializeField]
	HexMap hexMap;

	Queue<(Vector3Int, Vector3Int)> movementQueue = new Queue<(Vector3Int, Vector3Int)>();

	bool highlighted = false;

	[SerializeField]
	Text UnitModalNameText;
	[SerializeField]
	Text UnitModalDetailsText;


	public void Select(List<GameObject> clickedObjects) {
		foreach (GameObject clicked in clickedObjects) {
			CubicHexComponent clickedHexComponent = clicked.GetComponentInChildren<CubicHexComponent>();
			switch(clicked.GetComponentInChildren<SortingGroup>().sortingOrder) {
				case HexMap.SHIP_SORT_ORDER: {
					Debug.Log("Ship: " + clickedHexComponent.Info.name);
					break;
				}
				case HexMap.PLANET_SORT_ORDER: {
					Debug.Log("Planet: " + clickedHexComponent.Info.name);
					break;
				}
			}
			
		}
		/*if (spaceshipTM.HasTile(coord)) {
			clearHighlightHex();
			string spaceshipName = spaceshipTM.GetTile<SpaceshipTile>(coord).spaceshipName;
			int spaceshipHealth = spaceshipTM.GetTile<SpaceshipTile>(coord).health;
			Debug.Log("Spaceship: " + spaceshipName);
			highlightedCoord = coord;
			highlightedTileIsMovable = true;

			UIManager.UIM.UpdateUnitModal(spaceshipName,spaceshipHealth);
		} else if (selectedTile is PlanetTile) {
			string planetName = ((PlanetTile)selectedTile).planetName;
			int planetHealth = ((PlanetTile)selectedTile).health;
			Debug.Log("Planet: " + planetName);
			highlightedTileIsMovable = false;

			UIManager.UIM.UpdateUnitModal(planetName,planetHealth);
		} else if (!spaceshipTM.HasTile(coord)){
			Debug.Log("Terrain");
			if (highlighted && highlightedTileIsMovable) {
				movementQueue.Enqueue((highlightedCoord, coord));
				//check if move is possible in the future as well
			}
			highlightedTileIsMovable = false;
		}
		setHighlightHex(coord);*/
	}

	public void ClearSelect() {
		clearHighlightHex();
	}

	int temporaryEnemyTurn = 0;

	public void ProcessMovement() {
		/*while (movementQueue.Count > 0) {
			(Vector3Int, Vector3Int) movement = movementQueue.Dequeue();
			//temporarily only for spaceships
			spaceshipTM.SetTile(movement.Item2, spaceshipTM.GetTile(movement.Item1));
			spaceshipTM.SetTile(movement.Item1,null);
			FOWManager.FM.ClearFOW(movement.Item2);
		}
		spaceshipTM.SetTile(new Vector3Int(-7-temporaryEnemyTurn,2,0), spaceshipTM.GetTile(new Vector3Int(-6-temporaryEnemyTurn,2,0)));
		spaceshipTM.SetTile(new Vector3Int(-6-temporaryEnemyTurn,2,0), null);*/
		/*if (temporaryEnemyTurn%2 == 0) {
			planetTM.SetTile(new Vector3Int(-7+(temporaryEnemyTurn/2),(temporaryEnemyTurn/2)-1,0), planetTM.GetTile(new Vector3Int(-8+(temporaryEnemyTurn/2),temporaryEnemyTurn/2,0)));
			planetTM.SetTile(new Vector3Int(-8+(temporaryEnemyTurn/2),temporaryEnemyTurn/2,0), null);
		}*/
		
		/*
		if (temporaryEnemyTurn == 4) {
			UIManager.UIM.OpenFightModal();
		}
		//after processing, we clear all highlight activity
		clearHighlightHex();
		temporaryEnemyTurn += 1;*/
	}

	void setHighlightHex(Vector3Int highlightHere) {
        clearHighlightHex();
        highlighted = true;
        //overlayTM.SetTile(highlightHere, highlightTile);
        //highlightedCoord = highlightHere;
    }

    void clearHighlightHex() {
        if (highlighted) {
            highlighted = false;
            //overlayTM.SetTile(highlightedCoord, null);
        }
    }

    //Singleton implementation
	void Awake() {
		if(SM != null)
			GameObject.Destroy(SM);
		else
			SM = this;
		DontDestroyOnLoad(this);
	}
}
