using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Penalty
{
    public class PenaltyCharactersManager : MonoBehaviour
    {
        [SerializeField] ProgressBar progressBar;

        public positions position;
        public enum positions
        {
            CENTER,
            LEFT,
            RIGHT
        }

        Character character;
        Character goalkeeper;
        Ball ball;

        [SerializeField] float combaForce;
        [SerializeField] float limitJump_X;
        [SerializeField] float jumpTO;
        [SerializeField] float characterSpeed;
        [SerializeField] float force;
        [SerializeField] float goalkeeperSpeed;
        [SerializeField] float penalty_goalkeeper_reaction;

        CharacterBillboard characterBillboard;
        CharacterBillboard goalkeeperBillboard;
        [SerializeField] Animation camAnim;
        [SerializeField] Transform target;
        [SerializeField] Penalty.PenaltyGolkeeperCollider collider;
        [SerializeField] Penalty.PenaltyKick penaltyKick;
        System.Action OnKicked;

        private void Start()
        {
            combaForce =        Data.Instance.settings.GetSetting("penalty_combaForce");
            limitJump_X =       Data.Instance.settings.GetSetting("penalty_limitJump_X");
            characterSpeed =    Data.Instance.settings.GetSetting("penalty_characterSpeed");
            force =             Data.Instance.settings.GetSetting("penalty_force");
            goalkeeperSpeed =   Data.Instance.settings.GetSetting("penalty_goalkeeperSpeed");
            penalty_goalkeeper_reaction = Data.Instance.settings.GetSetting("penalty_goalkeeper_reaction");

            progressBar.SetOff();
            Events.OnPenaltyGoal += OnPenaltyGoal;
        }
        private void OnDestroy()
        {
            Events.OnPenaltyGoal -= OnPenaltyGoal;
        }
        public void Init()
        {
            progressBar.Init(0.5f, null);

            foreach (Character ch in GetComponentsInChildren<Character>())
            {
                if (ch.type == Character.types.GOALKEEPER)
                    goalkeeper = ch;
                else
                    character = ch;
            }
            ball = GetComponentInChildren<Ball>();

            characterBillboard = character.GetComponentInChildren<CharacterBillboard>();
            goalkeeperBillboard = goalkeeper.GetComponentInChildren<CharacterBillboard>();
            goalkeeperBillboard.transform.localScale = new Vector3(0.4f, 0.5f, 0.5f);
            collider.Init(this, goalkeeper);
        }
        public void OnKick(Vector3 targetPos, System.Action OnKicked)
        {
            this.OnKicked = OnKicked;
            ball.transform.LookAt(targetPos);
            StartCoroutine(KickOn());
        }
        IEnumerator KickOn()
        {
            progressBar.Stop();
            camAnim.Play();
            characterBillboard.SetAnim("run");
            goalkeeperBillboard.SetAnim("alert");
            Vector3 chPos = character.transform.position;
            while (character.transform.position.x < -0.25f)
            {
                chPos.x += characterSpeed * Time.deltaTime;
                character.transform.position = chPos;
                yield return new WaitForEndOfFrame();
            }
            OnKicked();
            progressBar.SetOff();
            characterBillboard.SetAnim("kick_power");
            float realForce =  force * ((progressBar.GetValue() + 1) * 2f);
            print("real force" + realForce);
            ball.rb.AddForce((ball.transform.forward * realForce) + Vector3.up*100);
            CalculateGoalkeeperJump();
            last_target_posistion = target.transform.position;
            kicked = true;
        }
        bool kicked;
        Vector2 last_target_posistion;
        void Update()
        {
            if (penaltyKick.state == PenaltyKick.states.KICK)
            {
                UpdateJumpToOnAir();
                AddComba();
            }
        }
        void AddComba()
        {
            float _x = 0;
            if (target.position.x < last_target_posistion.x) _x = -1;
            else
            if (target.position.x > last_target_posistion.x) _x = 1;
            last_target_posistion = target.position;

            if (_x != 0)
            {
                ball.rb.AddForce(Vector3.right * _x * combaForce * Time.deltaTime);               
            }
            jumpTO += _x * penalty_goalkeeper_reaction * Time.deltaTime;
            print("comba" + _x + " jumpto: " + jumpTO);
        }
        void CalculateGoalkeeperJump()
        {
            ForceGKToAvoidGoal();
            StartCoroutine(GoalKeeperJump());
        }
        void ForceGKToAvoidGoal()
        {
            position = GetTargetPos();
            SetJumpTo();
            GoalKeeperJump();
        }
        void SetJumpTo()
        {
            jumpTO = target.transform.position.x * 1.2f;
            SetJumpToLimits();
        }
        void SetJumpToLimits()
        {
            if (jumpTO < -limitJump_X) jumpTO = -limitJump_X;
            if (jumpTO > limitJump_X) jumpTO = limitJump_X;
        }
        void UpdateJumpToOnAir()
        {
            //if (ball.transform.position.x < goalkeeper.transform.localPosition.x)
            //    jumpTO -= penalty_goalkeeper_reaction;
            //else if (ball.transform.position.x > goalkeeper.transform.localPosition.x)
            //    jumpTO += penalty_goalkeeper_reaction;  
        }
        positions GetTargetPos()
        {
            float zone = 1.1f;
            if (target.position.x < zone && target.position.x > -zone)
                return positions.CENTER;
            else if (target.position.x < zone)
                return positions.LEFT;
            else
                return positions.RIGHT;
        }
        IEnumerator GoalKeeperJump()
        {
            Vector3 chPos = goalkeeper.transform.position;
            goalkeeperBillboard.SetAnim("jump2");

            if (goalkeeper.transform.position.x < jumpTO)
                goalkeeperBillboard.LookTo(false);
            else
                goalkeeperBillboard.LookTo(true);
            float timer = 0.8f;
            while (timer>0)
            {
                timer -= Time.deltaTime;
                SetJumpToLimits();
                if (Mathf.Abs(goalkeeper.transform.position.x - jumpTO)<0.5f)
                {
                    //nada:
                }
                else if (goalkeeper.transform.position.x < jumpTO)
                {
                    chPos.x += goalkeeperSpeed * Time.deltaTime;
                }
                else
                {
                    chPos.x -= goalkeeperSpeed * Time.deltaTime;
                }
                goalkeeper.transform.position = Vector3.Lerp(goalkeeper.transform.position, chPos, 0.1f);
                yield return new WaitForEndOfFrame();
            }
        }
        public void CatchBall()
        {
            penaltyKick.Catched();
        }
        void OnPenaltyGoal()
        {
            Events.OnGoal(character.teamID, character);
        }
    }
}
