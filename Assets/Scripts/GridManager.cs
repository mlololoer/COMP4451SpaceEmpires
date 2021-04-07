using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
	public Sprite planetSprite;
    // Start is called before the first frame update
    void Start()
    {
        /*SpaceshipTile a = ScriptableObject.CreateInstance<SpaceshipTile>();
        a.sprite = planetSprite;
        Debug.Log(a);
        Debug.Log(a.sprite);
        SpaceshipTile b = a;
        Debug.Log(b);
        Debug.Log(b.sprite);
        Debug.Log(a == b);
        a = null;
        Debug.Log(b);
        Debug.Log(b.sprite);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //rotate a ring around by a number of steps
    public void MoveRing(int steps) {

    }
}
