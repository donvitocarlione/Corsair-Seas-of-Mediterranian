/// <summary>
/// Represents the various factions in the Mediterranean Sea
/// </summary>
public enum FactionType
{
    None = 0,                // No faction assigned
    Player,                  // Player's faction
    
    // Pirate Factions
    BarbaryPirates,         // North African corsairs
    MalteseCorsairs,        // Knights of St. John privateers
    UscocPirates,           // Adriatic Sea pirates
    BarbarianCorsairs,      // Independent Barbary corsairs
    LevanticPirates,        // Eastern Mediterranean pirates
    GreekPirates,           // Aegean Sea pirates
    CilicianPirates,        // Ancient Mediterranean pirates
    ProvencalPirates,       // French Mediterranean pirates
    
    // Regional Powers
    AlgerianTaifa,          // Algerian privateers
    RenegadoPirates,        // European converts to Islam who became pirates
    
    // Generic Factions
    Pirates,                // Generic pirates
    Merchants,              // Trading vessels
    Navy,                   // Naval forces
    Neutral                 // Neutral vessels
}