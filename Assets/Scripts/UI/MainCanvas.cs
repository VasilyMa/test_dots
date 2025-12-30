using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    [SerializeField] Button _btnToolkit;
    [SerializeField] Transform _debugToolkit;

    void Start()
    {
        _btnToolkit.onClick.AddListener(OpenDebugToolkit);
    }
    
    public void OpenDebugToolkit()
    { 
        _debugToolkit.gameObject.SetActive(!_debugToolkit.gameObject.activeSelf);

        if (_debugToolkit.gameObject.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        } 
    }
}
