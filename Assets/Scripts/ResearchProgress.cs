//manages and stores player research including progress

public class ResearchProgress {
	/*	Research Hierarchy (for now)
		Spaceship Heat Shield 1 -> Spaceship Heat Shield 2 -> Spaceship Heat Shield 3	(higher heat shield: reach closer to central star)
		Spaceship Durability 1 -> Spaceship Durability 2 -> Spaceship Durability 3	(higher durability: unlock spaceships with more health and weapons)

		Ship heat shields (just two)
		Ship types (frigate only initially, destroyer -> cruiser)
		Ship Health Boost (1)
		Ship Damage Boost (1)
		Ship Limits (4 initially, 5->6.)
	*/	

	Empire ParentEmpire;
	readonly int[] heatShieldsTTC = new int[3] {Constants.TURNS_FOR_RESEARCH0,Constants.TURNS_FOR_RESEARCH1,Constants.TURNS_FOR_RESEARCH2};
	readonly int[] shipTypesTTC = new int[3] {Constants.TURNS_FOR_RESEARCH0,Constants.TURNS_FOR_RESEARCH1,Constants.TURNS_FOR_RESEARCH2};
	readonly int[] limitsTTC = new int[3] {Constants.TURNS_FOR_RESEARCH0,Constants.TURNS_FOR_RESEARCH1,Constants.TURNS_FOR_RESEARCH2};
	readonly int attackBonusTTC = Constants.TURNS_FOR_RESEARCH1;
	readonly int healBonusTTC = Constants.TURNS_FOR_RESEARCH1;
	readonly int[] heatShieldsRCost = new int[3] {Constants.RESOURCES_FOR_RESEARCH0,Constants.RESOURCES_FOR_RESEARCH1,Constants.RESOURCES_FOR_RESEARCH2};
	readonly int[] shipTypesRCost = new int[3] {Constants.RESOURCES_FOR_RESEARCH0,Constants.RESOURCES_FOR_RESEARCH1,Constants.RESOURCES_FOR_RESEARCH2};
	readonly int[] limitsRCost = new int[3] {Constants.RESOURCES_FOR_RESEARCH0,Constants.RESOURCES_FOR_RESEARCH1,Constants.RESOURCES_FOR_RESEARCH2};
	readonly int attackBonusRCost = Constants.RESOURCES_FOR_RESEARCH0;
	readonly int healBonusRCost = Constants.RESOURCES_FOR_RESEARCH0;
	//0
	int[] heatShieldsResearch = new int[3] {Constants.TURNS_FOR_RESEARCH0,0,0};
	int[] shipTypesResearch = new int[3] {Constants.TURNS_FOR_RESEARCH0,0,0};
	int[] limitsResearch = new int[3] {Constants.TURNS_FOR_RESEARCH0,0,0};
	//3
	int attackBonusResearch = 0;
	//4
	int healBonusResearch = 0;

	public ResearchProgress(Empire parentEmpire) {
		this.ParentEmpire = parentEmpire;
	}

	public bool researching {get; private set;} = false;
	public int researchingType {get;private set;} = 0;
	public int researchingIdx {get;private set;} = 0;

	public int GetHeatShieldsLevel() {
		int i = 0;
		for (; i < heatShieldsResearch.Length; ++i) {
			if (heatShieldsResearch[i] < heatShieldsTTC[i]) break;
		}
		return i-1;
	}

	public int GetShipTypesLevel() {
		int i = 0;
		for (; i < shipTypesResearch.Length; ++i) {
			if (shipTypesResearch[i] < shipTypesTTC[i]) break;
		}
		return i-1;
	}

	public int GetLimitsLevel() {
		int i = 0;
		for (; i < limitsResearch.Length; ++i) {
			if (limitsResearch[i] < limitsTTC[i]) break;
		}
		return i-1;
	}

	public bool GetAttackBonus() {
		return (attackBonusResearch >= attackBonusTTC);
	}

	public bool GetHealBonus() {
		return (healBonusResearch >= healBonusTTC);
	}

	public void ProcessTurn() {
		if (researching) {
			switch(researchingType) {
				case 0: {
					heatShieldsResearch[researchingIdx]++;
					if (heatShieldsResearch[researchingIdx] >= heatShieldsTTC[researchingIdx]) {
						researching = false;
					}
					break;
				}
				case 1: {
					shipTypesResearch[researchingIdx]++;
					if (shipTypesResearch[researchingIdx] >= shipTypesTTC[researchingIdx]) {
						researching = false;
					}
					break;
				}
				case 2: {
					limitsResearch[researchingIdx]++;
					if (limitsResearch[researchingIdx] >= limitsTTC[researchingIdx]) {
						researching = false;
					}
					break;
				}
				case 3: {
					attackBonusResearch++;
					if (attackBonusResearch >= attackBonusTTC) {
						researching = false;
					}
					break;
				}
				case 4: {
					healBonusResearch++;
					if (healBonusResearch >= healBonusTTC) {
						researching = false;
					}
					break;
				}
			}
		}
	}

	public bool SetResearch(int researchType, int researchIdx) {
		if (CanResearch(researchType,researchIdx) && !CompletedResearch(researchType,researchIdx)) {
			researching = true;
			researchingType = researchType;
			researchingIdx = researchIdx;
			switch(researchingType) {
				case 0: {ParentEmpire.SpendResources(heatShieldsRCost[researchIdx]);break;}
				case 1: {ParentEmpire.SpendResources(shipTypesRCost[researchIdx]);break;}
				case 2: {ParentEmpire.SpendResources(limitsRCost[researchIdx]);break;}
				case 3: {ParentEmpire.SpendResources(attackBonusRCost);break;}
				case 4: {ParentEmpire.SpendResources(healBonusRCost);break;}
			}
			return true;
		} else {
			return false;
		}
	}

	public bool CanResearch(int researchType, int researchIdx) {
		if (researchIdx == 0) return true;
		switch(researchType) {
			case 0: {
				if (heatShieldsResearch[researchIdx-1] >= heatShieldsTTC[researchIdx-1] && heatShieldsRCost[researchIdx] <= ParentEmpire.ownedResources) {
					return true;
				}
				break;
			}
			case 1: {
				if (shipTypesResearch[researchIdx-1] >= shipTypesTTC[researchIdx-1] && shipTypesRCost[researchIdx] <= ParentEmpire.ownedResources) {
					return true;
				}
				break;
			}
			case 2: {
				if (limitsResearch[researchIdx-1] >= limitsTTC[researchIdx-1] && limitsRCost[researchIdx] <= ParentEmpire.ownedResources) {
					return true;
				}
				break;
			}
			case 3: {
				if (attackBonusRCost <= ParentEmpire.ownedResources) {
					return true;
				}
				break;
			}
			case 4: {
				if (healBonusRCost <= ParentEmpire.ownedResources) {
					return true;
				}
				break;
			}
		}
		return false;
	}

	public bool CompletedResearch(int researchType, int researchIdx) {
		if (researchIdx == 0) return true;
		switch(researchType) {
			case 0: {
				if (heatShieldsResearch[researchIdx] >= heatShieldsTTC[researchIdx]) {
					return true;
				}
				break;
			}
			case 1: {
				if (shipTypesResearch[researchIdx] >= shipTypesTTC[researchIdx]) {
					return true;
				}
				break;
			}
			case 2: {
				if (limitsResearch[researchIdx] >= limitsTTC[researchIdx]) {
					return true;
				}
				break;
			}
			case 3: {
				if (attackBonusResearch >= attackBonusTTC) {
					return true;
				}
				break;
			}
			case 4: {
				if (healBonusResearch >= healBonusTTC) {
					return true;
				}
				break;
			}
		}
		return false;
	}
}