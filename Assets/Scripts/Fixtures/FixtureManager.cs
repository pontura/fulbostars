using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Fixture
{
    public class FixtureManager : MonoBehaviour
    {
        public List<int> teamsSelected;

        public void Init()
        {
            teamsSelected = new List<int>();
        }
    }
}
