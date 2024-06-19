using TMPro;
using Unity.Entities;
using UnityEngine;

public class LivesVisualizer : MonoBehaviour
{
    // ============== Serialized Fields ==============
    [SerializeField] TextMeshProUGUI textmesh;

    // =============== Private Fields ================
    ShipCollisionSystem shipCollisionSystem;



    // ===============================================
    // ============ UNITY EVENT FUNCTIONS ============
    // ===============================================
    private void Start()
    {
        shipCollisionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ShipCollisionSystem>();
        shipCollisionSystem.OnDeath += UpdateText;
    }

    private void OnDestroy()
    {
        shipCollisionSystem.OnDeath -= UpdateText;
    }



    // ===============================================
    // =============== EVENT LISTENERS ===============
    // ===============================================
    private void UpdateText(int lives)
    {
        textmesh.text = lives.ToString();
    }
}