using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rendering/Custom Post FX Settings")]
public class PostFXSettings : ScriptableObject
{
	[SerializeField]
	Shader shader = default;

	[Serializable]
	public struct BloomSettings
	{
		public bool ignoreRenderScale;

		[Range(0f, 16f)]
		public int maxIterations;

		[Min(1f)]
		public int downscaleLimit;

		public bool bicubicUpsampling;

		[Min(0f)]
		public float threshold;

		[Range(0f, 1f)]
		public float thresholdKnee;

		[Min(0f)]
		public float intensity;

		public bool fadeFireflies;

		public enum Mode { Additive, Scattering }

		public Mode mode;

		[Range(0.05f, 0.95f)]
		public float scatter;
	}

	[SerializeField]
	BloomSettings bloom = new BloomSettings
	{
		scatter = 0.7f
	};

	public BloomSettings Bloom => bloom;

	[Serializable]
	public struct ColorAdjustmentsSettings
	{
		public float postExposure;

		[Range(-100f, 100f)]
		public float contrast;

		[ColorUsage(false, true)]
		public Color colorFilter;

		[Range(-180f, 180f)]
		public float hueShift;

		[Range(-100f, 400f)]
		public float saturation;
	}

	[SerializeField]
	ColorAdjustmentsSettings colorAdjustments = new ColorAdjustmentsSettings
	{
		colorFilter = Color.white
	};

	public ColorAdjustmentsSettings ColorAdjustments => colorAdjustments;

	[Serializable]
	public struct WhiteBalanceSettings
	{
		[Range(-100f, 100f)]
		public float temperature, tint;
	}

	[SerializeField]
	WhiteBalanceSettings whiteBalance = default;

	public WhiteBalanceSettings WhiteBalance => whiteBalance;

	[Serializable]
	public struct SplitToningSettings
	{
		[ColorUsage(false)]
		public Color shadows, highlights;

		[Range(-100f, 100f)]
		public float balance;
	}

	[SerializeField]
	SplitToningSettings splitToning = new SplitToningSettings
	{
		shadows = Color.gray,
		highlights = Color.gray
	};

	public SplitToningSettings SplitToning => splitToning;

	[Serializable]
	public struct ChannelMixerSettings
	{
		public Vector3 red, green, blue;
	}

	[SerializeField]
	ChannelMixerSettings channelMixer = new ChannelMixerSettings
	{
		red = Vector3.right,
		green = Vector3.up,
		blue = Vector3.forward
	};

	public ChannelMixerSettings ChannelMixer => channelMixer;

	[Serializable]
	public struct ShadowsMidtonesHighlightsSettings
	{
		[ColorUsage(false, true)]
		public Color shadows, midtones, highlights;

		[Range(0f, 2f)]
		public float shadowsStart, shadowsEnd, highlightsStart, highLightsEnd;
	}

	[SerializeField]
	ShadowsMidtonesHighlightsSettings shadowsMidtonesHighlights = new ShadowsMidtonesHighlightsSettings
	{
		shadows = Color.white,
		midtones = Color.white,
		highlights = Color.white,
		shadowsEnd = 0.3f,
		highlightsStart = 0.55f,
		highLightsEnd = 1f
	};

	public ShadowsMidtonesHighlightsSettings ShadowsMidtonesHighlights => shadowsMidtonesHighlights;
	
	[Serializable]
	public struct ToneMappingSettings
	{
		public enum Mode { None, ACES, Neutral, Reinhard }

		public Mode mode;
	}

	[SerializeField]
	ToneMappingSettings toneMapping = default;

	public ToneMappingSettings ToneMapping => toneMapping;

	// color clamp
	[Serializable]
	public struct ColorClampSettings
	{
		public int enabled;
		public int colorClampCount;
	}

	[SerializeField]
	ColorClampSettings colorClamp = new ColorClampSettings
	{
		enabled = 0,
		colorClampCount = 16
	};

	public ColorClampSettings ColorClamp => colorClamp;

	// outlines
	[Serializable]
	public struct OutlineSettings
	{
		public int enabled;
		public float outlineScale;
		public Color outlineColor;
	}

	[SerializeField]
	OutlineSettings outline = new OutlineSettings
	{
		enabled = 0,
		outlineScale = 1f,
		outlineColor = Color.black
	};

	public OutlineSettings Outline => outline;

	// chromatic
	[Serializable]
	public struct ChromaticSettings
	{
		public int enabled;
		public float rDir, gDir, bDir;
		public float rAmount, gAmount, bAmount;
		public float blendFactor;
	}

	[SerializeField]
	ChromaticSettings chromatic = new ChromaticSettings
	{
		enabled = 0,
		rDir = -1f,
		gDir = 0f,
		bDir = 1f,
		rAmount = .0025f,
		gAmount = 0f,
		bAmount = .0025f,
		blendFactor = .25f
	};

	public ChromaticSettings Chromatic => chromatic;

	// xbr
	[Serializable]
	public struct XBRSettings
	{
		public int enabled;
	}

	[SerializeField]
	XBRSettings xbr = new XBRSettings
	{
		enabled = 0
	};

	public XBRSettings XBR => xbr;

	// contrastEtc
	[Serializable]
	public struct ContrastEtcSettings
	{
		public int enabled;
	}

	[SerializeField]
	ContrastEtcSettings contrastEtc = new ContrastEtcSettings
	{
		enabled = 0
	};

	public ContrastEtcSettings ContrastEtc => contrastEtc;

	// material
	[NonSerialized]
	Material material;

	public Material Material
	{
		get
		{
			if (material == null && shader != null)
			{
				material = new Material(shader);
				material.hideFlags = HideFlags.HideAndDontSave;
			}
			return material;
		}
	}
}