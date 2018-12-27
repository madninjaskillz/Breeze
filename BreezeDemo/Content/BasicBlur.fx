technique AcrylicBlur
{
    pass Pass1
    {
#ifdef SM4
		PixelShader = compile ps_4_0 main();
#else
		PixelShader = compile ps_3_0 main();
#endif
    }
}

texture2D ScreenTexture;
sampler TextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

float gfxWidth;
float gfxHeight;
int blurSize;
int noisePerc;

float normpdf(in float x, in float sigma)
{
    return 0.39894 * exp(-0.5 * x * x / (sigma * sigma)) / sigma;
}


float rand(float2 co)
{
    return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
}


float3 noisePixel(float2 ps)
{
    float r = rand(ps.xy);
    return float3(r, r, r);
}

float4 main(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float2 resolution = float2(gfxWidth, gfxHeight);

    float2 size = resolution.xy;

	float noiseAmount = float(noisePerc) / 100.0;

    const int mSize = blurSize;
    
    const int kSize = (mSize - 1) / 2.0;

    float kernel[48]; //this should be msize;
    float3 final_colour = float3(0.0,0.0,0.0);

    float sigma = 7.0;
    float Z = 0.0;

    for (int w = 0; w <= kSize; ++w)
    {
        kernel[kSize + w] = kernel[kSize - w] = normpdf(float(w), sigma);
    }

    for (int e = 0; e < mSize; ++e)
    {
        Z += kernel[e];
    }
    
    float3 noise = noisePixel(pos.xy);

    float step = 1.0;
    float calcStep = 1.0 + (mSize / 20.0);
    if (calcStep > 1.0)
    {
        step = calcStep;
    }

    for (float i = -kSize; i <= kSize; i=i+step)
    {
        for (float j = -kSize; j <= kSize; j=j+step)
        {

            float2 pixelPos = float2(float(i * step), float(j * step)).xy;
            
            float3 np = noisePixel(((pos.xy+pixelPos+(kSize*noise.xy)) / resolution.xy)+(noise.y/ resolution.xy));
            float3 pixel = (ScreenTexture.Sample(TextureSampler, ((pos.xy+pixelPos) / resolution.xy) ).rgb);
            float3 pixel2 = (ScreenTexture.Sample(TextureSampler, ((pos.xy+pixelPos+(kSize*np.xy)) / resolution.xy)+(np.y/ resolution.xy) ).rgb);
            
            pixel = (pixel2/2.0)+(pixel/2.0);

			float3 pixelWithNoise = (pixel * (1.0 - noiseAmount)) + (noise * noiseAmount);

            final_colour += kernel[kSize + (j)] * kernel[kSize + (i)] * pixelWithNoise;
        }
    }
float3 xx=(final_colour / (Z * Z)).rgb;
    float4 rr = float4(xx, 1.0);

    return rr;
}

