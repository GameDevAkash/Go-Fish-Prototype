using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GoFish
{
    [Serializable]
    public class CardSelectedEvent : UnityEvent<Card>
    {
    }

    public class MouseActions : MonoBehaviour
    {
        public CardSelectedEvent OnCardSelected = new CardSelectedEvent();

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Card card = MouseOverCard();

                if (card != null)
                {
                    //OnCardSelected.Invoke(card);
                    Game.Instance.OnCardSelected(card);
                }
            }
        }

        Card MouseOverCard()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                Card card = hit.transform.gameObject.GetComponent<Card>();
                if (card != null)
                {
                    return card;
                }
            }

            return null;
        }
    }
}

/*??? MouseActions.cs — Script Documentation
Namespace: GoFish
Purpose:
Handles user input — specifically mouse clicks — and detects when a card has been selected. It links visual player interaction to the game logic handled by the Game.cs script.

?? Class 1: CardSelectedEvent
csharp
Copy
Edit
[Serializable]
public class CardSelectedEvent : UnityEvent<Card> { }
Custom Unity event class derived from UnityEvent<T>.

Enables drag-and-drop event binding in the Unity Inspector if needed (not used actively here but allows extensibility).

Carries a reference to the Card that was selected when triggered.

?? Class 2: MouseActions
csharp
Copy
Edit
public class MouseActions : MonoBehaviour
A MonoBehaviour that:

Listens for mouse input

Casts a ray into the 2D world to detect clickable cards

Tells the Game class which card was clicked

?? Fields
Field	Type	Description
OnCardSelected	CardSelectedEvent	UnityEvent that can be subscribed to for card selection (currently unused in favor of direct call)

?? Unity Lifecycle Method
void Update()
Runs every frame and checks for mouse input.

What it does:

Detects when the left mouse button is released:

csharp
Copy
Edit
if (Input.GetMouseButtonUp(0)) { ... }
Checks whether the mouse is over a card using MouseOverCard()

If a card is found, it calls:

csharp
Copy
Edit
Game.Instance.OnCardSelected(card);
Which passes the selected card into the game logic controller.

?? Optional behavior (currently commented out):

csharp
Copy
Edit
OnCardSelected.Invoke(card);
This would allow other scripts to subscribe to the card click event using Unity’s event system.

?? Helper Method: MouseOverCard()
csharp
Copy
Edit
Card MouseOverCard()
Casts a 2D ray from the camera to the mouse pointer

Checks for colliders hit by the ray

If a Card component is found on the hit object, it is returned

csharp
Copy
Edit
RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
?? Note: Requires Card GameObjects to have colliders (usually BoxCollider2D) to be detected.

?? Teaching Concepts
Concept	Explanation
Raycasting	Technique to detect GameObjects using mouse or screen position
UnityEvent	Custom event system for decoupled communication
Input System	Unity’s way of handling player interaction (via Input.GetMouseButtonUp)
2D Physics	Raycast in 2D scene, using Physics2D.Raycast

?? Sample Use Case in Game.cs
csharp
Copy
Edit
void OnCardSelected(Card card)
{
    // Game logic to handle card selection
}
This is automatically triggered when the player clicks a card during their turn.

? Why This Design is Good
Feature	Benefit
Simple and focused	Does just one thing: detect a card click
Decoupled logic	Doesn’t hold game logic — just passes the click to Game
Scalable	Can be easily expanded to support touch input or other UI interaction

?? Teaching Tip
Ask your students:

"What happens if a card doesn’t have a collider?"
"How could we extend this to drag cards or show hover effects?"

Have them:

Add a BoxCollider2D to each card prefab

Debug the hit.collider.name to see what they're clicking on*/