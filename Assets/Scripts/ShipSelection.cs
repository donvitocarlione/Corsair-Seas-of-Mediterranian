using UnityEngine;

public class ShipSelection : MonoBehaviour
{
    private Material originalMaterial;
    private Renderer shipRenderer;

    void Awake()
    {
        shipRenderer = GetComponent<Renderer>();
        if (shipRenderer != null)
            originalMaterial = shipRenderer.material;
    }

    public void SetSelected(bool selected, Material selectedMaterial)
    {
        if (shipRenderer == null) return;
        shipRenderer.material = selected ? selectedMaterial : originalMaterial;
    }
}