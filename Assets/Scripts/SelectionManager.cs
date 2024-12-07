using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }

    [Header("References")]
    public GameObject selectionIndicatorPrefab;

    private GameObject currentIndicator;
    private bool isInitialized = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        if (!isInitialized)
        {
            if (selectionIndicatorPrefab == null)
            {
                Debug.LogError("Selection indicator prefab is not assigned!");
                return;
            }

            isInitialized = true;
        }
    }

    public void ShowSelectionAt(Transform target)
    {
        if (!isInitialized || target == null) return;

        if (currentIndicator == null)
        {
            currentIndicator = Instantiate(selectionIndicatorPrefab);
        }

        currentIndicator.transform.position = target.position;
        currentIndicator.transform.SetParent(target);
        currentIndicator.SetActive(true);

        Renderer targetRenderer = target.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            float targetSize = Mathf.Max(targetRenderer.bounds.size.x, targetRenderer.bounds.size.z);
            SelectionIndicator indicator = currentIndicator.GetComponent<SelectionIndicator>();
            if (indicator != null)
            {
                indicator.UpdateSize(targetSize * 0.6f);
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

    void OnDestroy()
    {
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
        }
    }
}
