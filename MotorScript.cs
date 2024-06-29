using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEditor;

public class MoveChilds2
{
    public List<string> Name { get; set; }
    public List<Vector3> Position { get; set; }
    public List<int> Level { get; set; }
    public List<Transform> Transforms { get; set; }

    public MoveChilds2()
    {
        Name = new List<string>();
        Position = new List<Vector3>();
        Level = new List<int>();
        Transforms = new List<Transform>();
    }

    public void Temizle()
    {
        Name.Clear();
        Position.Clear();
        Level.Clear();
        Transforms.Clear();
    }

    public void VeriEkle(string name, Vector3 position, int level, Transform transforms)
    {
        Name.Add(name);
        Position.Add(position);
        Level.Add(level);
        Transforms.Add(transforms);
    }
}

public class MotorScript : MonoBehaviour
{    
    MoveChilds2 childs2 = new MoveChilds2();

    public int distance = 4;
    private int current_level = 0;
    public float movDuration = 2;
    public Transform model = null;
    private int count = 0;

    public Dropdown Leveldropdown;
    public Dropdown Modeldropdown;
    public Button ResetButton;
    public Slider slider;

    private void ColliderAdder(Transform parent)
    {
        if (parent.childCount == 0)
        {
            if (!parent.GetComponent<Collider>())
            {
                MeshRenderer meshRenderer = parent.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    parent.gameObject.AddComponent<MeshCollider>();
                    MeshCollider meshCollider = parent.gameObject.GetComponent<MeshCollider>();
                    meshCollider.sharedMesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
                    count++;
                    childs2.VeriEkle(parent.gameObject.name, parent.position.normalized, 0, parent);
                }
                else
                {
                    childs2.VeriEkle(parent.gameObject.name, parent.position.normalized, 0, parent);
                    Debug.LogWarning("MeshRenderer bulunamadı: " + parent.name);
                }
            }
        }
        else
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                ColliderAdder(parent.GetChild(i));
            }
        }
    }

    private void ListModels()
    {
        Modeldropdown.options.Clear();
        Modeldropdown.options.Add(new Dropdown.OptionData(""));

        GameObject[] models = Resources.LoadAll<GameObject>("");

        List<string> modelNames = new List<string>();

        foreach (GameObject model in models)
        {
            string modelName = model.name; // GameObject'un adını al

            modelNames.Add(modelName);
        }
        foreach (string modelName in modelNames)
        {
            Modeldropdown.options.Add(new Dropdown.OptionData(modelName));
        }

        Modeldropdown.onValueChanged.AddListener(delegate {
            ModelDropdownValueChanged(Modeldropdown);
        });
    }

    private void GetLevels()
    {
        current_level = 0;
        model.position = new Vector3(0, 0, 0);
        Debug.Log(model.position.normalized);
        bool IsModelHasCenter = false;

        ColliderAdder(model);
        print(count);
        Debug.Log("bitti.");
        List<Transform> mainparts = new List<Transform>();
        for (int i = 0; i < model.childCount; i++)
        {
            mainparts.Add(model.GetChild(i));
        }
        List<int> hitcount = new List<int>();
        foreach (var item in mainparts)
        {
            if (item.name.Contains("Center") || item.name.Contains("center") || item.name.Contains("CENTER"))
            {
                RaycastHit[] hit = Physics.RaycastAll(model.position.normalized, -item.up, 100f, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal);
                hitcount.Add(hit.Length);
                for (int i = 0; i < hit.Length; i++)
                {
                    if (!childs2.Transforms.Any(Transform => Transform == hit[i].collider.transform))
                    {
                        childs2.VeriEkle(hit[i].collider.name, hit[i].collider.transform.position, hit.Length - i, hit[i].collider.transform);
                    }
                }

                Debug.DrawRay(model.position.normalized, -item.up, Color.blue, 100f);
                RaycastHit[] hit2 = Physics.RaycastAll(model.position.normalized, item.up, 100f, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal);
                hitcount.Add(hit2.Length);
                for (int i = 0; i < hit2.Length; i++)
                {
                    if (!childs2.Transforms.Any(str => str == hit2[i].collider.transform))
                    {
                        childs2.VeriEkle(hit2[i].collider.name, hit2[i].collider.transform.position, hit2.Length - i, hit2[i].collider.transform);
                    }
                }
                IsModelHasCenter = true;
            }
            else
            {
                RaycastHit[] hit = Physics.RaycastAll(model.position.normalized, -item.right, 100f, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal);
                hitcount.Add(hit.Length);
                for (int i = 0; i < hit.Length; i++)
                {
                    if (!childs2.Transforms.Any(str => str == hit[i].collider.transform))
                    {
                        childs2.VeriEkle(hit[i].collider.name, hit[i].collider.transform.position, hit.Length - i, hit[i].collider.transform);
                    }
                }
            }
        }
        if (!IsModelHasCenter)
        {
            RaycastHit[] hit = Physics.RaycastAll(model.position.normalized, -model.up, 100f, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal);
            RaycastHit[] hit2 = Physics.RaycastAll(model.position.normalized, model.up, 100f, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal);

            for (int i = 0; i < hit.Length; i++)
            {
                if (!childs2.Transforms.Any(Transform => Transform == hit[i].collider.transform))
                {
                    childs2.VeriEkle(hit[i].collider.name, hit[i].collider.transform.position, hit.Length - i, hit[i].collider.transform);
                }
                else
                {
                    int index = childs2.Transforms.IndexOf(hit[i].collider.transform);
                    childs2.Level[index] = hit.Length - i;
                }
            }
            for (int i = 0; i < hit2.Length; i++)
            {
                if (!childs2.Transforms.Any(Transform => Transform == hit2[i].collider.transform))
                {
                    childs2.VeriEkle(hit2[i].collider.name, hit2[i].collider.transform.position, hit2.Length - i, hit2[i].collider.transform);
                }
                else
                {
                    int index = childs2.Transforms.IndexOf(hit2[i].collider.transform);
                    childs2.Level[index] = hit2.Length - i;
                }
            }
        }

        int level_count = hitcount.Max();
        int level_index = hitcount.IndexOf(level_count);

        for (int i = 0; i < level_count; i++)
        {
            MoveChilds2 tempChildren = new MoveChilds2();
            for (int j = 0; j < childs2.Name.Count; j++)
            {
                if (childs2.Level[j] == i + 1)
                {
                    tempChildren.VeriEkle(childs2.Name[j], childs2.Position[j], childs2.Level[j], childs2.Transforms[j]);
                }
            }
            for (int j = 0; j < tempChildren.Name.Count; j++)
            {
                bool increaseBool = false;
                Collider[] colliders = Physics.OverlapBox(tempChildren.Transforms[j].GetComponent<MeshRenderer>().bounds.center, tempChildren.Transforms[j].GetComponent<MeshRenderer>().bounds.size / 2);
                if (colliders.Length > 0)
                {
                    foreach (var item in colliders)
                    {
                        if (!childs2.Transforms.Any(str => str == item.transform))
                        {
                            childs2.VeriEkle(item.name, item.transform.position, tempChildren.Level[j], item.transform);
                            increaseBool = true;
                        }
                    }
                    if (increaseBool)
                    {
                        if (childs2.Level[j] == level_count)
                        {
                            level_count++;
                        }
                        childs2.Level[j]++;
                    }
                }
            }
        }

        List<int> temp_levels = new List<int>();
        temp_levels = childs2.Level.Distinct().ToList();
        temp_levels.Sort();
        for (int i = 0; i < temp_levels.Count; i++)
        {
            if (childs2.Level.Any(str => str == temp_levels[i]))
            {
                int index = childs2.Level.IndexOf(temp_levels[i]);
                childs2.Level[index] = i + 1;
            }
        }
        print(childs2.Name.Count);
        AddLevels(temp_levels.Count());
    }

    private void AddExpLevel(Transform model)
    {
        ColliderAdder(model);
        for (int i = 0; i < childs2.Name.Count; i++)
        {
            RaycastHit[] hit = Physics.RaycastAll(model.position.normalized, childs2.Position[i], 100f, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal);
            for (int j = 0; j < hit.Count(); j++)
            {
                if (childs2.Transforms[i] == hit[j].transform)
                {
                    childs2.Level[i] = hit.Count() - j;
                }
                else
                {
                    var index = childs2.Transforms.FindIndex(a => a == hit[j].transform);
                    childs2.Level[index] = hit.Count() - j;
                }
            }
            if (childs2.Level[i] == 0)
            {
                for (int j = 0; j < hit.Count(); j++)
                {
                    Collider[] colliders = Physics.OverlapBox(hit[j].transform.GetComponent<MeshRenderer>().bounds.center, hit[j].transform.GetComponent<MeshRenderer>().bounds.size / 2);
                    var index = Array.FindIndex(colliders, a => a.transform == childs2.Transforms[i]);
                    if (index != -1)
                    {
                        childs2.Level[i] = hit.Count() - j;
                        for (int k = j; k < hit.Count(); k++)
                        {
                            childs2.Level[k] = hit.Count() - k + 1;
                        }
                        break;
                    }
                }
            }
        }
        List<int> temp_levels = new List<int>();
        temp_levels = childs2.Level.Distinct().ToList();
        temp_levels.Sort();
        for (int i = 0; i < temp_levels.Count; i++)
        {
            if (childs2.Level.Any(str => str == temp_levels[i]))
            {
                int index = childs2.Level.IndexOf(temp_levels[i]);
                childs2.Level[index] = i + 1;
            }
        }
        print(childs2.Name.Count);
        AddLevels(temp_levels.Count());
    }


    void Start()
    {
        ListModels();
        print("starts");
    }

    private void AddLevels(int count)
    {
        Leveldropdown.options.Clear();
        for (int i = 0; i <= count ; i++)
        {
            Leveldropdown.options.Add(new Dropdown.OptionData("Level " + i));
        }

        Leveldropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(Leveldropdown);
        });
    }

    private void DropdownValueChanged(Dropdown change)
    {
        int selectedOptionIndex = change.value;
        string selectedOptionText = change.options[selectedOptionIndex].text;
        int level = int.Parse(selectedOptionText.Split(" ")[1]);

        for (int i = 0; i < level; i++)
        {
            if (level - current_level > 0)
            {
                if (i <= current_level)
                {
                    for (int j = 0; j < level - current_level; j++)
                    {
                        MoveChild(i+1, distance);
                    }
                }
                else
                {
                    for (int j = 0; j < level - i + 1; j++)
                    {
                        MoveChild(i+1, distance);
                    }
                }
                
            }
        }
        //MoveChild(level);

        current_level = level;
    }

    private void MoveChild(int level, int child_distance)
    {
        for (int i = 0; i < childs2.Name.Count; i++)
        {
            Vector3 direction = new Vector3(0, 0, 0);
            if (childs2.Level[i] == level)
            {
                direction = childs2.Transforms[i].position.normalized;
                childs2.Transforms[i].position = Vector3.MoveTowards(childs2.Transforms[i].position, childs2.Transforms[i].position + direction, 5f * child_distance * Time.deltaTime);
            }
        }
    }

    public void OnSliderValueChanged()
    {
        int distance_diff = (int) slider.value;
        if ((int) slider.value != distance)
        {
            distance_diff = distance_diff - distance;
            for (int i = 1; i <= current_level; i++)
            {
                MoveChild(i, distance_diff);
            }
            distance = (int) slider.value;
        }
    }


    private void ModelDropdownValueChanged(Dropdown change)
    {
        
        int selectedOptionIndex = change.value;
        string selectedOptionText = change.options[selectedOptionIndex].text;
        if (model != null && model.name != selectedOptionText && model.gameObject.activeSelf == true)
        {
            model.gameObject.SetActive(false);
            Leveldropdown.options.Clear();
            current_level = 0;
            Leveldropdown.ClearOptions();
            Leveldropdown.value = 0;
            childs2.Temizle();
        }
        try
        {
            GameObject fbxModel = Resources.Load<GameObject>(selectedOptionText);
            if (fbxModel)
            {
                GameObject temp = Instantiate(fbxModel, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f));
                model = temp.transform;
                //GetLevels();
                AddExpLevel(model);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void Update()
    {
        OnSliderValueChanged();
    }
}
