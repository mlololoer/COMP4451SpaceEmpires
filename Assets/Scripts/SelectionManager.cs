using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;

public class SelectionManager : MonoBehaviour
{
	public static SelectionManager SM;

	//to keep track of whether the same hex is clicked again (to loop through all the objects in the hex)
	CubicHex prevSelectedHex = null;

	public GameObject SelectedGameObject = null;

	int selectIndex = 0;

	public void Select(List<GameObject> clickedObjects) {
		CubicHex clickedHexLocation = null;
		List<GameObject> hexObjs = new List<GameObject>();
		if (clickedObjects.Count == 0) return;
		bool onlyBackgroundTiles = true;
		//only filter out GOs that are hexes
		foreach (GameObject clicked in clickedObjects) {
			if (clicked.GetComponent<CubicHexComponent>() != null && clicked.GetComponent<SortingGroup>() != null) {
				//Debug.Log(clicked.GetComponent<CubicHexComponent>().Hex);
				clickedHexLocation = clicked.GetComponent<CubicHexComponent>().Hex;
				if (clicked.GetComponent<SortingGroup>().sortingOrder == (int)HexMapLayer.PLANET_LAYER
				|| clicked.GetComponent<SortingGroup>().sortingOrder == (int)HexMapLayer.ASTEROID_LAYER
				|| clicked.GetComponent<SortingGroup>().sortingOrder == (int)HexMapLayer.RESOURCE_LAYER
				|| clicked.GetComponent<SortingGroup>().sortingOrder == (int)HexMapLayer.SHIP_LAYER) {
					hexObjs.Add(clicked);
					onlyBackgroundTiles = false;
				}
			}
		}



		//clicked a location with only background tiles: deselect
		if (onlyBackgroundTiles){
			//move the overlay hex
			GameManager.GM.hexMap.ClearSelectionHex();
			SelectedGameObject = null;
			selectIndex = 0;
		}
		else if (prevSelectedHex != null && clickedHexLocation.GetCoords() == prevSelectedHex.GetCoords()) {
			//already selected here, don't change the overlay hex
			selectIndex = (selectIndex + 1) % hexObjs.Count;
			SelectedGameObject = hexObjs[selectIndex];
		} else {
			GameManager.GM.hexMap.SetSelectionHex(clickedHexLocation);
			selectIndex = 0;
			SelectedGameObject = hexObjs[selectIndex];
		}

		prevSelectedHex = clickedHexLocation;

		if (SelectedGameObject != null) {
			Debug.Log(SelectedGameObject.GetComponent<CubicHexComponent>().Hex);
			Debug.Log(SelectedGameObject.GetComponent<CubicHexComponent>().Info);
			UIManager.UIM.UpdateUnitModal();
		} else {
			UIManager.UIM.ClearUnitModal();
		}



/*
		if (hexObjs.Count > 0) {
			//do something with the current selection
			CubicHexComponent hexComponent = hexObjs[selectIndex].GetComponent<CubicHexComponent>();
			switch((HexMapLayer)hexObjs[selectIndex].GetComponent<SortingGroup>().sortingOrder) {
				case (HexMapLayer.SHIP_LAYER): {
					Debug.Log("Selected Ship: " + hexComponent.Info.name);
					//UIManager.UIM.UpdateUnitModal(spaceshipName,spaceshipHealth);
					break;
				}
				case (HexMapLayer.PLANET_LAYER): {
					Debug.Log("Selected Planet: " + hexComponent.Info.name);
					//UIManager.UIM.UpdateUnitModal(planetName,planetHealth);
					break;
				}
				case (HexMapLayer.RESOURCE_LAYER): {
					Debug.Log("Selected Resource: " + hexComponent.Info.name);
					//UIManager.UIM.UpdateUnitModal(planetName,planetHealth);
					break;
				}
				case (HexMapLayer.ASTEROID_LAYER): {
					Debug.Log("Selected Asteroid");
					//UIManager.UIM.UpdateUnitModal(planetName,planetHealth);
					break;
				}
			}
		}
		*/
	
		
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

	public void MoveOverlayOn() {
		/*
		//Get the ship's location by checking where the OVERLAY HEX is
		GameObject overlayHex = GameManager.GM.hexMap.GetSelectionHex();
		if (overlayHex == null) {
			Debug.Log("No overlay hex");
			return;
		}
		GameObject ship = GameManager.GM.hexMap.GetHex(overlayHex.GetComponent<CubicHexComponent>().Hex, HexMapLayer.SHIP_LAYER);*/
		if (SelectedGameObject != null && SelectedGameObject.GetComponent<SortingGroup>().sortingOrder == (int)HexMapLayer.SHIP_LAYER)
		{
			CubicHexComponent shipCHC = SelectedGameObject.GetComponent<CubicHexComponent>();
			if (((ShipInfo)shipCHC.Info).ParentEmpire == GameManager.GM.GetActiveEmpire()) {
				Debug.Log("Ship at "+ shipCHC.Hex.x + ", " + shipCHC.Hex.y+" ready to move");
			List<CubicHex> reachable = GameManager.GM.hexMap.GetHexesFromDist(shipCHC.Hex, ((ShipInfo)shipCHC.Info).remainingMoves);
			GameManager.GM.hexMap.SetHighlightHexes(reachable);
			} else {
				Debug.Log("Ship was not owned by active empire");
			}
			
		} else {
			Debug.Log("Ship was null");
		}
	}

	public void MoveOverlayOff(List<GameObject> clickedObjects) {
		CubicHex clickedHexLocation = null;
		if (clickedObjects.Count == 0) {
			Debug.Log("Nothing at the hex where RMB was lifted (outside map)");
			return;
		}
		//only filter out GOs that are hexes
		foreach (GameObject clicked in clickedObjects) {
			if (clicked.GetComponent<CubicHexComponent>() != null && clicked.GetComponent<SortingGroup>() != null) {
				clickedHexLocation = clicked.GetComponent<CubicHexComponent>().Hex;
			}
		}
		if (SelectedGameObject != null && SelectedGameObject.GetComponent<SortingGroup>().sortingOrder == (int)HexMapLayer.SHIP_LAYER) {
			//Check for whether the cursor was let go on a highlight hex (i.e. a reachable hex)
			CubicHexComponent shipCHC = SelectedGameObject.GetComponent<CubicHexComponent>();
			if (((ShipInfo)shipCHC.Info).ParentEmpire == GameManager.GM.GetActiveEmpire()) {
				if (GameManager.GM.hexMap.HexIsInHighlightHexes(clickedHexLocation)) {
					Debug.Log("Move a " + SelectedGameObject);
					Debug.Log("Move from "+shipCHC.Hex+" to "+clickedHexLocation);
					GameManager.GM.hexMap.MoveShip(SelectedGameObject, clickedHexLocation);
					((ShipInfo)shipCHC.Info).remainingMoves -= GameManager.GM.hexMap.GetDistToHex(clickedHexLocation);

				} else {
					Debug.Log("RMb lifted on a non-highlight hex");
				}
			} else {
				Debug.Log("Could not move ship owned by another empire");
			}
			
		} else {
			Debug.Log("Selected object is not a ship");
		}
		
		//Maybe get the shortest path by going backwards from the target hex using the distances array inside the GetHexesFromDist
		ClearAllSelections();

	}
	public void ClearAllSelections() {
		prevSelectedHex = null;
		SelectedGameObject = null;
		selectIndex = 0;
		GameManager.GM.hexMap.ClearSelectionHex();
		GameManager.GM.hexMap.ClearHighlightHexes();
		UIManager.UIM.ClearUnitModal();
	}

	// public void Move(List<GameObject> clickedObjects) {
	// 	CubicHex clickedHexLocation = null;
	// 	List<GameObject> hexObjs = new List<GameObject>();
	// 	if (clickedObjects.Count == 0) return;
	// 	bool onlyBackgroundTiles = true;
	// 	//CANNOT move here if the path is blocked!
	// 	foreach (GameObject clicked in clickedObjects) {
	// 		if (clicked.GetComponent<CubicHexComponent>() != null && clicked.GetComponent<SortingGroup>() != null) {
	// 			clickedHexLocation = clicked.GetComponent<CubicHexComponent>().Hex;
	// 			if (clicked.GetComponent<SortingGroup>().sortingOrder != (int)HexMapLayer.OVERLAY_LAYER && clicked.GetComponent<SortingGroup>().sortingOrder != (int)HexMapLayer.BACKGROUND_LAYER) {
	// 				hexObjs.Add(clicked);
	// 				onlyBackgroundTiles = false;
	// 			}
	// 		}
	// 	}
	// }


    //Singleton implementation
	void Awake() {
		if(SM != null)
			GameObject.Destroy(SM);
		else
			SM = this;
		DontDestroyOnLoad(this);
	}
}
