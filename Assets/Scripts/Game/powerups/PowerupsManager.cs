using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.UI;
using UnityEngine.TextCore.Text;

namespace Fulbo.Game.Powerups
{
	public class PowerupsManager : MonoBehaviour
	{
		[SerializeField] float delayForSpawner;
		[SerializeField] Grabbable powerupGrabbable;
        [SerializeField] PostProcessingFX postProcessingFX;
        [SerializeField] List<PowerupSpawner> spawners;

		[SerializeField] Transform container;
		[Serializable]
		public class PowerupData
		{
			public Powerup.types type;
			public Powerup powerup;
			public Sprite image;
            public Sprite title;
            public FX.FXManager.types FX_use;
		}
		public PowerupData[] all;
		[SerializeField] PowerupUIButton powerupUIButton_team1;
        [SerializeField] PowerupUIButton powerupUIButton_team2;

        void Start()
		{
			UpdateTeamPowerup();
            Events.CheatThrowPowerups += CheatThrowPowerups;
            Events.LoseBall += LoseBall;
            powerupUIButton_team1 = Fulbo.UI.UIMain.Instance.powerupUIButton_team1;
            powerupUIButton_team2 = Fulbo.UI.UIMain.Instance.powerupUIButton_team2;
            Utils.RemoveAllChildsIn(container);
		}
        PowerupUIButton GetPowerupButtonByTeam(int teamID)
		{
			if (teamID == 1) return powerupUIButton_team1; else return powerupUIButton_team2;
        }
        private void OnDestroy()
		{
			Events.LoseBall -= LoseBall;
            Events.CheatThrowPowerups -= CheatThrowPowerups;
        }
        void CheatThrowPowerups()
        {
            foreach (PowerupSpawner s in spawners)
                if (s.IsAvailable())
                    s.AddPowerup(powerupGrabbable);
        }

        void UpdateTeamPowerup()
        {
			Data.Instance.matchData.SetPowerup(2);
		}
		void LoseBall(Character character)
		{
			ResetPowerBar(character.teamID);
		}
		public void Init(GameObject container)
		{
			delayForSpawner = Data.Instance.settings.GetSetting("powerups_spawner_delay");
			spawners = new List<PowerupSpawner>();
			foreach (PowerupSpawner powerupSpawner in container.GetComponentsInChildren<PowerupSpawner>())
			{
				powerupSpawner.Init();
				spawners.Add(powerupSpawner);
			}
			Utils.Shuffle(spawners);
			Loop();
		}
		void Loop()
		{
			Invoke("LoopForActivateSpawner", delayForSpawner);
		}
		void LoopForActivateSpawner()
		{
			Loop();

           // if (DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep <= 2) return;

            foreach (PowerupSpawner s in spawners)
				if (s.IsAvailable())
				{
					s.AddPowerup(powerupGrabbable);
					return;
				}
		}
        public bool IsPowerupActive()
        {
            return activePowerup;
        }
        public bool IsFilled() // is READY TO USE:
        {
            return powerupUIButton_team1.IsFilled();
        }
		//POWERUPS
		public bool CheckForPowerUp(int buttonID, Character character, Ball ball)
		{
            if (character == null) return false;
            if (character.type == Character.types.GOALKEEPER) return false;
			if (character.states.currentState.type == CharacterStates.types.FREEZE) return false;

			if (Data.Instance.mode == Data.modes.PARTYMODE)
			{
				if (buttonID != 3) return false;
			}
			else
			{
				if (buttonID != 4) return false;
			}

			if (GetPowerupButtonByTeam(character.teamID) == null) return false;
            if (!character.states.CanMove()) return false;
            //if (!character.IsMoving() && powerupUIButton.IsFilled()) // all ready to init powerup action!
            if (GetPowerupButtonByTeam(character.teamID).IsFilled()) // all ready to init powerup action!
            {
                Powerup.types type = character.powerupsManager.GetPowerupType();
                switch (type)
				{
                    case Powerup.types.BOMB:
                    case Powerup.types.SPEED:
                        GetPowerupButtonByTeam(character.teamID).InitCharging(character, type);
						return true;
                    case Powerup.types.SUPERKICK:
                        if (ball.character != null && ball.character == character)
						{
                            if (character.teamID == 2)
                                character.states.LookTo(1);
                            else
                                character.states.LookTo(-1);
                            GetPowerupButtonByTeam(character.teamID).InitCharging(character, type);
							return true;
						}
						return false;
				}
			}
			return false;
		}
		public bool IsCharging(int teamID)
        {
			return GetPowerupButtonByTeam(teamID).IsCharging();
		}
		public void ResetPowerBar(int teamID)
		{
			if (GetPowerupButtonByTeam(teamID) == null) return;
			if (GetPowerupButtonByTeam(teamID).IsCharging())
            {
                AudioManager.Instance.PlaySoundOneShot("ui", "");
                Events.OnPowerupCharging(false, GetPowerupButtonByTeam(teamID).character);
            }
		}
        bool activePowerup;
		public void Activate(System.Action OnDone, float delay)
		{
            Debug.Log("#Activate");
			StartCoroutine(ActivePowerup(OnDone, delay));
		}
		IEnumerator ActivePowerup(System.Action OnDone, float delay)
		{
            activePowerup = true;
            Events.OnPostProcessingFX(true);
            yield return new WaitForSeconds(delay);
			if (OnDone != null)
            {
                Events.OnPostProcessingFX(false);
                OnDone();
			}
		}
		public void DestroyPowerup(Powerup powerup)
        {
            activePowerup = false;
            StopAllCoroutines();
			UpdateTeamPowerup();
			powerup.character.powerupsManager.Desactivate();
			Destroy(powerup.gameObject);
		}
		public void SetPowerup(Character character, Powerup.types type)
		{
			Powerup powerup = GetPowerupData(type).powerup;
			Powerup newPowerup = Instantiate(powerup, container);
			newPowerup.Init(this, character);
		}
		public PowerupData GetPowerupData(Powerup.types type)
		{
			foreach (PowerupData pu in all)
			{
				if (pu.type == type)
					return pu;
			}
			return null;
		}
	}
}
