using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeStaffTag : MonoBehaviour
{
    [SerializeField] GameObject freeTag;
    public Fulbo.UI.Shop.Shop.sectionType sectionType;

    // Start is called before the first frame update
    void Start()
    {
        Events.OnFreeStaffUpdate += OnFreeStaffUpdate;
    }

    private void OnDestroy() {
        Events.OnFreeStaffUpdate -= OnFreeStaffUpdate;
    }

    void OnFreeStaffUpdate(Fulbo.UI.Shop.Shop.sectionType type, bool enable) {
        if (type == sectionType)
            SetActive(enable);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool enable) { freeTag.SetActive(enable); }
}
