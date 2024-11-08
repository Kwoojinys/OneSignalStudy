using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;

public class Worker_Info
{
    public int id;

    public int basegold;

    public int lvupgold;

    public int reqgold;

    public string name;

    public void Set_Info(string json)
    {
        var data = JSON.Parse(json);

        FieldInfo[] FT = this.GetType().GetFields();

        foreach (KeyValuePair<string, JSONNode> kvp in (JSONObject)data)
        {
            FieldInfo Fi = this.GetType().GetField(kvp.Key);
            switch (kvp.Value.Tag)
            {
                case JSONNodeType.Number:
                    {
                        int this_data = kvp.Value;
                        Fi.SetValue(this, this_data);
                        break;
                    }
                case JSONNodeType.String:
                    {
                        string this_data = kvp.Value;
                        Fi.SetValue(this, this_data);
                        break;
                    }
            }
        }
    }
}

public class WorkerManager : MonoBehaviour
{
    public List<Worker_Info> WorkerData;

    private static WorkerManager instance;

    public static WorkerManager Instance
    {
        get
        {
            if (!instance)
            {
                GameObject container = new GameObject();
                container.name = "WorkerManager";
                instance = container.AddComponent(typeof(WorkerManager)) as WorkerManager;
            }

            return instance;
        }
    }

    public static string tableName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void Set_Worker_Data(string json)
    {
        WorkerData = new List<Worker_Info>();

        JSONArray Ja = JSON.Parse(json) as JSONArray;

        for (int i = 0; i < Ja.Count; i++)
        {
            Worker_Info Wi = new Worker_Info();
            Wi.Set_Info(Ja[i].ToString());
            WorkerData.Add(Wi);

            int Ra = Random.Range(0, 2);
            if(Ra == 1)
            {
                WorkerData.Add(Wi);
            }
        }
    }

    public IEnumerator Working_Time()
    {
        yield return new WaitForSeconds(5f);

        
    }

}
