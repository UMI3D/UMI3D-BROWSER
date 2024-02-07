using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class CreateItemInHierarchyUtility
{
    public static void CreatePrefab(string path)
    {
        GameObject newObject = PrefabUtility.InstantiatePrefab(Resources.Load(path)) as GameObject;
        Place(newObject);
    }

    public static void CreateObject(string name, params Type[] types)
    {
        GameObject newObject = ObjectFactory.CreateGameObject(name, types);
        Place(newObject);
    }

    public static void Place(GameObject gameObject)
    {
        // Find location
        SceneView lastView = SceneView.lastActiveSceneView;
        gameObject.transform.position = lastView ? lastView.pivot : Vector3.zero;

        // Make sure we place the object in the proper scene, with a relevant name
        StageUtility.PlaceGameObjectInCurrentStage(gameObject);
        GameObjectUtility.EnsureUniqueNameForSibling(gameObject);

        // Record undo, and select
        Undo.RegisterCreatedObjectUndo(gameObject, $"Create Object: {gameObject.name}");
        Selection.activeGameObject = gameObject;

        // For prefabs, let's mark the scene as dirty for saving
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}