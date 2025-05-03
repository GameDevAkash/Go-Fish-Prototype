using System.ComponentModel;
using UnityEngine;

namespace GoFish
{
    public static class Constants
    {
        public const float PLAYER_CARD_POSITION_OFFSET = 0.6f;
        public const float PLAYER_BOOK_POSITION_OFFSET = 2f;
        public const float DECK_CARD_POSITION_OFFSET = 0.2f;
        public const string CARD_BACK_SPRITE = "cardBack_blue2";
        public const float CARD_SELECTED_OFFSET = 0.3f;
        public const int PLAYER_INITIAL_CARDS = 7;
        public const float CARD_MOVEMENT_SPEED = 25.0f;
        public const float CARD_SNAP_DISTANCE = 0.01f;
        public const float CARD_ROTATION_SPEED = 8f;
        public const float BOOK_MAX_RANDOM_ROTATION = 15f;
        public const byte POOL_IS_EMPTY = 255;
    }

    public enum Suits
    {
        NoSuits = -1,
        Spades = 0,
        Clubs = 1,
        Diamonds = 2,
        Hearts = 3,
    }

    public enum Ranks
    {
        [Description("No Ranks")]
        NoRanks = -1,
        [Description("A")]
        Ace = 1,
        [Description("2")]
        Two = 2,
        [Description("3")]
        Three = 3,
        [Description("4")]
        Four = 4,
        [Description("5")]
        Five = 5,
        [Description("6")]
        Six = 6,
        [Description("7")]
        Seven = 7,
        [Description("8")]
        Eight = 8,
        [Description("9")]
        Nine = 9,
        [Description("10")]
        Ten = 10,
        [Description("J")]
        Jack = 11,
        [Description("Q")]
        Queen = 12,
        [Description("K")]
        King = 13,
    }
}

/*
?? Constants.cs — Script Documentation
Namespace: GoFish
Purpose:
This script defines:

Global numeric and visual constants

Game configuration settings

The Ranks and Suits enums for card types

It is used across all major scripts to keep values consistent and easy to update.

?? Class: Constants
A static utility class that stores all global values used by other classes like Card, Game, and CardAnimator.

?? Fields
Constant	Type	Description
PLAYER_CARD_POSITION_OFFSET	float	Horizontal spacing between cards in a player’s hand
PLAYER_BOOK_POSITION_OFFSET	float	Offset between books laid out beside a player
DECK_CARD_POSITION_OFFSET	float	Space between cards in the main deck (visual only)
CARD_BACK_SPRITE	string	Name of the sprite used for the card back
CARD_SELECTED_OFFSET	float	How much a card lifts when selected
PLAYER_INITIAL_CARDS	int	Number of cards each player starts with (default 7)
CARD_MOVEMENT_SPEED	float	Speed of card movement when animating
CARD_SNAP_DISTANCE	float	How close the card must be to snap into place
CARD_ROTATION_SPEED	float	How quickly the card rotates into position
BOOK_MAX_RANDOM_ROTATION	float	Random visual tilt for displayed books (for aesthetics)
POOL_IS_EMPTY	byte	Special marker value to indicate an empty deck

?? Why This Class Matters
Centralized values reduce hardcoding and make game tuning easier.

Improves maintainability: change card animation speed in one place.

Makes the code easier to read — Constants.CARD_SELECTED_OFFSET is clearer than just 0.3f.

?? Enums
Suits
Used to categorize cards into the 4 traditional suits.

csharp
public enum Suits
Name	Value	Description
NoSuits	-1	Default/uninitialized
Spades	0	Black suit
Clubs	1	Black suit
Diamonds	2	Red suit
Hearts	3	Red suit

Ranks
Used to represent card values (Ace to King), with display names using [Description] attributes.

csharp
Copy
Edit
public enum Ranks
Enum Name	Value	Description Attribute
NoRanks	-1	"No Ranks"
Ace	1	"A"
Two	2	"2"
Three	3	"3"
Four	4	"4"
Five	5	"5"
Six	6	"6"
Seven	7	"7"
Eight	8	"8"
Nine	9	"9"
Ten	10	"10"
Jack	11	"J"
Queen	12	"Q"
King	13	"K"

?? Where It’s Used
Card.cs: To fetch card descriptions, sprite names, and back visuals

CardAnimator.cs: To control animation distance, rotation, and speed

Game.cs: For initial card count, game-over signal

Across All Scripts: For clean and consistent game behavior tuning

?? Teaching Tip
Let students tweak constants like:

PLAYER_INITIAL_CARDS ? change game balance

CARD_SELECTED_OFFSET ? affect UX feel

CARD_MOVEMENT_SPEED ? smooth vs fast gameplay

This is a great intro to the concept of global config constants and enum display attributes in C#.
*/