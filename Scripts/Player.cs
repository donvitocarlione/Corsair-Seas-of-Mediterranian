using System.Collections.Generic;
using UnityEngine;

public class Player : Pirate
{
    public int level = 1;
    public float experience = 0f;
    public Dictionary<string, int> skills = new Dictionary<string, int>();
    public List<Quest> questLog = new List<Quest>();

    private void Start()
    {
        // Initialize player-specific components
    }

    public override void AddShip(Ship ship)
    {
        // Enforce one-ship limit for new players
        if (ships.Count == 0 || level >= 10)
        {
            base.AddShip(ship);
        }
        else
        {
            Debug.Log("Cannot add more ships until reaching level 10");
        }
    }

    public void GainExperience(float amount)
    {
        experience += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int newLevel = 1 + Mathf.FloorToInt(experience / 1000); // Level up every 1000 XP
        if (newLevel > level)
        {
            LevelUp(newLevel);
        }
    }

    private void LevelUp(int newLevel)
    {
        level = newLevel;
        GainSkillPoints();
        // Add level up effects, particle systems, etc.
        Debug.Log($"Level up! Now level {level}");
    }

    private void GainSkillPoints()
    {
        int currentPoints = 0;
        skills.TryGetValue("points", out currentPoints);
        skills["points"] = currentPoints + 1;
    }

    public void AddQuest(Quest quest)
    {
        if (!questLog.Contains(quest))
        {
            questLog.Add(quest);
        }
    }

    private void OnMouseDown()
    {
        // Handle player selection
        // You might want to implement additional player-specific selection behavior
    }
}
