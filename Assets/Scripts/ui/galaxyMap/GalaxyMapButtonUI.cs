using Fulbo.DB;
using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Fulbo.Settings;

namespace Fulbo.UI
{
    public class GalaxyMapButtonUI : ButtonCustom
    {
        public int stadiumID;
        GalaxyMapUI ui;

        public bool locked;

        [SerializeField] Transform container;
        [SerializeField] GalaxyMapButtonAsset asset;

        [SerializeField] Text nameField;
        [SerializeField] Text statsField;

        [SerializeField] GameObject progressAsset;
        [SerializeField] GameObject buttonAsset;

        [SerializeField] RectTransform progress;

        [SerializeField] Button infoButtonArea;
        public StadiumsData.StadiumData stadiumData;
        [SerializeField] Image[] to_colorize;

        public void Init(GalaxyMapUI ui, StadiumsData.StadiumData stadiumData)
        {
            //this.stadiumData = stadiumData;
            //this.ui = ui;

            //nameField.text = stadiumData.name;

            //List<LevelData> levels = StoryModeData.Instance.GetAllLevelsFromStadium(stadiumData.id);

            //statsField.text = stadiumData.GetDifficultyString();// "(" + levels[0].GetPercentStats() + "-" + levels[levels.Count - 1].GetPercentStats() + ")";

            //if (levels.Count > 0)
            //{
                
            //    float _min = 50; //Valor mánimo de width de la barrita
            //    float _max = 480; //Valor máximo de width de la barrita

            //    //Cantidad de niveles desbloqueados
            //    float levelsUnlocked = levels.Where(level => !level.locked).Count();

            //    //Seguro se puede optimizar
            //    float _lerp = Mathf.Lerp(_min, _max, levelsUnlocked / levels.Count);

            //    progress.sizeDelta = new Vector2(_lerp, progress.sizeDelta.y);

            //    if (stadiumData.id == 0)
            //        locked = false;
            //    else
            //        locked = levels[0].locked;
            //}

            //if (stadiumData.unavailable)
            //{
            //    progressAsset.SetActive(false);
            //    buttonAsset.SetActive(false);
            //    locked = true;
            //}

            //infoButtonArea.interactable = !locked;

            //AddAsset(stadiumData.button);
            //transform.localPosition = Vector3.zero;

            //foreach (Image image in to_colorize)
            //    image.color = stadiumData.color;
        }
        public void AddAsset(GalaxyMapButtonAsset _asset)
        {
            this.asset = Instantiate(_asset, container);
            asset.transform.localPosition = Vector3.zero;
            asset.transform.localEulerAngles = Vector3.zero;
            asset.SetLocked(locked);
        }
        public override void OnClick()
        {
            if (locked)
                return;

            base.OnClick();
            ui.AreaClicked(this.stadiumID);
            //Events.OnPopup(Data.Instance.texts.Get("stadium_unlocked"), null);
        }
    }
}