using System;
using System.Collections.Generic;

public interface IConsumable
{
	ItemType GetItemType();
	void Collect (IConsumer c);
	void Use (IConsumer c);
}

