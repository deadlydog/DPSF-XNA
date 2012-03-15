//====================== File Description ===========================
// DPSFEffects.fx
// 
// This file provides the default Vertex and Pixel Shaders used by
// the DPSF Default Particle System classes.
//
// Copyright Daniel Schroeder 2008
//===================================================================

//===================================================================
// XNA-to-HLSL Global Variables
//===================================================================
//-------------------------------------------------------------------
// Sprite and Textured Quad Specific Variables
//-------------------------------------------------------------------
// How much of the vertex Color should be blended in with the Texture's Color
float xColorBlendAmount = 0.5;	// 0.0 = use Texture's Color, 1.0 = use specified Color

// NOTE: The final opacity of the Texture is based on the vertex Color's alpha value
texture xTexture;				// The texture to use to draw the Particles

//-------------------------------------------------------------------
// Quad and Textured Quad Specific Variables
//-------------------------------------------------------------------
float4x4 xView;					// The View matrix
float4x4 xProjection;			// The Projection matrix
float4x4 xWorld;				// The World matrix

//===================================================================
// Texture Sampler
//===================================================================
sampler TextureSampler = sampler_state 
{ 
	texture = <xTexture>;

	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Point;

	AddressU = Clamp; // Wrap, Mirror, or Clamp
	AddressV = Clamp;
	AddressW = Clamp;
};


//===================================================================
// Sprite Pixel Shader Structures and Methods
//===================================================================

// Input to the Pixel Shader
struct SpritePixelShaderInput
{
	float4 Color : COLOR0;
	float2 NormalizedTextureCoordinate : TEXCOORD0;
};

// Pixel Shader for drawing Sprites
float4 SpritePixelShader(SpritePixelShaderInput input) : COLOR0
{
	// Get the Texture Coordinate
	float2 NormalizedTextureCoordinate = input.NormalizedTextureCoordinate.xy;

	// Get the Color of the Texture at the specific Coordinate
	float4 Color = tex2D(TextureSampler, NormalizedTextureCoordinate);
   
   // If this Pixel should not be completely transparent
	if (Color.a > 0.0)
	{
		// Convert the Texture Color to be Non-Premultiplied
		Color.rgb /= Color.a;

		// Blend the specified Color with the Texture's Color
		Color.rgb = (xColorBlendAmount * input.Color.rgb) + ((1 - xColorBlendAmount) * Color.rgb);
	
		// Scale the Texture's Alpha according to the specified Alpha
		// The following equation is equivalent to: Color.a -= ((1 - input.Color.a) * Color.a);
		Color.a *= input.Color.a;
	
		// Convert the color to a Premultiplied color
		Color.rgb *= Color.a;
	}

	return Color;
}


//===================================================================
// Quad Shader Structures and Methods
//===================================================================

// Input to the Vertex Shader
struct QuadVertexShaderInput
{
	float4 Position				: POSITION0;
	float4 Color				: COLOR0;
};

// Output from the Vertex Shader
struct QuadVertexShaderOutput
{
	float4 Position				: POSITION0;
	float4 Color				: COLOR0;
};

// Calculate the Position and Color of the Quad on the screen
QuadVertexShaderOutput QuadVertexShader(QuadVertexShaderInput input)
{
	QuadVertexShaderOutput Output;

	// Calculate the transformed Position of the vertex on the screen
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
	Output.Position = mul(input.Position, preWorldViewProjection);

	// Convert the given Color to a premultiplied color
	input.Color.rgb *= input.Color.a;
	Output.Color = input.Color;
	
	return Output;
}

// Pixel Shader for drawing Quads
float4 QuadPixelShader(QuadVertexShaderOutput input) : COLOR0
{    
	return input.Color;
}


//===================================================================
// Textured Quad Shader Structures and Methods
//===================================================================

// Input to the Vertex Shader
struct TexturedQuadVertexShaderInput
{
	float4 Position						: POSITION0;
	float2 NormalizedTextureCoordinate	: TEXCOORD0;
	float4 Color						: COLOR0;
};

// Output from the Vertex Shader
struct TexturedQuadVertexShaderOutput
{
	float4 Position						: POSITION0;
	float2 NormalizedTextureCoordinate	: COLOR1;
	float4 Color						: COLOR0;
};

// Vertex Shader for drawing Textured Quads
// Calculate the Position and Color of the Quad on the screen
TexturedQuadVertexShaderOutput TexturedQuadVertexShader(TexturedQuadVertexShaderInput input)
{
	TexturedQuadVertexShaderOutput Output;

	// Calculate the transformed Position of the vertex on the screen
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
	Output.Position = mul(input.Position, preWorldViewProjection);

	// Pass the rest of the properties through unchanged
	Output.NormalizedTextureCoordinate = input.NormalizedTextureCoordinate;
	Output.Color = input.Color;
	
	return Output;
}

// Pixel Shader for drawing Textured Quads
float4 TexturedQuadPixelShader(TexturedQuadVertexShaderOutput input) : COLOR0
{
	// Get the Texture Coordinate
	float2 NormalizedTextureCoordinate = input.NormalizedTextureCoordinate.xy;

	// Get the Color of the Texture at the specific Coordinate (already premultiplied)
	float4 Color = tex2D(TextureSampler, NormalizedTextureCoordinate);

	// If this Pixel should not be completely transparent
	if (Color.a > 0.0)
	{
		// Convert the Texture Color to be Non-Premultiplied
		Color.rgb /= Color.a;

		// Blend the specified Color with the Texture's Color
		Color.rgb = (xColorBlendAmount * input.Color.rgb) + ((1 - xColorBlendAmount) * Color.rgb);
	
		// Scale the Texture's Alpha according to the specified Alpha
		// The following equation is equivalent to: Color.a -= ((1 - input.Color.a) * Color.a);
		Color.a *= input.Color.a;
	
		// Convert the color to a Premultiplied color
		Color.rgb *= Color.a;
	}

	return Color;
}


//===================================================================
// Experimental Textured Quad Shader Structures and Methods - Color Blend Using PreMultiplied Colors
//===================================================================

// Calculate the Position and Color of the Quad on the screen
TexturedQuadVertexShaderOutput TexturedQuadVertexShaderExperimental(TexturedQuadVertexShaderInput input)
{
	TexturedQuadVertexShaderOutput Output;

	// Calculate the transformed Position of the vertex on the screen
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
	Output.Position = mul(input.Position, preWorldViewProjection);

	// Convert the Color to be premultiplied
	input.Color.rgb *= input.Color.a;
	Output.Color = input.Color;

	// Pass the rest of the properties through unchanged
	Output.NormalizedTextureCoordinate = input.NormalizedTextureCoordinate;
	
	return Output;
}

// Pixel Shader for drawing Textured Quads
float4 TexturedQuadPixelShaderExperimental(TexturedQuadVertexShaderOutput input) : COLOR0
{
	// Get the Texture Coordinate
	float2 NormalizedTextureCoordinate = input.NormalizedTextureCoordinate.xy;

	// Get the Color of the Texture at the specific Coordinate (already premultiplied)
	float4 Color = tex2D(TextureSampler, NormalizedTextureCoordinate);

	// Blend the specified Color with the Texture's Color according to the Blend Amount
	Color.rgb = ((1.0 - xColorBlendAmount) * Color.rgb) + (xColorBlendAmount * input.Color.rgb);
	
	// Scale the Texture's Alpha according to the specified Alpha
	Color.a *= input.Color.a;

	// Apply the Alpha channel to the colors (since this is a premultiplied color the drawing ignores the alpha channel)
	Color.rgb *= Color.a;

	return Color;
}


//===================================================================
// Techniques
//===================================================================

technique Sprites
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 SpritePixelShader();
	}
}

technique Quads
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 QuadVertexShader();
		PixelShader = compile ps_2_0 QuadPixelShader();
	}
}

technique TexturedQuads
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 TexturedQuadVertexShader();
		PixelShader = compile ps_2_0 TexturedQuadPixelShader();
	}
}

technique TexturedQuadsExperimental
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 TexturedQuadVertexShaderExperimental();
		PixelShader = compile ps_2_0 TexturedQuadPixelShaderExperimental();
	}
}