using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : IntersectionUnit {

	public int strength;
	public bool isActive = false;
	public bool actionPerformedThisTurn = false;
	public KnightRank rank;

	public Sprite inactiveSprite;
	public Sprite activeSprite;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void activateKnight(bool active) {
		SpriteRenderer knightSprite = GetComponentsInChildren<SpriteRenderer> () [0];
		if (active) {
			knightSprite.sprite = activeSprite;
			this.isActive = true;
			actionPerformedThisTurn = true;
		} else {
			knightSprite.sprite = inactiveSprite;
			this.isActive = false;
			actionPerformedThisTurn = true;
		}
	}

	void SetOwnerColor(Color colorToSet) {
		SpriteRenderer colorCircle = GetComponentsInChildren<SpriteRenderer> () [1];
		colorCircle.color = colorToSet;
	}
}

public enum KnightRank {
	Basic = 0,
	Strong,
	Mighty
}
