using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fulbo.Game.Xtras
{
    public class SwapperByEvent : MonoBehaviour
    {
        [SerializeField] EventSwapper[] events;
        [SerializeField] GameObject[] assets;

        [Serializable]
        public class EventSwapper
        {
            public eventTypes eventType;
            public bool initialStep;
            public float randomInitialDelay;
            public GameObject asset;
        }
        public enum eventTypes
        {
            IDLE,
            TUTORIAL_WIN,
            TUTORIAL_LOSE
        }

        void Start()
        {
            Events.OntutorialStepDone += OntutorialStepDone;
            foreach (EventSwapper es in events)
            {
                if (es.initialStep)
                {
                    PlayAnim(es);
                    return;
                }
            }
        }
        private void OnDestroy()
        {
            Events.OntutorialStepDone -= OntutorialStepDone;
        }
        private void Reset()
        {
            foreach (GameObject go in assets)
                go.SetActive(false);
        }
        void OntutorialStepDone(bool win)
        {
            foreach (EventSwapper eventSwapper in events)
            {
                if (eventSwapper.eventType == eventTypes.TUTORIAL_WIN && win)
                    PlayAnim(eventSwapper);
                else if (eventSwapper.eventType == eventTypes.TUTORIAL_LOSE && !win)
                    PlayAnim(eventSwapper);
            }
        }
        EventSwapper eventSwapper;
        void PlayAnim(EventSwapper eventSwapper)
        {
            this.eventSwapper = eventSwapper;
            Invoke("PlayDelayed", Utils.GetRandomFloatBetween(0, eventSwapper.randomInitialDelay));
        }
        void PlayDelayed()
        {
            Reset();
            eventSwapper.asset.SetActive(true);
        }
    }

}