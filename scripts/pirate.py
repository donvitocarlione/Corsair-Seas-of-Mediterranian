from typing import List
from .ship import Ship

class Pirate:
    def __init__(self, name):
        self.name = name
        self.ships: List[Ship] = []
        self.faction = None
        self.reputation = 0
        self.gold = 0
        self.selected_ship = None

    def add_ship(self, ship: Ship):
        """Add a ship to pirate's fleet"""
        self.ships.append(ship)
        ship.set_owner(self)
        if len(self.ships) == 1:  # If this is the first ship, auto-select it
            self.select_ship(ship)

    def remove_ship(self, ship: Ship):
        """Remove a ship from pirate's fleet"""
        if ship in self.ships:
            self.ships.remove(ship)
            ship.set_owner(None)
            if self.selected_ship == ship:
                self.selected_ship = self.ships[0] if self.ships else None

    def select_ship(self, ship: Ship) -> bool:
        """Select a specific ship from pirate's fleet"""
        if ship in self.ships:
            if self.selected_ship:
                self.selected_ship.deselect()
            self.selected_ship = ship
            ship.select()
            return True
        return False

    def join_faction(self, faction):
        """Join a faction"""
        if self.faction:
            self.faction.remove_member(self)
        faction.add_member(self)

    def leave_faction(self):
        """Leave current faction"""
        if self.faction:
            self.faction.remove_member(self)
