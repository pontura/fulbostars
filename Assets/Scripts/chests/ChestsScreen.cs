using Fulbo;
using Fulbo.UI.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class ChestsScreen : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] Transform container;
        [SerializeField] ButtonCustom button;
        [SerializeField] GameObject tap_signal;
        [SerializeField] ChestPiece chestPiece;

        System.Action OnReady;
        int state = 0;
        float timeScale;

        MatchData.ResponseFromServer.ChestDataFromDB chestData;

        List<ChestPiece> all;

        [SerializeField] float speed = 10;

        int totalPieces = 3;
        [SerializeField] float separation = 170;
        int id;
        [SerializeField] float _y = 100;

        private void Start()
        {
            Close();
            Events.OpenChest += OpenChest;
            button.Init(0, Continue);
        }
        private void OnDestroy()
        {
            Events.OpenChest -= OpenChest;
        }

        public void OpenChest(int _id, System.Action OnReady, MatchData.ResponseFromServer.ChestDataFromDB chestData)
        {
            all = new List<ChestPiece>();
            button.gameObject.SetActive(false);
            tap_signal.SetActive(false);
            if (Time.timeScale > 0)
                this.timeScale = Time.timeScale;
           
            panel.SetActive(true);
            this.OnReady = OnReady;

            this.chestData = chestData;

            StartCoroutine(Init(_id));
            print("OpenChest" + _id);
        }
        IEnumerator Init(int _id)
        {
            this.id = 0;
            ChestsData.ChestData d = ChestsData.Instance.GetChest(_id);
            GameObject go = d.GetAsset();
            GameObject asset = null;
            Utils.RemoveAllChildsIn(container);

            if (go != null)
            {
                asset = Instantiate(go, container);
                asset.transform.localScale = new Vector2(2, 2);
            }

            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_open_pack");

            yield return new WaitForSecondsRealtime(0.25f);

            Animator anim = asset.GetComponent<Animator>();
            if (anim != null)
                anim.Play("claim");
            yield return new WaitForSecondsRealtime(0.6f);            

            AddPiece("hard", chestData.hard);
            yield return new WaitForSecondsRealtime(0.15f);
            AddPiece("energy", chestData.energy);
            yield return new WaitForSecondsRealtime(0.15f);
            AddPiece("shards", chestData.shard);

            yield return new WaitForSecondsRealtime(1);

            tap_signal.SetActive(true);
            button.gameObject.SetActive(true);
        }
        void AddPiece(string type, int qty)
        {
            print("AddPiece " + id);
            ChestPiece p = Instantiate(chestPiece, container);
            p.Init(type, qty);
            p.transform.localPosition = Vector2.zero;
            float totalWidth = separation * ((float)totalPieces-1);
            Vector2 dest = new Vector2((separation * id) - (totalWidth/2), _y);
            id++;
            StartCoroutine(MoveParticle(p.gameObject, dest));
            all.Add(p);
        }
        IEnumerator MoveParticle(GameObject go, Vector2 dest)
        {
            while (Vector2.Distance(go.transform.localPosition, dest) > 1)
            {
                go.transform.localPosition = Vector2.Lerp(go.transform.localPosition, dest, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
        void Continue(int id)
        {
            tap_signal.SetActive(false);
            button.gameObject.SetActive(false);
            StartCoroutine(ParticlesMomment());
        }
        IEnumerator ParticlesMomment()
        {
            int totalCoinsParticles = chestData.hard; if (totalCoinsParticles > 20) totalCoinsParticles = 20;
            int totalEnergyParticles = chestData.energy; if (totalEnergyParticles > 20) totalCoinsParticles = 20;
            int totalShardsParticles = chestData.shard; if (totalShardsParticles > 20) totalShardsParticles = 20;

            GameObject go;
            go = all[0].gameObject;
            Events.OnFlyingParticles(totalCoinsParticles, FlyingParticlesUI.types.HARD, go.transform.position, chestData.hard_from, chestData.hard);
            go.SetActive(false);

            yield return new WaitForSecondsRealtime(0.1f);

            go = all[1].gameObject;
            Events.OnFlyingParticles(totalEnergyParticles, FlyingParticlesUI.types.ENERGY, go.transform.position, chestData.energy_from, chestData.energy);
            go.SetActive(false);

            yield return new WaitForSecondsRealtime(0.1f);

            go = all[2].gameObject;
            Events.OnFlyingParticles(totalShardsParticles, FlyingParticlesUI.types.SHARDS, go.transform.position, chestData.shard_from, chestData.shard);
            go.SetActive(false);

            yield return new WaitForSecondsRealtime(2);

            Close();
            OnReady();
        }
        public void Close()
        {
            panel.SetActive(false);
        }
    }
}
