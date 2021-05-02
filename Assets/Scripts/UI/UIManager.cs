using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class UIManager : MonoBehaviour
{
	public static UIManager UIM;

	public GameObject ResearchCanvas;

    public GameObject FightModalPanel;

    public Text ResourcesText;
    public Text UnitDisplayTitle;
    public Text UnitDisplayEmpire;
    public Text UnitDisplayDetails;
    public Button UnitDisplayBuildButton;
    public Button UnitDisplayAttackButton;
    void Update() {
        ResourcesText.text = "Empire "+GameManager.GM.GetActiveEmpire().empireName + " has " + GameManager.GM.GetActiveEmpire().ownedResources + " resources.";
    }
    public void Test() {
        Debug.Log(GameManager.GM.Empires[0].empireShips[0]);
    }
    //Research button top left
    public void OpenResearchModal() {
    	ResearchCanvas.SetActive(true);
    }
    public void CloseResearchModal() {
    	ResearchCanvas.SetActive(false);
    }

    //Unit panel bottom right
    public void UpdateUnitModal() {
        ClearUnitModal();
        if (SelectionManager.SM.SelectedGameObject == null || SelectionManager.SM.SelectedGameObject.GetComponent<CubicHexComponent>() == null) {
            return;
        }
        HexInfo info = SelectionManager.SM.SelectedGameObject.GetComponent<CubicHexComponent>().Info;
        if (info is ShipInfo) {
            if (info.ParentEmpire == GameManager.GM.GetActiveEmpire()) {
                if (((ShipInfo)info).buildable()) {
                    UnitDisplayBuildButton.gameObject.SetActive(true);
                }
                if (((ShipInfo)info).attackable()) {
                    UnitDisplayAttackButton.gameObject.SetActive(true);
                }
            }
            UnitDisplayTitle.text = "Ship";
            UnitDisplayEmpire.text = "Empire: "+info.ParentEmpire.empireName;
            UnitDisplayDetails.text = "Remaining moves: "+((ShipInfo)info).remainingMoves;
        }
        else if (info is PlanetInfo) {
            if (info.ParentEmpire == GameManager.GM.GetActiveEmpire() && ((PlanetInfo)info).canSpawnShip()) {
                UnitDisplayBuildButton.gameObject.SetActive(true);
            }
            UnitDisplayTitle.text = "Planet";
            UnitDisplayEmpire.text = "Empire: "+((info.ParentEmpire == null) ? "Unowned" : info.ParentEmpire.empireName);
            //UnitDisplayDetails.text = "Remaining moves: "+((PlanetInfo)info).remainingMoves;
            UnitDisplayDetails.text = "Health: "+info.health;
        }
        else if (info is ResourceInfo) {
            UnitDisplayTitle.text = "Resource";
            UnitDisplayEmpire.text = "Empire: "+ ((info.ParentEmpire == null) ? "Unowned" : info.ParentEmpire.empireName);
            //UnitDisplayDetails.text = "Remaining moves: "+((ResourceInfo)info).remainingMoves;
            UnitDisplayDetails.text = "Health: "+info.health;
        }
    }
    public void ClearUnitModal() {
        UnitDisplayTitle.text = "Nothing Selected";
        UnitDisplayEmpire.text = "";
        UnitDisplayDetails.text = "";
        UnitDisplayBuildButton.gameObject.SetActive(false);
        UnitDisplayAttackButton.gameObject.SetActive(false);
    }

    public void BuildAction() {
        CubicHexComponent selectedCHC = SelectionManager.SM.SelectedGameObject.GetComponent<CubicHexComponent>();
        if (selectedCHC.Info is PlanetInfo) {
            //GameManager.GM.GetActiveEmpire().SpawnShip(SelectionManager.SM.SelectedGameObject);
            ((PlanetInfo)selectedCHC.Info).spawnShip();

        } else if (selectedCHC.Info is ShipInfo) {
            ((ShipInfo)selectedCHC.Info).buildOnTile();
        } else {
            Debug.Log("Nothing selected for build");
        }
        UpdateUnitModal();
    }

    public void AttackAction() {
        CubicHexComponent selectedCHC = SelectionManager.SM.SelectedGameObject.GetComponent<CubicHexComponent>();
        if (selectedCHC.Info is ShipInfo) {
            ((ShipInfo)selectedCHC.Info).attackTile();
            //GameManager.GM.GetActiveEmpire().SpawnShip(SelectionManager.SM.SelectedGameObject);
        } else {
            Debug.Log("Nothing selected");
        }
        UpdateUnitModal();
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
