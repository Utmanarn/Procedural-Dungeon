using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public static class FileHandler
{
    public static void SaveToJSON<T>(List<T> toSave, string fileName)
    {
        Debug.Log(GetPath(fileName));
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(fileName), content);
    }

    public static void SaveToJSON<T>(T toSave, string fileName)
    {
        Debug.Log(GetPath(fileName));
        string content = JsonUtility.ToJson(toSave);
        WriteFile(GetPath(fileName), content);
    }

    public static void SaveToTXT(string toSave, string fileName)
    {
        WriteFile(GetPath(fileName), toSave);
    }

    public static void SaveArrayToTXT(char[,] array, string fileName)
    {
        WriteFileArray(GetPath(fileName), array);
    }

    public static List<T> ReadListFromJSON<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }
        
        List<T> res = JsonHelper.FromJson<T>(content).ToList();

        return res;
    }

    public static string ReadStringFromTXT(string fileName)
    {
        return ReadFile(GetPath(fileName));
    }
    
    public static char[,] ReadArrayFromTXT(string fileName)
    {
        return ReadFileArray(GetPath(fileName));
    }

    private static string GetPath(string fileName)
    {
        return Application.dataPath + "/Rooms/" + fileName;
    }
    
    private static void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        Debug.Log("Content: " + content);
        
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }
    
    private static void WriteFileArray(string path, char[,] array)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        Debug.Log("Content: " + array);

        string content = "";
        
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                content += array[i, j].ToString();
            }
        }
        
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }

        return "";
    }
    
    private static char[,] ReadFileArray(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                char[,] array = new char[4,4];
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        array[i, j] = content[i * 4 + j];
                    }
                }
                
                return array;
            }
        }

        return null;
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }
    
    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
