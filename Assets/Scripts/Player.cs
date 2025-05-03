using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoFish
{
    /// <summary>
    /// Manages the positions of the player's cards
    /// </summary>
    [Serializable]
    public class Player : IEquatable<Player>
    {
        public string PlayerId;
        public string PlayerName;
        public bool IsAI;
        public Vector2 Position;
        public Vector2 BookPosition;

        int NumberOfDisplayingCards;
        int NumberOfBooks;

        public List<Card> DisplayingCards = new List<Card>();

        public Vector2 NextCardPosition()
        {
            Vector2 nextPos = Position + Vector2.right * Constants.PLAYER_CARD_POSITION_OFFSET * NumberOfDisplayingCards;
            return nextPos;
        }

        public Vector2 NextBookPosition()
        {
            Vector2 nextPos = BookPosition + Vector2.right * Constants.PLAYER_BOOK_POSITION_OFFSET * NumberOfBooks;
            return nextPos;
        }

        public void SetCardValues(List<byte> values)
        {
            if (DisplayingCards.Count != values.Count)
            {
                Debug.LogError($"Displaying cards count {DisplayingCards.Count} is not equal to card values count {values.Count} for {PlayerId}");
                return;
            }

            for (int index = 0; index < values.Count; index++)
            {
                Card card = DisplayingCards[index];
                card.SetCardValue(values[index]);
                card.SetDisplayingOrder(index + 1);
            }
        }

        public void HideCardValues()
        {
            foreach (Card card in DisplayingCards)
            {
                card.SetFaceUp(false);
            }
        }

        public void ShowCardValues()
        {
            foreach (Card card in DisplayingCards)
            {
                card.SetFaceUp(true);
            }
        }

        public void ReceiveDisplayingCard(Card card)
        {
            DisplayingCards.Add(card);
            card.OwnerId = PlayerId;
            NumberOfDisplayingCards++;
        }

        public void ReceiveBook(Ranks rank, CardAnimator cardAnimator)
        {
            Vector2 targetPosition = NextBookPosition();
            List<Card> displayingCardsToRemove = new List<Card>();

            foreach (Card card in DisplayingCards)
            {
                if (card.Rank == rank)
                {
                    card.SetFaceUp(true);
                    float randomRotation = UnityEngine.Random.Range(-1 * Constants.BOOK_MAX_RANDOM_ROTATION, Constants.BOOK_MAX_RANDOM_ROTATION);
                    cardAnimator.AddCardAnimation(card, targetPosition, Quaternion.Euler(Vector3.forward * randomRotation));
                    displayingCardsToRemove.Add(card);
                }
            }

            DisplayingCards.RemoveAll(card => displayingCardsToRemove.Contains(card));
            RepositionDisplayingCards(cardAnimator);
            NumberOfBooks++;
        }

        public void RepositionDisplayingCards(CardAnimator cardAnimator)
        {
            NumberOfDisplayingCards = 0;
            foreach (Card card in DisplayingCards)
            {
                NumberOfDisplayingCards++;
                cardAnimator.AddCardAnimation(card, NextCardPosition());
            }
        }

        public void SendDisplayingCardToPlayer(Player receivingPlayer, CardAnimator cardAnimator, List<byte> cardValues, bool isLocalPlayer)
        {
            int playerDisplayingCardsCount = DisplayingCards.Count;

            if (playerDisplayingCardsCount < cardValues.Count)
            {
                Debug.LogError("Not enough displaying cards");
                return;
            }

            for (int index = 0; index < cardValues.Count; index++)
            {

                Card card = null;
                byte cardValue = cardValues[index];

                if (isLocalPlayer)
                {
                    foreach (Card c in DisplayingCards)
                    {
                        if (c.Rank == Card.GetRank(cardValue) && c.Suit == Card.GetSuit(cardValue))
                        {
                            card = c;
                            break;
                        }
                    }
                }
                else
                {
                    card = DisplayingCards[playerDisplayingCardsCount - 1 - index];
                    card.SetCardValue(cardValue);
                    card.SetFaceUp(true);
                }

                if (card != null)
                {
                    DisplayingCards.Remove(card);
                    receivingPlayer.ReceiveDisplayingCard(card);
                    cardAnimator.AddCardAnimation(card, receivingPlayer.NextCardPosition());
                    NumberOfDisplayingCards--;
                }
                else
                {
                    Debug.LogError("Unable to find displaying card.");
                }
            }

            RepositionDisplayingCards(cardAnimator);
        }

        public bool Equals(Player other)
        {
            if (PlayerId.Equals(other.PlayerId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

/*???????? Player.cs — Script Documentation
Namespace: GoFish
Purpose:
Represents a player entity (either human or AI) and controls:

Card ownership

Hand and book layout positions

Card animation interactions

Data equality for identifying players

?? Class Declaration
[Serializable]
public class Player : IEquatable<Player>
Marked as [Serializable] to allow data inspection in Unity Inspector or storage

Implements IEquatable<Player> so player comparison works based on PlayerId

?? Fields
Type	Name	Description
string	PlayerId	Unique identifier for this player
string	PlayerName	Display name
bool	IsAI	True if this is a bot player
Vector2	Position	Where the hand of cards is displayed
Vector2	BookPosition	Where collected books (4-of-a-kinds) are displayed
List<Card>	DisplayingCards	Cards currently held and shown on screen
int	NumberOfDisplayingCards	Internal counter for display positioning
int	NumberOfBooks	Number of books the player has collected

?? Position Helpers
Vector2 NextCardPosition()
Calculates the position where the next card in hand should be displayed.

Uses Constants.PLAYER_CARD_POSITION_OFFSET.

Vector2 NextBookPosition()
Calculates the position for the next completed book.

Uses Constants.PLAYER_BOOK_POSITION_OFFSET.

?? Visual and Value Synchronization
void SetCardValues(List<byte> values)
Assigns values to the cards in hand using a list of bytes.

Checks for mismatch between card objects and data.

Calls SetCardValue() and SetDisplayingOrder() for each.

void ShowCardValues() / HideCardValues()
Toggles the face-up visibility of all cards in hand.

?? Card Ownership & Management
void ReceiveDisplayingCard(Card card)
Adds a new card to the player's display list.

Sets the OwnerId of the card.

Increments the display count.

?? Book Collection
void ReceiveBook(Ranks rank, CardAnimator cardAnimator)
Called when a player forms a 4-of-a-kind book.

Moves cards to the book area and applies slight visual rotation.

Removes cards from hand and repositions the remaining ones.

?? Card Transfer Between Players
void SendDisplayingCardToPlayer(Player receiver, CardAnimator animator, List<byte> cardValues, bool isLocalPlayer)
Transfers specific cards to another player.

Logic differs slightly for local vs. AI:

Local player uses rank/suit match.

AI assumes card positions and sets values manually.

Updates animation, card ownership, and hand layout.

?? Re-layout of Hand
void RepositionDisplayingCards(CardAnimator cardAnimator)
Clears card positions and re-queues animation to display them again with spacing.

?? Equality Logic
bool Equals(Player other)
Compares PlayerId for equality checks (used in logic comparisons).

?? Teaching Tips
Topic	Example
Data Modeling	Use this class to teach how to represent real-world game objects (players) in code
Card Layout	Show how layout math (offsets) controls visuals
Object Interaction	Emphasize how cards are moved between players
Equality Design	Use of IEquatable<T> to compare player identity via ID

? Sample Flow: A Card Transfer
csharp
Copy
Edit
// Bot gives 2 cards to player
player.SendDisplayingCardToPlayer(player2, animator, new List<byte> { 4, 8 }, isLocalPlayer: false);
Bot finds 2 cards

Transfers them visually and logically

Repositions its own hand

 
 
 
 
 
 
 ?? Equality Design — IEquatable<Player> in the Player Class
?? What Is IEquatable<T>?
IEquatable<T> is an interface in C# that allows a class to define what it means for two objects to be equal.

Instead of relying on default reference equality (i.e., whether two variables point to the same memory location), it lets you customize what makes two instances "equal."

?? Why We Use It in the Player Class
In the Player class:
public class Player : IEquatable<Player>
This means: “Players should be compared based on some custom logic — not just whether they’re the same object.”

The logic is defined here:

public bool Equals(Player other)
{
    return PlayerId.Equals(other.PlayerId);
}
So when we write:

csharp
Copy
Edit
if (player1.Equals(player2)) { ... }
It will check if their PlayerIds match, not if they’re the same object in memory.

?? Why This Matters in Go Fish
You might have two different Player objects, one created at game start, another recreated later.

They are technically different instances, but if they share the same PlayerId, they are logically the same player in the context of the game.

?? Example: Without IEquatable
csharp
Copy
Edit
Player p1 = new Player { PlayerId = "p001" };
Player p2 = new Player { PlayerId = "p001" };

bool result = p1 == p2;   // false! Different instances
With IEquatable<Player> implemented:

csharp
Copy
Edit
bool result = p1.Equals(p2); // true! Same PlayerId, so same player logically
?? How to Teach This to Students
Analogy:

Think of two printed ID cards with the same student number. Even if they are printed separately, if the ID number is the same, we know they represent the same student.

Code Exercise:

Ask students to create two Player objects with the same ID and test == vs .Equals().

Discussion Prompt:

Ask: “Why might it be risky to assume two players are equal only if they’re the exact same object?”

? Benefits of This Design
Benefit	Description
?? Safer Comparisons	Prevents bugs where logically identical players are treated differently
?? Flexible Matching	Can compare players across sessions or network messages
?? Cleaner Code	Allows for intuitive .Equals() checks instead of writing custom comparisons everywhere
 */