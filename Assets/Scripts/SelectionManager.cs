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

	[SerializeField]
	Text UnitModalNameText;
	[SerializeField]
	Text UnitModalDetailsText;

	GameObject overlayHex = null;
	//to keep track of whether the same hex is clicked again (to loop through all the objects in the hex)
	CubicHex prevSelectedHex = null;

	int selectIndex = 0;

	public void Select(List<GameObject> clickedObjects) {
		CubicHex clickedHexLocation = null;
		List<GameObject> hexObjs = new List<GameObject>();
		if (clickedObjects.Count == 0) return;
		bool onlyBackgroundTiles = true;
		//only filter out GOs that are hexes
		foreach (GameObject clicked in clickedObjects) {
			if (clicked.GetComponent<CubicHexComponent>() != null && clicked.GetComponent<SortingGroup>() != null) {
				clickedHexLocation = clicked.GetComponent<CubicHexComponent>().Hex;
				if (clicked.GetComponent<SortingGroup>().sortingOrder != (int)HexMapLayer.OVERLAY_LAYER && clicked.GetComponent<SortingGroup>().sortingOrder != (int)HexMapLayer.BACKGROUND_LAYER) {
					hexObjs.Add(clicked);
					onlyBackgroundTiles = false;
				}
				
			}
		}

		
		//clicked a location with only background tiles: deselect
		if (onlyBackgroundTiles){
			//move the overlay hex
			if (overlayHex != null) {
				GameObject.Destroy(overlayHex);
			}
			overlayHex = null;
			selectIndex = 0;
		}
		else if (prevSelectedHex == null) {
			overlayHex = GameManager.GM.hexMap.InitializeOverlayHex(clickedHexLocation);
			selectIndex = 0;
		}
		//clicked the same location again: increment selection index
		else if (clickedHexLocation.GetCoords() == prevSelectedHex.GetCoords()) {
			//already selected here, don't change the overlay hex

			selectIndex = (selectIndex + 1) % hexObjs.Count;
		} else {
			if (overlayHex != null) {
				GameObject.Destroy(overlayHex);
			}
			overlayHex = GameManager.GM.hexMap.InitializeOverlayHex(clickedHexLocation);
			selectIndex = 0;
		}
		if (hexObjs.Count > 0) {
			//do something with the current selection
			CubicHexComponent hexComponent = hexObjs[selectIndex].GetComponent<CubicHexComponent>();
			switch((HexMapLayer)hexObjs[selectIndex].GetComponent<SortingGroup>().sortingOrder) {
				case (HexMapLayer.SHIP_LAYER): {
					Debug.Log("Ship: " + hexComponent.Info.name);
					//UIManager.UIM.UpdateUnitModal(spaceshipName,spaceshipHealth);
					break;
				}
				case (HexMapLayer.PLANET_LAYER): {
					Debug.Log("Planet: " + hexComponent.Info.name);
					//UIManager.UIM.UpdateUnitModal(planetName,planetHealth);
					break;
				}
				case (HexMapLayer.RESOURCE_LAYER): {
					Debug.Log("Resource: " + hexComponent.Info.name);
					//UIManager.UIM.UpdateUnitModal(planetName,planetHealth);
					break;
				}
				case (HexMapLayer.ASTEROID_LAYER): {
					Debug.Log("Asteroid");
					//UIManager.UIM.UpdateUnitModal(planetName,planetHealth);
					break;
				}
			}
		}
	
		prevSelectedHex = clickedHexLocation;
		//we should use RMB to handle MOVING. Only LMB for selecting.
		/*else if (!spaceshipTM.HasTile(coord)){
			Debug.Log("Terrain");
			if (highlighted && highlightedTileIsMovable) {
				movementQueue.Enqueue((highlightedCoord, coord));
				//check if move is possible in the future as well
			}
			highlightedTileIsMovable = false;
		}*/
	}

	public void CreateMoveOverlay() {
		//Get the ship's location by checking where the OVERLAY HEX is
		if (overlayHex == null) return;
		GameObject ship = GameManager.GM.hexMap.GetHex(overlayHex.GetComponent<CubicHexComponent>().Hex, HexMapLayer.SHIP_LAYER);
		if (ship != null)
		{
			CubicHexComponent shipCHC = ship.GetComponent<CubicHexComponent>();
			Debug.Log("Ship at "+ shipCHC.Hex.x + ", " + shipCHC.Hex.y+" ready to move");
		}
		
	}

	public void Move(List<GameObject> clickedObjects) {
		CubicHex clickedHexLocation = null;
		List<GameObject> hexObjs = new List<GameObject>();
		if (clickedObjects.Count == 0) return;
		bool onlyBackgroundTiles = true;
		//CANNOT move here if the path is blocked!
		foreach (GameObject clicked in clickedObjects) {
			if (clicked.GetComponent<CubicHexComponent>() != null && clicked.GetComponent<SortingGroup>() != null) {
				clickedHexLocation = clicked.GetComponent<CubicHexComponent>().Hex;
				if (clicked.GetComponent<SortingGroup>().sortingOrder != (int)HexMapLayer.OVERLAY_LAYER && clicked.GetComponent<SortingGroup>().sortingOrder != (int)HexMapLayer.BACKGROUND_LAYER) {
					hexObjs.Add(clicked);
					onlyBackgroundTiles = false;
				}
			}
		}
	}

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

    //Singleton implementation
	void Awake() {
		if(SM != null)
			GameObject.Destroy(SM);
		else
			SM = this;
		DontDestroyOnLoad(this);
	}
}
