using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Saves and loads game saves.
    /// </summary>
    public static class SerializationManager
    {
        /// <summary>
        /// Save game save
        /// </summary>
        /// <param name="saveName">The name of the save. Example "exampleSave"</param>
        /// <param name="save">The game save to save</param>
        /// <returns>Whether the save was successful</returns>
        public static bool Save(string saveName, object save)
        {
            BinaryFormatter bFormatter = GetBinaryFormatter();

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

        /// <summary>
        /// Load game save
        /// </summary>
        /// <param name="path">The path to the save file</param>
        /// <returns>Whether the load was successful</returns>
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

        /// <summary>
        /// Generated a binary formatter with all serialization surrogates
        /// </summary>
        /// <returns>BinaryFormatter ready to be used for serialization/deserialization of the game saves</returns>
        public static BinaryFormatter GetBinaryFormatter()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            SurrogateSelector selector = new SurrogateSelector();

            Vector3SerializationSurrogate vector3Surrogate = new Vector3SerializationSurrogate();
            Vector2IntSerializationSurrogate vectorInt2Surrogate = new Vector2IntSerializationSurrogate();
            Vector2SerializationSurrogate vector2Surrogate = new Vector2SerializationSurrogate();
            QuaternionSerializationSurrogate quaternionSurrogate = new QuaternionSerializationSurrogate();
            ColorSerializationSurrogate colorSurrogate = new ColorSerializationSurrogate();

            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
            selector.AddSurrogate(typeof(Vector2Int), new StreamingContext(StreamingContextStates.All), vectorInt2Surrogate);
            selector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), vector2Surrogate);
            selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);
            selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSurrogate);

            formatter.SurrogateSelector = selector;

            return formatter;
        }
    }
}
