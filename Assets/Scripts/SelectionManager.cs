using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public GameObject selectionIndicatorPrefab;
    private GameObject currentIndicator;
    private static SelectionManager instance;

    public static SelectionManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowSelectionAt(Transform target)
    {
        if (currentIndicator == null)
        {
            currentIndicator = Instantiate(selectionIndicatorPrefab);
        }

        // Position the indicator at the target's position
        currentIndicator.transform.position = target.position;
        
        // Parent to the target to follow it
        currentIndicator.transform.SetParent(target);
        currentIndicator.SetActive(true);

        // If the target has a specific size, adjust the indicator
        Renderer targetRenderer = target.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            float targetSize = Mathf.Max(targetRenderer.bounds.size.x, targetRenderer.bounds.size.z);
            SelectionIndicator indicator = currentIndicator.GetComponent<SelectionIndicator>();
            if (indicator != null)
            {
                indicator.UpdateSize(targetSize * 0.6f); // Make the circle slightly smaller than the ship
            }
        }
    }

    public void HideSelection()
    {
        if (currentIndicator != null)
        {
            currentIndicator.SetActive(false);
            currentIndicator.transform.SetParent(null);
        }
    }
}
