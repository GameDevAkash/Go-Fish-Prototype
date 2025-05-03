using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity;
using UnityEngine.UI;
using TMPro;

namespace GoFish
{
    public class Game : MonoBehaviour
    {
        public static Game Instance;
        public TextMeshProUGUI MessageText;

        CardAnimator cardAnimator;

        public GameDataManager gameDataManager;

        public List<Transform> PlayerPositions = new List<Transform>();
        public List<Transform> BookPositions = new List<Transform>();

        Player localPlayer;
        Player remotePlayer;

        Player currentTurnPlayer;
        Player currentTurnTargetPlayer;

        Card selectedCard;
        Ranks selectedRank;

        public enum GameState
        {
            Idel,
            GameStarted,
            TurnStarted,
            TurnSelectingNumber,
            TurnConfirmedSelectedNumber,
            TurnWaitingForOpponentConfirmation,
            TurnOpponentConfirmed,
            TurnGoFish,
            GameFinished
        };

        public GameState gameState = GameState.Idel;

        private void Awake()
        {
            Instance = this;
            localPlayer = new Player();
            localPlayer.PlayerId = "offline-player";
            localPlayer.PlayerName = "Player";
            localPlayer.Position = PlayerPositions[0].position;
            localPlayer.BookPosition = BookPositions[0].position;

            remotePlayer = new Player();
            remotePlayer.PlayerId = "offline-bot";
            remotePlayer.PlayerName = "Bot";
            remotePlayer.Position = PlayerPositions[1].position;
            remotePlayer.BookPosition = BookPositions[1].position;
            remotePlayer.IsAI = true;

            cardAnimator = FindObjectOfType<CardAnimator>();
        }

        void Start()
        {
            gameState = GameState.GameStarted;
            GameFlow();
        }

        //****************** Game Flow *********************//
        public void GameFlow()
        {
            if (gameState > GameState.GameStarted)
            {
                CheckPlayersBooks();
                ShowAndHidePlayersDisplayingCards();

                if (gameDataManager.GameFinished())
                {
                    gameState = GameState.GameFinished;
                }
            }

            switch (gameState)
            {
                case GameState.Idel:
                    {
                        Debug.Log("IDEL");
                        break;
                    }
                case GameState.GameStarted:
                    {
                        Debug.Log("GameStarted");
                        OnGameStarted();
                        break;
                    }
                case GameState.TurnStarted:
                    {
                        Debug.Log("TurnStarted");
                        OnTurnStarted();
                        break;
                    }
                case GameState.TurnSelectingNumber:
                    {
                        Debug.Log("TurnSelectingNumber");
                        OnTurnSelectingNumber();
                        break;
                    }
                case GameState.TurnConfirmedSelectedNumber:
                    {
                        Debug.Log("TurnComfirmedSelectedNumber");
                        OnTurnConfirmedSelectedNumber();
                        break;
                    }
                case GameState.TurnWaitingForOpponentConfirmation:
                    {
                        Debug.Log("TurnWaitingForOpponentConfirmation");
                        OnTurnWaitingForOpponentConfirmation();
                        break;
                    }
                case GameState.TurnOpponentConfirmed:
                    {
                        Debug.Log("TurnOpponentConfirmed");
                        OnTurnOpponentConfirmed();
                        break;
                    }
                case GameState.TurnGoFish:
                    {
                        Debug.Log("TurnGoFish");
                        OnTurnGoFish();
                        break;
                    }
                case GameState.GameFinished:
                    {
                        Debug.Log("GameFinished");
                        OnGameFinished();
                        break;
                    }
            }
        }

        void OnGameStarted()
        {
            gameDataManager = new GameDataManager(localPlayer, remotePlayer);
            gameDataManager.Shuffle();
            gameDataManager.DealCardValuesToPlayer(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            gameDataManager.DealCardValuesToPlayer(remotePlayer, Constants.PLAYER_INITIAL_CARDS);

            cardAnimator.DealDisplayingCards(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer, Constants.PLAYER_INITIAL_CARDS);

            gameState = GameState.TurnStarted;
        }

        void OnTurnStarted()
        {
            SwitchTurn();
            gameState = GameState.TurnSelectingNumber;
            GameFlow();
        }

        public void OnTurnSelectingNumber()
        {
            ResetSelectedCard();

            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Your turn. Pick a card from your hand.");
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName}'s turn");
            }

            if (currentTurnPlayer.IsAI)
            {
                selectedRank = gameDataManager.SelectRandomRanksFromPlayersCardValues(currentTurnPlayer);
                gameState = GameState.TurnConfirmedSelectedNumber;
                GameFlow();
            }
        }

        public void OnTurnConfirmedSelectedNumber()
        {
            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Asking {currentTurnTargetPlayer.PlayerName} for {selectedRank}s...");
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName} is asking for {selectedRank}s...");
            }

            gameState = GameState.TurnWaitingForOpponentConfirmation;
            GameFlow();
        }

        public void OnTurnWaitingForOpponentConfirmation()
        {
            if (currentTurnTargetPlayer.IsAI)
            {
                gameState = GameState.TurnOpponentConfirmed;
                GameFlow();
            }
        }

        public void OnTurnOpponentConfirmed()
        {
            List<byte> cardValuesFromTargetPlayer = gameDataManager.TakeCardValuesWithRankFromPlayer(currentTurnTargetPlayer, selectedRank);

            if (cardValuesFromTargetPlayer.Count > 0)
            {
                gameDataManager.AddCardValuesToPlayer(currentTurnPlayer, cardValuesFromTargetPlayer);

                bool senderIsLocalPlayer = currentTurnTargetPlayer == localPlayer;
                currentTurnTargetPlayer.SendDisplayingCardToPlayer(currentTurnPlayer, cardAnimator, cardValuesFromTargetPlayer, senderIsLocalPlayer);
                gameState = GameState.TurnSelectingNumber;
            }
            else
            {
                gameState = GameState.TurnGoFish;
                GameFlow();
            }
        }

        public void OnTurnGoFish()
        {
            SetMessage($"Go fish!");

            byte cardValue = gameDataManager.DrawCardValue();

            if (cardValue == Constants.POOL_IS_EMPTY)
            {
                Debug.LogError("Pool is empty");
                return;
            }

            if (Card.GetRank(cardValue) == selectedRank)
            {
                cardAnimator.DrawDisplayingCard(currentTurnPlayer, cardValue);
            }
            else
            {
                cardAnimator.DrawDisplayingCard(currentTurnPlayer);
                gameState = GameState.TurnStarted;
            }

            gameDataManager.AddCardValueToPlayer(currentTurnPlayer, cardValue);
        }

        public void OnGameFinished()
        {
            if (gameDataManager.Winner() == localPlayer)
            {
                SetMessage($"You WON!");
            }
            else
            {
                SetMessage($"You LOST!");
            }
        }

        //****************** Helper Methods *********************//
        public void ResetSelectedCard()
        {
            if (selectedCard != null)
            {
                selectedCard.OnSelected(false);
                selectedCard = null;
                selectedRank = 0;
            }
        }

        void SetMessage(string message)
        {
            MessageText.text = message;
        }

        public void SwitchTurn()
        {
            if (currentTurnPlayer == null)
            {
                currentTurnPlayer = localPlayer;
                currentTurnTargetPlayer = remotePlayer;
                return;
            }

            if (currentTurnPlayer == localPlayer)
            {
                currentTurnPlayer = remotePlayer;
                currentTurnTargetPlayer = localPlayer;
            }
            else
            {
                currentTurnPlayer = localPlayer;
                currentTurnTargetPlayer = remotePlayer;
            }
        }

        public void PlayerShowBooksIfNecessary(Player player)
        {
            Dictionary<Ranks, List<byte>> books = gameDataManager.GetBooks(player);

            if (books != null)
            {
                foreach (var book in books)
                {
                    player.ReceiveBook(book.Key, cardAnimator);

                    gameDataManager.RemoveCardValuesFromPlayer(player, book.Value);
                }

                gameDataManager.AddBooksForPlayer(player, books.Count);
            }
        }

        public void CheckPlayersBooks()
        {
            List<byte> playerCardValues = gameDataManager.PlayerCards(localPlayer);
            localPlayer.SetCardValues(playerCardValues);
            PlayerShowBooksIfNecessary(localPlayer);

            playerCardValues = gameDataManager.PlayerCards(remotePlayer);
            remotePlayer.SetCardValues(playerCardValues);
            PlayerShowBooksIfNecessary(remotePlayer);
        }

        public void ShowAndHidePlayersDisplayingCards()
        {
            localPlayer.ShowCardValues();
            remotePlayer.HideCardValues();
        }

        //****************** User Interaction *********************//
        public void OnCardSelected(Card card)
        {
            if (gameState == GameState.TurnSelectingNumber)
            {
                if (card.OwnerId == currentTurnPlayer.PlayerId)
                {
                    if (selectedCard != null)
                    {
                        selectedCard.OnSelected(false);
                        selectedRank = 0;
                    }

                    selectedCard = card;
                    selectedRank = selectedCard.Rank;
                    selectedCard.OnSelected(true);
                    SetMessage($"Ask {currentTurnTargetPlayer.PlayerName} for {selectedCard.Rank}s ?");
                }
            }
        }

        public void OnOkSelected()
        {
            if (gameState == GameState.TurnSelectingNumber && localPlayer == currentTurnPlayer)
            {
                if (selectedCard != null)
                {
                    gameState = GameState.TurnConfirmedSelectedNumber;
                    GameFlow();
                }
            }
            else if (gameState == GameState.TurnWaitingForOpponentConfirmation && localPlayer == currentTurnTargetPlayer)
            {
                gameState = GameState.TurnOpponentConfirmed;
                GameFlow();
            }
        }

        //****************** Animator Event *********************//
        public void AllAnimationsFinished()
        {
            GameFlow();
        }
    }
}

/*
?? Game.cs — Script Documentation
Namespace: GoFish
Purpose:
This script is the main game controller that manages the state of the game, player turns, input, card transfers, and messages shown to the player. It acts as the state machine driving the turn-based flow of the Go Fish card game.

?? Class Overview
public class Game : MonoBehaviour
This class uses Unity’s MonoBehaviour and is attached to a GameObject in the scene. It controls everything that happens from the moment the game starts to the end result (win/loss).

?? Fields & Members
Type	Name	Description
TextMeshProUGUI	MessageText	UI element for displaying in-game messages.
CardAnimator	cardAnimator	Reference to the animation controller for card movement.
GameDataManager	gameDataManager	Manages logical card data and rule enforcement.
List<Transform>	PlayerPositions, BookPositions	Positions in the scene for players and their books.
Player	localPlayer, remotePlayer	The two players in the game — one human, one bot.
Player	currentTurnPlayer, currentTurnTargetPlayer	Whose turn it is, and who they are asking.
Card	selectedCard	The card selected by the player on their turn.
Ranks	selectedRank	The rank chosen by the player to ask about.

?? Game States
public enum GameState
A finite state machine controls game progression:

State	Description
Idel	Initial state before game starts
GameStarted	Initial setup: shuffling and dealing
TurnStarted	Beginning of a player’s turn
TurnSelectingNumber	Player picks a rank to request
TurnConfirmedSelectedNumber	After selection, waiting for confirmation
TurnWaitingForOpponentConfirmation	Waiting for the opponent’s response
TurnOpponentConfirmed	Opponent has responded
TurnGoFish	No matching cards — draw from deck
GameFinished	Game over

?? Key Methods
void Awake()
Initializes players and positions.

Creates AI and local players.

Sets references to CardAnimator.

void Start()
Starts the game loop with GameStarted state.

Calls GameFlow() to kick off the logic.

void GameFlow()
The heart of the game. A switch-case handler that:

Checks for books

Evaluates win conditions

Runs methods based on the current game state

?? Turn Phases
Each game state triggers a method:

Method	Purpose
OnGameStarted()	Initializes players, shuffles deck, deals cards
OnTurnStarted()	Switches turns and transitions to card selection
OnTurnSelectingNumber()	Waits for player or AI to select a rank
OnTurnConfirmedSelectedNumber()	Announces the request
OnTurnWaitingForOpponentConfirmation()	Handles delay or bot auto-response
OnTurnOpponentConfirmed()	Transfers cards or triggers "Go Fish"
OnTurnGoFish()	Draws a card if no match found
OnGameFinished()	Announces winner

?? Helper Methods
Method	Purpose
ResetSelectedCard()	Deselects a previously selected card
SetMessage(string msg)	Updates the message UI
SwitchTurn()	Alternates between players
CheckPlayersBooks()	Validates if any books are formed
ShowAndHidePlayersDisplayingCards()	Reveals local cards, hides bot cards
PlayerShowBooksIfNecessary(Player)	Displays collected sets of 4 cards

?? Input Handling
OnCardSelected(Card card)
Triggered when the player clicks a card.

Highlights the card and sets selectedRank.

OnOkSelected()
Confirms the player’s choice.

Triggers the request logic or opponent response.

?? Animation Event Handling
AllAnimationsFinished()
Triggered when CardAnimator completes all animations.

Re-enters GameFlow() to continue the turn.

?? Teaching Tips
Think of this class as a conductor in an orchestra — it doesn’t play music (handle visuals or logic directly), but it coordinates everything.

GameFlow() is a state machine — great concept to teach with flowcharts or diagrams.

This file teaches how to separate game logic from visuals — which is excellent practice in Unity development.

? Example Turn Flow (Student Visualization)
css
Copy
Edit
Player clicks a card ?
Game sets selected rank ?
OK button is pressed ?
Game asks opponent ?
Opponent has cards? 
   ? YES ? Cards transferred
   ? NO  ? Go Fish ? Draw a card
*/