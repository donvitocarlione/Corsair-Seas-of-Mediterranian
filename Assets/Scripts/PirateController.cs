using UnityEngine;

public class PirateController : MonoBehaviour
{
    public Pirate pirate;
    private Ship controlledShip;
    private DamageSystem shipDamageSystem;

    public void Initialize(Pirate pirate)
    {
        this.pirate = pirate;
        SetupShip();
    }

    private void SetupShip()
    {
        controlledShip = GetComponent<Ship>();
        if (controlledShip != null)
        {
            controlledShip.Initialize(pirate.faction, pirate.name + "'s Ship");
            shipDamageSystem = controlledShip.GetComponent<DamageSystem>();
        }
    }

    public void Attack(Ship targetShip)
    {
        if (controlledShip != null)
        {
            controlledShip.Attack(targetShip);
        }
    }

    public void MoveTo(Vector3 position)
    {
        if (controlledShip != null)
        {
            controlledShip.MoveTo(position);
        }
    }

    public float GetHealthPercentage()
    {
        return controlledShip != null ? controlledShip.GetHealthPercentage() : 0f;
    }
}
