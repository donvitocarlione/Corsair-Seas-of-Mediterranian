using System.Collections.Generic;
using UnityEngine;

public class Player : Pirate
{
    [Header("Player Stats")]
    public int level = 1;
    public float experience = 0f;
    public Dictionary<string, float> skills = new Dictionary<string, float>();
    public List<Quest> questLog = new List<Quest>();

    private void Start()
    {
        // Initialize player-specific components
        InitializeSkills();
    }

    private void InitializeSkills()
    {
        // Initialize basic skills
        skills["Navigation"] = 1f;
        skills["Combat"] = 1f;
        skills["Trading"] = 1f;
        skills["Leadership"] = 1f;
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
        // Add skill points that can be distributed
        if (!skills.ContainsKey("AvailablePoints"))
        {
            skills["AvailablePoints"] = 0;
        }
        skills["AvailablePoints"] += 1;
    }

    public void AddQuest(Quest quest)
    {
        if (!questLog.Contains(quest))
        {
            questLog.Add(quest);
        }
    }

    // Improve a specific skill using available points
    public bool ImproveSkill(string skillName)
    {
        if (skills.ContainsKey("AvailablePoints") && skills["AvailablePoints"] > 0)
        {
            if (skills.ContainsKey(skillName))
            {
                skills[skillName] += 1f;
                skills["AvailablePoints"] -= 1;
                return true;
            }
        }
        return false;
    }

    private void OnMouseDown()
    {
        // Handle player selection
        Debug.Log("Player selected");
    }
}
