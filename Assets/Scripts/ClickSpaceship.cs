using UnityEngine.Tilemaps;
using UnityEngine;

public class ClickSpaceship : MonoBehaviour
{
    public Tilemap spaceshipTilemap;
    //just for initializing a test spaceship
    public Sprite spaceshipSprite;

    Vector3Int getCoordFromMainCameraMouse(Tilemap atThisTilemap) {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return atThisTilemap.WorldToCell(mouseWorldPos);
    }

    
    void Start() {
        SpaceshipTile tileInstance = ScriptableObject.CreateInstance<SpaceshipTile>();
        tileInstance.sprite = spaceshipSprite;
        spaceshipTilemap.SetTile(new Vector3Int(-8,0,0), tileInstance);
    }

    public void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3Int clickedCoord = getCoordFromMainCameraMouse(spaceshipTilemap);
            Debug.Log("Lclick tile: "+clickedCoord);
            SelectionManager.SM.Select(spaceshipTilemap.GetTile<Tile>(clickedCoord), clickedCoord);


            /*if (spaceshipTilemap.HasTile(clickedCoord)) {
                //display the highlight hex around this tile (automatically erase from previously highlighted)
                setHighlightHex(clickedCoord);
                tileToMove = spaceshipTilemap.GetTile<Tile>(clickedCoord);

                
                
            } else if (highlighted){
                clearHighlightHex();
                spaceshipTilemap.SetTile(clickedCoord, tileToMove);
                spaceshipTilemap.SetTile(highlightedCoord, null);
                tileToMove = null;
                //spaceshipTilemap.SwapTile(spaceshipTilemap.GetTile<Tile>(clickedCoord),spaceshipTilemap.GetTile<Tile>(highlightedCoord));
            }*/
        }
        else if (Input.GetMouseButtonDown(1)) {
            Vector3Int clickedCoord = getCoordFromMainCameraMouse(spaceshipTilemap);
            Debug.Log("Rclick tile: "+clickedCoord);
            if (spaceshipTilemap.HasTile(clickedCoord)) {
                SpaceshipTile temp = spaceshipTilemap.GetTile<SpaceshipTile>(clickedCoord);
                Debug.Log(temp.health);
                //spaceshipTilemap.SetTile(coordinate, temp);
            }
        }
    }
}