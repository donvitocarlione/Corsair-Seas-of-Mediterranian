/// <summary>
/// Represents all possible factions in the Mediterranean Sea
/// </summary>
public enum FactionType
{
    None = 0,               // No faction assigned
    Player,                 // Player's faction
    
    // Major Powers
    OttomanEmpire,          // Ottoman Turkish Empire
    VenetianRepublic,       // Republic of Venice
    SpanishEmpire,          // Spanish Habsburg Empire
    KnightsOfMalta,         // Knights Hospitaller/Order of St. John
    PapalStates,            // The Catholic Church's territories
    
    // North African Powers
    AlgiersCorsairs,        // Barbary Corsairs of Algiers
    TunisCorsairs,          // Barbary Corsairs of Tunis
    TripoliCorsairs,        // Barbary Corsairs of Tripoli
    MoroccanCorsairs,       // Corsairs of the Moroccan Sultanate
    
    // Mediterranean Pirates
    UscocPirates,           // Adriatic Sea pirates
    GreekPirates,           // Aegean Sea pirates
    LevanticPirates,        // Eastern Mediterranean pirates
    BarbarianPirates,       // Independent Barbary pirates
    RenegadoPirates,        // European converts who became pirates
    
    // Merchant Factions
    VenetianMerchants,      // Venetian trading fleet
    GenoeseMerchants,      // Genoese trading network
    RagusanMerchants,      // Dubrovnik/Ragusa merchants
    GreekMerchants,        // Greek trading vessels
    JewishMerchants,       // Jewish trading networks
    
    // Generic Types
    Pirates,                // Generic pirate faction
    Merchants,              // Generic merchant faction
    Navy,                   // Generic naval forces
    Neutral                 // Neutral vessels
}
