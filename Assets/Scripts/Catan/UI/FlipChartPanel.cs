using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlipChartPanel : MonoBehaviour {
	
	public GameObject tradeHolder;
	public GameObject politicsHolder;
	public GameObject scienceHolder;

	public List<Image> tradeActive;
	public List<Image> politicsActive;
	public List<Image> scienceActive;
	public List<Image> tradeCosts;
	public List<Image> politicsCosts;
	public List<Image> scienceCosts;

	public Text	tradeSpecialAbilitylbl;
	public Text politicsSpecialAbilitylbl;
	public Text scienceSpecialAbilitylbl;

	public Text	tradeSpecialAbility;
	public Text politicsSpecialAbility;
	public Text scienceSpecialAbility;

	public Text tradePlus;
	public Text politicsPlus;
	public Text sciencePlus;

	public Text tradeCardLabel;
	public Text politicsCardLabel;
	public Text scienceCardLabel;

	public Text nextUpTrade;
	public Text nextUpPolitics;
	public Text nextUpScience;

	public Image tradeMax;
	public Image politicsMax;
	public Image scienceMax;

	public Image yellow;
	public Image blue;
	public Image green;

	public List<Image> yellowDice;
	public List<Image> blueDice;
	public List<Image> greenDice;

	public Button tradeUp;
	public Button politicsUp;
	public Button scienceUp;

	public Image tradeMetropolisOwnerDisplay;
	public Image politicsMetropolisOwnerDisplay;
	public Image scienceMetropolisOwnerDisplay;

	public Button buildTrade;
	public Button buildPolitics;
	public Button buildscience;

	public Vector3 scale;

	void Start() {
		scale = this.transform.localScale;
	}

	public void openPanel(int x,int y, int z){
		//add methods to get lvls from player this needs to be a tuple in asset of lvls from 0 to 5
		int tradelvl = x;
		int politicslvl = y;
		int sciencelvl = z;
		this.gameObject.SetActive (true);

		//setting the active levels for each section
		if (tradelvl == 5) {
			yellow.gameObject.SetActive (true);
			for (int i = 0; i < 6; i++) {
				yellowDice [i].gameObject.SetActive (true);
			}
			tradeCardLabel.gameObject.SetActive (true);
			tradePlus.gameObject.SetActive (true);

			tradeHolder.gameObject.SetActive (false);
			tradeMax.gameObject.SetActive (true);
			nextUpTrade.text="No Further Upgrade Available";
			tradeSpecialAbilitylbl.gameObject.SetActive (true);
			tradeSpecialAbility.gameObject.SetActive (true);
			tradeUp.gameObject.SetActive (false);

			for (int i = 0; i < 5; i++) {
				tradeCosts [i].gameObject.SetActive (false);
			}

		} else {
			tradeHolder.gameObject.SetActive (true);
			tradeMax.gameObject.SetActive (false);
			// remove shaders for active lvls
			for (int i = 0; i < tradelvl; i++) {
				tradeActive [i].gameObject.SetActive (false);
			}
			//set cost array
			int cost = tradelvl + 1;
			if (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playedCrane) {
				cost= cost - 1;
			}
			for (int i = 0; i < 5; i++) {
				if (i < cost) {
					tradeCosts [i].gameObject.SetActive (true);
				} else {
					tradeCosts [i].gameObject.SetActive (false);
				}
			}

			nextUpTrade.text="Next Upgrade Cost:";
			tradeUp.gameObject.SetActive (true);
			if (tradelvl > 0) {
				yellow.gameObject.SetActive (true);
				for (int i = 0; i < 6; i++) {
					if (i < tradelvl+1) {
						yellowDice [i].gameObject.SetActive (true);
					} else {
						yellowDice [i].gameObject.SetActive (false);
					}
				}
				tradeCardLabel.gameObject.SetActive (true);
				tradePlus.gameObject.SetActive (true);
			} else {
				yellow.gameObject.SetActive (false);
				tradeCardLabel.gameObject.SetActive (false);
				tradePlus.gameObject.SetActive (false);
				for (int i = 0; i < 6; i++) {
					yellowDice [i].gameObject.SetActive (false);
				}
			}
			if (tradelvl >= 3) {
				tradeSpecialAbilitylbl.gameObject.SetActive (true);
				tradeSpecialAbility.gameObject.SetActive (true);
			} else {
				tradeSpecialAbilitylbl.gameObject.SetActive (false);
				tradeSpecialAbility.gameObject.SetActive (false);
			}
		}
		if(politicslvl == 5) {
			blue.gameObject.SetActive (true);
			for (int i = 0; i < 6; i++) {
				blueDice [i].gameObject.SetActive (true);
			}
			politicsCardLabel.gameObject.SetActive (true);
			politicsPlus.gameObject.SetActive (true);
			politicsHolder.gameObject.SetActive (false);
			politicsMax.gameObject.SetActive (true);
			nextUpPolitics.text="No Further Upgrade Available";
			politicsSpecialAbilitylbl.gameObject.SetActive (true);
			politicsSpecialAbility.gameObject.SetActive (true);
			politicsUp.gameObject.SetActive (false);

			for (int i = 0; i < 5; i++) {
				politicsCosts [i].gameObject.SetActive (false);
			}
		} else {
			politicsHolder.gameObject.SetActive (true);
			politicsMax.gameObject.SetActive (false);
			// remove shaders for active lvls
			for (int i = 0; i < politicslvl; i++) {
				politicsActive [i].gameObject.SetActive (false);
			}
			//set cost array
			int cost = politicslvl + 1;
			if (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playedCrane) {
				cost= cost - 1;
			}
			for (int i = 0; i < 5; i++) {
				if (i < cost) {
					politicsCosts [i].gameObject.SetActive (true);
				} else {
					politicsCosts [i].gameObject.SetActive (false);
				}
			}

			nextUpPolitics.text="Next Upgrade Cost:";
			politicsUp.gameObject.SetActive (true);
			if (politicslvl > 0) {
				blue.gameObject.SetActive (true);
				for (int i = 0; i < 6; i++) {
					if (i < politicslvl+1) {
						blueDice [i].gameObject.SetActive (true);
					} else {
						blueDice [i].gameObject.SetActive (false);
					}
				}
				politicsCardLabel.gameObject.SetActive (true);
				politicsPlus.gameObject.SetActive (true);
			} else {
				blue.gameObject.SetActive (false);
				for (int i = 0; i < 6; i++) {
					blueDice [i].gameObject.SetActive (false);
				}
				politicsCardLabel.gameObject.SetActive (false);
				politicsPlus.gameObject.SetActive (false);
			}
			if (politicslvl >= 3) {
				politicsSpecialAbilitylbl.gameObject.SetActive (true);
				politicsSpecialAbility.gameObject.SetActive (true);
			} else {
				politicsSpecialAbilitylbl.gameObject.SetActive (false);
				politicsSpecialAbility.gameObject.SetActive (false);
			}
		}

		if (sciencelvl == 5) {
			green.gameObject.SetActive (true);
			for (int i = 0; i < 6; i++) {
				greenDice [i].gameObject.SetActive (true);
			}
			scienceHolder.gameObject.SetActive (false);
			scienceMax.gameObject.SetActive (true);
			scienceCardLabel.gameObject.SetActive (true);
			sciencePlus.gameObject.SetActive (true);
			nextUpScience.text="No Further Upgrade Available";
			scienceSpecialAbilitylbl.gameObject.SetActive (true);
			scienceSpecialAbility.gameObject.SetActive (true);
			scienceUp.gameObject.SetActive (false);

			for (int i = 0; i < 5; i++) {
				scienceCosts [i].gameObject.SetActive (false);
			}

		} else {
			scienceHolder.gameObject.SetActive (true);
			scienceMax.gameObject.SetActive (false);
			// remove shaders for active lvls
			for (int i = 0; i < sciencelvl; i++) {
				scienceActive [i].gameObject.SetActive (false);
			}
			//set cost array
			int cost = sciencelvl + 1;
			if (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playedCrane) {
				cost= cost - 1;
			}
			for (int i = 0; i < 5; i++) {
				if (i < cost) {
					scienceCosts [i].gameObject.SetActive (true);
				} else {
					scienceCosts [i].gameObject.SetActive (false);
				}
			}

			nextUpPolitics.text="Next Upgrade Cost:";
			politicsUp.gameObject.SetActive (true);
			if (sciencelvl > 0) {
				green.gameObject.SetActive (true);
				for (int i = 0; i < 6; i++) {
					if (i < sciencelvl+1) {
						greenDice [i].gameObject.SetActive (true);
					} else {
						greenDice [i].gameObject.SetActive (false);
					}
				}
				scienceCardLabel.gameObject.SetActive (true);
				sciencePlus.gameObject.SetActive (true);
			} else {
				scienceCardLabel.gameObject.SetActive (false);
				sciencePlus.gameObject.SetActive (false);
				green.gameObject.SetActive (false);
				for (int i = 0; i < 6; i++) {
					greenDice [i].gameObject.SetActive (false);
				}
			}
			if (sciencelvl >= 3) {
				scienceSpecialAbilitylbl.gameObject.SetActive (true);
				scienceSpecialAbility.gameObject.SetActive (true);
			} else {
				scienceSpecialAbilitylbl.gameObject.SetActive (false);
				scienceSpecialAbility.gameObject.SetActive (false);
			}
		}

		if (sciencelvl >= 4 && CatanManager.instance.players[CatanManager.instance.currentPlayerTurn].showMetropolisBuildButton((int)MetropolisType.Science)) {
			buildscience.gameObject.SetActive (true);
		}
		if (politicslvl >= 4 && CatanManager.instance.players[CatanManager.instance.currentPlayerTurn].showMetropolisBuildButton((int)MetropolisType.Politics)) {
			buildPolitics.gameObject.SetActive (true);
		}
		if (tradelvl >= 4 && CatanManager.instance.players[CatanManager.instance.currentPlayerTurn].showMetropolisBuildButton((int)MetropolisType.Trade)) {
			buildTrade.gameObject.SetActive (true);
		}
		this.gameObject.SetActive (true);

	}

	public void upgradeTrade(){
		CityImprovementType trade = CityImprovementType.Trade;

		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].canUpgrade (trade)) {
			AssetTuple upgradeCost = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.nextImprovementCost (trade);
			if (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playedCrane) {
				upgradeCost.SetValueAtIndex(7,upgradeCost.GetValueAtIndex(7)-1);
				CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playedCrane = false;
			}
				
			EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, false, upgradeCost);
			EventTransferManager.instance.OnUpgradeCity (CatanManager.instance.currentPlayerTurn, (int)trade);

			int scienceLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Science];
			int politicsLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Politics];
			int tradeLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Trade];

			openPanel (tradeLevel, politicsLevel, scienceLevel);
		}
	}

	public void upgradePolitics(){
		CityImprovementType politics = CityImprovementType.Politics;

		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].canUpgrade (politics)) {
			AssetTuple upgradeCost = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.nextImprovementCost (politics);
			if (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playedCrane) {
				upgradeCost.SetValueAtIndex(6,upgradeCost.GetValueAtIndex(6)-1);
				CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playedCrane = false;
			}


			EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, false, upgradeCost);
			EventTransferManager.instance.OnUpgradeCity (CatanManager.instance.currentPlayerTurn, (int)politics);

			int scienceLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Science];
			int politicsLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Politics];
			int tradeLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Trade];

			openPanel (tradeLevel, politicsLevel, scienceLevel);
		}
	}

	public void upgradeScience(){
		CityImprovementType science = CityImprovementType.Science;

		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].canUpgrade (science)) {
			AssetTuple upgradeCost = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.nextImprovementCost (science);
			if (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playedCrane) {
				upgradeCost.SetValueAtIndex(5,upgradeCost.GetValueAtIndex(5)-1);
				CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playedCrane = false;
			}
				
			EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, false, upgradeCost);
			EventTransferManager.instance.OnUpgradeCity (CatanManager.instance.currentPlayerTurn, (int)science);

			int scienceLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Science];
			int politicsLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Politics];
			int tradeLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Trade];

			openPanel (tradeLevel, politicsLevel, scienceLevel);
		}
	}

	public void buildTradeMetropolis(){
		StartCoroutine (buildMetropolisCoroutine(MetropolisType.Trade));
	}

	public void buildPoliticsMetropolis(){
		StartCoroutine (buildMetropolisCoroutine(MetropolisType.Politics));
	}

	public void buildScienceMetropolis(){
		StartCoroutine (buildMetropolisCoroutine(MetropolisType.Science));
	}

	public IEnumerator buildMetropolisCoroutine(MetropolisType metropolisType) {
		this.transform.localScale = Vector3.zero;
		yield return StartCoroutine (EventTransferManager.instance.ClientUpgradeCity(PhotonNetwork.player.ID - 1, (int)metropolisType));
		this.transform.localScale = scale;
		this.gameObject.SetActive (false);
	}
}
