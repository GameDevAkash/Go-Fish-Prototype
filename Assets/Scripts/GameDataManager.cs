using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoFish
{
    [Serializable]
    public class GameDataManager
    {
        Player localPlayer;
        Player remotePlayer;

        [SerializeField]
        ProtectedData protectedData;

        public GameDataManager(Player local, Player remote)
        {
            localPlayer = local;
            remotePlayer = remote;
            protectedData = new ProtectedData(localPlayer.PlayerId, remotePlayer.PlayerId);
        }

        public void Shuffle()
        {
            List<byte> cardValues = new List<byte>();

            for (byte value = 0; value < 52; value++)
            {
                cardValues.Add(value);
            }

            List<byte> poolOfCards = new List<byte>();

            for (int index = 0; index < 52; index++)
            {
                int valueIndexToAdd = UnityEngine.Random.Range(0, cardValues.Count);

                byte valueToAdd = cardValues[valueIndexToAdd];
                poolOfCards.Add(valueToAdd);
                cardValues.Remove(valueToAdd);
            }

            protectedData.SetPoolOfCards(poolOfCards);
        }

        public void DealCardValuesToPlayer(Player player, int numberOfCards)
        {
            List<byte> poolOfCards = protectedData.GetPoolOfCards();

            int numberOfCardsInThePool = poolOfCards.Count;
            int start = numberOfCardsInThePool - 1 - numberOfCards;

            List<byte> cardValues = poolOfCards.GetRange(start, numberOfCards);
            poolOfCards.RemoveRange(start, numberOfCards);

            protectedData.AddCardValuesToPlayer(player, cardValues);
        }

        public byte DrawCardValue()
        {
            List<byte> poolOfCards = protectedData.GetPoolOfCards();

            int numberOfCardsInThePool = poolOfCards.Count;

            if (numberOfCardsInThePool > 0)
            {
                byte cardValue = poolOfCards[numberOfCardsInThePool - 1];
                poolOfCards.Remove(cardValue);

                return cardValue;
            }

            return Constants.POOL_IS_EMPTY;
        }

        public List<byte> PlayerCards(Player player)
        {
            return protectedData.PlayerCards(player);
        }

        public void AddCardValuesToPlayer(Player player, List<byte> cardValues)
        {
            protectedData.AddCardValuesToPlayer(player, cardValues);
        }

        public void AddCardValueToPlayer(Player player, byte cardValue)
        {
            protectedData.AddCardValueToPlayer(player, cardValue);
        }

        public void RemoveCardValuesFromPlayer(Player player, List<byte> cardValuesToRemove)
        {
            protectedData.RemoveCardValuesFromPlayer(player, cardValuesToRemove);
        }

        public void AddBooksForPlayer(Player player, int numberOfNewBooks)
        {
            protectedData.AddBooksForPlayer(player, numberOfNewBooks);
        }

        public Player Winner()
        {
            string winnerPlayerId = protectedData.WinnerPlayerId();
            if (winnerPlayerId.Equals(localPlayer.PlayerId))
            {
                return localPlayer;
            }
            else
            {
                return remotePlayer;
            }
        }

        public bool GameFinished()
        {
            return protectedData.GameFinished();
        }

        public List<byte> TakeCardValuesWithRankFromPlayer(Player player, Ranks ranks)
        {
            List<byte> playerCards = protectedData.PlayerCards(player);

            List<byte> result = new List<byte>();

            foreach (byte cv in playerCards)
            {
                if (Card.GetRank(cv) == ranks)
                {
                    result.Add(cv);
                }
            }

            protectedData.RemoveCardValuesFromPlayer(player, result);

            return result;
        }

        public Dictionary<Ranks, List<byte>> GetBooks(Player player)
        {
            List<byte> playerCards = protectedData.PlayerCards(player);

            var groups = playerCards.GroupBy(Card.GetRank).Where(g => g.Count() == 4);

            if (groups.Count() > 0)
            {
                Dictionary<Ranks, List<byte>> setOfFourDictionary = new Dictionary<Ranks, List<byte>>();

                foreach (var group in groups)
                {
                    List<byte> cardValues = new List<byte>();

                    foreach (var value in group)
                    {
                        cardValues.Add(value);
                    }

                    setOfFourDictionary[group.Key] = cardValues;
                }

                return setOfFourDictionary;
            }

            return null;
        }

        public Ranks SelectRandomRanksFromPlayersCardValues(Player player)
        {
            List<byte> playerCards = protectedData.PlayerCards(player);
            int index = UnityEngine.Random.Range(0, playerCards.Count);

            return Card.GetRank(playerCards[index]);
        }
    }
}

/*
?? GameDataManager.cs — Script Documentation
Namespace: GoFish
Purpose:
This class manages all the game logic and data, such as:

Shuffling the deck

Dealing cards

Drawing from the pool

Detecting “books” (sets of 4 cards)

Determining the winner

It acts as the logic layer between the Game controller and the ProtectedData storage class.

?? Class Summary
public class GameDataManager
Non-MonoBehaviour C# class

Called and controlled entirely by Game.cs

Manages stateful data through ProtectedData

?? Fields
Type	Name	Description
Player	localPlayer, remotePlayer	Player references for logic comparison
ProtectedData	protectedData	Secure object holding all card hands, deck, and book counts

?? Constructor
csharp
Copy
Edit
public GameDataManager(Player local, Player remote)
Accepts two Player objects

Initializes a new ProtectedData instance for this game

?? Core Game Logic Methods
void Shuffle()
Creates a list of all 52 card values (0–51)

Randomizes the order using UnityEngine.Random.Range

Sets this as the deck pool in ProtectedData

void DealCardValuesToPlayer(Player player, int numberOfCards)
Pulls N cards from the end of the deck pool

Removes those cards from the pool

Adds them to the player's hand in ProtectedData

byte DrawCardValue()
Removes and returns the last card from the pool

If the pool is empty, returns a special constant: Constants.POOL_IS_EMPTY

?? Card Management
Method	Purpose
PlayerCards(Player)	Returns list of card values for a player
AddCardValuesToPlayer(Player, List<byte>)	Adds multiple cards to a player’s hand
AddCardValueToPlayer(Player, byte)	Adds a single card
RemoveCardValuesFromPlayer(Player, List<byte>)	Removes cards from a player
TakeCardValuesWithRankFromPlayer(Player, Ranks)	Returns and removes all cards of a given rank from a player

?? Book Handling
Dictionary<Ranks, List<byte>> GetBooks(Player player)
Checks the player's cards

Finds all sets of 4 cards of the same rank

Returns a dictionary: Rank ? List of 4 cards

void AddBooksForPlayer(Player, int numberOfBooks)
Adds to a player’s book count in ProtectedData

?? Game Status Methods
bool GameFinished()
Asks ProtectedData if the game-ending condition is met

Player Winner()
Returns the player with the most books based on data in ProtectedData

?? AI Method
Ranks SelectRandomRanksFromPlayersCardValues(Player player)
Used by the bot

Selects a random card from its hand

Returns the rank to ask for

?? How it Works in the Game
This class does not interact with visuals or UI.

It is entirely logic-focused.

All requests from Game.cs come here to evaluate outcomes:

“Give me all 7s from the opponent”

“Draw a card from the deck”

“Who has the most books?”

?? Design Highlights
Feature	Description
Separation of Concerns	Keeps logic separate from UI or animation
Data Delegation	Uses ProtectedData to abstract actual card storage
Supports AI	Has logic for random rank selection
Expandable	Can be adapted for online play with minimal changes

?? Teaching Tips
Use this script to teach:

How to shuffle and deal cards programmatically

How to group and detect patterns (books = 4 same ranks)

How to design clean, logic-only classes

How to return custom structures like Dictionary<Ranks, List<byte>>

? Sample Usage in Game.cs
gameDataManager.Shuffle();
gameDataManager.DealCardValuesToPlayer(player, 7);
gameDataManager.TakeCardValuesWithRankFromPlayer(opponent, selectedRank);
gameDataManager.GetBooks(player);*/
