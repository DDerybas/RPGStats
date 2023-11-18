#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatsContainer))]
public class StatsContainerTabsEditor : Editor
{
    private int _tabIndex, _arrayLength;
    private List<string> _tabs = new List<string>();

    private StatsContainer _statsContainer;
    private SerializedProperty _statListProperty;

    private GUIStyle _statLabelStyle = new GUIStyle();
    private GUIStyle _removeButtonStyle = new GUIStyle();
    private GUIStyle _darkBackgroundStyle = new GUIStyle();
    private GUIStyle _valuesBackStyle = new GUIStyle();
    private GUIStyle _statIconStyle = new GUIStyle();

    private Texture2D _darkBackgroundTexture;
    private Texture2D _valuesBackTexture;
    private Texture2D _removeIcon;

    Color _labelAndTypeBackColor = new Color(.17f, .17f, .17f);
    Color _valuesColor = new Color(.12f, .12f, .12f);

    private const string EMPTY_TAB_NAME = "NULL";
    private const string EMPTY_NAME = "_" + EMPTY_TAB_NAME + "_";
    private const string STATS_DATA_PREFIX = "_stats.Array.data[{0}].";

    private void OnEnable()
    {
        _removeIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/StatsContainer/Scripts/Editor/Icons/remove.png");
        _statsContainer = (StatsContainer)target;

        _statLabelStyle.fontSize = 14;
        _statLabelStyle.fontStyle = FontStyle.Bold;
        _statLabelStyle.normal.textColor = Color.white;

        _darkBackgroundStyle.fontSize = 14;
        _darkBackgroundStyle.fontStyle = FontStyle.Normal;
        _darkBackgroundStyle.fixedHeight = 22;
        _darkBackgroundStyle.contentOffset = new Vector2(10, 1);
        _darkBackgroundTexture = new Texture2D(1, 1);
        _darkBackgroundTexture.SetPixel(0, 0, _labelAndTypeBackColor);
        _darkBackgroundTexture.Apply();
        _darkBackgroundStyle.normal.background = _darkBackgroundTexture;

        _valuesBackTexture = new Texture2D(1, 1);
        _valuesBackTexture.SetPixel(0, 0, _valuesColor);
        _valuesBackTexture.Apply();
        _valuesBackStyle.normal.background = _valuesBackTexture;
        _valuesBackStyle.margin = new RectOffset(5, 0, 0, 0);
        _valuesBackStyle.fontStyle = FontStyle.Italic;

        _removeButtonStyle.fixedWidth = 16;
        _removeButtonStyle.fixedHeight = 16;
        _removeButtonStyle.normal.background = _removeIcon;
        _removeButtonStyle.margin = new RectOffset(5, 5, 3, 5);

        _statIconStyle.fixedHeight = 16;
        _statIconStyle.fixedWidth = 16;
        _statIconStyle.contentOffset = new Vector2(2.5f, 3f);
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        EditorGUILayout.BeginVertical(); 
        _tabIndex = GUILayout.Toolbar(_tabIndex, _tabs.ToArray());
        EditorGUILayout.EndVertical();
        
        _statListProperty = serializedObject.FindProperty("_stats");

        if (_statListProperty == null)
        {
            Debug.LogError("Container stats is null. Have you changed the variable name?");
            return;
        }

        CalculateTabs();

        if (_statListProperty.isArray && _tabs.Count > 0)
        {
            EditorGUI.BeginChangeCheck();

            // Skip generic field.
            _statListProperty.Next(true);

            // Advance to array size field.
            _statListProperty.Next(true);                   
            
            _arrayLength = _statListProperty.intValue;

            // Iterates through the stat array.
            for (int i = 0; i < _arrayLength; i++)
            {
                EditorGUILayout.BeginVertical();
                try
                {
                    var _statType = serializedObject.FindProperty(FormatPropName("StatType", i));
                    string _statName = string.Empty;
                    if (_statType != null && _statType.objectReferenceValue != null)
                    {
                        var child = new SerializedObject(_statType.objectReferenceValue);
                        _statName = child.FindProperty("m_Name").stringValue;
                    }
                    else
                        _statName = EMPTY_NAME;

                    if (_tabIndex > _tabs.Count - 1)
                        _tabIndex = _tabs.Count - 1;
                    if (!_statName.Contains(_tabs[_tabIndex]))
                        continue;

                    EditorGUILayout.BeginHorizontal(_darkBackgroundStyle);
                    if (_statName != EMPTY_NAME)
                    {
                        int startIndex = _statName.LastIndexOf('_') + 1;
                        int endIndex = _statName.IndexOf('(');
                        endIndex = endIndex <= 0 ? _statName.Length - startIndex : endIndex - startIndex;
                        GUILayout.Box((new SerializedObject(_statType.objectReferenceValue).targetObject as StatType).Icon.texture, _statIconStyle);
                        EditorGUILayout.LabelField(_statName.Substring(startIndex, endIndex), _statLabelStyle);
                    }
                    DrawRemoveButton(i, out int buttonsLeft);
                    if (buttonsLeft <= 0)
                         break;

                    EditorGUILayout.EndHorizontal();

                    #region ValuesPanel
                    EditorGUILayout.BeginVertical(_valuesBackStyle);

                    var _value = serializedObject.FindProperty(FormatPropName("Value", i));
                    var _maxValue = serializedObject.FindProperty(FormatPropName("MaxValue", i));

                    EditorGUILayout.PropertyField(_value);
                    EditorGUILayout.PropertyField(_maxValue);

                    // Type and remove button
                    EditorGUILayout.BeginHorizontal(_darkBackgroundStyle);
                    EditorGUILayout.PropertyField(_statType);
                    EditorGUILayout.EndHorizontal();
                    // ---

                    EditorGUILayout.EndVertical();
                    #endregion
                }
                finally
                {
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.Separator(); 
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
            }
            EditorGUILayout.Separator();
        }

        DrawAddButton();
        EditorGUILayout.Separator();

        serializedObject.ApplyModifiedProperties();

        // Uncomment if you need the standard array representation below.
        //DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
            CalculateTabs();
    }

    private string FormatPropName(string propName, int arrayID) =>
        string.Format(STATS_DATA_PREFIX, arrayID) + string.Format("<{0}>k__BackingField", propName);
    
    private void DrawAddButton()
    {
        var c = GUI.color;
        GUI.color = Color.green;
        if (GUILayout.Button("Add Item"))
        {
            _statsContainer.AddEmptyStat();
            CalculateTabs();
            _tabIndex = FindTabIndex(EMPTY_TAB_NAME);
        }
        GUI.color = c;
    }

    private void DrawRemoveButton(int index, out int arraySize)
    {
        _statListProperty = serializedObject.FindProperty("_stats");

        if (GUILayout.Button(string.Empty, _removeButtonStyle))
        {
            _statListProperty.DeleteArrayElementAtIndex(index);

            _tabIndex = _tabs.Count - 2;
            if (_tabIndex < 0) _tabIndex = 0;
            CalculateTabs();
            EditorUtility.SetDirty(serializedObject.targetObject);
        }

        arraySize = _statListProperty.arraySize;
    }

    // Calculates tabs to display stat categories.
    private void CalculateTabs()
    {
        _tabs.Clear();
        _statListProperty = serializedObject.FindProperty("_stats");
        var statsCount = _statListProperty.arraySize;

        for (int i = 0; i < statsCount; i++)
        {
            var tabName = string.Empty;
            var oType = (_statListProperty.serializedObject.targetObject as StatsContainer).GetStatByIndex(i).StatType;
            if (oType != null)
            {
                var sName = oType.name;
                int firstIndex = sName.IndexOf('_');
                int lastIndex = sName.LastIndexOf('_');
                tabName = sName.Substring(firstIndex + 1, (lastIndex - firstIndex) - 1);
            }
            else tabName = EMPTY_TAB_NAME;
            if (!_tabs.Contains(tabName))
                _tabs.Add(tabName);
        }
    }

    private int FindTabIndex(string tab)
    {
        for (int i = 0; i < _tabs.Count; i++)
        {
            if (_tabs[i] == tab)
                return i;
        }

        return 0;
    }
}
#endif