using UnityEngine.Tilemaps;
using UnityEngine;

public class ClickPlanet : MonoBehaviour
{
    public Tilemap planetTilemap;
    //just for initializing a test planet
    public Sprite planetSprite;

    Vector3Int getCoordFromMainCameraMouse(Tilemap atThisTilemap) {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return atThisTilemap.WorldToCell(mouseWorldPos);
    }

    
    void Start() {
        PlanetTile tileInstance = ScriptableObject.CreateInstance<PlanetTile>();
        tileInstance.sprite = planetSprite;
        planetTilemap.SetTile(new Vector3Int(-8,0,0), tileInstance);
    }

    public void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3Int clickedCoord = getCoordFromMainCameraMouse(planetTilemap);
            Debug.Log("Lclick tile: "+clickedCoord);
            SelectionManager.SM.Select(planetTilemap.GetTile<Tile>(clickedCoord), clickedCoord);


            /*if (planetTilemap.HasTile(clickedCoord)) {
                //display the highlight hex around this tile (automatically erase from previously highlighted)
                setHighlightHex(clickedCoord);
                tileToMove = planetTilemap.GetTile<Tile>(clickedCoord);

                
                
            } else if (highlighted){
                clearHighlightHex();
                planetTilemap.SetTile(clickedCoord, tileToMove);
                planetTilemap.SetTile(highlightedCoord, null);
                tileToMove = null;
                //planetTilemap.SwapTile(planetTilemap.GetTile<Tile>(clickedCoord),planetTilemap.GetTile<Tile>(highlightedCoord));
            }*/
        }
        else if (Input.GetMouseButtonDown(1)) {
            Vector3Int clickedCoord = getCoordFromMainCameraMouse(planetTilemap);
            Debug.Log("Rclick tile: "+clickedCoord);
            if (planetTilemap.HasTile(clickedCoord)) {
                PlanetTile temp = planetTilemap.GetTile<PlanetTile>(clickedCoord);
                Debug.Log(temp.health);
                //planetTilemap.SetTile(coordinate, temp);
            }
        }
    }
}