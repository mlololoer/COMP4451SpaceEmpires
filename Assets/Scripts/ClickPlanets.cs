using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class ClickPlanets : MonoBehaviour
{
    //public Grid grid;
    public Tilemap overlayTilemap;
    public Tilemap planetTilemap;

    //display highlight hex
    public Tile highlightTile;
    bool highlighted = false;
    Vector3Int highlightedCoord;
    Tile tileToMove;

    Vector3Int getCoordFromMainCameraMouse(Tilemap atThisTilemap)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return atThisTilemap.WorldToCell(mouseWorldPos);
    }

    void setHighlightHex(Vector3Int coordToHighlight)
    {
        clearHighlightHex();
        highlighted = true;
        overlayTilemap.SetTile(coordToHighlight, highlightTile);
        highlightedCoord = coordToHighlight;
    }

    void clearHighlightHex()
    {
        if (highlighted)
        {
            highlighted = false;
            overlayTilemap.SetTile(highlightedCoord, null);
        }
    }



    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int clickedCoord = getCoordFromMainCameraMouse(planetTilemap);
            Debug.Log(clickedCoord);

            if (planetTilemap.HasTile(clickedCoord))
            {
                //display the highlight hex around this tile (automatically erase from previously highlighted)
                setHighlightHex(clickedCoord);
                tileToMove = planetTilemap.GetTile<Tile>(clickedCoord);


                //SpaceshipTile tileInstance = ScriptableObject.CreateInstance<SpaceshipTile>();
                //Texture2D tex = Resources.Load<Texture2D>("Textures/fantasyhextiles_v3");
                //Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 200, 100), new Vector2(0.5f, 0.5f));
                //tileInstance.sprite = sprite;
                //spaceshipTilemap.SetTile(clickedCoord, tileInstance);
            }
            else if (highlighted)
            {
                clearHighlightHex();
                planetTilemap.SetTile(clickedCoord, tileToMove);
                planetTilemap.SetTile(highlightedCoord, null);
                tileToMove = null;
                //spaceshipTilemap.SwapTile(spaceshipTilemap.GetTile<Tile>(clickedCoord),spaceshipTilemap.GetTile<Tile>(highlightedCoord));
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3Int coordinate = getCoordFromMainCameraMouse(planetTilemap);

            if (planetTilemap.HasTile(coordinate))
            {
                PlanetTile temp = planetTilemap.GetTile<PlanetTile>(coordinate);
                Debug.Log(temp.resource);
                //spaceshipTilemap.SetTile(coordinate, temp);
            }
        }
    }
}
