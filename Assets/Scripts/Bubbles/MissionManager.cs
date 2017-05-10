using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MissionManager 
{
	public static MissionManager Instance;
	//List of mission player will need to be pass
	public List<Mission> missionScores = new List<Mission>();
	
	public MissionManager()
	{
		Instance = this;
	}
	//Init all score for player by missions from levels
	public void InitScore(List<Mission> missions)
	{
		//Init all score to 0
		missionScores.Clear();
		for(int i=0; i<missions.Count;i++)
		{
			Mission score = new Mission(0,missions[i].type);
			missionScores.Add(score);
		}
	}
	//Insert score by mission type
	//Ex: if user get one Ring, we will add 1 with type is MissionType.Ring
	public void AddScore(int amount, MissionType missionType)
	{
		Mission score = missionScores.Find(obj=>obj.type == missionType);
		if(score != null)
		{
			score.amount += amount;

		}
	}
	public Mission GetMission(MissionType missionType)
	{
		return missionScores.Find(obj=>obj.type == missionType);
	}
	//Check level complete or failed
	public bool IsWin()
	{
		//If not all mission finished, return false
		foreach(Mission mission in LevelData.requestMissions)
		{
			if(GetMission(mission.type).amount < mission.amount)
			{
				return false;
			}
		}
		//check all missions
		return true;
	}
}
