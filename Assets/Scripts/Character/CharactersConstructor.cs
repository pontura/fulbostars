using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class CharactersConstructor : MonoBehaviour
    {
        [SerializeField] private Character character;
        [SerializeField] private Character goalKeeper;

        float size_x;
        float size_y;
        float _y = 0.54f;

        public void AddCharacters()
        {
            AddCharacters(1);
            AddCharacters(2);
        }
        public void AddCharacters(int teamID)
        {
            Transform container;
            if (teamID == 1)
                container = Fulbo.Game.GameManager.Instance.charactersManager.containerTeam1.transform;
            else
                container = Fulbo.Game.GameManager.Instance.charactersManager.containerTeam2.transform;

            CharactersPositions.PositionsData positionsData = Data.Instance.matchData.GetPositionsForTeam(teamID);
            int characterNum = 0;

            size_x = Fulbo.Game.GameManager.Instance.stadiumData.active.GetAssetBySelectedSize().size_x;
            size_y = Fulbo.Game.GameManager.Instance.stadiumData.active.GetAssetBySelectedSize().size_y;

            foreach (CharactersPositions.CharacterPositionData d in positionsData.posData)
            {
                characterNum++;

                // TO-DO
                if (teamID == 1 && characterNum > Data.Instance.matchData.totalCharacters_team1
                    || teamID == 2 && characterNum > Data.Instance.matchData.totalCharacters_team2)
                    return;
                Character thisCharacter;

                if (d.type == Character.types.GOALKEEPER)
                    thisCharacter = Instantiate(goalKeeper);
                else thisCharacter = Instantiate(character);

                thisCharacter.type = d.type;
                thisCharacter.transform.SetParent(container);
                thisCharacter.transform.localScale = Vector3.one;

                if (GameRecorder.Manager.Instance().state == GameRecorder.Manager.states.PLAYING)
                    SetPositionByRecord(thisCharacter, characterNum-1, teamID);
                else
                    SetPosition(d, teamID, thisCharacter);
                thisCharacter.SetCharacterPositionData(d);

            }
        }
        void SetPosition(CharactersPositions.CharacterPositionData d, int teamID, Character character)
        {
            float _x = d.pos[0] * size_x / 2;
            float _z = d.pos[1] * size_y / 2;

            if (teamID == 2)
                _x *= -1;

            SetPositionType(character, _z);
            character.transform.localPosition = new Vector3(_x, _y, _z);
        }
        void SetPositionByRecord(Character character, int characterNum, int teamID)
        {
            GameRecorder.ParsedMatchSettings parsedSettings = GameRecorder.Manager.Instance().timeLine.parsedSettings;
            Vector3 pos;
            if (teamID == 1)
                pos = parsedSettings.team1_positions[characterNum];
            else
                pos = parsedSettings.team2_positions[characterNum];
            float _x = pos.x;
            float _z = pos.z;
            character.transform.localPosition = new Vector3(_x, _y, _z);
            SetPositionType(character, _z);
        }
        void SetPositionType(Character character, float _z)
        {
            if (_z > 1)
                character.fieldPosition = Character.fieldPositions.UP;
            else if (_z < -1)
                character.fieldPosition = Character.fieldPositions.DOWN;
            else
                character.fieldPosition = Character.fieldPositions.CENTER;
        }
    }
}