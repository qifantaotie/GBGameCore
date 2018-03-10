using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System;
using System.Reflection;

public class E2A : EditorWindow
{
    public static string savePath = Application.dataPath + "/_Scripts/Game/Mode/GameData/";

    public static string saveAssetPath = Application.dataPath + "/Resources/Data/";

   
    [MenuItem("Window/excel转asset")]
    public static void ShowE2A()
    {
        
        E2A e2a = EditorWindow.CreateInstance<E2A>();
        e2a.titleContent = new GUIContent("excel转asset");
        eFilePath = PlayerPrefs.GetString("YOUKEXUEYUAN_ExcelPath");
        e2a.Show();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }



    static string eFilePath = ""; 

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("excel的路径：" + eFilePath);
                if (GUILayout.Button("选择路径"))
                {
                    eFilePath = EditorUtility.OpenFolderPanel("选择excel所在路径", eFilePath, "");
                    PlayerPrefs.SetString("YOUKEXUEYUAN_ExcelPath", eFilePath);
                    PlayerPrefs.Save();
                }
                GUILayout.Space(30);
            } GUILayout.EndHorizontal();

            

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(30);
                if (!string.IsNullOrEmpty(eFilePath))
                {
                    if (GUILayout.Button("生成cs"))
                    {
                        FileUtil.DeleteFileOrDirectory(savePath);
                        List<string> fileNames = ReadExcelToAssets(eFilePath);
                        foreach (string fileName in fileNames)
                        {
                            List<DataTable> tabs = ReadExcel(fileName);
                            foreach (DataTable data in tabs)
                            {
                                CreateCs(data);
                            }
                        }
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                    }
                    else if (GUILayout.Button("生成asset"))
                    {
                        FileUtil.DeleteFileOrDirectory(saveAssetPath);
                        List<string> fileNames = ReadExcelToAssets(eFilePath);
                        foreach (string fileName in fileNames)
                        {
                            List<DataTable> tabs = ReadExcel(fileName);
                            foreach (DataTable data in tabs)
                            {
                                CreateAsset(data);

                            }
                        }
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                }
                GUILayout.Space(30);
            } GUILayout.EndHorizontal();
           
            
        } GUILayout.EndVertical();

      

        
    }

    static List<string> ReadExcelToAssets(string file)
    {
        List<string> list = new List<string>();
        string[] files = Directory.GetFiles(file);
        foreach (string str in files)
        {
            string fileName = str.Replace("\\", "/");
            if ((fileName.EndsWith(".xls") || fileName.EndsWith(".xlsx")) && !Path.GetFileName(fileName).StartsWith("~$"))
            {
                list.Add(fileName);
            }
        }
        return list;
    }

    public static List<DataTable> ReadExcel(string excelFilePath)
    {
        List<DataTable> list = new List<DataTable>();
        //try
        {

            FileStream stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();

            foreach (DataTable data in result.Tables)
            {
                if (data != null && data.Rows.Count > 3 && data.Columns.Count > 0)
                {
                    list.Add(data);
                }
                
            }
            stream.Close();
            
        }
        //catch (System.Exception ex)
        {
            //Debug.Log(ex.ToString());
        }
        //finally
        {
        }
        return list;

    }

    public class SaveAssetInfo
    {
        public FieldInfo info;
        public object obj;
    }
    public static void CreateAsset(DataTable data)
    {
       
        if (data == null
           || data.Rows.Count < 4
           || data.Columns.Count <= 0
           || string.IsNullOrEmpty(data.TableName))
        {
            return;
        }
        string fileName = saveAssetPath + data.TableName + "Serialize.asset";
        List<List<SaveAssetInfo>> mData = new List<List<SaveAssetInfo>>();
        Assembly assem = Assembly.Load("Assembly-CSharp");
        DataRow nameRow = data.Rows[0];
        DataRow typeRow = data.Rows[1];
        Type t = assem.GetType(data.TableName);
        FieldInfo[] infos = t.GetFields();
        
        Debug.LogError(data.TableName +"---"+ data.Rows.Count + "----" + data.Columns.Count);
        //return; 
        /*Debug.LogError(infos.Length);*/
        for (int i = 3; i < data.Rows.Count;i++ )
        {
            List<SaveAssetInfo> list = new List<SaveAssetInfo>();
            var row = data.Rows[i];
            
            foreach (DataColumn column in data.Columns)
            {
                object value = row[column];
                string name = nameRow[column].ToString();
                string type = typeRow[column].ToString();
                value = GetType(type, value);
                SaveAssetInfo info = new SaveAssetInfo();
                for (int j = 0; j < infos.Length; j++)
                {
                    //Debug.LogError(infos[i]);
                    if (infos[j].Name == name)
                    {
                        info.info = infos[j];
                    }
                }
                info.obj = value;
                list.Add(info);
            }
            mData.Add(list);
        }
        SaveToAsset(assem,t, mData, fileName);

    }

    /// <summary>
    /// 保存asset
    /// </summary>
    /// <param name="assem">程序集</param>
    /// <param name="t">脚本类型</param>
    /// <param name="data">所有要保存的数据</param>
    /// <param name="filePath">要保存到的路径</param>
    public static void SaveToAsset(Assembly assem, Type t, List<List<SaveAssetInfo>> data, string filePath)
    {
        if (data == null)
        {
            return;
        }
        //文件名 也是类名//
        string defName = Path.GetFileNameWithoutExtension(filePath);
        //单个数据的类名//
        string defNamec = defName.Replace("Serialize","");
        //要保存的路径//
        string dir = Path.GetDirectoryName(filePath);
        ///当路径不存在就创建这个路径//
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        List<object> list = new List<object>();
        foreach (List<SaveAssetInfo> dic in data)
        {
            //通过反射创建一个单个保存的对象//
            object obj = Activator.CreateInstance(t);
            foreach (SaveAssetInfo info in dic)
            {
                //通过反射 设置这个对象的值//
                info.info.SetValue(obj, info.obj);
                
            }
            //把单个数据保存到 list中//
            list.Add(obj);
        }
        
        //通过反射拿到要生成asset的类//
        Type Serialize = assem.GetType(defName);
        //根据类名创建一个ScriptableObject 实例//
        object serializeObj = ScriptableObject.CreateInstance(defName);

        //从要生成asset的类中反射获取一个叫 SetDatas的函数//
        MethodInfo mi = Serialize.GetMethod("SetDatas");
        //调用 asset对象中的这个函数 传入我们上一步保存的数据//
        mi.Invoke(serializeObj, new object[] { list.ToArray() });

        //把asset的对象保存到指定路径中//
        AssetDatabase.CreateAsset(serializeObj as UnityEngine.Object, FileUtil.GetProjectRelativePath(filePath));
       
    }

    public static object GetType(string t,object obj)
    {
        try
        {
            if (t == "double")
            {
                obj = double.Parse(obj.ToString());
            }
            else if (t == "int")
            {
                obj = int.Parse(obj.ToString());
            }
            else if (t == "string")
            {
                obj = obj.ToString();
            }
            else if (t == "float")
            {
                obj = float.Parse(obj.ToString());
            }
            else if (t == "bool")
            {
                obj = bool.Parse(obj.ToString());
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("ex:" + ex);
        }
        return obj;
    }

    public static void CreateCs(DataTable data)
    {
        if (data == null 
            || data.Rows.Count < 4 
            || data.Columns.Count <= 0
            || string.IsNullOrEmpty(data.TableName))
        {
            return;
        }

        DataRow descriptionRow = data.Rows[2];
        DataRow nameRow = data.Rows[0];
        DataRow typeRow = data.Rows[1];
        List<fileDef> list = new List<fileDef>();
        string fileName = savePath + data.TableName + ".cs";
        foreach (DataColumn column in data.Columns)
        {
            fileDef def = new fileDef();
            def.description = descriptionRow[column].ToString();
            def.name = nameRow[column].ToString();
            def.type = typeRow[column].ToString();
            //Debug.Log(def.description + "/" + def.name + "/" + def.type);
            
//             string type = (data.Rows[2][column]).ToString();
//             string variableName = (data.Rows[1][column]).ToString();
//             object value = row[column];
            list.Add(def);
        }
        SaveToCs(list, fileName);
        
        //AssetDatabase.Refresh();
    }

    public class fileDef
    {
        public string name;
        public string type;
        public string description;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="filePath"></param>
    public static void SaveToCs(List<fileDef> list,string filePath)
    {
        
        if (list == null)
        {
            return;
        }
        string defName = Path.GetFileNameWithoutExtension(filePath);
       
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
       
        //创建代码//
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// ");
        sb.AppendLine("/// </summary>");
        sb.AppendLine();

        sb.AppendLine("[System.Serializable]");
        sb.AppendFormat("public class {0} \r\n{{", defName);
        sb.AppendLine();
       
        foreach (fileDef def in list)
        {
            sb.AppendFormat("\t /// <summary>{0}</summary> \n \tpublic {1} {2};\n",def.description,def.type,def.name);
            sb.AppendLine();
        }
        sb.AppendLine("}");


        TextAsset text = AssetDatabase.LoadAssetAtPath("Assets/Editor/E2A/AssetMode.txt", typeof(TextAsset)) as TextAsset;
        string writeall = text.text.Replace("#ScriptName#", defName);
        File.WriteAllText(filePath.Replace(defName, defName + "Serialize"), writeall, new UTF8Encoding(true));
        File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
    }

}
