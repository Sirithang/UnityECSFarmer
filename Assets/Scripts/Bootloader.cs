using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;

public class Bootloader : MonoBehaviour
{
    protected EntityManager entitiymanager;
    protected Entity playerEntity;

    private void Start()
    {
        //Loading Player;
        Addressables.Instantiate<GameObject>("player").Completed += LoadPlayer;


    }


    void LoadPlayer(IAsyncOperation<GameObject> op)
    {
        var ent = op.Result.GetComponent<GameObjectEntity>();

        playerEntity = ent.Entity;
        entitiymanager = ent.EntityManager;

        entitiymanager.AddComponent(playerEntity, typeof(Unity.Transforms.Position));
        entitiymanager.AddComponent(playerEntity, typeof(Farmer.Core.Velocity));
        entitiymanager.AddComponent(playerEntity, typeof(Farmer.Core.PlayerInput));
        entitiymanager.AddComponent(playerEntity, typeof(Unity.Transforms.TransformMatrix));

        //needed because we use "stock" sprite rendering, so need to sync our entity position to the transform of the Gameobject
        //linked to the entity. In a perfect futur world, we will have a SpriteRendererComponent to add to the entity to get rid of Gameobject entierly
        entitiymanager.AddComponent(playerEntity, typeof(Unity.Transforms.CopyTransformToGameObject));

        Addressables.LoadAsset<SpriteAnimations>("playerAnimation").Completed += op =>
        {
            entitiymanager.AddSharedComponentData(playerEntity, op.Result.GetAnimation("walk_d"));
        };
    }
}
