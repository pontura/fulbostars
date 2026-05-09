using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class SidePlayerSignal : MonoBehaviour
    {
        [SerializeField] SidePlayerSignalPlayer sidePlayerSignalPlayer;
        public List<SidePlayerSignalPlayer> all;
        CameraInGame cameraInGame;
        public float offsetZ = 2;

        void Start()
        {
            cameraInGame = GameManager.Instance.cameraInGame;
            Events.CharacterCatchBall += CharacterCatchBall;
            Events.OnGoal += OnGoal;
            Events.SwapCharacter += SwapCharacter;
            Loop();
            for (int a = 0; a < Data.Instance.matchData.players.Length; a++)
            {
                int control_id = Data.Instance.matchData.players[a];
                if (control_id > 0)
                {
                    SidePlayerSignalPlayer sp = Instantiate(sidePlayerSignalPlayer, transform);
                    sp.transform.localScale = Vector3.one;
                    // Color c = Data.Instance.settings.GetTeamSettings(teamID).color;
                    sp.Init(a + 1, Color.white);
                    all.Add(sp);
                }
            }

        }
        void OnDestroy()
        {
            Events.CharacterCatchBall -= CharacterCatchBall;
            Events.OnGoal -= OnGoal;
            Events.SwapCharacter -= SwapCharacter;
        }
        void OnGoal(int id, Character ch)
        {
            foreach (SidePlayerSignalPlayer sp in all)
                sp.SetOn(false);
        }
        void CharacterCatchBall(Character character)
        {
            if (character.isBeingControlled)
                ChangeCharacter(character);
        }
        void SwapCharacter(Character character)
        {
            if (character.isBeingControlled)
                ChangeCharacter(character);
        }
        void ChangeCharacter(Character character)
        {
            foreach (SidePlayerSignalPlayer sp in all)
            {
                if (sp.control_id == character.control_id)
                    sp.character = character;
            }
        }
        void Loop()
        {
            Invoke("Loop", 0.05f);
            int dir = 1;
            foreach (SidePlayerSignalPlayer sp in all)
            {
                if (sp.character != null)
                {
                    Vector3 character_pos = cameraInGame.cam.WorldToScreenPoint(sp.character.transform.localPosition);
                    float fromCenter = character_pos.x - (Screen.width / 2);
                    if (Mathf.Abs(fromCenter) > Screen.width / 2)
                    {
                        if (fromCenter < 0) dir = -1;
                        SetTransform(sp, character_pos.y, dir);
                    }
                    else sp.SetOn(false);
                }
            }
        }
        void SetTransform(SidePlayerSignalPlayer sp, float _y, int direction)
        {
            sp.SetOn(true);
            Vector3 p = sp.transform.position;
            p.y = Mathf.Lerp(p.y, _y, 0.08f);
            sp.transform.position = p;
            sp.SetScale(direction);
        }
    }
}
