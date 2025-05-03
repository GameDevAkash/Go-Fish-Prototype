using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GoFish
{
    public class CardAnimation
    {
        public Card card;
        public Vector2 destination;
        public Quaternion rotation;

        public CardAnimation(Card c, Vector2 pos)
        {
            card = c;
            destination = pos;
            rotation = Quaternion.identity;
        }

        public CardAnimation(Card c, Vector2 pos, Quaternion rot)
        {
            card = c;
            destination = pos;
            rotation = rot;
        }

        public bool Play()
        {
            bool finished = false;

            if (Vector2.Distance(card.transform.position, destination) < Constants.CARD_SNAP_DISTANCE)
            {
                card.transform.position = destination;
                finished = true;
            }
            else
            {
                card.transform.position = Vector2.MoveTowards(card.transform.position, destination, Constants.CARD_MOVEMENT_SPEED * Time.deltaTime);
                card.transform.rotation = Quaternion.Lerp(card.transform.rotation, rotation, Constants.CARD_ROTATION_SPEED * Time.deltaTime);
            }

            return finished;
        }
    }

    /// <summary>
    /// Controls all card animations in the game
    /// </summary>
    public class CardAnimator : MonoBehaviour
    {
        public GameObject CardPrefab;

        public List<Card> DisplayingCards;

        public Queue<CardAnimation> cardAnimations;

        CardAnimation currentCardAnimation;

        Vector2 startPosition = new Vector2(-5f, 1f);

        // invoked when all queued card animations have been played
        public UnityEvent OnAllAnimationsFinished = new UnityEvent();

        bool working = false;

        void Start()
        {
            
        }

        private void Awake()
        {
            cardAnimations = new Queue<CardAnimation>();
            InitializeDeck();
        }

        void InitializeDeck()
        {
            DisplayingCards = new List<Card>();

            for (byte value = 0; value < 52; value++)
            {
                Vector2 newPosition = startPosition + Vector2.right * Constants.DECK_CARD_POSITION_OFFSET * value;
                GameObject newGameObject = Instantiate(CardPrefab, newPosition, Quaternion.identity);
                newGameObject.transform.parent = transform;
                Card card = newGameObject.GetComponent<Card>();
                card.SetDisplayingOrder(value);
                card.transform.position = newPosition;
                DisplayingCards.Add(card);
            }
        }

        public void DealDisplayingCards(Player player, int numberOfCard)
        {
            int start = DisplayingCards.Count - 1;
            int finish = DisplayingCards.Count - 1 - numberOfCard;
            Debug.Log(DisplayingCards.Count);
            List<Card> cardsToRemoveFromDeck = new List<Card>();

            for (int i = start; i > finish; i--)
            {
                Debug.Log(i);
                Debug.Log(DisplayingCards[i] == null);
                Card card = DisplayingCards[i];
                player.ReceiveDisplayingCard(card);
                cardsToRemoveFromDeck.Add(card);
                AddCardAnimation(card, player.NextCardPosition());
            }

            foreach (Card card in cardsToRemoveFromDeck)
            {
                DisplayingCards.Remove(card);
            }
        }

        public void DrawDisplayingCard(Player player)
        {
            int numberOfDisplayingCard = DisplayingCards.Count;

            if (numberOfDisplayingCard > 0)
            {
                Card card = DisplayingCards[numberOfDisplayingCard - 1];
                player.ReceiveDisplayingCard(card);
                AddCardAnimation(card, player.NextCardPosition());

                DisplayingCards.Remove(card);
            }
        }

        public void DrawDisplayingCard(Player player, byte value)
        {
            int numberOfDisplayingCard = DisplayingCards.Count;

            if (numberOfDisplayingCard > 0)
            {
                Card card = DisplayingCards[numberOfDisplayingCard - 1];
                card.SetCardValue(value);
                card.SetFaceUp(true);
                player.ReceiveDisplayingCard(card);
                AddCardAnimation(card, player.NextCardPosition());

                DisplayingCards.Remove(card);
            }
        }

        public void AddCardAnimation(Card card, Vector2 position)
        {
            CardAnimation ca = new CardAnimation(card, position);
            cardAnimations.Enqueue(ca);
            working = true;
        }

        public void AddCardAnimation(Card card, Vector2 position, Quaternion rotation)
        {
            CardAnimation ca = new CardAnimation(card, position, rotation);
            cardAnimations.Enqueue(ca);
            working = true;
        }

        private void Update()
        {
            if (currentCardAnimation == null)
            {
                NextAnimation();
            }
            else
            {
                if (currentCardAnimation.Play())
                {
                    NextAnimation();
                }
            }
        }

        void NextAnimation()
        {
            currentCardAnimation = null;

            if (cardAnimations.Count > 0)
            {
                CardAnimation ca = cardAnimations.Dequeue();
                currentCardAnimation = ca;
            }
            else
            {
                if (working)
                {
                    working = false;
                    OnAllAnimationsFinished.Invoke();
                }
            }
        }
    }
}

/*
CardAnimator.cs — Script Documentation
Namespace: GoFish
Purpose:
This script handles card animation, dealing, and movement in the game. It animates how cards are created, assigned, and smoothly moved to each player's area during gameplay.

It uses a queue-based system so that card movements happen one after the other instead of all at once.

?? Class 1: CardAnimation
A lightweight helper class that defines:

Which card to move

Where to move it

What rotation it should end up with

?? Fields:
Type	Name	Description
Card	card	The specific card being animated.
Vector2	destination	The position the card should move to.
Quaternion	rotation	The rotation the card should end up with.

?? Constructors:
public CardAnimation(Card c, Vector2 pos)
public CardAnimation(Card c, Vector2 pos, Quaternion rot)
First version sets position only.

Second version includes rotation.

?? Method:
public bool Play()
Smoothly animates the card's movement and rotation.

Returns true when the card has reached its destination.

Uses MoveTowards and Lerp for frame-by-frame motion.

?? Class 2: CardAnimator
Main controller class for managing animations and visual state of all cards in the game.

?? Fields:
Type	Name	Description
GameObject	CardPrefab	Prefab used to instantiate card GameObjects.
List<Card>	DisplayingCards	All cards currently in the deck or on the table.
Queue<CardAnimation>	cardAnimations	Queue of animations to be played.
UnityEvent	OnAllAnimationsFinished	Event fired when animation queue is empty.

?? Lifecycle Methods
Awake()
Initializes the animation queue.

Calls InitializeDeck() to generate all 52 cards.

Start()
Currently unused but available for future use.

?? Core Methods
void InitializeDeck()
Instantiates 52 cards with CardPrefab.

Spreads them out in a line using Constants.DECK_CARD_POSITION_OFFSET.

Adds them to DisplayingCards.

void DealDisplayingCards(Player player, int numberOfCard)
Removes the specified number of cards from the deck.

Assigns each one to the player.

Queues animations to move each card to the player's hand.

void DrawDisplayingCard(Player player)
Removes one card from the deck.

Assigns it to the player.

Adds its animation to the queue.

void DrawDisplayingCard(Player player, byte value)
Same as above, but sets a specific card value manually (used when AI or player knows the card).

?? Animation Methods
void AddCardAnimation(Card card, Vector2 position)
Queues a basic animation to move the card to a new position.

void AddCardAnimation(Card card, Vector2 position, Quaternion rotation)
Queues an animation to move and rotate the card.

?? Update() Method (Every Frame)
Handles processing the current animation:

If there’s no active animation, pull the next from the queue.

If there is one, Play() it.

Once finished, start the next.

If all done, trigger OnAllAnimationsFinished.

void NextAnimation()
Clears the current animation.

Pulls the next one from the queue, if any.

If the queue is empty and animations were in progress, raises the finished event.

?? Key Teaching Concepts
Concept	Explanation
Animation Queue	Prevents all cards from animating at once — they move one-by-one for clarity.
Prefab Instantiation	Dynamically creates 52 card objects in InitializeDeck().
UnityEvent	OnAllAnimationsFinished lets other scripts know when it's safe to continue (used in Game.cs).
Lerp/MoveTowards	Used for smooth frame-by-frame movement.

?? Real-World Analogy
Imagine a card dealer sliding cards across the table to players — one at a time. This class is that dealer. It also keeps track of which card is being dealt and makes sure it finishes before moving the next.

?? Sample Usage
cardAnimator.DealDisplayingCards(player, 7); // Give player 7 cards with animation
cardAnimator.DrawDisplayingCard(bot);        // Bot draws a card*/
