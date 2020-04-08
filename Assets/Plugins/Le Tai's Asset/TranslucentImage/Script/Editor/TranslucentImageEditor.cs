using System.Linq;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace LeTai.Asset.TranslucentImage.Editor
{
[CustomEditor(typeof(TranslucentImage))]
[CanEditMultipleObjects]
public class TranslucentImageEditor : ImageEditor
{
    SerializedProperty spriteBlending;
    SerializedProperty source;
    SerializedProperty vibrancy;
    SerializedProperty brightness;
    SerializedProperty flatten;

    bool materialUsedInDifferentSource;
    bool usingIncorrectShader;

    Shader correctShader;

    protected override void OnEnable()
    {
        base.OnEnable();

        source         = serializedObject.FindProperty("source");
        spriteBlending = serializedObject.FindProperty("spriteBlending");
        vibrancy       = serializedObject.FindProperty("vibrancy");
        brightness     = serializedObject.FindProperty("brightness");
        flatten        = serializedObject.FindProperty("flatten");

        var self = serializedObject.targetObject as TranslucentImage;

        if (self)
        {
            CheckMaterialUsedInDifferentSource(self);

            correctShader = Shader.Find("UI/TranslucentImage");
            CheckCorrectShader(self);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var ti = serializedObject.targetObject as TranslucentImage;
        Debug.Assert(ti != null, "Translucent Image Editor serializedObject target is null");
        var oldSource = ti.source;
        var oldMaterial = ti.material;

        base.OnInspectorGUI();
        if (usingIncorrectShader)
        {
            EditorGUILayout.HelpBox("Material should use shader UI/Translucent Image",
                                    MessageType.Error);
        }

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(source);
        if (materialUsedInDifferentSource)
        {
            EditorGUILayout.HelpBox("Translucent Images with different Sources" +
                                    " should also use different Materials",
                                    MessageType.Error);
        }

        EditorGUILayout.PropertyField(spriteBlending);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shared settings", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.PropertyField(vibrancy);
        EditorGUILayout.PropertyField(brightness);
        EditorGUILayout.PropertyField(flatten);
        serializedObject.ApplyModifiedProperties();

        if (ti.source != oldSource)
            OnSourceChanged(ti);
        if (ti.material != oldMaterial)
            OnMaterialChanged(ti);
    }

    void OnSourceChanged(TranslucentImage self)
    {
        CheckMaterialUsedInDifferentSource(self);
    }

    void OnMaterialChanged(TranslucentImage self)
    {
        CheckMaterialUsedInDifferentSource(self);
        CheckCorrectShader(self);
    }

    private void CheckCorrectShader(TranslucentImage self)
    {
        usingIncorrectShader = self.material.shader != correctShader;
    }

    private void CheckMaterialUsedInDifferentSource(TranslucentImage self)
    {
        var diffSource = FindObjectsOfType<TranslucentImage>()
                        .Where(ti => ti.source != self.source)
                        .ToList();

        if (!diffSource.Any())
        {
            materialUsedInDifferentSource = false;
            return;
        }

        var sameMat = diffSource.GroupBy(ti => ti.material).ToList();

        materialUsedInDifferentSource = sameMat.All(group => group.Key == self.material);
    }
}
}
