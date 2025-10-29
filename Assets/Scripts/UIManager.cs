using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct UI
{
    public string name;
    public Transform transform;
}

public class UIManager : Singleton<UIManager>
{

    [SerializeField] private List<UI> ui = new List<UI>();

    private List<UI> ui_spawn_list = new List<UI>();

    protected override void Awake()
    {
        base.Awake();
    }

    private Transform GetListUI(string name)
    {
        return ui.Find(x => x.name == name).transform;
    }

    public Transform GetUI(string name)
    {
        return ui_spawn_list.Find(x => x.name == name).transform;
    }

    private int GetSpawnIndexUI(string name)
    {
        return ui_spawn_list.FindIndex(x => x.name == name);
    }

    public void SpawnUIByString(string name)
    {
        if (GetUI(name)) return;

        Transform temp = (GameObject.Instantiate(GetListUI(name).gameObject, Vector3.zero, Quaternion.identity, transform)).transform;

        UI temp_ui = new UI();
        temp_ui.name = name;
        temp_ui.transform = temp;
        ui_spawn_list.Add(temp_ui);
    }

    public void DestroyUIByString(string name)
    {
        if (!GetUI(name)) return;

        Destroy(GetUI(name).gameObject);
        ui_spawn_list.RemoveAt(GetSpawnIndexUI(name));
    }
}
