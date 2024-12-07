class Faction:
    def __init__(self, name, influence_level=0):
        self.name = name
        self.influence_level = influence_level
        self.members = []  # List of pirates in this faction
        self.controlled_ports = []  # List of ports under faction control
        self.relationships = {}  # Dictionary storing relationships with other factions

    def add_member(self, pirate):
        if pirate not in self.members:
            self.members.append(pirate)
            pirate.faction = self

    def remove_member(self, pirate):
        if pirate in self.members:
            self.members.remove(pirate)
            pirate.faction = None

    def set_relationship(self, other_faction, value):
        """Set relationship value with another faction (-100 to 100)"""
        self.relationships[other_faction] = max(-100, min(100, value))

    def get_relationship(self, other_faction):
        return self.relationships.get(other_faction, 0)
