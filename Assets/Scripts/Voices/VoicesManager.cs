using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fulbo.Voices
{
    public class VoicesManager : MonoBehaviour
    {
        public List<AudioClip> init;
        public List<AudioClip> travesanio;
        public List<AudioClip> palo;
        public List<AudioClip> lujitos;

        public List<AudioClip> numbers;
        public List<AudioClip> to;

        public List<AudioClip> salen;
        public List<AudioClip> salereferi;

        public List<AudioClip> penalty;
        public List<AudioClip> penalty_lo_patea;
        public List<AudioClip> penalty_ataja;
        public List<AudioClip> penalty_ataja_comenta;

        public List<AudioClip> pitaarrancaelshow;
        public List<AudioClip> comments;
        public List<AudioClip> ladomina;
        public List<AudioClip> pelotade;
        public List<AudioClip> sigueconlapelota;
        public List<AudioClip> ataja;
        public List<AudioClip> atajavuelo;
        public List<AudioClip> lasacaconlospunios;
        public List<AudioClip> pelotazo;
        public List<AudioClip> pateaalarco;
        public List<AudioClip> chilena;
        public List<AudioClip> volea;
        public List<AudioClip> clearence; // GK kick Hard
        public List<AudioClip> globito;
        public List<AudioClip> pasecorto;
        public List<AudioClip> centro;
        public List<AudioClip> cabezaso;
        public List<AudioClip> pita;
        public List<AudioClip> arqueroespera;
        public List<AudioClip> gol;
        public List<AudioClip> golencontra;
        public List<AudioClip> pidecomentario;
        public List<AudioClip> respondecomentario;
        public List<AudioClip> mirareloj;
        public List<AudioClip> terminoelshow;
        public List<AudioClip> stoletheball;
        public List<AudioClip> catchtheball;
        public List<AudioClip> gameover;
        public List<AudioClip> mirenlaalegriade;

        public List<AudioClip> trainingintro;
        public List<AudioClip> trainingok;
        public List<AudioClip> trainingstep1;
        public List<AudioClip> trainingstep2;
        public List<AudioClip> trainingstep3;
        public List<AudioClip> trainingstep4;
        public List<AudioClip> trainingstep5;
        public List<AudioClip> trainingstep6;
        public List<AudioClip> trainingstep7;

        public List<AudioClip> trainingkickoff;
        public List<AudioClip> trainingendmatch;

        public List<AudioClip> foul;
        public List<AudioClip> generic;

        public List<AudioClip> pusupershot;
        public List<AudioClip> pusupershotactive;
        public List<AudioClip> pubomb;
        public List<AudioClip> pubombactive;
        public List<AudioClip> purun;
        public List<AudioClip> purunactive;

        public List<AudioClip> cupwon;
        public List<AudioClip> cuplose;

        public List<AudioClip> cup10intro;
        public List<AudioClip> cup20intro;
        public List<AudioClip> cup30intro;
        public List<AudioClip> cup40intro;
        public List<AudioClip> cup50intro;
        public List<AudioClip> cup60intro;
        public List<AudioClip> cup70intro;
        public List<AudioClip> cup80intro;
        public List<AudioClip> cup90intro;
        public List<AudioClip> cup100intro;

        public int characterGoalID;

        public void Init(System.Action OnReady) // By DATA:
        {
            Debug.Log("init Voices");
            string lang = Data.Instance.langsManager.GetLang();
            AssetsBundle.AssetsBundleManager assetsBundleManager = AssetsBundle.AssetsBundleManager.Instance;
            string[] all = assetsBundleManager.assetsBundleLoader.bundles[lang + "/voices.1_100"].GetAllAssetNames();
            string assetBundle = lang + "/voices.1_100";
            string folderForAll = "assets/voices/" + lang + "/";
            foreach (string fileName in all)
            {
                string[] arr = fileName.Split("_"[0]);
                if(arr.Length>1)
                {
                    string folder = arr[0];
                    AudioClip asset = assetsBundleManager.assetsBundleLoader.GetAssetAsAudioClip(assetBundle, fileName);
                    asset.LoadAudioData();
                    GetAudioClipsFor(folder).Add(asset);
                }
            }
            OnReady();
        }
        public List<AudioClip> GetAudioClipsFor(string folder)
        {
            string[] arr = folder.Split("/"[0]);
            string s = arr[arr.Length - 1];
            switch (s)
            {
                case "init": return init;
                case "arqueroespera": return arqueroespera;
                case "ataja": return ataja;
                case "atajavuelo": return atajavuelo;
                case "cabezaso": return cabezaso;
                case "centro": return centro;
                case "chilena": return chilena;
                case "comments": return comments;
                case "globito": return globito;
                case "gol": return gol;
                case "golencontra": return golencontra;
                case "ladomina": return ladomina;
                case "lasacaconlospunios": return lasacaconlospunios;
                case "mirareloj": return mirareloj;
                case "mirenlaalegriade": return mirenlaalegriade;
                case "gameover": return gameover;
                case "numbers": return numbers;
                case "to": return to;
                case "palo": return palo;
                case "pasecorto": return pasecorto;
                case "pateaalarco": return pateaalarco;
                case "pelotade": return pelotade;
                case "pelotazo": return pelotazo;
                case "pidecomentario": return pidecomentario;
                case "pita": return pita;
                case "pitaarrancaelshow": return pitaarrancaelshow;
                case "respondecomentario": return respondecomentario;
                case "salen": return salen;
                case "salereferi": return salereferi;
                case "sigueconlapelota": return sigueconlapelota;
                case "terminoelshow": return terminoelshow;
                case "stoletheball": return stoletheball;
                case "catchtheball": return catchtheball;
                case "travesanio": return travesanio;
                case "clearence": return clearence;
                case "lujitos": return lujitos;

                case "trainingintro": return trainingintro;
                case "trainingok": return trainingok;
                case "trainingstep1": return trainingstep1;
                case "trainingstep2": return trainingstep2;
                case "trainingstep3": return trainingstep3;
                case "trainingstep4": return trainingstep4;
                case "trainingstep5": return trainingstep5;
                case "trainingstep6": return trainingstep6;
                case "trainingstep7": return trainingstep7;
                case "trainingendmatch": return trainingendmatch;
                case "trainingkickoff": return trainingkickoff;
                case "foul": return foul;
                    //powerups:
                case "generic": return generic;
                case "pusupershot": return pusupershot;
                case "pusupershotactive": return pusupershotactive;
                case "purun": return purun;
                case "purunactive": return purunactive;
                case "pubomb": return pubomb;
                case "pubombactive": return pubombactive;
                    //cups
                case "cupwon": return cupwon;
                case "cuplose": return cuplose;
                case "cup10intro": return cup10intro;
                case "cup20intro": return cup20intro;
                case "cup30intro": return cup30intro;
                case "cup40intro": return cup40intro;
                case "cup50intro": return cup50intro;
                case "cup60intro": return cup60intro;
                case "cup70intro": return cup70intro;
                case "cup80intro": return cup80intro;
                case "cup90intro": return cup90intro;
                case "cup100intro": return cup100intro;

                default: return volea;
            }
        }

    }    
}