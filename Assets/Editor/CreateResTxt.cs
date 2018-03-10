using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateResTxt : Editor {

    [MenuItem("Tool/生成Res.Txt")]
    public static void CreateResourcesTxt()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        string pathRes = Application.dataPath + "/Resources/";
        string pathTxt = pathRes + "/res.txt";
        if (File.Exists(pathTxt))
        {
            File.Delete(pathTxt);
        }

        CreateResInfo(pathRes, ref dic);
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, string> keyValue in dic)
        {
            list.Add(keyValue.Key + "=" + keyValue.Value);

        }
        File.WriteAllLines(pathRes + "/res.txt", list.ToArray());
        Debug.Log("生成完成");
        AssetDatabase.Refresh();
    }

    private static void CreateResInfo(string path, ref Dictionary<string, string> dic)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        if (!dir.Exists)
        {
            return;
        }
        FileInfo[] files = dir.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {

            FileInfo info = files[i];
            if (!(info.Name.IndexOf(".meta", 0) > 0))
            {
                string pathDir =
                    info.FullName.Replace("\\", "/")
                        .Replace((Application.dataPath + "/Resources/"), "")
                        .Replace(info.Name, "")
                        .TrimEnd('/');
                string fileName = Path.GetFileNameWithoutExtension(info.Name);
                if (!dic.ContainsKey(fileName))
                {
                    dic.Add(fileName, pathDir);
                }
                else
                {
                    Debug.LogError("存在相同的资源名称，名称为：" + info.Name + "/path1=" + dic[fileName] + "/path2" + pathDir);
                }
            }

        }
        DirectoryInfo[] dirs = dir.GetDirectories();
        if (dirs.Length > 0)
        {
            for (int i = 0; i < dirs.Length; i++)
            {
                string tempPath = Path.Combine(path, dirs[i].Name);
                CreateResInfo(tempPath, ref dic);
            }
        }
    }
}
