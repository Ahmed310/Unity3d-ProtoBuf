using UnityEngine;
using UnityEditor;
using BnBCustomEditor.Utils.Proto;


public class Proto_Editor : EditorWindow
{

    private SettingData m_settingData;
    private const string DATABASE_PATH = @"Assets/proto-buf/Editor/Database/ProtoSettingDatabase.asset";

    private ProtoSettingDatabase m_protoSetting;
    private Vector2 _scrollPos;

    [MenuItem("Tools/ProtoBuf")]
    public static void Init()
    {
        Proto_Editor window = EditorWindow.GetWindow<Proto_Editor>();
        window.minSize = new Vector2(800, 400);
        window.Show();
        // m_settingData = new SettingData();
    }

    void OnEnable()
    {
        if (m_settingData == null) LoadDatabase();
    }


    void OnGUI()
    {
        //EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        DisplayUIInfo();
        //EditorGUILayout.EndHorizontal();
    }

    void LoadDatabase()
    {
        m_protoSetting = (ProtoSettingDatabase)AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(ProtoSettingDatabase));
        m_settingData = m_protoSetting.GetDatabase();
        if (m_protoSetting == null) CreateDatabase();
    }

    void CreateDatabase()
    {
        m_protoSetting = ScriptableObject.CreateInstance<ProtoSettingDatabase>();
        AssetDatabase.CreateAsset(m_protoSetting, DATABASE_PATH);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }



    private void CustomSetDirty()
    {
        EditorUtility.SetDirty(m_protoSetting);
    }
    private void DisplayUIInfo()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Set Tool ", GUILayout.Width(120)))
        {
            string path = EditorUtility.OpenFilePanel("Select Tool ProtoGen.exe in ProtoBuf", "", "");
            Debug.Log(path);
            m_settingData.ToolPath = path.Replace("/", "\\");
            Debug.Log(m_settingData.ToolPath);
            CustomSetDirty();

        }
        m_settingData.ToolPath = EditorGUILayout.TextField(new GUIContent(""), m_settingData.ToolPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Set Output Dir", GUILayout.Width(120)))
        {
            string path = EditorUtility.OpenFilePanel("Select Output Dir for C# files", "", "");

            m_settingData.OutputPath = path.Replace("/", "\\");
            CustomSetDirty();
        }

        m_settingData.OutputPath = EditorGUILayout.TextField(new GUIContent(""), m_settingData.OutputPath);

        EditorGUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Proto Files Dir", GUILayout.Width(120)))
        {
            string path = EditorUtility.OpenFilePanel("Select Proto Files Dir", "", "");

            m_settingData.ProtoFilesPath = path.Replace("/", "\\");
            CustomSetDirty();
        }

        m_settingData.ProtoFilesPath = EditorGUILayout.TextField(new GUIContent(""), m_settingData.ProtoFilesPath);

        EditorGUILayout.EndHorizontal();




        EditorGUILayout.Space();
        //newSprite.Id = EditorGUILayout.TextField(new GUIContent("Name : "), newSprite.Id);
        //newSprite.SpritePath = EditorGUILayout.TextField(new GUIContent("Sprite Path : "), newSprite.SpritePath);
        //newSprite.Type = (SpriteCatagory)EditorGUI.EnumPopup(GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true)), "Catagory:", newSprite.Type);

        EditorGUILayout.BeginVertical();   //GUILayout.ExpandWidth(true)
        EditorGUILayout.LabelField("Protobuf File Names");
        for (int i = 0; i < m_settingData.ProtoFiles.Count; i++)
        {
            var fileName = m_settingData.ProtoFiles[i];
            fileName = EditorGUILayout.TextField(new GUIContent((i + 1).ToString() + " : "), fileName);
        }
        //if (GUILayout.Button("Remove Last Name", GUILayout.Width(120)))
        //{

        //}
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        DropPrefabArea(m_settingData);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate C# source code from Proto code", GUILayout.ExpandWidth(true)))
        {
            //  m_settingData.ToolPath + " " +
            m_settingData.Command = "--proto_path=" + m_settingData.ProtoFilesPath;
            for (int i = 0; i < m_settingData.ProtoFiles.Count; i++)
            {
                m_settingData.Command += " " + m_settingData.ProtoFiles[i];
            }
            m_settingData.Command += " " + "-output_directory=" + m_settingData.OutputPath;

            // Debug.Log(m_settingData.Command);


            // var cmd = System.Diagnostics.Process.Start("cmd.exe", m_settingData.Command);
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WorkingDirectory = @"e:\";
            startInfo.FileName = m_settingData.ToolPath;
            startInfo.Arguments = m_settingData.Command;
            System.Diagnostics.Process.Start(startInfo);
        }

        if (GUILayout.Button("Refresh AssetDatabase", GUILayout.ExpandWidth(true)))
        {
            AssetDatabase.Refresh();
        }
    }

    public void DropPrefabArea(SettingData data)
    {
        Event evt = Event.current;
        GUI.color = Color.green;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Drag Proto files Here one by one or all together");
        GUI.color = Color.white;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    m_settingData.ProtoFiles = new System.Collections.Generic.List<string>();
                    foreach (string dragged_object in DragAndDrop.paths)
                    {
                        int offset = dragged_object.IndexOf("Resources/");
                        string processedString = (dragged_object.Split('.')[0]).Remove(0, offset + "Resources/".Length);
                        //  data.SpritePath = processedString;
                        var list = dragged_object.Split('/');
                        //Debug.Log(list[list.Length - 1]);
                        m_settingData.ProtoFiles.Add(list[list.Length - 1]);
                    }
                    CustomSetDirty();
                }
                break;
        }
    }
}