using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipModal : MonoBehaviour
{
	public SpaceshipTile tile;
	public Text title;

    public void updateModal() {
    	title.text = (tile.spaceshipName);
    }
}
