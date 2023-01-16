
/*
 * Copyright (c) 2023, Sangko.Deng
 * All rights reserved.
 *
 * QuickDemoScene is a Unity tool for quickly getting a demo scenes,
 * This script is intended for use under the terms of the MIT license.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;
using System.IO;

public class QuickSceneTool : EditorWindow
{
    private static string version = "1.0.0";
    // private static string autor = "Sangko.Deng";

    int selectedSceneOption = 0;
    string[] options_demoScene = { "Scene1- basic demo secne", "Scene2- FPS demo scene", "Scene2- Parkour demo scene" };
    int lastSelectedSceneOption = 0;


    bool cb_CameraController = true;
    bool cb_CharacterController = true;



    static string currentScriptPath;
    static string resourcesPath;

    static string materialPath;


    GameObject player;
    GameObject camera;


    private void OnEnable()
    {

    }



    [MenuItem("Tools/QuickDemoScene")]
    static void ShowWindow()
    {
        EditorWindow win = GetWindow<QuickSceneTool>("QuickDemoScene " + "v" + version);
        win.minSize = new Vector2(360f, 360f);
        win.maxSize = new Vector2(700f, 700f);
    }
    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();



        //LOGO
        // GUILayout.Label("QuickScene TOOL", EditorStyles.boldLabel);
        string logo_name = "QuickDemoScene";

        GUIStyle logoStyle = new GUIStyle();
        logoStyle.fontSize = 36;
        logoStyle.normal.textColor = Color.green;
        logoStyle.alignment = TextAnchor.MiddleCenter;
        logoStyle.fontStyle = FontStyle.BoldAndItalic;

        GUILayout.Label(logo_name, logoStyle);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.normal.textColor = Color.green;
        buttonStyle.fontSize = 36;
        buttonStyle.normal.background = GUI.skin.button.normal.background;
        buttonStyle.normal.textColor = GUI.skin.button.normal.textColor;
        buttonStyle.active.background = GUI.skin.button.active.background;
        buttonStyle.hover.background = GUI.skin.button.hover.background;

        /// ////////////////////////////////////////////////////BOX1
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Please select an demo scene to create");
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Demo Scene", GUILayout.Width(96));
        selectedSceneOption = EditorGUILayout.Popup(selectedSceneOption, options_demoScene);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        if (lastSelectedSceneOption != selectedSceneOption)
        {
            lastSelectedSceneOption = selectedSceneOption;
            // Do something when the option is changed
            Debug.Log("Selected demo scene: " + options_demoScene[selectedSceneOption]);
        }



        /// ////////////////////////////////////////////////////BOX2
        //Checkbox
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Please select controller");

        CreateCheckbox("Camera Controller", ref cb_CameraController);
        CreateCheckbox("Character Controller", ref cb_CharacterController);

        EditorGUILayout.EndVertical();




        /// ////////////////////////////////////////////////////Button

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create Scene", buttonStyle))
        {
            if (selectedSceneOption == 0)
            {
                buildDemoScene_basic01();
            }

            EditorUtility.DisplayDialog("Create Scene", "Scene is created successfully!", "OK");

        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();


        /// when GUI element update
        if (EditorGUI.EndChangeCheck())
        {
            //When the elements on the panel change, the code here will be executed
            player = GameObject.FindGameObjectWithTag("Player");
            camera = GameObject.FindGameObjectWithTag("MainCamera");

            if (player != null && player.GetComponent<CharacterController>() != null)
            {
                player.GetComponent<CharacterController>().enabled = cb_CharacterController;
            }
            else
            {
                Debug.LogError("No GameObject with tag 'Player' found in the scene.");
            }


            if (camera != null && camera.GetComponent<CameraController>() != null)
            {
                camera.GetComponent<CameraController>().enabled = cb_CameraController;
            }
            else
            {
                Debug.LogError("No GameObject with tag 'Main Camera' found in the scene.");
            }
        }
    }



    ///////////////////////////////////////////////////////////////////////////////////////////SCENES
    private void buildDemoScene_basic01()
    {
        // Create Environment
        GameObject environment = new GameObject("Environment");

        // Create Ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.parent = environment.transform;
        ground.name = "Ground";
        ground.AddComponent<BoxCollider>();


        // Create Player
        player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.transform.parent = environment.transform;
        player.transform.position = new Vector3(0, 0.5f, 0);
        player.name = "Player";
        player.AddComponent<Rigidbody>();

        //Determine whether there is a tag which we need
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
        if (!tags.Any(x => x == "Ground"))
        {
            UnityEditorInternal.InternalEditorUtility.AddTag("Ground");
        }
        if (!tags.Any(x => x == "Player"))
        {
            UnityEditorInternal.InternalEditorUtility.AddTag("Player");
        }
        if (!tags.Any(x => x == "MainCamera"))
        {
            UnityEditorInternal.InternalEditorUtility.AddTag("MainCamera");
        }
        ground.gameObject.tag = "Ground";
        player.gameObject.tag = "Player";


        // Create MainCamera
        camera = GameObject.FindWithTag("MainCamera");
        if (camera != null)
        {
            camera.transform.parent = environment.transform;

            // if (cb_CameraController) return;
            camera.transform.position = new Vector3(0, 10, -10);
            camera.transform.rotation = Quaternion.Euler(60, 0, 0);
            camera.transform.LookAt(player.transform);
        }
        else
        {
            camera = new GameObject("MainCamera", typeof(Camera));
            camera.name = "MainCamera";
            camera.tag = "MainCamera";
            camera.transform.parent = environment.transform;
            // if (cb_CameraController) return;
            camera.transform.position = new Vector3(0, 10, -10);
            camera.transform.rotation = Quaternion.Euler(60, 0, 0);
            camera.transform.LookAt(player.transform);
        }

        // Create Material
        CreateFoldersInCurrentScriptLocation();
        if (!AssetDatabase.IsValidFolder(resourcesPath))
        {
            AssetDatabase.CreateFolder(currentScriptPath, "Resources");
        }

        if (!AssetDatabase.IsValidFolder(materialPath))
        {
            AssetDatabase.CreateFolder(resourcesPath, "Material");
        }

        Shader shader;
        if (GraphicsSettings.renderPipelineAsset != null)
        {
            if (GraphicsSettings.renderPipelineAsset.GetType() == typeof(UniversalRenderPipelineAsset))
            {
                shader = Shader.Find("Universal Render Pipeline/Lit");
            }
            else
            {
                shader = Shader.Find("Standard");
            }
        }
        else
        {
            shader = Shader.Find("Standard");
        }

        Material blackMaterial = new Material(shader);
        Material grayMaterial = new Material(shader);
        blackMaterial.color = Color.black;
        grayMaterial.color = Color.gray;
        blackMaterial.mainTexture = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Resources/unity_builtin_extra/Default-Checker-Gray.png");
        grayMaterial.mainTexture = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Resources/unity_builtin_extra/Default-Checker-Gray.png");
        AssetDatabase.CreateAsset(blackMaterial, materialPath + "/Mat_ground.mat");
        AssetDatabase.CreateAsset(grayMaterial, materialPath + "/Mat_player.mat");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        ground.GetComponent<Renderer>().material = blackMaterial;
        player.GetComponent<Renderer>().material = grayMaterial;



        /* Attach Controller */

        string[] guids = AssetDatabase.FindAssets("t:Script CharacterController");
        string[] guids_cam = AssetDatabase.FindAssets("t:Script CameraController");

        if (guids.Length == 0)
        {
            Debug.LogError("CharacterController.cs not found!");
            return;
        }
        if (guids_cam.Length == 0)
        {
            Debug.LogError("CameraController.cs not found!");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        string path_cam = AssetDatabase.GUIDToAssetPath(guids_cam[0]);

        if (player != null && !player.GetComponent<CharacterController>())
        {
            player.AddComponent<CharacterController>().enabled = cb_CharacterController;
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Player' found in the scene.");
        }


        if (camera != null && !camera.GetComponent<CameraController>())
        {
            camera.AddComponent<CameraController>().enabled = cb_CameraController;
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Main Camera' found in the scene.");
        }





    }




    /// <summary>
    ///  Create Folders In Current Script Location
    /// </summary>
    public static void CreateFoldersInCurrentScriptLocation()
    {
        currentScriptPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(ScriptableObject.CreateInstance<QuickSceneTool>())));
        resourcesPath = currentScriptPath + "/Resources";
        materialPath = resourcesPath + "/Material";

        if (!AssetDatabase.IsValidFolder(resourcesPath))
        {
            AssetDatabase.CreateFolder(currentScriptPath, "Resources");
        }

        if (!AssetDatabase.IsValidFolder(materialPath))
        {
            AssetDatabase.CreateFolder(resourcesPath, "Material");
        }
    }



    /// <summary>
    ///  CreateCheckbox - title is max left / checkbox is max right(10%)
    /// </summary>
    /// <param name="label"></param>
    /// <param name="checkbox"></param>
    void CreateCheckbox(string label, ref bool checkbox_state)
    {
        bool lastCheckboxState = checkbox_state;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.currentViewWidth - EditorGUIUtility.currentViewWidth * 0.1f));
        checkbox_state = EditorGUILayout.Toggle(checkbox_state);
        if (checkbox_state != lastCheckboxState) //防止拖动或缩放面板导致也输出
        {
            Debug.Log(label + ": " + checkbox_state);
        }
        EditorGUILayout.EndHorizontal();
    }


}