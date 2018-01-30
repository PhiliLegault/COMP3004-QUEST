﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public  List<Card> _hand{
		get;
		set;
	}
	private int _playerId;							//Player ID
	private int _rank;								//Players current ranks 0-2; 3 ends the game
	private int _shieldCounter; 					//Shield Counter
	private List<Card> _allies;  //List of cards
	private int _bp;								//Current BP
	public static int limit = 12;

	//Constructor 
	public Player(int id){
		_playerId = id;
		_rank = 1;
		_shieldCounter = 0;
		_bp = 5;
		_hand = new List<Card>();
		_allies = new List<Card>();
	}


	//Add Cards
	public void addCard(Card card){
		_hand.Add(card);	//Adds card to hand
	}
	
	// Plays Cards
	void playCard(List<Card> cards){
		
	}
	
	//s as number of shields to add, should we force rank up?
	void AddShields (int s) {
		_shieldCounter += s;
	}

/* 
	Rank-up:
		Squire = 5 BP
		Knight = 10 BP
		Champion Knight = 20 BP
*/
	void Rankup () {
		int[] requirements = new int[]{ 5, 7, 10 };
		if (_rank == 3) {
			//end game
		}
		while (_shieldCounter >= requirements [_rank]) {
			_shieldCounter -= requirements [_rank];
			_rank++;
			if (_rank == 3) {
				//END GAME HERE
				break;
			}
		}
	}

	//Game should draw and assign the cards
	void DrawCards(List<Card> cards) {
		
	}
}
