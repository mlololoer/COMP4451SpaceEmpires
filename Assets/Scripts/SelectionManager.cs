using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Tilemaps;

public class SelectionManager : MonoBehaviour
{
	public static SelectionManager SM;

	[SerializeField]
	Tilemap overlayTM;
	[SerializeField]
	Tile highlightTile;
	[SerializeField]
	Tilemap spaceshipTM;
	public Text InfoText;
	public Text UnitText;

	bool highlighted = false;
	Vector3Int highlightedCoord;

	Vector3Int selectedCoord;

	public void Select(Tile selectedTile, Vector3Int coord) {
		selectedCoord = coord;
		if (selectedTile is SpaceshipTile) {
			Debug.Log("Spaceship: " + ((SpaceshipTile)selectedTile).spaceshipName);
			UnitText.text = ("Spaceship: " + ((SpaceshipTile)selectedTile).spaceshipName);
			InfoText.text = ("Health: " + ((SpaceshipTile)selectedTile).health);
		}
		setHighlightHex(coord);
	}

	public void ClearSelect() {
		clearHighlightHex();
	}

	void setHighlightHex(Vector3Int highlightHere) {
        clearHighlightHex();
        highlighted = true;
        overlayTM.SetTile(highlightHere, highlightTile);
        highlightedCoord = highlightHere;
    }

    void clearHighlightHex() {
        if (highlighted) {
            highlighted = false;
            overlayTM.SetTile(highlightedCoord, null);
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
