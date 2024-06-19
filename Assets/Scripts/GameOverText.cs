using TMPro;
using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    /// <summary>
    /// Visualizes the game over state to the player.
    /// </summary>
    public class GameOverText : MonoBehaviour
    {
        // ============== Serialized Fields ==============
        [SerializeField] TextMeshProUGUI textMesh;

        // =============== Private Fields ================
        ShipCollisionSystem shipCollisionSystem;



        // ===============================================
        // ============ UNITY EVENT FUNCTIONS ============
        // ===============================================
        private void Start()
        {
            shipCollisionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ShipCollisionSystem>();
            shipCollisionSystem.OnDeath += ShowGameOver;

            textMesh.enabled = false;
        }

        private void OnDestroy()
        {
            shipCollisionSystem.OnDeath -= ShowGameOver;
        }



        // ===============================================
        // =============== EVENT LISTENERS ===============
        // ===============================================
        private void ShowGameOver(int lives)
        {
            if (lives == 0) textMesh.enabled = true;
        }
    }
}