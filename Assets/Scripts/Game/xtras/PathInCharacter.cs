using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class PathInCharacter : MonoBehaviour
    {
        List<Vector3> path;
        Vector3 offset;
        int id = 0;
        bool loop;
        Vector3 dest;
        Character character;
        bool isDone;
        System.Action OnFinish;

        public void Init(List<Vector3> path, bool loop, System.Action OnFinish, bool collithersOff = false)
        {
            this.OnFinish = OnFinish;
            this.loop = loop;
            this.path = path;
            SetNewPath();
            character = GetComponent<Character>();
        }
        public void SetSpeed(float speed)
        {
            character.speed = speed;
        }
        public void SetCollidersOFF()
        {
            character.SetCollidersOff(1000);
        }
        public void SetOffset(Vector3 offset)
        {
            this.offset = offset;
        }
        public void Reset()
        {
            id = 0;
        }
        private void Update()
        {
            if (Fulbo.Game.GameManager.Instance.state != Fulbo.Game.GameManager.states.PLAYING) return;
            if (isDone) return;
            float _x = 0;
            float _z = 0;
            float DiffX = dest.x - transform.position.x;
            float DiffZ = dest.z - transform.position.z;

            if (Mathf.Abs(DiffX) > 0.1f) { if (DiffX > 0) _x = 1; else _x = -1; }
            if (Mathf.Abs(DiffZ) > 0.1f) { if (DiffZ > 0) _z = 1; else _z = -1; }


            if (_x == 0 && _z == 0)
                SetNewPath();
            else
                character.MoveTo(_x, _z);
        }
        void SetNewPath()
        {            
            if (id > path.Count-1)
            {
                if (loop)
                {
                    Reset();
                    dest = path[id] + offset;
                }
                else
                {
                    isDone = true;
                    OnFinish();
                    return;
                }
            }
            dest = path[id] + offset;
            id++;
        }
    }
}
