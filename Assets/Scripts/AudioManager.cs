using Unity.Entities;
using UnityEngine;

/// <summary>
/// Simple AudioManager that handles all SFX in the game.
/// </summary>
[RequireComponent(typeof(AudioManager))]
public class AudioManager : MonoBehaviour
{
    // ============== Serialized Fields ==============
    [SerializeField] AudioClip explosionAsteroidSound;
    [SerializeField] AudioClip explosionShipSound;
    [SerializeField] AudioClip laserSound;
    [SerializeField] AudioSource thrustSource;

    // =============== Private Fields ================
    AudioSource audioSource;
    ShipCollisionSystem shipCollisionSystem;
    ProjectileHitDetectionSystem projectileHitDetectionSystem;
    ShootingSystem shootingSystem;
    ShipMovementSystem shipMovementSystem;



    // ===============================================
    // ============ UNITY EVENT FUNCTIONS ============
    // ===============================================
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) Debug.LogError("Please add an audio source to the AudioManager.", gameObject);

        shipCollisionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ShipCollisionSystem>();
        shipCollisionSystem.OnDeath += PlayExplosionShip;

        projectileHitDetectionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ProjectileHitDetectionSystem>();
        projectileHitDetectionSystem.OnHit += PlayExplosionAsteroid;

        shootingSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ShootingSystem>();
        shootingSystem.OnShoot += PlayLaser;

        shipMovementSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ShipMovementSystem>();
        shipMovementSystem.OnThrust += ToggleThrust;
    }

    private void OnDestroy()
    {
        shipCollisionSystem.OnDeath -= PlayExplosionShip;
        projectileHitDetectionSystem.OnHit -= PlayExplosionAsteroid;
        shootingSystem.OnShoot -= PlayLaser;
        shipMovementSystem.OnThrust -= ToggleThrust;
    }



    // ===============================================
    // =============== EVENT LISTENERS ===============
    // ===============================================
    private void PlayExplosionShip(int lives)
    {
        thrustSource.Stop();
        audioSource.PlayOneShot(explosionShipSound);
    }

    private void PlayExplosionAsteroid()
    {
        audioSource.PlayOneShot(explosionAsteroidSound);
    }

    private void PlayLaser()
    {
        audioSource.PlayOneShot(laserSound, 0.5f);
    }

    private void ToggleThrust(bool active)
    {
        if (active) thrustSource.Play();
        else thrustSource.Stop();
    }
}