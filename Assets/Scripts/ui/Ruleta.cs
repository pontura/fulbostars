using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using static Fulbo.CharactersData;

public class Ruleta : MonoBehaviour {

    public GameObject tapa;
    public float spacing;
    float InitialSpeed;
    public int speed;
    float acceleration = 25;
    public int selectedID;

    public Transform container;
    public RuletaItem ruletaItem;
    public List<RuletaItem> items;
    public int itemsHeight;
    public Color[] colors;

    public int teamID;
    public types type;
    public enum types
    {
        TEAM,
        PLAYER
    }

    public float totalHeight;

    [Serializable]
    public class ItemData
    {
        public CharacterData characterData;
        public Sprite sprite;
        public int id;
    }
    public states state;
    public enum states
    {
        IDLE,
        ROLLING,
        REPOSITION,
        FINISH
    }
    float mySpeed = 5;
    float repositionTo;
    System.Action<int> OnDone;

    public void Init (List<CharacterData> all) {
        tapa.SetActive(true);
        items.Clear();

        int id = 0;
        foreach (CharacterData cd in all)
        {
            Add(id, cd);           
            id++;
        }
        totalHeight = items.Count * (itemsHeight + spacing);
        Add(id, all[0]);
    }
    void Add(int id, CharacterData cd)
    {
        ItemData item = new ItemData();
        item.characterData = cd;
        item.sprite = cd.thumb;
        item.id = id;
        RuletaItem newItem = Instantiate(ruletaItem);
        newItem.characterData = cd;
        newItem.transform.SetParent(container);
        newItem.transform.transform.localScale = Vector2.one;
        newItem.transform.localPosition = Vector3.zero;
        newItem.Init(item, itemsHeight);
        items.Add(newItem);
    }
    public void SetOn(System.Action<int> OnDone)
    {

        tapa.SetActive(false);
        this.OnDone = OnDone;
        InitialSpeed = UnityEngine.Random.Range(5, 20);
        mySpeed = InitialSpeed;
        state = states.ROLLING;
    }
    void Update()
    {
        if (state == states.ROLLING) Rolling();
        else if (state == states.REPOSITION) Repositionate();
    }
    float lastY = 0;
    void Rolling()
    {
        mySpeed -= acceleration * Time.deltaTime;
        float newY = container.transform.localPosition.y + mySpeed;
        lastY += mySpeed;
        if (mySpeed <= 0)
        {
            //if(type == types.TEAM)
            //    AudioManager.Instance.PlaySound("common", "slotMachine_club_" + teamID, false);
            //else
                AudioManager.Instance.PlaySound("common", "ui/ui_click3", false);
            CalculateItem();
            state = states.REPOSITION;            
        }
        if(lastY > itemsHeight)
        {
            lastY = 0;
            AudioManager.Instance.PlaySound("common", "ui/ui_click2", false);
        }
        if (container.localPosition.y > (totalHeight))
            ResetPosition();
        else
            container.localPosition = new Vector3(0, newY + (Time.deltaTime * speed) , 0);
    }
    private void CalculateItem()
    {
        selectedID = (int)Mathf.Round((container.localPosition.y) / itemsHeight);
        if (selectedID >= items.Count-1) selectedID = 0;

        repositionTo = (itemsHeight + spacing) * selectedID;
    }
    void Repositionate()
    {
        if (container.transform.localPosition.y > repositionTo)
            RepositionateUp();
        else
            RepositionateDown();
    }
    void RepositionateUp()
    {
        mySpeed += Time.deltaTime + acceleration;
        float newY = container.transform.localPosition.y - mySpeed;
        container.transform.localPosition = new Vector3(0, newY + (Time.deltaTime * speed), 0);

        if (container.transform.localPosition.y <= repositionTo) Ready();

    }
    void RepositionateDown()
    {
        mySpeed += Time.deltaTime + acceleration;
        float newY = container.transform.localPosition.y + mySpeed;
        container.transform.localPosition = new Vector3(0, newY + (Time.deltaTime * speed), 0);

        if (container.transform.localPosition.y >= repositionTo) Ready();

    }
    void Ready()
    {
        container.transform.localPosition = new Vector3(0, repositionTo, 0);
        state = states.FINISH;
        OnDone(items[selectedID].characterData.id);
    }
    void ResetPosition()
    {
        container.transform.localPosition = Vector3.zero;     
    }
}
