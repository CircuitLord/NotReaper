using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage
{
/// <summary>
/// Dynamic blur-behind UI element
/// </summary>
public partial class TranslucentImage : Image, IMeshModifier
{
    /// <summary>
    /// Source of blur for this image
    /// </summary>
    public TranslucentImageSource source;

    /// <summary>
    /// (De)Saturate them image, 1 is normal, 0 is grey scale, below zero make the image negative
    /// </summary>
    [Tooltip("(De)Saturate them image, 1 is normal, 0 is black and white, below zero make the image negative")]
    [Range(-1, 3)]
    public float vibrancy = 1;

    /// <summary>
    /// Brighten/darken them image
    /// </summary>
    [Tooltip("Brighten/darken them image")] [Range(-1, 1)]
    public float brightness = 0;

    /// <summary>
    /// Flatten the color behind to help keep contrast on varying background
    /// </summary>
    [Tooltip("Flatten the color behind to help keep contrast on varying background")] [Range(0, 1)]
    public float flatten = .1f;


    Shader correctShader;

    static int _vibrancyPropId;
    static int _brightnessPropId;
    static int _flattenPropId;
    static int _blurTexPropId;
    static int _cropRegionPropId;


    protected override void Start()
    {
        base.Start();

        PrepareShader();

        oldVibrancy   = vibrancy;
        oldBrightness = brightness;
        oldFlatten    = flatten;

        source = source ? source : FindObjectOfType<TranslucentImageSource>();
        material.SetTexture(_blurTexPropId, source.BlurredScreen);

#if UNITY_5_6_OR_NEWER
        if (canvas)
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
#endif
    }

    void PrepareShader()
    {
        correctShader     = Shader.Find("UI/TranslucentImage");
        _vibrancyPropId   = Shader.PropertyToID("_Vibrancy");
        _brightnessPropId = Shader.PropertyToID("_Brightness");
        _flattenPropId    = Shader.PropertyToID("_Flatten");
        _blurTexPropId    = Shader.PropertyToID("_BlurTex");
        _cropRegionPropId = Shader.PropertyToID("_CropRegion");
    }

    void LateUpdate()
    {
        if (!source)
        {
            Debug.LogError(
                "TranslucentImageSource is missing. Add TranslucentImageSource component to your main camera, then assign it camera to the Source field of the Translucent Image(s)");
            return;
        }

        if (!IsActive() || !source.BlurredScreen)
            return;

        if (!material || material.shader != correctShader)
        {
            Debug.LogError("Translucent Image require a material using \"UI/TranslucentImage\" shader");
        }

        materialForRendering.SetTexture(_blurTexPropId, source.BlurredScreen);
        materialForRendering.SetVector(_cropRegionPropId, source.BlurRegion.ToMinMaxVector());

#if UNITY_EDITOR
        material.SetTexture(_blurTexPropId, source.BlurredScreen);
        material.SetVector(_cropRegionPropId, source.BlurRegion.ToMinMaxVector());
#endif
    }

    void Update()
    {
        if (_vibrancyPropId == 0 || _brightnessPropId == 0 || _flattenPropId == 0)
            return;

        SyncMaterialProperty(_vibrancyPropId,   ref vibrancy,   ref oldVibrancy);
        SyncMaterialProperty(_brightnessPropId, ref brightness, ref oldBrightness);
        SyncMaterialProperty(_flattenPropId,    ref flatten,    ref oldFlatten);
    }

    float oldVibrancy, oldBrightness, oldFlatten;

    /// <summary>
    /// Sync material property with instance
    /// </summary>
    /// <param name="propId">material property id</param>
    /// <param name="value"></param>
    /// <param name="oldValue"></param>
    void SyncMaterialProperty(int propId, ref float value, ref float oldValue)
    {
        float matValue = materialForRendering.GetFloat(propId);
        if (Mathf.Abs(matValue - value) > 1e-4)
        {
            if (Mathf.Abs(value - oldValue) > 1e-4)
            {
                material.SetFloat(propId, value);
                materialForRendering.SetFloat(propId, value);
                SetMaterialDirty();
            }
            else
            {
                value = matValue;
            }
        }

        oldValue = value;
    }
}
}
