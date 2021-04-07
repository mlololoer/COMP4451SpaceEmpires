using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class UIManager : MonoBehaviour
{
	public static UIManager UIM;

	public GameObject ResearchCanvas;

    public void OpenTileModal(Tile tile) {
    	if (tile is SpaceshipTile) {

    	} else if (tile is PlanetTile) {
    		
    	}
    }

    public void OpenResearchModal() {
    	ResearchCanvas.SetActive(true);
    }
    public void CloseResearchModal() {
    	ResearchCanvas.SetActive(false);
    }

    void Awake() {
		if(UIM != null) {
			GameObject.Destroy(UIM);
		} else {
			UIM = this;
		}
		DontDestroyOnLoad(this);
	}
}
