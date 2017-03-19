using UnityEngine;
using System.Collections.Generic;

public class FoodItem : Item
{
	// the default behavior for items is to be used immediately, as in food/soda.
	// the Collect method should be used to inventory collecatble items.
	public override void Collect(IConsumer c)
	{
		Use (c);
		gameObject.SetActive (false);
	}


	public override void Use(IConsumer c)
	{
		c.Consume (this);
	}
}