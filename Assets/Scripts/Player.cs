using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;      //Allows us to use SceneManager

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject, IConsumer
{
	public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
	public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;


	private Animator animator;                  //Used to store a reference to the Player's animator component.
	private int food;                           //Used to store player food points total during level.
	private GameManager gameManager;


	//Start overrides the Start function of MovingObject
	protected override void Start ()
	{
		//Get a component reference to the Player's animator component
		animator = GetComponent<Animator>();

		//Get the current food point total stored in GameManager.instance between levels.
		gameManager = GameManager.instance;
		food = gameManager.playerFoodPoints;
		UpdateFood (food);

		//Call the Start function of the MovingObject base class.
		base.Start ();
	}


	//This function is called when the behaviour becomes disabled or inactive.
	private void OnDisable ()
	{
		//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
		gameManager.playerFoodPoints = food;
	}


	private void Update ()
	{
		//If it's not the player's turn, exit the function.
		if(!gameManager.playersTurn) return;

		int horizontal = 0;     //Used to store the horizontal move direction.
		int vertical = 0;       //Used to store the vertical move direction.


		//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));

		//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
		vertical = (int) (Input.GetAxisRaw ("Vertical"));

		//Check if moving horizontally, if so set vertical to zero.
		if(horizontal != 0)
		{
			vertical = 0;
		}

		//Check if we have a non-zero value for horizontal or vertical
		if(horizontal != 0 || vertical != 0)
		{
			//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
			//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
			AttemptMove<Wall> (horizontal, vertical);
		}
	}

	private void UpdateFood(int food)
	{
		gameManager.StateChanged ("player:food", food);

		if (food <= 0) {
			gameManager.StateChanged ("player:dead", 0);
		}
	}

	//AttemptMove overrides the AttemptMove function in the base class MovingObject
	//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
		base.AttemptMove <T> (xDir, yDir);

		//Hit allows us to reference the result of the Linecast done in Move.
		RaycastHit2D hit;

		//If Move returns true, meaning Player was able to move into an empty space.
		if (Move (xDir, yDir, out hit)) 
		{
			SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
			//Every time player moves, subtract from food points total.
			food--;
			UpdateFood (food);
		}

		//Set the playersTurn boolean of GameManager to false now that players turn is over.
		gameManager.playersTurn = false;
	}


	//OnCantMove overrides the abstract function OnCantMove in MovingObject.
	//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
	protected override void OnCantMove <T> (T component)
	{
		//Set hitWall to equal the component passed in as a parameter.
		Wall hitWall = component as Wall;

		//Call the DamageWall function of the Wall we are hitting.
		hitWall.DamageWall (wallDamage);

		//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
		animator.SetTrigger ("playerChop");
	}


	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
	private void OnTriggerEnter2D (Collider2D other)
	{
		//Check if the tag of the trigger collided with is Exit.
		if (other.tag == "Exit") {
			//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
			Invoke ("Restart", restartLevelDelay);

			//Disable the player object since level is over.
			enabled = false;
		} else {
			// other is an item of some kind
			if (other.tag == "Item") {
				Consume (other.gameObject.GetComponent<Item>());
			}
		}
	}


	//Restart reloads the scene when called.
	private void Restart ()
	{
		//Load the last scene loaded, in this case Main, the only scene in the game.
		SceneManager.LoadScene (0);
	}
		

	public void TakeHit (int loss)
	{
		//Set the trigger for the player animator to transition to the playerHit animation.
		animator.SetTrigger ("playerHit");

		//Subtract lost food points from the players total.
		food -= loss;
		UpdateFood (food);
	}

	public void Consume(IConsumable c){
		Dictionary<string, int> attributes = c.GetAttributes ();

		food += attributes["value"];
		UpdateFood (food);
		c.Consumed ();

		switch ((ItemType)attributes["type"]) {
			case ItemType.Food:
				SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				break;
			case ItemType.Soda:
				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				break;
		}
	}
}