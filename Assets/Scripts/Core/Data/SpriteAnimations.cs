using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "spriteAnim", menuName = "Farmer/Sprite Animation")]
public class SpriteAnimations : ScriptableObject
{
    [System.Serializable]
    public struct Animation : ISharedComponentData
    {
        public Sprite[] sprites;
        public float time;
    }

    public Animation[] animations;
    public string[] names;

    public Animation GetAnimation(string name)
    {
        int idx = System.Array.IndexOf(names, name);

        if (idx == -1)
        {
            //return an empty animation
            Debug.LogError($"No Animation named {name}");
            return new Animation
            {
                sprites = new Sprite[0],
                time = 0
            };
        }
        else
        {
            return animations[idx];
        }

    }
}
