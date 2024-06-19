using TMPro;
using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    /// <summary>
    /// Visualizes the current score of the player.
    /// </summary>
    public class ScoreVisualizer : MonoBehaviour
    {
        // ============== Serialized Fields ==============
        [SerializeField] TextMeshProUGUI textmesh;

        // =============== Private Fields ================
        ScoringSystem scoringSystem;



        // ===============================================
        // ============ UNITY EVENT FUNCTIONS ============
        // ===============================================
        private void Start()
        {
            scoringSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ScoringSystem>();
            scoringSystem.OnScoring += UpdateText;
        }

        private void OnDestroy()
        {
            scoringSystem.OnScoring -= UpdateText;
        }



        // ===============================================
        // =============== EVENT LISTENERS ===============
        // ===============================================
        private void UpdateText(int score)
        {
            textmesh.text = score.ToString();
        }
    }
}