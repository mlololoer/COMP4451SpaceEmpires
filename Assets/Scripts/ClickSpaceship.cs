using UnityEngine.Tilemaps;
using UnityEngine;

public class ClickSpaceship : MonoBehaviour
{
    //public Grid grid;
    public Tilemap tilemap;
    bool moving = false;
    // Update is called once per frame
    Tile temp;
	public void Update()
    {
    	if (Input.GetMouseButtonDown(0)) {
    		Vector3 mousePos = Input.mousePosition;
    		mousePos.z = -Camera.main.transform.position.z;
	        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
	        Vector3Int coordinate = tilemap.WorldToCell(mouseWorldPos);

            if (tilemap.HasTile(coordinate)) {
                temp = tilemap.GetTile<Tile>(coordinate);

                Debug.Log(coordinate);
                SpaceshipTile tileInstance = ScriptableObject.CreateInstance<SpaceshipTile>();
                Texture2D tex = Resources.Load<Texture2D>("Textures/fantasyhextiles_v3");
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 200, 100), new Vector2(0.5f, 0.5f));
                tileInstance.sprite = sprite;
                tilemap.SetTile(coordinate, tileInstance);
                
                //tilemap.SetTile(coordinate, tile);
	        }
    	}
        if (Input.GetMouseButtonDown(1)) {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3Int coordinate = tilemap.WorldToCell(mouseWorldPos);
            Debug.Log(coordinate);
            if (tilemap.HasTile(coordinate)) {
                SpaceshipTile temp = tilemap.GetTile<SpaceshipTile>(coordinate);
                Debug.Log(temp.health);
                //tilemap.SetTile(coordinate, temp);
            }
        }
    }
}