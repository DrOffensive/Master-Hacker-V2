using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SerializeUtility
{

    public static void Save<T>(T obj, string path, string fileName, string fileExtension)
    {
        string destination = Application.dataPath + path + "/" + fileName + "." + fileExtension;
        FileStream file;

        FolderCheck(Application.dataPath + path, true);

        if (File.Exists(destination))
            file = File.OpenWrite(destination);
        else
            file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, obj);
        file.Close();
        Debug.Log("Saved " + fileName + "." + fileExtension + " to:" + Application.dataPath + path);
    }

    public static T Load<T>(string path, string fileName, string fileExtension)
    {
        string destination = Application.dataPath + path + "/" + fileName + "." + fileExtension;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError(fileName + "." + fileExtension + " not found");
            return default(T);
        }

        BinaryFormatter bf = new BinaryFormatter();
        T data = (T)bf.Deserialize(file);
        file.Close();

        Debug.Log("Loaded " + fileName + "." + fileExtension + " from:" + Application.dataPath + path);
        return data;
    }

    public static T Load<T>(string fullpathandfilename, bool relative)
    {
        string destination = relative ? Application.dataPath + fullpathandfilename : fullpathandfilename;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError(fullpathandfilename + " not found");
            return default(T);
        }

        BinaryFormatter bf = new BinaryFormatter();
        T data = (T)bf.Deserialize(file);
        file.Close();

        return data;
    }

    public static bool FolderCheck(string path, bool create)
    {
        if (!Directory.Exists(path))
        {
            if (create)
                Directory.CreateDirectory(path);
            else
                Debug.LogWarning("Folder: '" + path + "/' Not found");

            return false;
        }

        return true;
    }

    public static FileInfo[] GetFilesAtDirectory(string path)
    {
        if (!FolderCheck(Application.dataPath + path, false))
        {
            return null;
        }
        DirectoryInfo info = new DirectoryInfo(Application.dataPath + path);
        FileInfo[] files = info.GetFiles();
        Debug.Log("Found " + files.Length + " files @ " + Application.dataPath + path);
        return files;
    }

    public static FileInfo[] GetFilesAtDirectory(string path, string extension)
    {
        if (!FolderCheck(Application.dataPath + path, false))
        {
            return null;
        }
        DirectoryInfo info = new DirectoryInfo(Application.dataPath + path);
        FileInfo[] files = info.GetFiles();
        List<FileInfo> sortedFiles = new List<FileInfo>();
        foreach(FileInfo file in files)
        {
            if (file.Name.EndsWith("." + extension))
                sortedFiles.Add(file);
        }
        Debug.Log("Found " + sortedFiles.Count + " files of type '." + extension +"' @ " + Application.dataPath + path);
        return sortedFiles.ToArray();
    }

    public static List<T> GetDeserializedFilesAtDirectory<T>(string path, string extension)
    {
        if (!FolderCheck(Application.dataPath + path, false))
        {
            return null;
        }
        DirectoryInfo info = new DirectoryInfo(Application.dataPath + path);
        List<T> objs = new List<T>();
        foreach (FileInfo file in info.GetFiles())
        {
            if (file.Name.EndsWith("." + extension))
            {
                objs.Add(Load<T>(file.FullName, false));
            }
        }
        Debug.Log("Found " + objs.Count + " files of type '." + extension + "' @ " + Application.dataPath + path);
        return objs;
    }

    public static void DeleteFile (string fullNameAndPath, bool relative)
    {
        string destination = relative ? Application.dataPath + fullNameAndPath : fullNameAndPath;
        File.Delete(destination);
    }

    public static bool FileCheck (string path, string filename, string extension)
    {
        if (File.Exists(Application.dataPath + path + "/" + filename + "." + extension))
            return true;

        return false;
    }
}