using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoFish
{
    /// <summary>
    /// Stores the important data of the game
    /// We will encypt the fields in a multiplayer game.
    /// </summary>
    [Serializable]
    public class ProtectedData
    {
        [SerializeField]
        List<byte> poolOfCards = new List<byte>();
        [SerializeField]
        List<byte> player1Cards = new List<byte>();
        [SerializeField]
        List<byte> player2Cards = new List<byte>();
        [SerializeField]
        int numberOfBooksForPlayer1;
        [SerializeField]
        int numberOfBooksForPlayer2;
        [SerializeField]
        string player1Id;
        [SerializeField]
        string player2Id;

        public ProtectedData(string p1Id, string p2Id)
        {
            player1Id = p1Id;
            player2Id = p2Id;
        }

        public void SetPoolOfCards(List<byte> cardValues)
        {
            poolOfCards = cardValues;
        }

        public List<byte> GetPoolOfCards()
        {
            return poolOfCards;
        }

        public List<byte> PlayerCards(Player player)
        {
            if (player.PlayerId.Equals(player1Id))
            {
                return player1Cards;
            }
            else
            {
                return player2Cards;
            }
        }

        public void AddCardValuesToPlayer(Player player, List<byte> cardValues)
        {
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.AddRange(cardValues);
                player1Cards.Sort();
            }
            else
            {
                player2Cards.AddRange(cardValues);
                player2Cards.Sort();
            }
        }

        public void AddCardValueToPlayer(Player player, byte cardValue)
        {
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.Add(cardValue);
                player1Cards.Sort();
            }
            else
            {
                player2Cards.Add(cardValue);
                player2Cards.Sort();
            }
        }

        public void RemoveCardValuesFromPlayer(Player player, List<byte> cardValuesToRemove)
        {
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
            else
            {
                player2Cards.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
        }

        public void AddBooksForPlayer(Player player, int numberOfNewBooks)
        {
            if (player.PlayerId.Equals(player1Id))
            {
                numberOfBooksForPlayer1 += numberOfNewBooks;
            }
            else
            {
                numberOfBooksForPlayer2 += numberOfNewBooks;
            }
        }

        public bool GameFinished()
        {
            if (poolOfCards.Count == 0)
            {
                return true;
            }

            if (player1Cards.Count == 0)
            {
                return true;
            }

            if (player2Cards.Count == 0)
            {
                return true;
            }

            return false;
        }

        public string WinnerPlayerId()
        {
            if (numberOfBooksForPlayer1 > numberOfBooksForPlayer2)
            {
                return player1Id;
            }
            else
            {
                return player2Id;
            }
        }
    }
}

/*?? ProtectedData.cs — Script Documentation
Namespace: GoFish
Purpose:
This class holds all the sensitive game data like:

Player hands

Deck state

Number of books collected

Player identity (via PlayerId)

In a multiplayer version of the game, this class would be used to encrypt or hide game-critical data to prevent cheating or data exposure.

?? Class Summary
csharp
Copy
Edit
[Serializable]
public class ProtectedData
Marked [Serializable] so it can be saved, logged, or networked in the future.

Called by GameDataManager to handle all backend storage and querying.

?? Fields
Field	Type	Description
poolOfCards	List<byte>	The current remaining deck (cards not dealt yet)
player1Cards	List<byte>	Hand of Player 1
player2Cards	List<byte>	Hand of Player 2
numberOfBooksForPlayer1	int	Count of books Player 1 has
numberOfBooksForPlayer2	int	Count of books Player 2 has
player1Id / player2Id	string	Used to match cards to players

?? Constructor
csharp
Copy
Edit
public ProtectedData(string p1Id, string p2Id)
Initializes the class by assigning player IDs

Allows all internal methods to track which hand belongs to which player

?? Core Data Methods
SetPoolOfCards(List<byte> cardValues)
Accepts a shuffled list of all 52 card values

Sets it as the current draw deck

GetPoolOfCards()
Returns the current state of the card pool

?? Player Card Management
List<byte> PlayerCards(Player player)
Returns the list of card values in a player’s hand based on their ID

AddCardValuesToPlayer(Player, List<byte>)
Adds a list of card values to a player’s hand

Sorts the list for easy grouping (used for checking books)

AddCardValueToPlayer(Player, byte)
Adds a single card to a player’s hand and sorts

RemoveCardValuesFromPlayer(Player, List<byte>)
Removes a list of card values from the player's hand

?? Book Tracking
void AddBooksForPlayer(Player, int numberOfNewBooks)
Increments the number of books a player has

?? Game Status & Outcome
bool GameFinished()
Game ends if:

Deck is empty

Either player has no cards left

string WinnerPlayerId()
Returns the player ID with the most books

Used by Game.cs to declare the winner

?? Teaching Concepts
Concept	Teaching Value
Encapsulation	Data is private and only accessible through well-defined methods
Secure Design	Future-proofing for encryption or networked play
Sorting Cards	Makes book detection easy (GroupBy(rank) logic in GameDataManager)
Separation of Logic	Keeps raw data separated from gameplay logic or UI handling

? Example Interaction
csharp
Copy
Edit
// Add cards to a player
protectedData.AddCardValuesToPlayer(player, new List<byte> { 0, 1, 2, 3 });

// Remove them later if a book is formed
protectedData.RemoveCardValuesFromPlayer(player, new List<byte> { 0, 1, 2, 3 });

// Check who won
string winnerId = protectedData.WinnerPlayerId();
?? Future Multiplayer Use
This class is named ProtectedData because in a multiplayer game:

You don’t want each client to know the opponent’s cards

You may need to encrypt fields like player1Cards, poolOfCards

You might sync just aggregates (like book count), but not actual hands*/