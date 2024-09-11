#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class EditorTool : MonoBehaviour
{
    [MenuItem("Custom/Clear Save Data")]
    private static void ClearData()
    {
        PlayerPrefs.DeleteAll(); // Xóa PlayerPref
        Debug.Log("<color=lime> Data cleared! </color>");
    }

    [MenuItem("Custom/Play Immediately")]
    public static void SwitchToSceneLoadingAndPlay()
    {
        EditorSceneManager.SaveOpenScenes(); // Lưu scene hiện tại
        OpenScene0(); // Mở scene Loading
        EditorApplication.isPlaying = true; // Bật chế độ chơi game
    }

    [MenuItem("Custom/-> Open Scene 0 <-")]
    public static void OpenScene0()
    {
        EditorSceneManager.SaveOpenScenes(); // Lưu scene hiện tại
        string scenePath = SceneUtility.GetScenePathByBuildIndex(0);
        EditorSceneManager.OpenScene(scenePath);
        Debug.Log("<color=lime> Scene 0 opened! </color>");
    }

    [MenuItem("Custom/-> Open Scene 1 <-")]
    public static void OpenScene1()
    {
        EditorSceneManager.SaveOpenScenes(); // Lưu scene hiện tại
        string scenePath = SceneUtility.GetScenePathByBuildIndex(1);
        EditorSceneManager.OpenScene(scenePath);
        Debug.Log("<color=lime> Scene 1 opened! </color>");
    }

    [MenuItem("Custom/-> Open Scene 2 <-")]
    public static void OpenScene2()
    {
        EditorSceneManager.SaveOpenScenes(); // Lưu scene hiện tại
        string scenePath = SceneUtility.GetScenePathByBuildIndex(2);
        EditorSceneManager.OpenScene(scenePath);
        Debug.Log("<color=lime> Scene 2 opened! </color>");
    }
}
#endif