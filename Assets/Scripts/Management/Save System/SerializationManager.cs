using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager
{
    public static bool Save(string saveName, object save)
    {
        BinaryFormatter bFormatter = new BinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        string savePath = Application.persistentDataPath + "/saves/" + saveName + ".pbag";

        FileStream file = File.Create(savePath);
        bFormatter.Serialize(file, save);
        file.Close();

        return true;
    }

    public static object Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File not found!");
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            Debug.Log("Loaded save at path " + path);
            return save;
        }
        catch
        {
            Debug.LogError("Failed to load save file at path " + path);
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter() 
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        Vector3SerializationSurrogate vector3Surrogate = new Vector3SerializationSurrogate();
        QuaternionSerializationSurrogate quaternionSurrogate = new QuaternionSerializationSurrogate();
        ColorSerializationSurrogate colorSurrogate = new ColorSerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);
        selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }
}
