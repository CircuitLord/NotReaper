using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


[Serializable]
[PostProcess(typeof(BlurRenderer), PostProcessEvent.AfterStack, "Custom/Blur")]
public sealed class Blur : PostProcessEffectSettings
{
	// ReSharper disable FieldCanBeMadeReadOnly.Global
	[Range(0, 2)]
	public IntParameter downsample = new IntParameter { value = 1 };

	[Range(0.0f, 10.0f)]
	public FloatParameter blurSize = new FloatParameter { value = 3f };

	[Range(0, 4)]
	public IntParameter blurIterations = new IntParameter { value = 2 };
}

public sealed class BlurRenderer : PostProcessEffectRenderer<Blur>
{
	private Shader shader;
	private int parameterID;

	public override void Init()
	{
		base.Init();
		shader = Shader.Find("Hidden/Blur");
		parameterID = Shader.PropertyToID("_Parameter");
	}

	public override void Render(PostProcessRenderContext context)
	{
		int nbIterations = settings.blurIterations.value;
		if (nbIterations == 0)
		{
			context.command.Blit(context.source, context.destination);
			return;
		}

		var cmd = context.command;
		var sheet = context.propertySheets.Get(shader);

		float widthMod = 1.0f / (1.0f * (1 << settings.downsample.value));
		float blurSize = settings.blurSize.value;
		sheet.properties.SetFloat(parameterID, blurSize * widthMod);

		int rtW = context.width >> settings.downsample.value;
		int rtH = context.height >> settings.downsample.value;

		// downsample
		RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, context.sourceFormat);
		rt.filterMode = FilterMode.Bilinear;
		cmd.BlitFullscreenTriangle(context.source, rt, sheet, 0);

		for (int i = 0; i < nbIterations; i++)
		{
			float iterationOffs = i * 1.0f;
			sheet.properties.SetFloat(parameterID, blurSize * widthMod + iterationOffs);

			// vertical blur
			RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, context.sourceFormat);
			rt2.filterMode = FilterMode.Bilinear;
			cmd.BlitFullscreenTriangle(rt, rt2, sheet, 1);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;

			// horizontal blur
			rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, context.sourceFormat);
			rt2.filterMode = FilterMode.Bilinear;
			cmd.BlitFullscreenTriangle(rt, rt2, sheet, 2);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;
		}

		context.command.Blit(rt, context.destination);
		RenderTexture.ReleaseTemporary(rt);
	}
}
