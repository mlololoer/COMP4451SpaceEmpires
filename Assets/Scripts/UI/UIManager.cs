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
    public Button UnitDisplayFrigateButton;
    public Button UnitDisplayDestroyerButton;
    public Button UnitDisplayCruiserButton;

    void Update() {
        ResourcesText.text = "Empire "+GameManager.GM.GetActiveEmpire().empireName + " has " + GameManager.GM.GetActiveEmpire().ownedResources + " resources.";
    }
    public void Test() {
        Debug.Log(GameManager.GM.Empires[0].empireShips[0]);
    }
    //Research button top left
    public void OpenResearchModal() {
    	ResearchCanvas.SetActive(true);
        ResearchCanvas.GetComponentInChildren<ResearchManager>().UpdateColors();
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
        //SHIP
        if (info is ShipInfo) {
            //load buttons
            if (info.ParentEmpire == GameManager.GM.GetActiveEmpire()) {
                HexMapLayer buildLayer = ((ShipInfo)info).buildable();
                if (buildLayer != HexMapLayer.BACKGROUND_LAYER) {
                    UnitDisplayBuildButton.gameObject.SetActive(true);
                    switch(buildLayer) {
                        case (HexMapLayer.RESOURCE_LAYER): {
                            UnitDisplayBuildButton.GetComponentInChildren<Text>().text = "Build Mining Post";
                            break;
                        }
                        case (HexMapLayer.PLANET_LAYER): {
                            UnitDisplayBuildButton.GetComponentInChildren<Text>().text = "Build City";
                            break;
                        }
                        case (HexMapLayer.DYSON_LAYER): {
                            UnitDisplayBuildButton.GetComponentInChildren<Text>().text = "Build Dyson Comp.";
                            break;
                        }
                    }
                }
                HexMapLayer attackLayer = ((ShipInfo)info).attackable();
                if (attackLayer != HexMapLayer.BACKGROUND_LAYER) {
                    UnitDisplayAttackButton.gameObject.SetActive(true);
                    switch(attackLayer) {
                        case (HexMapLayer.RESOURCE_LAYER): {
                            UnitDisplayAttackButton.GetComponentInChildren<Text>().text = "Attack Mining Post";
                            break;
                        }
                        case (HexMapLayer.PLANET_LAYER): {
                            UnitDisplayAttackButton.GetComponentInChildren<Text>().text = "Attack City";
                            break;
                        }
                    }
                }
            }
            //load text
            switch (((ShipInfo)info).shipType) {
                case (ShipType.FRIGATE): {
                    UnitDisplayTitle.text = "Frigate ship";
                    break;
                }
                case (ShipType.DESTROYER): {
                    UnitDisplayTitle.text = "Destroyer ship";
                    break;
                }
                case (ShipType.CRUISER): {
                    UnitDisplayTitle.text = "Cruiser ship";
                    break;
                }
                default: {
                    UnitDisplayTitle.text = "Unknown ship type";
                    break;
                }
            }
            UnitDisplayEmpire.text = "Empire: "+info.ParentEmpire.empireName;
            UnitDisplayDetails.text = "Remaining moves: "+((ShipInfo)info).remainingMoves;
        }
        //PLANET
        else if (info is PlanetInfo) {
            if (info.ParentEmpire == GameManager.GM.GetActiveEmpire()) {
                if (((PlanetInfo)info).canSpawnShip(ShipType.FRIGATE)) {
                    UnitDisplayFrigateButton.gameObject.SetActive(true);
                }
                if (((PlanetInfo)info).canSpawnShip(ShipType.DESTROYER)) {
                    UnitDisplayDestroyerButton.gameObject.SetActive(true);
                }
                if (((PlanetInfo)info).canSpawnShip(ShipType.CRUISER)) {
                    UnitDisplayCruiserButton.gameObject.SetActive(true);
                }
            }
            UnitDisplayTitle.text = "Planet";
            UnitDisplayEmpire.text = "Empire: "+((info.ParentEmpire == null) ? "Unowned" : info.ParentEmpire.empireName);
            //UnitDisplayDetails.text = "Remaining moves: "+((PlanetInfo)info).remainingMoves;
            UnitDisplayDetails.text = "Health: "+info.health;
        }
        //RESOURCE
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
        UnitDisplayFrigateButton.gameObject.SetActive(false);
        UnitDisplayDestroyerButton.gameObject.SetActive(false);
        UnitDisplayCruiserButton.gameObject.SetActive(false);
    }

    public void BuildAction() {
        CubicHexComponent selectedCHC = SelectionManager.SM.SelectedGameObject.GetComponent<CubicHexComponent>();
        if (selectedCHC.Info is ShipInfo) {
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
            Debug.Log("Nothing selected for attack");
        }
        UpdateUnitModal();
    }

    public void FrigateAction() {
        CubicHexComponent selectedCHC = SelectionManager.SM.SelectedGameObject.GetComponent<CubicHexComponent>();
        if (selectedCHC.Info is PlanetInfo) {
            ((PlanetInfo)selectedCHC.Info).spawnShip(ShipType.FRIGATE);
        } else {
            Debug.Log("Frigate not spawned");
        }
        UpdateUnitModal();
    }

    public void DestroyerAction() {
        CubicHexComponent selectedCHC = SelectionManager.SM.SelectedGameObject.GetComponent<CubicHexComponent>();
        if (selectedCHC.Info is PlanetInfo) {
            ((PlanetInfo)selectedCHC.Info).spawnShip(ShipType.DESTROYER);
        } else {
            Debug.Log("Destroyer not spawned");
        }
        UpdateUnitModal();
    }

    public void CruiserAction() {
        CubicHexComponent selectedCHC = SelectionManager.SM.SelectedGameObject.GetComponent<CubicHexComponent>();
        if (selectedCHC.Info is PlanetInfo) {
            ((PlanetInfo)selectedCHC.Info).spawnShip(ShipType.CRUISER);
        } else {
            Debug.Log("Cruiser not spawned");
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
