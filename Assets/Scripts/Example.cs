using UnityEngine;

/// <summary>
/// An example class that receives links to health, health regeneration and armor stats. Uses the health regeneration stat value to increase the health stat value every second.
/// Displays values ​​via OnGUI.
/// </summary>
public class Example : MonoBehaviour
{
    [SerializeField] private Stat _statHealth, _statHealthRegen, _statArmor;
    [SerializeField] private StatsContainer _statsContainer;

    private GUIStyle _guiStyle;

    private float _regenTime = 1;
    private float _timer;

    void Start()
    {
        // Gets stats from the stats container.
        _statHealth = Stat.BindStat(_statHealth, _statsContainer);
        _statHealthRegen = Stat.BindStat(_statHealthRegen, _statsContainer);
        _statArmor = Stat.BindStat(_statArmor, _statsContainer);

        _guiStyle = new GUIStyle();
        _guiStyle.alignment = TextAnchor.MiddleCenter;
        _guiStyle.normal.textColor = Color.white;
        _guiStyle.fontSize = 22;
    }


    void Update()
    {
        // A simple timer that runs every second.
        _timer += Time.deltaTime;
        if (_timer >= _regenTime)
        {
            _statHealth.AddValue(_statHealthRegen.Value);
            _timer = 0;
        }
    }

    // Displays stats.
    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 100, 100, 30), $"Health: {_statHealth.Value} / {_statHealth.MaxValue}", _guiStyle);
        GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 70, 100, 30), $"Armor: {_statArmor.Value}", _guiStyle);
    }
}
