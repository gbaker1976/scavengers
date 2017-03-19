using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

	private List<IConsumable> loot = new List<IConsumable>();

	public static InventoryManager instance = null;
	public Player player;

	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    
	}


	// Use this for initialization
	void Start () {
		
	}


	// Update is called once per frame
	void Update () {
		
	}


	public void AddToInventory(IConsumable c)
	{
		loot.Add (c);
	}
}
		