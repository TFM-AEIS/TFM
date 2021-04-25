using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager
{
    private static string savePath = Application.persistentDataPath + "/save.dat";

    public static SaveData Load()
    {
        if (File.Exists(savePath))
        {
            SaveData data;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            data = (SaveData)bf.Deserialize(file);
            file.Close();
            return data;
        }
        return new SaveData(); // Primera vez, no se ha ejecutado Save todavía
    }

    public static void Save(SaveData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, data);
        file.Close();
    }
}
