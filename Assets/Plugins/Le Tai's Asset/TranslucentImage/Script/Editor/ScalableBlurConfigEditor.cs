using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace LeTai.Asset.TranslucentImage.Editor
{
    [CustomEditor(typeof(ScalableBlurConfig))]
    [CanEditMultipleObjects]
    public class ScalableBlurConfigEditor : UnityEditor.Editor //BlurConfigEditor
    {
        int      tab, previousTab;
        AnimBool useAdvancedControl = new AnimBool(false);


#region const

        const int Min          = 0;
        const int MaxIteration = 6;

        readonly GUIContent radiusLabel = new GUIContent(
            "Radius",
            "Blurriness. Does NOT affect performance"
        );

        readonly GUIContent iterationLabel = new GUIContent(
            "Iteration",
            "The number of times to run the algorithm to increase the smoothness of the effect. Can affect performance when increase"
        );

        readonly GUIContent depthLabel = new GUIContent(
            "Max Depth",
            "Decrease will reduce flickering, blurriness and performance"
        );

#endregion


        public void Awake()
        {
            LoadTabSelection();
            useAdvancedControl.value = tab > 0;
        }

        public void OnEnable()
        {
            //Smoothly switch tab
            useAdvancedControl.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            ScalableBlurConfig config = (ScalableBlurConfig) target;
            DrawTabView(config);
            Undo.RecordObject(target, "Modify Blur Config");
        }

        void DrawTabView(ScalableBlurConfig config)
        {
            EditorGUILayout.Space();

            using (var v = new EditorGUILayout.VerticalScope())
            {
                GUI.Box(v.rect, GUIContent.none);
                EditorGUILayout.LabelField("Blur settings", EditorStyles.centeredGreyMiniLabel);

                DrawTabBar();
                EditorGUILayout.Space();

                DrawTabsContent(config);
                EditorGUILayout.Space();

                config.MaxDepth = EditorGUILayout.IntField(depthLabel, config.MaxDepth);
                EditorGUILayout.Space();
            }
        }

        void DrawTabBar()
        {
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                tab = GUILayout.Toolbar(
                    tab,
                    new[] {"Simple", "Advanced"},
                    GUILayout.MinWidth(0),
                    GUILayout.MaxWidth(EditorGUIUtility.pixelsPerPoint * 192)
                );

                GUILayout.FlexibleSpace();
            }

            if (tab != previousTab)
            {
                GUI.FocusControl(""); // Defocus 
                SaveTabSelection();
                previousTab = tab;
            }

            useAdvancedControl.target = tab == 1;
        }

        void DrawTabsContent(ScalableBlurConfig config)
        {
            //Simple tab
            if (EditorGUILayout.BeginFadeGroup(1 - useAdvancedControl.faded))
            {
                config.Strength = Mathf.Max(0, EditorGUILayout.FloatField("Strength", config.Strength));
            }

            EditorGUILayout.EndFadeGroup();

            //Advanced tab
            if (EditorGUILayout.BeginFadeGroup(useAdvancedControl.faded))
            {
                config.Radius = EditorGUILayout.FloatField(radiusLabel, config.Radius);

                config.Iteration = EditorGUILayout.IntSlider(
                    iterationLabel,
                    config.Iteration,
                    Min,
                    MaxIteration
                );
            }

            EditorGUILayout.EndFadeGroup();
        }

        //Persist selected tab between sessions and instances
        void SaveTabSelection()
        {
            EditorPrefs.SetInt("tab", tab);
        }

        void LoadTabSelection()
        {
            if (EditorPrefs.HasKey("tab"))
                tab = EditorPrefs.GetInt("tab");
        }
    }
}