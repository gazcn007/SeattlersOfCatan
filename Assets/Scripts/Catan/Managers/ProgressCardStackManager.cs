using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCardStackManager : MonoBehaviour {


	public GameObject progressCardPrefab;

	public ProgressCardType[] yellowCards;
	public ProgressCardType[] blueCards;
	public ProgressCardType[] greenCards;

	public List<ProgressCard> yellowCardsQueue;
	public List<ProgressCard> blueCardsQueue;
	public List<ProgressCard> greenCardsQueue;
	// Use this for initialization
	void Start () {

	}
	public void generateCards () {
		//mix the 3 arrays if this is client otherwise spawn order is already defined
		//instantiate the 3 queues with their cards
		for (int i = 0; i < yellowCards.Length; i++) {
			GameObject card = Instantiate (progressCardPrefab);
			ProgressCard curr = card.GetComponent<ProgressCard> ();
			card.transform.parent = this.transform;
			card.name = yellowCards [i].ToString() + " Card";

			curr.type = yellowCards [i];
			curr.color = ProgressCardColor.Yellow;

			yellowCardsQueue.Add (curr);
		}
		for (int i = 0; i < blueCards.Length; i++) {
			GameObject card = Instantiate (progressCardPrefab);
			ProgressCard curr = card.GetComponent<ProgressCard> ();
			card.transform.parent = this.transform;
			card.name = blueCards [i].ToString() + " Card";

			curr.type = blueCards[i];
			curr.color = ProgressCardColor.Blue;

			blueCardsQueue.Add (curr);
		}
		for (int i = 0; i < greenCards.Length; i++) {
			GameObject card = Instantiate (progressCardPrefab);
			ProgressCard curr = card.GetComponent<ProgressCard> ();
			card.transform.parent = this.transform;
			card.name = greenCards [i].ToString() + " Card";

			curr.type = greenCards[i];
			curr.color = ProgressCardColor.Green;

			greenCardsQueue.Add (curr);
		}
		
	}

	//used by masterclient to shuffled the cards
	public void shuffleCards(){
		yellowCards = shuffle (yellowCards);
		blueCards = shuffle (blueCards);
		greenCards = shuffle (greenCards);
	}
	//mixes order of cards
	private ProgressCardType[] shuffle(ProgressCardType[] arr){
		for (int i = arr.Length-1; i > 0; i--) {
			int r = Random.Range(0,i);
			ProgressCardType tmp = arr[i];
			arr[i] = arr[r];
			arr[r] = tmp;
		}
		return arr;
	}
}