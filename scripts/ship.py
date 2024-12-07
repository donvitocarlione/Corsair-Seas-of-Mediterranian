class Ship:
    def __init__(self, name, ship_type, max_crew, max_cargo):
        self.name = name
        self.ship_type = ship_type
        self.max_crew = max_crew
        self.current_crew = 0
        self.max_cargo = max_cargo
        self.current_cargo = 0
        self.health = 100
        self.owner = None
        self.position = (0, 0)  # X, Y coordinates on the map
        self.selected = False

    def select(self):
        """Select this ship"""
        self.selected = True
        return True

    def deselect(self):
        """Deselect this ship"""
        self.selected = False

    def set_owner(self, owner):
        """Set the owner of this ship"""
        self.owner = owner

    def move_to(self, x, y):
        """Move ship to new coordinates"""
        self.position = (x, y)

    def take_damage(self, amount):
        """Take damage and return True if ship is destroyed"""
        self.health -= amount
        return self.health <= 0

    def repair(self, amount):
        """Repair ship"""
        self.health = min(100, self.health + amount)
