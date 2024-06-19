using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class LivesVisualizer : MonoBehaviour
{
    // ============== Public Variables ===============

    // ============== Serialized Fields ==============
    [SerializeField] TextMeshProUGUI textmesh;

    // =============== Private Fields ================
    ShipCollisionSystem shipCollisionSystem;
    // ============== Public Properties ==============



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


    // ===============================================
    // =============== CLASS FUNCTIONS ===============
    // ===============================================
}