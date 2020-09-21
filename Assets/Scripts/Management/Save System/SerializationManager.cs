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

    public static object Load(string saveName)
    {
        string path = Application.persistentDataPath + "/saves/" + saveName + ".pbag";

        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
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
        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);

        QuaternionSerializationSurrogate quaternionSurrogate = new QuaternionSerializationSurrogate();
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);

        return formatter;
    }
}
