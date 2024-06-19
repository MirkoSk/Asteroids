using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    // ============== Serialized Fields ==============
    [SerializeField] float reloadDelayOnDeath = 3f;

    // =============== Private Fields ================
    ShipCollisionSystem shipCollisionSystem;
    float deathTimestamp;



    // ===============================================
    // ============ UNITY EVENT FUNCTIONS ============
    // ===============================================
    private void Start()
    {
        shipCollisionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ShipCollisionSystem>();
        shipCollisionSystem.OnDeath += ReloadScene;
    }

    private void Update()
    {
        if (deathTimestamp == 0f) return;

        if (Time.timeSinceLevelLoad - deathTimestamp >= reloadDelayOnDeath)
        {
            deathTimestamp = 0f;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnDestroy()
    {
        shipCollisionSystem.OnDeath -= ReloadScene;
    }



    // ===============================================
    // =============== EVENT LISTENERS ===============
    // ===============================================
    private void ReloadScene(int lives)
    {
        if (lives == 0)
        {
            deathTimestamp = Time.timeSinceLevelLoad;
        }
    }
}