using UnityEngine;
using System;
using System.Collections.Generic;

public class Item : MonoBehaviour, IConsumable
{
	[Serializable]
	public class PlayerStat
	{
		public PlayerStatsType stat;
		public int value;

		public PlayerStat (PlayerStatsType s, int v)
		{
			stat = s;
			value = v;
		}
	}
		
	public bool useImmediately;
	public ItemType type = ItemType.Food;
	public PlayerStat[] statsToModify;


	public ItemType GetItemType()
	{
		return type;
	}

	// the default behavior for items is to be used immediately, as in food/soda.
	// the Collect method should be used to inventory collecatble items.
	public virtual void Collect(IConsumer c)
	{
		if (useImmediately) {
			Use (c);
		} else {
			InventoryManager.instance.AddToInventory(this);
		}

		gameObject.SetActive (false);
	}


	public virtual void Use(IConsumer c)
	{
		int i;
		PlayerStat stat;

		for (i = 0; i < statsToModify.Length; i++) {
			stat = statsToModify[i];
			switch(stat.stat){
				case PlayerStatsType.HitPoint:
					c.Consume (this);
					c.AddHitPoints (stat.value);
					break;
				case PlayerStatsType.MagicPoint:
					c.Consume (this);
					c.AddMagicPoints (stat.value);
					break;
				case PlayerStatsType.Madness:
					c.Consume (this);
					c.AddMadnessPoints(stat.value);
					break;
			}
		}
	}
}