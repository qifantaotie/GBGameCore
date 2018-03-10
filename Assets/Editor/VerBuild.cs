using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;

public class VerBuild : Editor
{

    private static string mABPathRoot = Application.dataPath + "/Ab";
    private static List<string> BulidExtensions = new List<string>()
    {
        ".prefab"
    };
    [MenuItem("Tool/生成ab")]
    private static void BulidAB()
    {
        FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
        Directory.CreateDirectory(Application.streamingAssetsPath);
        List<string> listDir = new List<string>();
        GetDirs(mABPathRoot, ref listDir);
        Dictionary<string, string> dic = new Dictionary<string, string>();
        //  string[] dirs = Directory.GetDirectories(mABPathRoot);
        foreach (string dir in listDir)
        {
            string[] files = Directory.GetFiles(dir);
            if (files.Length > 0)
            {
                string path = Application.streamingAssetsPath + "/" + Path.GetFileName(dir);
                List<string> list = new List<string>();
                List<Object> assets = new List<Object>();
                foreach (string file in files)
                {
                    if (BulidExtensions.Contains(Path.GetExtension(file)))
                    {
                        dic[Path.GetFileName(file)] = Path.GetFileName(dir);
                        assets.Add(AssetDatabase.LoadMainAssetAtPath(FileUtil.GetProjectRelativePath(file)));
                        list.Add(Path.GetFileName(file));
                    }
                }
                //BuildPipeline.BuildAssetBundle(null, assets.ToArray(), path + ".bytes");
            }

        }


        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, string> keyValuePair in dic)
        {
            string fileName = keyValuePair.Key.Substring(0, keyValuePair.Key.LastIndexOf('.'));
            sb.AppendLine(fileName+"|"+keyValuePair.Key.Replace(fileName,"") + "|" + keyValuePair.Value+".bytes");

        }
        File.WriteAllText(Application.dataPath + "/Resources/Ver.txt", sb.ToString());
        AssetDatabase.Refresh();
        Log.Debug("生成完成");
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath,BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    private static void GetDirs(string path, ref List<string> list)
    {
        foreach (string dir in Directory.GetDirectories(path))
        {
            if (Directory.GetDirectories(dir).Length > 0)
            {
                GetDirs(dir, ref list);
            }
            else
            {
                list.Add(dir);
            }
        }
    }

    
}
