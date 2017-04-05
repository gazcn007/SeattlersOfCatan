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

	public Text nextUpTrade;
	public Text nextUpPolitics;
	public Text nextUpScience;

	public Image tradeMax;
	public Image politicsMax;
	public Image scienceMax;

	public Button tradeUp;
	public Button politicsUp;
	public Button scienceUp;



	public void openPanel(int x,int y, int z){
		//add methods to get lvls from player this needs to be a tuple in asset of lvls from 0 to 5
		int tradelvl=x;
		int politicslvl=y;
		int sciencelvl=z;

		//setting the active levels for each section
		if (tradelvl == 5) {
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
			for (int i = 0; i < 4; i++) {
				if (i < cost) {
					tradeCosts [i].gameObject.SetActive (true);
				} else {
					tradeCosts [i].gameObject.SetActive (false);
				}
			}

			nextUpTrade.text="Next Upgrade Cost:";
			tradeUp.gameObject.SetActive (true);

			if (tradelvl >= 3) {
				tradeSpecialAbilitylbl.gameObject.SetActive (true);
				tradeSpecialAbility.gameObject.SetActive (true);
			} else {
				tradeSpecialAbilitylbl.gameObject.SetActive (false);
				tradeSpecialAbility.gameObject.SetActive (false);
			}
		}
		if(politicslvl == 5) {
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
			for (int i = 0; i < 5; i++) {
				if (i < cost) {
					politicsCosts [i].gameObject.SetActive (true);
				} else {
					politicsCosts [i].gameObject.SetActive (false);
				}
			}

			nextUpPolitics.text="Next Upgrade Cost:";
			politicsUp.gameObject.SetActive (true);

			if (politicslvl >= 3) {
				politicsSpecialAbilitylbl.gameObject.SetActive (true);
				politicsSpecialAbility.gameObject.SetActive (true);
			} else {
				politicsSpecialAbilitylbl.gameObject.SetActive (false);
				politicsSpecialAbility.gameObject.SetActive (false);
			}
		}
		if (sciencelvl == 5) {
			scienceHolder.gameObject.SetActive (false);
			scienceMax.gameObject.SetActive (true);
			nextUpScience.text="No Further Upgrade Available";
			scienceSpecialAbilitylbl.gameObject.SetActive (true);
			scienceSpecialAbility.gameObject.SetActive (true);
			scienceUp.gameObject.SetActive (false);

		} else {
			scienceHolder.gameObject.SetActive (true);
			scienceMax.gameObject.SetActive (false);
			// remove shaders for active lvls
			for (int i = 0; i < sciencelvl; i++) {
				scienceActive [i].gameObject.SetActive (false);
			}
			//set cost array
			int cost = sciencelvl + 1;
			for (int i = 0; i < 5; i++) {
				if (i < cost) {
					scienceCosts [i].gameObject.SetActive (true);
				} else {
					scienceCosts [i].gameObject.SetActive (false);
				}
			}

			nextUpPolitics.text="Next Upgrade Cost:";
			politicsUp.gameObject.SetActive (true);

			if (sciencelvl >= 3) {
				scienceSpecialAbilitylbl.gameObject.SetActive (true);
				scienceSpecialAbility.gameObject.SetActive (true);
			} else {
				scienceSpecialAbilitylbl.gameObject.SetActive (false);
				scienceSpecialAbility.gameObject.SetActive (false);
			}
		}
		this.gameObject.SetActive (true);

	}
	public void upgradeTrade(){

	}
	public void upgradePolitics(){

	}
	public void upgradeScience(){

	}
}
