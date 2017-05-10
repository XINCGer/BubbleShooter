using UnityEngine;
using System.Collections;

public enum MissionType
{
	Stars,
	CircleEntity,
	Entity
}
public class Mission 
{
	public MissionType type = MissionType.Stars;
	public int amount = 0;
	public Mission(int pAmount, MissionType missionType)
	{
		amount = pAmount;
		type = missionType;
	}
}
