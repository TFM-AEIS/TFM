using System;
using UnityEngine;
using UnityEngine.UI;

public class Logic : MonoBehaviour
{
    private SaveData data;

    [SerializeField] private InputField nameInput;
    [SerializeField] private InputField levelInput;

    private void Awake()
    {
        this.Load();
    }

    public void Load()
    {
        this.data = SaveManager.Load();

        this.nameInput.text = this.data.name;
        this.levelInput.text = this.data.level.ToString();
    }

    public void Save()
    {
        this.data.name = this.nameInput.text;
        this.data.level = Int32.Parse(this.levelInput.text);
        SaveManager.Save(this.data);
    }
}
