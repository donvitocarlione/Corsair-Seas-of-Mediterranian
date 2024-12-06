# Corsair Seas of Mediterranean

A Unity game project featuring immersive corsair adventures in the Mediterranean Sea. Command your ship, engage in naval battles, and navigate through a complex web of alliances and rivalries.

## 🎮 Game Features

### Naval Combat
- Advanced ship-to-ship combat system
- Zone-based damage system (hull, sails, deck)
- Critical hit mechanics and realistic damage effects
- Dynamic ship health and repair system

### Ship Mechanics
- Realistic ship physics and buoyancy
- Weather-affected movement
- Multiple ship types with unique characteristics
- Advanced camera controls for tactical views

### Faction System
- Multiple factions with unique relationships
- Dynamic diplomacy system
- Reputation management
- Alliance and rivalry mechanics

### AI and NPCs
- Smart AI-controlled ships
- Dynamic combat behavior
- Patrol and escort missions
- Trading and piracy mechanics

## 🛠️ Technical Features

### Physics System
- Realistic water physics
- Buoyancy simulation
- Weather effects on ship movement
- Collision detection and damage

### Combat System
- Zone-based damage calculation
- Status effects (fire, water damage)
- Visual damage representation
- Sound effects system

## 🔧 Installation

1. Requirements:
   - Unity 2021.3 or later
   - Git

2. Setup:
   ```bash
   git clone https://github.com/donvitocarlione/Corsair-Seas-of-Mediterranian.git
   ```

3. Unity Setup:
   - Open Unity Hub
   - Add project from disk
   - Select the cloned repository folder
   - Open project with Unity 2021.3 or later
   - Load the main scene from Assets/Scenes

## 📁 Project Structure

### Core Systems
- `Assets/Scripts/` - All game scripts and logic
- `Assets/Prefabs/` - Ship and game object prefabs
- `Assets/Scenes/` - Game scenes
- `Assets/Materials/` - Materials and shaders

### Key Scripts
- `Ship.cs` - Core ship behavior and properties
- `ShipMovement.cs` - Physics-based movement system
- `DamageSystem.cs` - Advanced damage calculations and effects
- `AIShipController.cs` - AI behavior and pathfinding
- `DiplomacySystem.cs` - Faction relationships
- `Buoyancy.cs` - Water physics

## 🎮 Controls

- Left Click: Select ship
- Right Click: Move selected ship
- CTRL + Click: Multi-select ships
- WASD: Camera movement
- Q/E: Camera rotation
- Mouse Wheel: Camera zoom

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 🌊 Game Mechanics

### Ship Types
- Light ships (fast, low armor)
- Medium ships (balanced)
- Heavy ships (slow, high armor)

### Damage Zones
- Hull: Critical for ship integrity
- Sails: Affects movement speed
- Deck: Crew and equipment

### Weather Effects
- Wind direction affects ship speed
- Storms create challenging conditions
- Day/night cycle affects visibility
