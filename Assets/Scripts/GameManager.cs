using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
	public float turnDelay = 0.1f;                          //Delay between each Player turn.
	public int playerFoodPoints = 100;                      //Starting value for Player food points.
	public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
	[HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.


	private GameObject levelImage;
	private Text levelText;
	private Text foodText;
	private bool doingSetup = false;
	private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
	private int level = 1;                                  //Current level number, expressed in game as "Day 1".
	private List<Enemy> enemies;                          //List of all Enemy units, used to issue them move commands.
	private bool enemiesMoving;                             //Boolean to check if enemies are moving.
	public Player player;
	public AudioClip gameOverSound;



	//Awake is always called before any Start functions
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    

		DontDestroyOnLoad(gameObject);
		enemies = new List<Enemy>();
		boardScript = GetComponent<BoardManager>();
		InitGame(level);
	}

	//This is called each time a scene is loaded.
	void OnLevelWasLoaded(int index)
	{
		InitGame(++level);
	}

	//Initializes the game for each level.
	void InitGame(int level)
	{
		doingSetup = true;

		levelImage = GameObject.Find("Level Image");
		levelText = GameObject.Find("Level Text").GetComponent<Text>();
		foodText = GameObject.Find("Food Text").GetComponent<Text>();

		levelText.text = "Day " + level;

		levelImage.SetActive(true);

		Invoke("HideLevelImage", levelStartDelay);

		enemies.Clear();
		boardScript.SetupScene(level);

	}

	public void StateChanged(string key, int value)
	{
		switch (key) {
			case "player:food" :
				foodText.text = "Food: " + value;
				break;
			case "player:dead":
				GameOver ();
				break;
			default:
				break;
		}
	}

	void HideLevelImage()
	{
		levelImage.SetActive(false);
		doingSetup = false;
	}


	//Update is called every frame.
	void Update()
	{
		//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
		if(playersTurn || enemiesMoving || doingSetup)
			return;

		StartCoroutine (MoveEnemies ());
	}

	//Call this to add the passed in Enemy to the List of Enemy objects.
	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}


	//GameOver is called when the player reaches 0 food points
	public void GameOver()
	{
		SoundManager.instance.PlaySingle(gameOverSound);
		SoundManager.instance.musicSource.Stop ();

		levelText.text = "After " + level + " days, you died.";

		levelImage.SetActive(true);

		//Disable this GameManager.
		enabled = false;
	}

	//Coroutine to move enemies in sequence.
	IEnumerator MoveEnemies()
	{
		//While enemiesMoving is true player is unable to move.
		enemiesMoving = true;

		//Wait for turnDelay seconds, defaults to .1 (100 ms).
		yield return new WaitForSeconds(turnDelay);

		//If there are no enemies spawned (IE in first level):
		if (enemies.Count == 0) 
		{
			//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
			yield return new WaitForSeconds(turnDelay);
		}

		//Loop through List of Enemy objects.
		for (int i = 0; i < enemies.Count; i++)
		{
			//Call the MoveEnemy function of Enemy at index i in the enemies List.
			enemies[i].MoveEnemy ();

			//Wait for Enemy's moveTime before moving next Enemy, 
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		//Once Enemies are done moving, set playersTurn to true so player can move.
		playersTurn = true;

		//Enemies are done moving, set enemiesMoving to false.
		enemiesMoving = false;
	}
}