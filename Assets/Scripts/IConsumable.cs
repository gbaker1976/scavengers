using System;
using System.Collections.Generic;

public interface IConsumable
{
	Dictionary<string, int> GetAttributes();
	void Consumed ();
}

