using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ObjectEntitiesReferences : MonoBehaviour
{
    public static ObjectEntitiesReferences Instance = null;

    /* ENTITY REFERENCES NAMES */

    public const string PLAYER_ENTITY_NAME = "PlayerEntity";

    
    /* PROPERTIES */
    
    public Dictionary<string, Entity> EntityReferences { get; private set; } = new Dictionary<string, Entity>();

    public GameObject PlayerGO { get; private set; }
    public Entity PlayerEntity { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            Instance = null;
        }

        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(Instance);
    }

    /// <summary>
    /// Adds an entity to the <see cref="EntityReferences"/> dictionary by a string as key and the related entity as value.
    /// <br/> <paramref name="customEntityName"/> Can be used to assign a
    /// </summary>
    /// <param name="relatedEntity"></param>
    /// <param name="customEntityName"></param>
    private void AddToDictionary(string enityNameKey , Entity relatedEntityValue)
    {
        if (!EntityReferences.ContainsKey(enityNameKey))
            EntityReferences.Add(enityNameKey, relatedEntityValue);
    }

    public void SetPlayer(GameObject playerObject, Entity playerEntity)
    {
        PlayerGO = playerObject; 
        PlayerEntity = playerEntity;

        AddToDictionary(PLAYER_ENTITY_NAME, PlayerEntity);
    }
}
