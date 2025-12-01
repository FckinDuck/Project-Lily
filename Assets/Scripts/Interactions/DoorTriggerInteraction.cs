using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractBased
{
    [Header("Spawn To")]
    [SerializeField] private DoorToSpawnAt DoorToSpawnTo;
    [SerializeField] private SceneField _sceneToLoad;
 
       public enum DoorToSpawnAt
    {
        None,One,Two,Three,Four,Five,Six,
    }
    [Space(10f)]
    [Header("This Door")]
    public DoorToSpawnAt CurrentDoorPosition;

    public override void Interact()
    {
        SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, DoorToSpawnTo);
    }

}