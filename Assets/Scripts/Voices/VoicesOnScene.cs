using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.UI;
using Fulbo.Game;
using Fulbo.Game.Powerups;

namespace Fulbo.Voices
{
    public class VoicesOnScene : MonoBehaviour
    {
        [SerializeField]  AudioSource audioSource;
        [SerializeField] AudioSource audioSourceComentarios;
        public int characterGoalID;
        Character character;
        [SerializeField] VoicesManager voicesManager;

        [SerializeField] int priority = 0;

        Dictionary<string, int> usedTracks;

        float defaultVolume1;
        float defaultVolume2;

        public void SetMute()
        {
            if (!Data.Instance.settings.mainSettings.announcer_on)
            {
                audioSource.volume = 0;
                audioSourceComentarios.volume = 0;
            }
            else
            {
                audioSource.volume = defaultVolume1;
                audioSourceComentarios.volume = defaultVolume2;
            }
        }
        void Start()
        {
            
            defaultVolume1 = audioSource.volume;
            defaultVolume2 = audioSourceComentarios.volume;

            if (Data.Instance.langsManager.GetLang() == "en")
            {
                audioSource.pitch = 1.05f;
                audioSourceComentarios.pitch = 1.3f;
            }

            usedTracks = new Dictionary<string, int>();

            Events.CharacterCatchBallFrom += CharacterCatchBallFrom;
            Events.OnRelatorSay += OnRelatorSay;
            Events.CharacterCatchBall += CharacterCatchBall;
            Events.KickToGoal += KickToGoal;
            Events.OnBallKicked += OnBallKicked;
            Events.OnGoal += OnGoal;
            Events.OnIntroSound += OnIntroSound;
            Events.OnPenalty += OnPenalty;
            Events.OnPenaltyWaitingToKick += OnPenaltyWaitingToKick;
            Events.OnOutroSound += OnOutroSound;
            Events.OnBallHitCharacter += OnBallHitCharacter;
            Events.SayPalo += SayPalo;
            Events.SayCharacterName += SayCharacterName;
            Events.SayResults += SayResults;
            Events.Lujito += Lujito;
            Events.OnVoiceSay += OnVoiceSay;
            Events.OnRelatorSayRecorded += OnRelatorSayRecorded; // para cuando el partido es grabado:
            Events.OnPowerupIncrease += OnPowerupIncrease;
            Events.OnPowerupActivated += OnPowerupActivated;
            Events.GameInit += GameInit;
            Events.GameOver += GameOver;
        }
        void OnDestroy()
        {
            Events.OnRelatorSay -= OnRelatorSay;
            Events.CharacterCatchBall -= CharacterCatchBall;
            Events.CharacterCatchBallFrom -= CharacterCatchBallFrom;
            Events.OnBallKicked -= OnBallKicked;
            Events.KickToGoal -= KickToGoal;
            Events.OnGoal -= OnGoal;
            Events.OnIntroSound -= OnIntroSound;
            Events.OnPenalty -= OnPenalty;
            Events.OnPenaltyWaitingToKick -= OnPenaltyWaitingToKick;
            Events.OnOutroSound -= OnOutroSound;
            Events.OnBallHitCharacter -= OnBallHitCharacter;
            Events.SayPalo -= SayPalo;
            Events.SayCharacterName -= SayCharacterName;
            Events.SayResults -= SayResults;
            Events.Lujito -= Lujito;
            Events.OnVoiceSay -= OnVoiceSay;
            Events.OnPowerupIncrease -= OnPowerupIncrease;
            Events.OnPowerupActivated -= OnPowerupActivated;
            Events.GameInit -= GameInit;
            Events.GameOver -= GameOver;
        }
       
        void OnPowerupActivated( Character ch)
        {
            Powerup.types powerupType = ch.powerupsManager.GetPowerupType();
            List<AudioClip> arr = voicesManager.pubomb;
            switch (powerupType) {
                case Powerup.types.BOMB: arr = voicesManager.pubomb; break;
                case Powerup.types.SPEED: arr = voicesManager.purun; break;
                case Powerup.types.SUPERKICK: arr = voicesManager.pusupershot; break;
            }
            PlayAudios(new AudioClip[] { GetRandomAudioClip(arr) }, null, 15);
        }
        void OnPowerupIncrease(Character ch)
        {
            Powerup.types powerupType = ch.powerupsManager.GetPowerupType();
            List<AudioClip> arr = voicesManager.pubombactive;
            switch (powerupType)
            {
                case Powerup.types.BOMB: arr = voicesManager.pubombactive; break;
                case Powerup.types.SPEED: arr = voicesManager.purunactive; break;
                case Powerup.types.SUPERKICK: arr = voicesManager.pusupershotactive; break;
            }
            PlayAudios(new AudioClip[] { GetRandomAudioClip(arr) }, null, 15);
        }
        void Lujito()
        {
            //AudioManager.Instance.PlaySound("crowd", "_new/ambience/crowd_good", false, true);
            //AudioManager.Instance.PlaySpecificSound(Fulbo.Game.GameManager.Instance.stadiumData.active.crowd_good, "crowd", false, true);
            PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.lujitos) }, null, 10);            
        }
        void OnVoiceSay(string type, System.Action OnSaid)
        {
            print("OnVoiceSay " + type);

            List<AudioClip> audios = null;
            switch(type)
            {
                case "trainingintro":   audios = voicesManager.trainingintro; break;
                case "trainingok":      audios = voicesManager.trainingok; break;
                case "trainingstep1":   audios = voicesManager.trainingstep1; break;
                case "trainingstep2":   audios = voicesManager.trainingstep2; break;
                case "trainingstep3":   audios = voicesManager.trainingstep3; break;
                case "trainingstep4":   audios = voicesManager.trainingstep4; break;
                case "trainingstep5":   audios = voicesManager.trainingstep5; break;
                case "trainingstep6":   audios = voicesManager.trainingstep6; break;
                case "trainingstep7":   audios = voicesManager.trainingstep7; break;
                case "trainingendmatch": audios = voicesManager.trainingendmatch; break;

                case "cup10intro": audios = voicesManager.cup10intro; break;
                case "cup20intro": audios = voicesManager.cup20intro; break;
                case "cup30intro": audios = voicesManager.cup30intro; break;
                case "cup40intro": audios = voicesManager.cup40intro; break;
                case "cup50intro": audios = voicesManager.cup50intro; break;
                case "cup60intro": audios = voicesManager.cup60intro; break;
                case "cup70intro": audios = voicesManager.cup70intro; break;
                case "cup80intro": audios = voicesManager.cup80intro; break;
                case "cup90intro": audios = voicesManager.cup90intro; break;
                case "cup100intro": audios = voicesManager.cup100intro; break;

                case "cupwon": audios = voicesManager.cupwon; break;
                case "cuplose": audios = voicesManager.cuplose; break;
            }
            if(audios != null)
                Say(new AudioClip[] { GetRandomAudioClip(audios) }, OnSaid);
        }
        void OnRelatorSay(AudioClip ac)
        {
            PlayAudios(new AudioClip[] { ac });
        }
        void OnBallHitCharacter(Character character)
        {
            if (character.type == Character.types.GOALKEEPER)
            {
                //AudioManager.Instance.PlaySound("crowd", "_new/ambience/crowd_good", false, true);
                //AudioManager.Instance.PlaySpecificSound(Fulbo.Game.GameManager.Instance.stadiumData.active.crowd_good, "crowd", false, true);
                PlayAudios(new AudioClip[] {
                GetRandomAudioClip(voicesManager.lasacaconlospunios)
            }
              );
            }
            else
            {
                PlayAudios(new AudioClip[] {
                GetRandomAudioClip(voicesManager.pelotazo)
            }
             );
            }
        }
        bool CanBeSaid()
        {
            if (Data.Instance.newScene == "Tutorial")
                return false;
            return true;
        }
        void OnGameOverVoiceHappy()
        {
            CharactersData.CharacterData data = Data.Instance.matchData.GetWinnerCharacter();
            if (data == null) return;
            PlayAudios(new AudioClip[] {
            GetRandomAudioClip(voicesManager.gameover),
            GetRandomAudioClip(voicesManager.mirenlaalegriade),
            GetRandomAudioClip(data.audio_names) }, null, 10000
            );
        }
        void OnOutroSound()
        {
            PlayAudios(new AudioClip[]
            {
            GetRandomAudioClip(voicesManager.terminoelshow)
            }, OnGameOverVoiceHappy, 10000
          );
        }
        void OnPenaltyWaitingToKick(Character character, System.Action OnDone)
        {
            PlayAudios(new AudioClip[] {
            GetRandomAudioClip(voicesManager.penalty_lo_patea),
            GetRandomAudioClip(character.dataSources.audio_names) }, OnDone
            );
        }
        void SayResults()
        {
            Vector2 score = UIMain.Instance.GetScore();
            if (score == Vector2.zero)
            {
                if (Data.Instance.matchData.IsTutorial()) // inicia tutorial
                {
                    PlayAudios(new AudioClip[] {
                    //GetRandomAudioClip(voicesManager.pitaarrancaelshow),
                    GetRandomAudioClip(voicesManager.trainingkickoff)
                    }, null, 100);
                }
                else
                {
                    PlayAudios(new AudioClip[] {
                    GetRandomAudioClip(voicesManager.pitaarrancaelshow),
                    GetRandomAudioClip(voicesManager.init)
                    }, null, 100);
                }
            }
            else if (score.x < 14 && score.y < 14)
            {
                AudioClip score1, score2;
                if (score.x >= score.y)
                {
                    score1 = GetAudioForNum((int)score.x, false);
                    score2 = GetAudioForNum((int)score.y, true);
                }
                else
                {
                    score1 = GetAudioForNum((int)score.y, false);
                    score2 = GetAudioForNum((int)score.x, true);
                }
                if (score1 == null || score2 == null) return;
                PlayAudios(new AudioClip[] { score1, voicesManager.to[0], score2 }, null);
            }
        }
        AudioClip GetAudioForNum(int num, bool isSecondary)
        {
            string add = "";
            if (isSecondary) add = "-";
            foreach (AudioClip auc in voicesManager.numbers)
                if (auc.name == "numbers_" + num.ToString() + add)
                    return auc;
           return null;
        }
        void SayCharacterName(int characterID, bool isGoalkeeper)
        {
            CharactersData.CharacterData data = CharactersData.Instance.GetCharacterData(characterID, isGoalkeeper);
            AudioClip nameClip = GetRandomAudioClip(data.audio_names);
            PlayAudios(new AudioClip[] { nameClip });
        }
        void OnIntroSound(int id, Character character)
        {
            AudioClip nameClip = null;
            if (character != null)
                nameClip = GetRandomAudioClip(character.dataSources.audio_names);
            if (id == 1)
                PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.salen) });
            else if (id == 2)
                PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.salereferi), nameClip });
            else
                PlayAudios(new AudioClip[] { nameClip });

        }
        void OnGoal(int teamID, Character character)
        {

            this.characterGoalID = character.data.id;
            Reset();
           
            if (Fulbo.Game.GameManager.Instance.state == Fulbo.Game.GameManager.states.PENALTY && teamID == Data.Instance.matchData.penaltyGoalKeeperTeamID)
            {
                PlayAudios(new AudioClip[] {
                GetRandomAudioClip(voicesManager.penalty_ataja),
                GetRandomAudioClip(character.dataSources.audio_names),
                GetRandomAudioClip(voicesManager.penalty_ataja_comenta),
            }, SayComentarioGoal, 1000);
                return;
            }
            if (teamID == character.teamID)
            {
                if (character.dataSources.audio_goal.Count > 0)
                {
                    // agrega un gol default si tiene solo 1 personal
                    if(character.dataSources.audio_goal.Count == 1)
                        character.dataSources.audio_goal.Add(voicesManager.gol[UnityEngine.Random.Range(0,voicesManager.gol.Count-1)]);

                    PlayAudios(new AudioClip[] {
                    GetRandomAudioClip(character.dataSources.audio_goal)
                }, SayComentarioGoal, 1000);
                }
                else
                {
                    PlayAudios(new AudioClip[] {
                    GetRandomAudioClip(voicesManager.gol),
                    GetRandomAudioClip(character.dataSources.audio_names)
                }, SayComentarioGoal, 1000);
                }
            }
            else
            {
                PlayAudios(new AudioClip[] {
                GetRandomAudioClip(voicesManager.golencontra),
                GetRandomAudioClip(character.dataSources.audio_names)
            }, SayComentarioGoalEnContra, 1000);
            }
        }
        void SayComentarioGoal()
        {
            if (Fulbo.Game.GameManager.Instance.state == Fulbo.Game.GameManager.states.PENALTY)
                Events.OnRestartGame();
            else      
                PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.pidecomentario) }, Pide_Done, 100);
        }
        void Pide_Done()
        {
            AudioClip comentario_goal;
            comentario_goal = GetRandomAudioClip(character.dataSources.comments_goal);
            if (comentario_goal == null)
                comentario_goal = GetRandomAudioClip(voicesManager.comments);
            PlayAudiosComentarista(new AudioClip[] { comentario_goal }, SayGoalEnd);
        }
        void SayComentarioGoalEnContra()
        {
            //AudioManager.Instance.PlaySound("crowd", "_new/ambience/crowd_bad", false, true);
            AudioManager.Instance.PlayCrowd(Fulbo.Game.GameManager.Instance.stadiumData.active.crowd_bad);
            AudioClip comentario_goal;
            comentario_goal = GetRandomAudioClip(character.dataSources.comments_goal);
            if (comentario_goal == null)
                comentario_goal = GetRandomAudioClip(voicesManager.comments);
            PlayAudiosComentarista(new AudioClip[] { GetRandomAudioClip(voicesManager.comments) }, SayGoalEnd);
        }
        void SayGoalEnd()
        {
            PlayAudios(new AudioClip[] {
                GetRandomAudioClip(voicesManager.respondecomentario)
            }, OnGoalDone, 100);
        }
        void OnGoalDone()
        {
            Events.OnGoalDone();
        }
        private void Reset()
        {
            genericState = GenericStates.IDLE;
        }
        void OnBallKicked(CharacterStates.kickTypes kickType, float forceForce, Character character)
        {
            Reset();
            switch (kickType)
            {
                case CharacterStates.kickTypes.DESPEJE_GOALKEEPER:
                    //AudioManager.Instance.PlaySound("crowd", "_new/ambience/crowd_good", false, true);
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.lasacaconlospunios) });
                    break;
                case CharacterStates.kickTypes.CHILENA:
                    //AudioManager.Instance.PlaySound("crowd", "_new/ambience/crowd_good", false, true);
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.chilena) }, null, 10);
                    break;
                case CharacterStates.kickTypes.HARD:
                    if (character != null && character.type == Character.types.GOALKEEPER) {
                        AudioClip characterName = GetRandomAudioClip(character.dataSources.audio_names);
                        PlayAudios(new AudioClip[] { characterName, GetRandomAudioClip(voicesManager.clearence) });
                    } else {
                        //AudioManager.Instance.PlaySound("crowd", "_new/ambience/crowd_good", false, true);
                        PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.pateaalarco) });
                    }
                    break;
                case CharacterStates.kickTypes.BALOON:
                    if (character != null && character.type == Character.types.GOALKEEPER)
                    {
                        AudioClip characterName = GetRandomAudioClip(character.dataSources.audio_names);
                        PlayAudios(new AudioClip[] { characterName, GetRandomAudioClip(voicesManager.volea) });
                    }
                    else
                        PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.globito) });
                    break;
                case CharacterStates.kickTypes.SOFT:
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.pasecorto) });
                    break;
                case CharacterStates.kickTypes.HEAD:
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.cabezaso) });
                    break;
                case CharacterStates.kickTypes.KICK_TO_GOAL:
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.pateaalarco) });
                    break;
                case CharacterStates.kickTypes.CENTRO:
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.centro) });
                    break;
                case CharacterStates.kickTypes.BAD_KICK:
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.foul) });
                    break;
            }
        }
        void KickToGoal()
        {
            Reset();
            PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.volea) });
        }
        int sigueID;
        void SaySigue()
        {
            if (character != null)
            {
                if (character.type == Character.types.GOALKEEPER)
                {
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.arqueroespera) });
                }
                else
                {
                    int rand = UnityEngine.Random.Range(0, 10);
                    sigueID++;
                    AudioClip characterName = GetRandomAudioClip(character.dataSources.audio_names);
                    if (sigueID == 1)
                    {
                        if(rand <5)
                            PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.pelotade), characterName });
                        else
                            PlayAudios(new AudioClip[] { characterName });
                    }
                    else if (rand < 4)
                    {
                        PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.generic) }, null, 1);
                    }
                    else if (rand < 8)
                        PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.sigueconlapelota), characterName });
                }
            }
        }
        void CharacterCatchBall(Character character)
        {
            genericState = GenericStates.CONTINUE_WITH_BALL;
            if (character.dataSources.audio_names != null && character.dataSources.audio_names.Count > 0)
            {
                AudioClip characterName = GetRandomAudioClip(character.dataSources.audio_names);
                if (character.type == Character.types.GOALKEEPER)
                {
                    int rand = UnityEngine.Random.Range(0, 10);
                    if (rand > 5)
                        PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.ataja), characterName });
                    else
                        PlayAudios(new AudioClip[] { characterName, GetRandomAudioClip(voicesManager.atajavuelo) });
                    return;
                }
            }
        }
        void CharacterCatchBallFrom(Character character, Character from)
        {
            Reset();
            int rand = UnityEngine.Random.Range(0, 10);
            this.character = character;

            genericState = GenericStates.CONTINUE_WITH_BALL;

            if (character.dataSources.audio_names != null && character.dataSources.audio_names.Count > 0)
            {
                AudioClip characterName = GetRandomAudioClip(character.dataSources.audio_names);         
               
                if (character.type == Character.types.GOALKEEPER)
                {
                    if (rand > 3)
                        PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.ataja), characterName });
                    else
                        PlayAudios(new AudioClip[] { characterName });
                    return;
                }
                else if (from != null)
                {
                    if (character.states.currentState.type == CharacterStates.types.DASH)
                        PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.foul) }, null, 5); //hitted:
                    else
                    {
                        if (rand > 5)
                            PlayAudios(new AudioClip[] { characterName, GetRandomAudioClip(voicesManager.stoletheball) });
                        else
                            PlayAudios(new AudioClip[] { characterName });
                    }
                }
                else if(rand>7)
                    PlayAudios(new AudioClip[] { characterName, GetRandomAudioClip(voicesManager.catchtheball) });
                else
                    PlayAudios(new AudioClip[] { characterName });

            }
            else
            {
                Debug.Log("No grabaron audio para el nombre de: " + character.data.id);
            }
        }
        AudioClip GetRandomAudioClip(List<AudioClip> audioClips)
        {
            if (audioClips.Count == 0)
                return null;

            string keyValue = audioClips[0].name.ToString();
            int id = GetValueFor(keyValue, audioClips);

           //  Debug.Log(id + "____________keyValue " + keyValue + " Dic length:_ " + usedTracks.Count + " id_ " + id);
            return audioClips[id];
        }
        int GetValueFor(string keyValue, List<AudioClip> audioClips)
        {
            foreach (KeyValuePair<string, int> utrack in usedTracks)
            {
                if (utrack.Key == keyValue)
                {
                    int value = utrack.Value;
                    //  print(keyValue + "  value: " + value + " length: " + audioClips.Count + "    audioClips.Length: " + audioClips.Count);
                    value++;
                    if (value >= audioClips.Count)
                        value = 0;
                    usedTracks[keyValue] = value;
                    return value;
                }
            }
            if (audioClips.Count > 1)
            {
               //  print("::::::::::::::::::::::::::::::::::: shuffle " + keyValue);
                Utils.Shuffle(audioClips);
            }
            usedTracks.Add(keyValue, 0);
            return 0;
        }
        void PlayAudios(AudioClip[] audioClips, System.Action _OnDone = null, int _priority = 0)
        {
            if (!CanBeSaid()) return;
            if(_priority < priority)
                return;
            StopAllCoroutines();
            priority = _priority;
            StartCoroutine(WaitForSound(audioClips, _OnDone, audioSource));
        }
        void Say(AudioClip[] audioClips, System.Action _OnDone = null)
        {
            StopAllCoroutines();
            priority = 20000;
            StartCoroutine(WaitForSound(audioClips, _OnDone, audioSource));
        }
        void OnResetPriority()
        {
            priority = 0;
        }
        void PlayAudiosComentarista(AudioClip[] audioClips, System.Action OnDone = null)
        {
            StartCoroutine(WaitForSound(audioClips, OnDone, audioSourceComentarios));
        }
        IEnumerator WaitForSound(AudioClip[] audioClips, System.Action _OnDone, AudioSource aSource)
        {
            foreach (AudioClip audioClip in audioClips)
            {
                aSource.clip = audioClip;
                aSource.Play();

                if (GameManager.Instance != null && GameManager.Instance.gameRecorder != null && GameManager.Instance.gameRecorder.state == GameRecorder.Manager.states.RECORDING)
                    GameManager.Instance.gameRecorder.KeyframeRecorder.RecordVoice(audioClip.name);

                yield return new WaitUntil(() => aSource.isPlaying == false);
            }
            priority = 0;
            if (_OnDone != null)
                _OnDone();
        }
        void OnPenalty(Character character)
        {
            //AudioManager.Instance.PlaySound("crowd", "_new/ambience/crowd_bad", false, true);
            AudioManager.Instance.PlayCrowd(Fulbo.Game.GameManager.Instance.stadiumData.active.crowd_chance);
            AudioClip characterName = GetRandomAudioClip(character.dataSources.audio_names);
            PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.penalty), characterName }, OnPenalContinue);
        }
        void OnPenalContinue()
        {
            Data.Instance.LoadLevel("Penalty");
        }
        void SayPalo(int id)
        {
            //AudioManager.Instance.PlaySound("crowd", "_new/ambience/crowd_bad", false, true);
            if (Fulbo.Game.GameManager.Instance != null)
                AudioManager.Instance.PlayCrowd(Fulbo.Game.GameManager.Instance.stadiumData.active.crowd_chance);
            if (id == 0)
                PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.travesanio) }, null, 10);
            else
                PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.palo) }, null, 10);
        }



        void OnRelatorSayRecorded(string value)
        {
            string[] arr = value.Split("_"[0]);
            if (arr.Length <= 1) return;
            List<AudioClip> clips = voicesManager.GetAudioClipsFor(arr[0]);
            if (clips == null || clips.Count < 2 || arr[1].Length>2) return;
            int clipNum;
            if (int.TryParse(arr[1], out clipNum) != false)
            {
                if (clips.Count < clipNum - 1)
                {
                    audioSource.clip = clips[clipNum];
                    audioSource.Play();
                }
            }
        }




        bool playing;
        Coroutine genericVoices;
        GenericStates genericState;
        enum GenericStates
        {
            IDLE,
            CONTINUE_WITH_BALL
        }
        void GameInit()
        {
            genericState = GenericStates.IDLE;
            playing = true;
            Invoke("LoopForAudios", UnityEngine.Random.Range(6, 10));
        }
        void LoopForAudios()
        {
            if (!playing) return;
            print("GENERIC " + genericState);
            if (GameManager.Instance.state == GameManager.states.GOAL) { }
            else if (genericState == GenericStates.CONTINUE_WITH_BALL && UnityEngine.Random.Range(0, 10) < 4)
                SaySigue();
            else
            {
                PlayAudios(new AudioClip[] { GetRandomAudioClip(voicesManager.generic) }, null, 10);
            }

            Invoke("LoopForAudios", UnityEngine.Random.Range(4, 13));
        }
        void GameOver()
        {
            playing = false;
        }
    }
}