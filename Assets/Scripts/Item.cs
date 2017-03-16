using UnityEngine;
using System.Collections.Generic;

public class Item : MonoBehaviour, IConsumable
{
	public int value = 10;
	public ItemType type = ItemType.Food;

	public Dictionary<string, int> GetAttributes()
	{
		Dictionary<string, int> attributes = new Dictionary<string, int>();
		attributes.Add ("type", 1);
		attributes.Add ("value", value);

		return attributes;
	}

	public void Consumed(){
		gameObject.SetActive (false);
	}
}