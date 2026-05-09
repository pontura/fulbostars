using Fulbo.UI.EditTeam;
using UnityEngine;
using UnityEngine.UI;
namespace Fulbo.Dashoard
{
    public class DashboardHeaderData : MonoBehaviour
    {
        [SerializeField] Text results;
        [SerializeField] ClubShield clubShield_left;
        [SerializeField] ClubShield clubShield_right;
        private void OnEnable()
        {
            results.text = Data.Instance.matchData.score.y + "-" + Data.Instance.matchData.score.x;
            clubShield_left.Init(Data.Instance.clubsData.GetData(2));
            clubShield_right.Init(Data.Instance.clubsData.GetData(1));
        }
    }
}
