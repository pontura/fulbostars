using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fulbo.UI
{
    public class FlyingParticlesUI : MonoBehaviour
    {
        [SerializeField] float smoothIn  = 3;
        [SerializeField] float smoothOut = 1;
        [SerializeField] float range = 200;
        [SerializeField] float gap = 10f; // desfazaje
        [SerializeField] float duration1 = 1;
        [SerializeField] float duration2 = 1;
        [SerializeField] Transform container;
        [SerializeField] Animator bacgroundAnimator;
        [SerializeField] GameObject particlesDark;

        public List<ParticleData> particlesData;

        public enum types { ENERGY, COINS, LIFE, CARD, HARD, SHARDS }
        [Serializable] public class ParticleData
        {
            public types type;
            public FlyingParticle asset;
            public Transform target;
            public GameObject[] particlesBigReward;
        }

        Vector2 destTotal;
        void Start()
        {
            Events.OnFlyingParticles += OnFlyingParticles;
            Events.ShowParticlesDark += ShowParticlesDark;
        }
        void OnDestroy()
        {
            Events.OnFlyingParticles -= OnFlyingParticles;
            Events.ShowParticlesDark -= ShowParticlesDark;
        }
        void OnFlyingParticles(int qty, types type, Vector2 initialPos, float from, float plus)
        {
            bacgroundAnimator.gameObject.SetActive(true);
            bacgroundAnimator.Play("on",0,0);
            //float from = 0;

            //switch (type) {
            //    case types.COINS: from = DB.DBManager.Instance.DbUserData.data.score; break;
            //    case types.ENERGY: from = DB.DBManager.Instance.DbUserData.data.gameData.energyData.available; break;
            //    case types.HARD: from = DB.DBManager.Instance.DbUserData.data.hard_currency; break;
            //    case types.SHARDS: from = DB.DBManager.Instance.DbUserData.data.shards; break;
            //}

            print(type + "  OnFlyingParticles Energy init:  plus: " + plus + " from: "+ from);
            StartCoroutine(InitParticles(qty, type, initialPos, plus, from));
        }       
        ParticleData GetParticle(types type)
        {
            foreach (ParticleData pData in particlesData)
                if (pData.type == type)
                    return pData;
            return null;
        }
        IEnumerator InitParticles(int qty, types type, Vector2 initialPos, float plus, float from)
        {
            GameObject goContainer = Instantiate(new GameObject(), container);

            ParticleData pData = GetParticle(type);
            destTotal = pData.target.transform.position;

            GameObject bigReward = null;

            switch (type)
            {
                case types.HARD:
                    if(qty == 21)
                    {
                        bigReward = Instantiate(pData.particlesBigReward[1], transform);
                        AudioManager.Instance.PlaySoundOneShot("ui", "ui/hardCurrency/ui_diamond");
                    }
                    else
                    {
                        bigReward = Instantiate(pData.particlesBigReward[0], transform);
                        AudioManager.Instance.PlaySoundOneShot("ui", "ui/hardCurrency/ui_prize_diamond");
                    }
                    break;
                default:
                    if (pData.particlesBigReward.Length > 0)
                        bigReward = Instantiate(pData.particlesBigReward[0], transform);
                    AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_prize");
                    break;
            }
            if (bigReward != null)
            {
                bigReward.transform.position = initialPos;
                bigReward.gameObject.SetActive(true);
            }

            if (type == types.HARD && qty == 21)
                yield return new WaitForSeconds(2.3f);

            int num = 0;
            while (num < qty)// pueden desaparecer sin que terminen de aparecer todas!
            {
                GameObject gameO = Data.Instance.pool.Get(pData.asset.name);
                if (gameO != null)
                {
                    FlyingParticle go = gameO.GetComponent<FlyingParticle>();
                    go.transform.SetParent(goContainer.transform);
                    go.transform.localScale = Vector2.one;
                    go.id = num;
                    go.type = type;
                    go.totalParticles = qty;
                    go.from = from;
                    go.to = from + plus;
                    go.transform.position = initialPos;
                    go.gameObject.SetActive(false);

                    Vector2 _dest = new Vector2();
                    _dest.x = initialPos.x + UnityEngine.Random.Range(-range, range);
                    _dest.y = initialPos.y + UnityEngine.Random.Range(-range, range);

                    go.gameObject.SetActive(true);
                    StartCoroutine(go.Fly(duration1, duration2, smoothIn, smoothOut, _dest, destTotal, goContainer.transform, OnDone));
                    num++;
                    yield return new WaitForSeconds(gap / 100);
                }
            }
            if (bigReward != null)
            {
                yield return new WaitForSeconds(0.5f);
                GameObject.Destroy(bigReward);
            }
            yield return null;
        }
        void OnDone(Transform container, FlyingParticle fp)
        {
            if (container != null && fp != null) {
                float f = 1 - ((float)(fp.totalParticles - fp.id - 1) / (float)fp.totalParticles);
                //print(fp.type + "  percent: " + f + "  from: " + fp.from + "  to: " + fp.to + "  id: " + fp.id + "  totalParticles: " + fp.totalParticles);

                if (fp.id >= fp.totalParticles - 1) {
                    f = 1;
                    Events.OnFlyingPArrives(fp.type, 1, fp.from, fp.to);
                    Data.Instance.pool.Pool(fp.gameObject);
                    Utils.RemoveAllChildsIn(container);
                    Destroy(container.gameObject);
                    if (particlesDark.activeSelf)
                        particlesDark.SetActive(false);
                } else {
                    Events.OnFlyingPArrives(fp.type, f, fp.from, fp.to);
                    Data.Instance.pool.Pool(fp.gameObject);
                }
            }
        }

        public void ShowParticlesDark(bool enable) {
            particlesDark.SetActive(enable);
        }
    }
}
