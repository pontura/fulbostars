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
        // Apagados hasta el saque inicial; se vuelven a apagar en cada gol hasta el saque del medio.
        bool isActive;

        void Start()
        {
            cameraInGame = GameManager.Instance.cameraInGame;
            Events.CharacterCatchBall += CharacterCatchBall;
            Events.OnGoal += OnGoal;
            Events.OnGameStatusChanged += OnGameStatusChanged;
            Events.OnBallKicked += OnBallKicked;
            Events.SwapCharacter += SwapCharacter;
            Loop();
            for (int a = 0; a < Data.Instance.matchData.players.Length; a++)
            {
                int control_id = Data.Instance.matchData.players[a];
                if (control_id > 0)
                {
                    SidePlayerSignalPlayer sp = Instantiate(sidePlayerSignalPlayer, transform);
                    sp.transform.localScale = Vector3.one;
                    sp.Init(a + 1, Color.white);
                    sp.SetOn(false);
                    all.Add(sp);
                }
            }

        }
        void OnDestroy()
        {
            Events.CharacterCatchBall -= CharacterCatchBall;
            Events.OnGoal -= OnGoal;
            Events.OnGameStatusChanged -= OnGameStatusChanged;
            Events.OnBallKicked -= OnBallKicked;
            Events.SwapCharacter -= SwapCharacter;
        }
        void OnGoal(int id, Character ch)
        {
            TurnAllOff();
        }
        void OnGameStatusChanged(GameManager.states state)
        {
            if (state == GameManager.states.GOAL || state == GameManager.states.GAMEOVER)
                TurnAllOff();
        }
        void OnBallKicked(CharacterStates.kickTypes kickType, float force, Character character)
        {
            if (GameManager.Instance.state == GameManager.states.PLAYING)
                isActive = true;
        }
        void TurnAllOff()
        {
            isActive = false;
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
                {
                    sp.character = character;
                    sp.SetColor(Data.Instance.clubsData.GetTeamColor(character.teamID));
                }
            }
        }
        void Loop()
        {
            Invoke("Loop", 0.05f);
            if (!isActive) return;
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
