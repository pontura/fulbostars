using UnityEngine;
namespace Fulbo.Game
{
    public class PhotoCamera : MonoBehaviour
    {
        [SerializeField] float offsetX = 0.9f;
        [SerializeField] float cam_size = 2;
        [SerializeField] Vector3 offset = new Vector3(0, 3f, -8);
        [SerializeField] Transform target;
        Camera cam;

        void Start()
        {
            cam = GetComponent<Camera>();
            Events.CharacterCatchBall += CharacterCatchBall;
            Events.TakePhoto += TakePhoto;
           // gameObject.SetActive(false);
        }
        void OnDestroy()
        {
            Events.CharacterCatchBall -= CharacterCatchBall;
            Events.TakePhoto += TakePhoto;
        }
        void CharacterCatchBall(Character ch)
        {
            TakePhoto(ch, Vector2.zero);
        }
        void TakePhoto(Character character, Vector3 _offset)
        {
            cam.orthographicSize = cam_size;
            target = character.transform;
            gameObject.SetActive(true);
            Vector3 pos = target.position;
            pos.x *= offsetX;
            transform.position = pos + offset + _offset;
            transform.localEulerAngles = new Vector3(20, target.position.x * 15 / 20, target.position.x * 5 / 20);
        }
    }
}