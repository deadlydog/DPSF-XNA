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
// Sprite, Point Sprite, and Textured Quad Specific Variables
//-------------------------------------------------------------------
// How much of the vertex Color should be blended in with the Texture's Color
float xColorBlendAmount = 0.5;	// 0.0 = use Texture's Color, 1.0 = use specified Color

// NOTE: The final opacity of the Texture is based on the vertex Color's alpha value
texture xTexture;				// The texture to use to draw the Particles

//-------------------------------------------------------------------
// Pixel, Point Sprite, Quad, and Textured Quad Specific Variables
//-------------------------------------------------------------------
float4x4 xView;					// The View matrix
float4x4 xProjection;			// The Projection matrix
float4x4 xWorld;				// The World matrix

//-------------------------------------------------------------------
// Point Sprite Specific Variables
//-------------------------------------------------------------------
// The Height of the Viewport (needed for re-sizing Point Sprites according to their distance from the Camera)
float xViewportHeight = 0;	// Set this to zero to not use perspective scaling (i.e. don't re-size Point Sprites)

//===================================================================
// Texture Sampler
//===================================================================
sampler TextureSampler = sampler_state 
{ 
	texture = <xTexture>; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = POINT; 
	AddressU = Border; //Wrap, Mirror, Clamp, Border, or MirrorOnce
	AddressV = Border;
	AddressW = Border;
};

//===================================================================
// Pixel Vertex Shader Structures and Methods
//===================================================================

// Input to the Vertex Shader
struct PixelVertexShaderInput
{
	float4 Position				: POSITION0;
	float4 Color				: COLOR0;
};

// Output from the Vertex Shader
struct PixelVertexShaderOutput
{
	float4 Position				: POSITION0;
	float4 Color				: COLOR0;
};

// Calculate the Position of the Pixel on the screen
PixelVertexShaderOutput PixelVertexShader(PixelVertexShaderInput input)
{
    PixelVertexShaderOutput Output;

	// Calculate the transformed Position of the vertex on the screen
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
	Output.Position = mul(input.Position, preWorldViewProjection);

	// Pass the rest of the properties through unchanged
    Output.Color = input.Color;
    
    return Output;
}

//===================================================================
// Pixel Pixel Shader Structures and Methods
//===================================================================

// Input to the Pixel Shader
struct PixelPixelShaderInput
{
	float4 Color : COLOR0;
};

// Pixel Shader for drawing Pixels
float4 PixelPixelShader(PixelPixelShaderInput input) : COLOR0
{
	// Draw the Pixel with the specified Color
    return input.Color;    
}

//===================================================================
// Sprite Pixel Shader Structures and Methods
//===================================================================

// Input to the Pixel Shader
struct SpritePixelShaderInput
{
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

// Pixel Shader for drawing Sprites
float4 SpritePixelShader(SpritePixelShaderInput input) : COLOR0
{
	// Get the Texture Coordinate
	float2 TextureCoordinate = input.TextureCoordinate.xy;

    // Get the Color of the Texture at the specific Coordinate
    float4 Color = tex2D(TextureSampler, TextureCoordinate);
   
	// Blend the specified Color with the Texture's Color
	Color.rgb = (xColorBlendAmount * input.Color.rgb) + ((1 - xColorBlendAmount) * Color.rgb);
	
	// If this Pixel should not be completely transparent
	if (Color.a > 0.0)
	{
		// Scale the Texture's Alpha according to the specified Alpha
		Color.a -= ((1 - input.Color.a) * Color.a);
    }

    return Color;
}

//===================================================================
// Point Sprite Vertex Shader Structures and Methods
//===================================================================

// Input to the Vertex Shader
struct PointSpriteVertexShaderInput
{
    float4 Position : POSITION0;
	float1 Size		: PSIZE;
	float4 Color	: COLOR0;
	float1 Rotation : TEXCOORD0;
};

// Output from the Vertex Shader
struct PointSpriteVertexShaderOutput
{
    float4 Position	: POSITION0;
    float1 Size 	: PSIZE;
    float4 Color	: COLOR0;
    float4 Rotation	: COLOR1;
};

// Vertex Shader helper for computing the Rotation of a Point Sprite
float4 ComputePointSpriteRotationMatrix(float rotationAngleInRadians)
{    
    // Compute a 2x2 rotation matrix
    float c = cos(rotationAngleInRadians);
    float s = sin(rotationAngleInRadians);
    float4 RotationMatrix = float4(c, -s, s, c);
    
    // Normally we would output this matrix using a texture coordinate interpolator,
    // but texture coordinates are generated directly by the hardware when drawing
    // point sprites. So we have to use a color interpolator instead. Only trouble
    // is, color interpolators are clamped to the range 0 to 1. Our rotation values
    // range from -1 to 1, so we have to scale them to avoid unwanted clamping.
    RotationMatrix *= 0.5;
    RotationMatrix += 0.5;
    
    return RotationMatrix;
}

// Calculate the Position and Size of the Point Sprite on the screen
PointSpriteVertexShaderOutput PointSpriteVertexShader(PointSpriteVertexShaderInput input)
{
    PointSpriteVertexShaderOutput Output;

	// Calculate the transformed Position of the vertex on the screen
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
	Output.Position = mul(input.Position, preWorldViewProjection);

	// If Perspective Scaling should be performed
	if (xViewportHeight > 0)
	{
		// Scale the point sprite according to the distance it is from the Camera along with the Viewport Height
		Output.Size = input.Size * xProjection._m11 / Output.Position.w * xViewportHeight / 2;
    }
    // Else don't modify the Particle's size
    else
    {
		// Pass the Size through unchanged
		Output.Size = input.Size;
	}

	// Calculate the Rotation
	Output.Rotation = ComputePointSpriteRotationMatrix(input.Rotation);

	// Pass the Color through unchanged
	Output.Color = input.Color;
  
    return Output;
}

//===================================================================
// Point Sprite Pixel Shader Structures and Methods
//===================================================================

// Pixel Shader input structure for Point Sprites
struct PointSpritePixelShaderInput
{
    float4 Color : COLOR0;
    float4 Rotation : COLOR1;
    
    #ifdef XBOX 
        float4 TextureCoordinate : SPRITETEXCOORD; 
    #else 
        float2 TextureCoordinate : TEXCOORD0; 
    #endif 
};

// Pixel Shader for drawing Point Sprites. It is not actually
// possible to rotate a point sprite, so instead we rotate our texture
// coordinates. Leaving the sprite the regular way up but rotating the
// texture has the exact same effect as if we were able to rotate the
// point sprite itself.
float4 PointSpritePixelShader(PointSpritePixelShaderInput input) : COLOR0
{
	// Get the Texture Coordinate 
    #ifdef XBOX 
        float2 TextureCoordinate = abs(input.TextureCoordinate.zw);
    #else 
        float2 TextureCoordinate = input.TextureCoordinate.xy; 
    #endif 

	// We want to rotate around the middle of the Point Sprite, not the origin,
	// so we offset the texture coordinate accordingly.
	TextureCoordinate -= 0.5;
    
	// Apply the rotation matrix, after rescaling it back from the packed
	// color interpolator format into a full -1 to 1 range.
	float4 Rotation = input.Rotation * 2 - 1;
    
	TextureCoordinate = mul(TextureCoordinate, float2x2(Rotation));
    
	// Point sprites are squares. So are textures. When we rotate one square
	// inside another square, the corners of the texture will go past the
	// edge of the point sprite and get clipped. To avoid this, we scale
	// our texture coordinates to make sure the entire square can be rotated
	// inside the point sprite without any clipping.
	TextureCoordinate *= sqrt(2);
    
	// Undo the offset used to control the rotation origin.
	TextureCoordinate += 0.5;


	// Get the Color of the Texture at the specific rotated Coordinate
    float4 Color = tex2D(TextureSampler, TextureCoordinate);
    
	// Blend the specified Color with the Texture's Color
	Color.rgb = (xColorBlendAmount * input.Color.rgb) + ((1 - xColorBlendAmount) * Color.rgb);

	// If this Pixel should not be completely transparent
	if (Color.a > 0.0)
	{
		// Scale the Texture's Alpha according to the specified Alpha
		Color.a -= ((1 - input.Color.a) * Color.a);
    }
    
    return Color;
}

//===================================================================
// Quad Vertex Shader Structures and Methods
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

	// Pass the rest of the properties through unchanged
    Output.Color = input.Color;
    
    return Output;
}

//===================================================================
// Quad Pixel Shader Structures and Methods
//===================================================================

// Input to the Pixel Shader
struct QuadPixelShaderInput
{
	float4 Color : COLOR0;
};

// Pixel Shader for drawing Quads
float4 QuadPixelShader(QuadPixelShaderInput input) : COLOR0
{    
    return input.Color;
}

//===================================================================
// Textured Quad Vertex Shader Structures and Methods
//===================================================================

// Input to the Vertex Shader
struct TexturedQuadVertexShaderInput
{
	float4 Position				: POSITION0;
	float2 TextureCoordinate	: TEXCOORD0;
	float4 Color				: COLOR0;
};

// Output from the Vertex Shader
struct TexturedQuadVertexShaderOutput
{
	float4 Position				: POSITION0;
	float2 TextureCoordinate	: TEXCOORD0;
	float4 Color				: COLOR0;
};

// Calculate the Position and Color of the Quad on the screen
TexturedQuadVertexShaderOutput TexturedQuadVertexShader(TexturedQuadVertexShaderInput input)
{
    TexturedQuadVertexShaderOutput Output;

	// Calculate the transformed Position of the vertex on the screen
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
	Output.Position = mul(input.Position, preWorldViewProjection);

	// Pass the rest of the properties through unchanged
	Output.TextureCoordinate = input.TextureCoordinate;
    Output.Color = input.Color;
    
    return Output;
}

//===================================================================
// Textured Quad Pixel Shader Structures and Methods
//===================================================================

// Input to the Pixel Shader
struct TexturedQuadPixelShaderInput
{
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

// Pixel Shader for drawing Textured Quads
float4 TexturedQuadPixelShader(TexturedQuadPixelShaderInput input) : COLOR0
{
	// Get the Texture Coordinate
	float2 TextureCoordinate = input.TextureCoordinate.xy;

	// Get the Color of the Texture at the specific Coordinate
    float4 Color = tex2D(TextureSampler, TextureCoordinate);
    
	// Blend the specified Color with the Texture's Color
	Color.rgb = (xColorBlendAmount * input.Color.rgb) + ((1 - xColorBlendAmount) * Color.rgb);
	
	// If this Pixel should not be completely transparent
	if (Color.a > 0.0)
	{
		// Scale the Texture's Alpha according to the specified Alpha
		Color.a -= ((1 - input.Color.a) * Color.a);
    }
    
    return Color;
}

//===================================================================
// Techniques
//===================================================================
technique Pixels
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 PixelVertexShader();
		PixelShader = compile ps_2_0 PixelPixelShader();
	}
}

technique Sprites
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 SpritePixelShader();
	}
}

technique PointSprites
{
    pass Pass1
    {
		PointSpriteEnable = true;	// Make sure Point Sprites are enabled
        VertexShader = compile vs_2_0 PointSpriteVertexShader();
        PixelShader = compile ps_2_0 PointSpritePixelShader();
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