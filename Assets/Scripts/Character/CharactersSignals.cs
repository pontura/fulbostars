using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fulbo.Game
{
    public class CharactersSignals : MonoBehaviour
    {
        public CharacterSignal signal_to_add;
        public List<CharacterSignal> all;

        public void Add(Character character)
        {
            CharacterSignal s = Instantiate(signal_to_add);
            character.SetSignal(s);
            s.Init(Data.Instance.clubsData.GetData(character.teamID).GetColor(1), character.control_id);
            all.Add(s);
        }

        public void ChangeSignal(Character from, Character to)
        {
            to.SetSignal(from.characterSignal);
            from.characterSignal = null;
        }
        public void Remove()
        {
            int i = all.Count;
            while (i > 0)
            {
                CharacterSignal s = all[i - 1];
                all.Remove(s);
                Destroy(s.gameObject);
                i--;
            }
        }
    }

}