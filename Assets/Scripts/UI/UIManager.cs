using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class UIManager : MonoBehaviour
{
	public static UIManager UIM;

	public GameObject ResearchCanvas;

    public GameObject FightModalPanel;

    public Text UnitDisplayTitle;
    public Text UnitDisplayEmpire;
    public Text UnitDisplayDetails;

    //Research button top left
    public void OpenResearchModal() {
    	ResearchCanvas.SetActive(true);
    }
    public void CloseResearchModal() {
    	ResearchCanvas.SetActive(false);
    }

    //Unit panel bottom right
    public void UpdateUnitModal(HexInfo info) {
        if (info == null) {
            ClearUnitModal();
            return;
        }
        if (info is ShipInfo) {
            UnitDisplayTitle.text = "Ship";
            UnitDisplayEmpire.text = "Empire: "+info.ParentEmpire.empireName;
            UnitDisplayDetails.text = "Remaining moves: "+((ShipInfo)info).remainingMoves;
        }
        else if (info is PlanetInfo) {
            UnitDisplayTitle.text = "Planet";
            UnitDisplayEmpire.text = "Empire: "+info.ParentEmpire.empireName;
            //UnitDisplayDetails.text = "Remaining moves: "+((PlanetInfo)info).remainingMoves;
            UnitDisplayDetails.text = "Health: "+info.health;
        }
        else if (info is ResourceInfo) {
            UnitDisplayTitle.text = "Resource";
            //UnitDisplayEmpire.text = "Empire: "+ ((info.ParentEmpire.empireName == null) ? "Unowned" : info.ParentEmpire.empireName);
            //UnitDisplayDetails.text = "Remaining moves: "+((ResourceInfo)info).remainingMoves;
            UnitDisplayDetails.text = "Health: "+info.health;
        }
    }
    public void ClearUnitModal() {
        UnitDisplayTitle.text = "Nothing Selected";
        UnitDisplayEmpire.text = "";
        UnitDisplayDetails.text = "";
    }

    public void BuildAction() {
        if (SelectionManager.SM.SelectedGameObject.GetComponent<CubicHexComponent>().Info is PlanetInfo) {
            GameManager.GM.GetActiveEmpire().SpawnShip(SelectionManager.SM.SelectedGameObject);
        } else {
            Debug.Log("Nothing selected");
        }
        
        
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
