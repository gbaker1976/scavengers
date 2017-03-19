using System;

public interface IConsumer
{
	void Consume(IConsumable c);
	int AddHitPoints(int ahp);
	int AddMagicPoints(int amp);
	int AddMadnessPoints(int mdp);
	int SubtractHitPoints(int shp);
	int SubtractMagicPoints(int smp);
	int SubtractMadnessPoints(int mdp);
}