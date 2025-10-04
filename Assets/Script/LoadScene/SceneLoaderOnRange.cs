using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderOnRange : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    
    [Header("Scene Loading Settings")]
    public string sceneName;
    public float detectionRange = 10.0f;
    
    [Header("Loading Options")]
    public bool loadOnEnter = true;
    public bool oneTimeLoad = true;
    public float loadDelay = 0f;
    public bool useAsyncLoading = true;
    
    [Header("Loading Screen (Optional)")]
    public string loadingScreenSceneName = "LoadingScreen";
    public bool useLoadingScreen = false;
    
    [Header("Visual Feedback")]
    public bool showRangeAlways = true;
    public Color rangeColor = Color.cyan;
    
    private bool hasLoaded = false;
    private bool targetInRange = false;
    private bool isLoading = false;
    
    void Update()
    {
        if (target == null || hasLoaded && oneTimeLoad || isLoading)
            return;
            
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        bool wasInRange = targetInRange;
        targetInRange = distanceToTarget <= detectionRange;
        
        if (loadOnEnter && !wasInRange && targetInRange && !hasLoaded)
        {
            StartLoadSequence();
        }
        else if (!loadOnEnter && wasInRange && !targetInRange && !hasLoaded)
        {
            StartLoadSequence();
        }
    }
    
    void StartLoadSequence()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene name is not set!");
            return;
        }
        
        if (oneTimeLoad)
        {
            hasLoaded = true;
        }
        
        isLoading = true;
        StartCoroutine(LoadSceneRoutine());
    }
    
    IEnumerator LoadSceneRoutine()
    {
        // Wait for delay
        if (loadDelay > 0)
        {
            yield return new WaitForSeconds(loadDelay);
        }
        
        // Load loading screen first if specified
        if (useLoadingScreen && !string.IsNullOrEmpty(loadingScreenSceneName))
        {
            SceneManager.LoadScene(loadingScreenSceneName);
            yield return null;
        }
        
        // Load the target scene
        if (useAsyncLoading)
        {
            yield return StartCoroutine(LoadSceneAsync(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    
    IEnumerator LoadSceneAsync(string sceneToLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;
        
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // Check if the load has finished (0.9 means the scene is loaded but not activated)
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
    
    // Visualize detection range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = rangeColor;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        if (targetInRange && target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
    
    void OnDrawGizmos()
    {
        if (showRangeAlways)
        {
            Gizmos.color = new Color(rangeColor.r, rangeColor.g, rangeColor.b, 0.2f);
            Gizmos.DrawSphere(transform.position, detectionRange);
        }
    }
    
    // Public methods
    public void ManuallyLoadScene()
    {
        if (!isLoading && !hasLoaded)
        {
            StartLoadSequence();
        }
    }
    
    public void SetSceneToLoad(string newSceneName)
    {
        sceneName = newSceneName;
        hasLoaded = false;
        isLoading = false;
    }
}