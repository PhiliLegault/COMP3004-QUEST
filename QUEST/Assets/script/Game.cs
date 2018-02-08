﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public GameObject Card;									//General Card Prefab 
	public GameObject WeaponCard;							//Weapon Card Prefab
	public GameObject FoeCard;								//Foe Card Prefab
	public GameObject AllyCard;								//Ally Card Prefab
	public GameObject QuestCard;

	public GameObject playArea;								//Play Zone
	
	public GameObject stage1;							    //Stage1 of quest
	public GameObject stage2;
	public GameObject stage3;
	public GameObject stage4;
	public GameObject stage5;

	public GameObject drawCardArea;							//DrawCardArea
	public GameObject Hand; 								//Play Area Hand Reference
 
	public GameObject playerIdTxt;							//Player Id
	public GameObject shieldCounterTxt;						//Shield Counter
	private List<Player> _players = new List<Player>(); 	//List of players
	private int _numPlayers;								//Number of players
	private int _turnId; 									//Player ID of who's turn
	private Deck _adventureDeck;
	private Deck _storyDeck;
	private Deck _discardPileAdventure;
	private Deck _discardPileStory;
	private CardArea _storyArea;
	private CardArea _playArea;
	private bool _running;
	private bool _drawn;

	private List<List<Card>> _Quest;
	private int _questSponsor;
	private List<int> _playersIn;
	private bool _questInPlay;

	//if the user can end their turn
	private bool _standardTurn;
	private bool _canEnd;

	

	//Draws Card 
	public void DrawCard(){
		//_storyDeck
		if (_drawn == false) {

			//Clears out drawCardArea
			foreach (Transform child in drawCardArea.transform) {	
				GameObject.Destroy (child.gameObject);
			}

			Card currCard = _storyDeck.Draw (); //We need discard pile
			
			_discardPileStory.Discard(currCard);
			
			string currCardAsset = currCard.asset; //Pulls the card asset

			Sprite card = Resources.Load<Sprite> (currCardAsset); //Card Sprite

			GameObject storyCard = null;

			if (currCard.GetType () == typeof(QuestCard)){	//Instantiate Quest Card Prefab
				storyCard = Instantiate (QuestCard, new Vector3 (-10.5f, -3.5f, -10.5f), new Quaternion (0, 0, 0, 0));
				_questInPlay = true;
			}

			storyCard.gameObject.GetComponent<Image> ().sprite = card;
			storyCard.transform.SetParent (drawCardArea.transform);
			_drawn = true;


			if(_questInPlay == true){
				initQuest(_turnId, (QuestCard)currCard);		//Initialize Quest Should ALSO TAKE IN QUEST CARD
			}

			//Check What card i have drawn and initialize a quest
			debugPrint();

			System.Type cardType = currCard.GetType();
			if (cardType.Equals(typeof(QuestCard))) {
				//quest
				QuestCard questCard = (QuestCard)currCard;
				Debug.Log (questCard.stages);
			} else if (cardType.Equals(typeof(TournamentCard))) {
				//tournament
				TournamentCard tournamentCard = (TournamentCard)currCard;
			} else if (cardType.Equals(typeof(EventCard))) {
				//event
				EventCard eventCard = (EventCard)currCard;
			} else {
				
			}
			
								
		}
	}
	

	//Quest Initialization
	private void initQuest(int currPlayer , QuestCard card){		
		bool sponsor = sponsorPrompt(currPlayer); 

		currPlayer++;
		while(sponsor == false){				//Find Sponsor
			if(currPlayer == _turnId){			//We asked all the players so break
				break;
			}
			if (currPlayer >= 3) {		
				currPlayer = 0;					//Reset asking because maybe we started at the end 
			}
			sponsor = sponsorPrompt(currPlayer);
		}

		if(sponsor == true){					//Someone has sponsored so we must ask if people want to play
			_questSponsor = currPlayer;			//Quest Sponsor is the current player
			for(var i = 0 ; i < _numPlayers ; i++){	
				if(i != _questSponsor){			//Skip quest sponsor
					if(playPrompt(i)){			//Ask them and add them to players in this quest
						_playersIn.Add(i);
					}
				}
			}
			if(_playersIn.Count == 0){	//If no one joins end quest
				_questInPlay = false;
			}

			else{
				//Quest Loop
				//Pay Players Shields
			}			 
		}

		_questInPlay = false;
			 
	}

	//End Turn
	public void EndTurn(){
		if (_canEnd) {
			//Clear Old Hand
			foreach (Transform child in Hand.transform) {	
				GameObject.Destroy (child.gameObject);
			}

			_turnId++;
			if (_turnId >= 3) {
				_turnId = 0;
			}

			loadHand(_turnId);
			//Debug.Log("End Turn");
			_drawn = false;
			_canEnd = false;
		}
	}

	private bool sponsorPrompt(int playerId){
		loadHand(playerId);
		Debug.Log(playerId +" Do you want to sponsor");
		return false;
	}

	private bool playPrompt(int playerId){
		loadHand(playerId);
		Debug.Log(playerId +" Do you want to play");

		return false;
	}

	private void acceptSponsor() {
		_questSponsor = _turnId;
		_canEnd = false;
	}

	// Use this for initialization
	void Awake() {
		//assume 3 players
		_players.Add(new Player(1));
		_players.Add(new Player(2));
		_players.Add(new Player(3));

		//Set up the decks
		_adventureDeck = new Deck("Adventure");
		_storyDeck = new Deck("Story");
		_discardPileAdventure = new Deck ("");
		_discardPileStory = new Deck ("");

		_turnId = 0;
		_numPlayers = 3;
		_running = true;
		_drawn = false;
		_standardTurn = true;
		_canEnd = true;
		_questInPlay = false;

		//Populates Player Hands
		for(int i = 0; i < _players.Count ; i++){
			for(int x = 0 ; x < 12 ; x++){
				_players[i].addCard((_adventureDeck.Draw()));
			}
		}
		loadHand(_turnId);
		//debugPrint(); 
	}
	
	//Input : Integer Player Id
	//Functionality : Loads player hand onto the UI
	void loadHand(int playerId){
		
		List<Card> currHand = _players[playerId].hand;
		Card currCard;

		//Set Player ID text
		playerIdTxt.GetComponent<UnityEngine.UI.Text>().text = "Player ID : "+ (playerId+1).ToString(); //For User Friendly

		//Get current players shield
		int currPlayerShield = _players[playerId].shieldCounter;
		shieldCounterTxt.GetComponent<UnityEngine.UI.Text>().text = "# Shield: "+ (currPlayerShield).ToString(); //For User Friendly

		//Create Card Game Object
		for(int i = 0 ; i < currHand.Count; i++){
			currCard = currHand[i];


			GameObject CardUI = null; 

			if (currCard.GetType () == typeof(WeaponCard)) {
				//Is this convention ?
				CardUI = Instantiate (WeaponCard, new Vector3 (-10.5f, -3.5f, -10.5f), new Quaternion (0, 0, 0, 0));
				CardUI.GetComponent<WeaponCard>().name = currCard.name;
				CardUI.GetComponent<WeaponCard>().asset = currCard.asset;
				//CardUI.GetComponent<WeaponCard>().power = currCard.power;
				
			}
			if (currCard.GetType () == typeof(FoeCard)) {
				CardUI = Instantiate (FoeCard, new Vector3 (-10.5f, -3.5f, -10.5f), new Quaternion (0, 0, 0, 0));
				CardUI.GetComponent<FoeCard>().name    = currCard.name;
				//CardUI.GetComponent<FoeCard>().loPower = currCard.loPower;
				//CardUI.GetComponent<FoeCard>().hiPower = currCard.hiPower;
				//CardUI.GetComponent<FoeCard>().special = currCard.special;
				CardUI.GetComponent<FoeCard>().asset   = currCard.asset;
			}

			if (currCard.GetType () == typeof(AllyCard)) {
				CardUI = Instantiate (AllyCard, new Vector3 (-10.5f, -3.5f, -10.5f), new Quaternion (0, 0, 0, 0));
				CardUI.GetComponent<AllyCard>().name    = currCard.name;
				//CardUI.GetComponent<FoeCard>().loPower = currCard.loPower;
				//CardUI.GetComponent<FoeCard>().hiPower = currCard.hiPower;
				//CardUI.GetComponent<FoeCard>().special = currCard.special;
				CardUI.GetComponent<AllyCard>().asset   = currCard.asset;
			}


			Sprite card = Resources.Load<Sprite>(currCard.asset); //Card Sprite

			Debug.Log(card);
			CardUI.gameObject.GetComponent<Image>().sprite = card;
			CardUI.transform.SetParent(Hand.transform);
		}
	}

	//Purpose is for printing deck values
	void debugPrint(){
		//Populate Hand of All Player
		Debug.Log(_players.Count);
 
		//Test Deck Printing
		Debug.Log("Adventure Deck:");
		for(int i = 0; i <_adventureDeck.GetSize();i++){
			Debug.Log(_adventureDeck.GetDeck()[i].ToString());
		}
		//Test Print Story
		Debug.Log("Story Deck:");
		for(int i = 0; i <_storyDeck.GetSize();i++){
			Debug.Log(_storyDeck.GetDeck()[i].ToString());
		}

		Debug.Log("Player Hands:");
		//Trying to print player hand
		for(int i = 0 ; i <_players.Count; i++){
			Debug.Log("Player:"+ i);
			//Debug.Log(_players[i]._hand.Count);
			for(int x = 0; x <_players[i].hand.Count; x++){
				Debug.Log(_players[i].hand[x].ToString());
			}
		}	
	}
		
	// Update is called once per frame
	void Update () {
		
	}

	void TwoPlayerMode() {
		
	}

	void ThreePlayerMode() {

	}



}
