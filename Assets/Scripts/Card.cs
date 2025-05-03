using System.ComponentModel;
using System.Reflection;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.U2D;

namespace GoFish
{
    public class Card : MonoBehaviour
    {
        public static Ranks GetRank(byte value)
        {
            return (Ranks)(value / 4 + 1);
        }

        public static Suits GetSuit(byte value)
        {
            return (Suits)(value % 4);
        }
        public SpriteAtlas Atlas;

        public Suits Suit = Suits.NoSuits;
        public Ranks Rank = Ranks.NoRanks;

        public string OwnerId;

        SpriteRenderer spriteRenderer;

        bool faceUp = false;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            UpdateSprite();
        }

        public void SetFaceUp(bool value)
        {
            faceUp = value;
            UpdateSprite();

            // Setting faceup to false also resets card's value.
            if (value == false)
            {
                Rank = Ranks.NoRanks;
                Suit = Suits.NoSuits;
            }
        }

        public void SetCardValue(byte value)
        {
            // 0-3 are 1's
            // 4-7 are 2's
            // ...
            // 48-51 are kings's
            Rank = (Ranks)(value / 4 + 1);

            // 0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 44, 48 are Spades(0)
            Suit = (Suits)(value % 4);
        }

        void UpdateSprite()
        {
            if (faceUp)
            {
                spriteRenderer.sprite = Atlas.GetSprite(SpriteName());
            }
            else
            {
                spriteRenderer.sprite = Atlas.GetSprite(Constants.CARD_BACK_SPRITE);
            }
        }

        string GetRankDescription()
        {
            FieldInfo fieldInfo = Rank.GetType().GetField(Rank.ToString());
            DescriptionAttribute[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes[0].Description;
        }

        string SpriteName()
        {
            string testName = $"card{Suit}{GetRankDescription()}";
            return testName;
        }

        public void SetDisplayingOrder(int order)
        {
            spriteRenderer.sortingOrder = order;
        }

        public void OnSelected(bool selected)
        {
            if (selected)
            {
                transform.position = (Vector2)transform.position + Vector2.up * Constants.CARD_SELECTED_OFFSET;
            }
            else
            {
                transform.position = (Vector2)transform.position - Vector2.up * Constants.CARD_SELECTED_OFFSET;
            }
        }
    }
}

/*
Card.cs — Script Documentation
Namespace: GoFish
Purpose:
The Card class represents a single playing card in the Go Fish game. It stores information about the card’s rank, suit, ownership, and visual representation on the screen. It handles flipping the card face up or down, updating its sprite, and reacting to user selection.

?? Class Summary
public class Card : MonoBehaviour
This class extends Unity's MonoBehaviour, meaning it's a component that can be attached to a GameObject in the Unity scene (typically a card prefab).

?? Fields & Properties
Type	Name	Description
SpriteAtlas	Atlas	Contains all the sprite images for card faces and backs.
Suits	Suit	Suit of the card (Hearts, Clubs, etc.).
Ranks	Rank	Rank of the card (Ace, Two, ..., King).
string	OwnerId	ID of the player who owns the card. Useful for tracking and logic.
SpriteRenderer	spriteRenderer	Internal reference to render the card sprite in the scene.
bool	faceUp	Whether the card is currently facing up (visible rank/suit).

?? Unity Methods
void Awake()
Called when the script instance is being loaded.

Initializes the spriteRenderer component.

void Start()
Called before the first frame update.

Triggers an initial sprite update to match current faceUp state.

?? Core Methods
void SetFaceUp(bool value)
Function: Flips the card face up (true) or face down (false).

Important: When flipped down, it resets the card's Rank and Suit to NoRanks and NoSuits.

void SetCardValue(byte value)
Function: Sets the card’s rank and suit based on a numeric value (0–51).

Logic:

Divides value by 4 to get the rank (e.g., 0–3 = Ace, 4–7 = 2, ..., 48–51 = King).

Takes modulo 4 to get the suit (0 = Spades, 1 = Hearts, 2 = Diamonds, 3 = Clubs).

void UpdateSprite()
Function: Updates the card’s visual sprite:

If face up: Sets to the correct face using rank/suit.

If face down: Uses the standard back image (Constants.CARD_BACK_SPRITE).

void SetDisplayingOrder(int order)
Function: Controls the draw order of the card in the scene using sorting layers.

Use Case: Helpful when overlapping cards — cards with higher values appear on top.

void OnSelected(bool selected)
Function: Animates the card when selected (moves it upward slightly).

Mechanism: Moves the card's transform vertically using Constants.CARD_SELECTED_OFFSET.

?? Static Utility Methods
static Ranks GetRank(byte value)
Converts a numeric card ID into a Ranks enum value.

static Suits GetSuit(byte value)
Converts a numeric card ID into a Suits enum value.

?? Internal Helpers
string GetRankDescription()
Uses reflection to extract the [Description("X")] attribute from the Ranks enum.

This helps match card asset names like "cardHeartsAce" based on metadata.

string SpriteName()
Combines Suit and Rank into a formatted string used to fetch the correct card sprite.

Example Output: "cardSpadesQueen".

?? Conceptual Notes for Students
Rank/Suit Mapping: Each card is uniquely represented by a value from 0–51. The rank is the quotient when divided by 4, and the suit is the remainder.

Face Up / Down: Changing faceUp affects both logic and visuals. Resetting the card’s data when face down prevents incorrect reuse.

Sprite Atlas: All card faces and backs are stored in a single atlas for optimized rendering.

Owner ID: Used for multiplayer or AI logic to keep track of which player holds the card.

?? Sample Use Case in Game
When dealing cards:
card.SetCardValue(cardId);   // Assign rank and suit
card.SetFaceUp(false);       // Hide it until needed
card.SetDisplayingOrder(i);  // Arrange it visually

When revealing:
card.SetFaceUp(true);        // Show the card

When clicked:
card.OnSelected(true);       // Lift it visually 
 */