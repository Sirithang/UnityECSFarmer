using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay
{

    public class PlayerSystem
    {
        //TODO : find a better way? the game will forever test if it's ready, no really happy with that
        public bool isReady = false;

        public float speed = 10;

        private Vector2 _position;

        private GameObject _playerGO;
        private Collider2D _collider;

        private RaycastHit2D[] _resultCache = new RaycastHit2D[16];

        public void Init()
        {
            _position = Vector2.zero;

            Addressables.Instantiate<GameObject>("player", new UnityEngine.ResourceManagement.InstantiationParameters(_position, Quaternion.identity, null)).Completed += op =>
            {
                _playerGO = op.Result;
                _collider = _playerGO.GetComponent<Collider2D>();
                isReady = true;
            };
        }

        public void Tick()
        {
            const float inflate = 0.009f;

            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            Vector2 move = new Vector2(horizontal, vertical);

            if (move.sqrMagnitude > 1.0f)
                move.Normalize();

            move *= Time.deltaTime * speed;

            Vector2 colliderSize = _collider.bounds.size;


            int count = 0;

            if (Mathf.Abs(move.y) > 0.01f)
            {
                count = Physics2D.BoxCastNonAlloc(_position + _collider.offset, _collider.bounds.size, 0, Vector2.up, _resultCache, move.y + inflate * Mathf.Sign(move.y), ~(1 << 8));
                for (int i = 0; i < count; ++i)
                {
                    if (_resultCache[i].distance < Mathf.Abs(move.y))
                        move.y = (_resultCache[i].distance - inflate) * Mathf.Sign(move.y);
                }

                _position.y += move.y;
            }

            if (Mathf.Abs(move.x) > 0.01f)
            {
                count = Physics2D.BoxCastNonAlloc(_position + _collider.offset, _collider.bounds.size, 0, Vector2.right, _resultCache, move.x + inflate * Mathf.Sign(move.x), ~(1 << 8));
                for (int i = 0; i < count; ++i)
                {
                    if (_resultCache[i].distance < Mathf.Abs(move.x))
                        move.x = (_resultCache[i].distance - inflate) * Mathf.Sign(move.x);
                }

                _position.x += move.x;
            }

            //---- sync with unity
            _playerGO.transform.position = _position;
        }
    }

}