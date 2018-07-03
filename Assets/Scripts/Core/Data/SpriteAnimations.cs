using Unity.Entities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
[CustomEditor(typeof(SpriteAnimations))]
public class SpriteAnimationsEditor : Editor
{
    SpriteAnimations _target;

    int _currentFoldout = -1;
    Vector2 _scrollPos;

    GUILayoutOption[] _previewOptions = new GUILayoutOption[2];

    private void OnEnable()
    {
        _target = target as SpriteAnimations;
        _scrollPos = Vector2.zero;

        _previewOptions[0] = GUILayout.Width(64);
        _previewOptions[1] = GUILayout.Height(64);
    }

    public override void OnInspectorGUI()
    {
        for(int  i = 0; i < _target.animations.Length; ++i)
        {
            if(EditorGUILayout.Foldout(_currentFoldout == i, _target.names[i]))
            {
                _currentFoldout = i;
                Undo.RecordObject(_target, "Changed Name");
                _target.names[i] = EditorGUILayout.DelayedTextField(_target.names[i]);

                Undo.RecordObject(_target, "Changed Time");
                _target.animations[i].time = EditorGUILayout.DelayedFloatField(_target.animations[i].time);

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                EditorGUILayout.BeginHorizontal();
                for(int j = 0; j < _target.animations[i].sprites.Length; ++j)
                {
                    Undo.RecordObject(_target, "Changed Sprite");
                    _target.animations[i].sprites[j] = EditorGUILayout.ObjectField(_target.animations[i].sprites[j], typeof(Sprite), false, _previewOptions) as Sprite;
                }

                Sprite newSprite = EditorGUILayout.ObjectField(null, typeof(Sprite), false, _previewOptions) as Sprite;
                if(newSprite != null)
                {
                    Undo.RecordObject(_target, "Added Sprite");
                    ArrayUtility.Add(ref _target.animations[i].sprites, newSprite);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }
            else if(_currentFoldout == i)
            {//got closed
                _currentFoldout = -1;
            }
        }

        if(GUILayout.Button("Add New Animation"))
        {
            var anim = new SpriteAnimations.Animation();
            anim.sprites = new Sprite[0];

            ArrayUtility.Add(ref _target.animations, anim);
            ArrayUtility.Add(ref _target.names, "");
        }
    }
}
#endif