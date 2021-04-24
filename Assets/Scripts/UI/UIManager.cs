using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class UIManager : MonoBehaviour
{
	public static UIManager UIM;

	public GameObject ResearchCanvas;

    public GameObject FightModalPanel;

    public Text UnitModalName;
    public Text UnitModalDetails;

    

    public void OpenResearchModal() {
    	ResearchCanvas.SetActive(true);
    }
    public void CloseResearchModal() {
    	ResearchCanvas.SetActive(false);
    }

    public void UpdateUnitModal(string unitName, int unitDetails) {
        UnitModalName.text = unitName;
        UnitModalDetails.text = unitDetails.ToString();

    }

    public void OpenFightModal() {
        FightModalPanel.SetActive(true);
    }

    /*public void OpenTileModal(Tile tile) {
        if (tile is SpaceshipTile) {

        } else if (tile is PlanetTile) {
            
        }
    }*/

    void Awake() {
		if(UIM != null) {
			GameObject.Destroy(UIM);
		} else {
			UIM = this;
		}
		DontDestroyOnLoad(this);
	}
}
