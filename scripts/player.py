from .pirate import Pirate
from .ship import Ship

class Player(Pirate):
    def __init__(self, name):
        super().__init__(name)
        self.level = 1
        self.experience = 0
        self.skills = {}
        self.quest_log = []

    def add_ship(self, ship: Ship):
        """Override add_ship to enforce one-ship limit for new players"""
        if not self.ships:  # Only allow adding a ship if player has none
            super().add_ship(ship)
            return True
        elif self.level >= 10:  # Allow multiple ships after reaching level 10
            super().add_ship(ship)
            return True
        return False

    def gain_experience(self, amount):
        """Gain experience and handle level ups"""
        self.experience += amount
        # Check for level up (simple level up system)
        new_level = 1 + (self.experience // 1000)  # Level up every 1000 XP
        if new_level > self.level:
            self.level_up(new_level)

    def level_up(self, new_level):
        """Handle level up effects"""
        self.level = new_level
        # Add level up bonuses, skill points, etc.
        self.gain_skill_points()

    def gain_skill_points(self):
        """Add skill points when leveling up"""
        self.skills['points'] = self.skills.get('points', 0) + 1

    def add_quest(self, quest):
        """Add a new quest to the player's quest log"""
        self.quest_log.append(quest)
