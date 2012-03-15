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
float xViewportHeight = 0;		// Set this to zero to not use perspective scaling (i.e. don't re-size Point Sprites)

// The Width and Height of the Texture to take the Texture Coordinates from.
// These must be set if using the PointSpritesTextureCoordinates technique.
float xTextureWidth = 0;
float xTextureHeight = 0;

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
	float2 NormalizedTextureCoordinate : TEXCOORD0;
};

// Pixel Shader for drawing Sprites
float4 SpritePixelShader(SpritePixelShaderInput input) : COLOR0
{
	// Get the Texture Coordinate
	float2 NormalizedTextureCoordinate = input.NormalizedTextureCoordinate.xy;

    // Get the Color of the Texture at the specific Coordinate
    float4 Color = tex2D(TextureSampler, NormalizedTextureCoordinate);
   
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

// Input to the Vertex Shader for Point Sprites with no Texture Coordinates
struct PointSpriteVertexShaderInput
{
    float4 Position : POSITION0;
	float1 Size		: PSIZE;
	float4 Color	: COLOR0;
	float1 Rotation : NORMAL0;
};

// Output from the Vertex Shader for Point Sprites with no Texture Coordinates
struct PointSpriteVertexShaderOutput
{
    float4 Position	: POSITION0;
    float1 Size 	: PSIZE;
    float4 Color	: COLOR0;
    float4 Rotation	: COLOR1;
};

// Input to the Vertex Shader for Point Sprites with Texture Coordinates, but no Rotation
struct PointSpriteTextureCoordinatesNoRotationVertexShaderInput
{
    float4 Position : POSITION0;
	float1 Size		: PSIZE;
	float4 Color	: COLOR0;
	float4 TextureCoordinateRange : TEXCOORD0;
};

// Output from the Vertex Shader for Point Sprites with Texture Coordinates, but no Rotation
struct PointSpriteTextureCoordinatesNoRotationVertexShaderOutput
{
    float4 Position	: POSITION0;
    float1 Size 	: PSIZE;
    float4 Color	: COLOR0;
    float4 TextureCoordinateRange : COLOR1;
};

// Input to the Vertex Shader for Point Sprites with Texture Coordinates, but no Color
struct PointSpriteTextureCoordinatesNoColorVertexShaderInput
{
    float4 Position : POSITION0;
	float1 Size		: PSIZE;
	float1 Rotation : NORMAL0;
	float4 TextureCoordinateRange : TEXCOORD0;
};

// Output from the Vertex Shader for Point Sprites with Texture Coordinates, but no Color
struct PointSpriteTextureCoordinatesNoColorVertexShaderOutput
{
    float4 Position	: POSITION0;
    float1 Size 	: PSIZE;
    float4 Rotation	: COLOR0;
    float4 TextureCoordinateRange : COLOR1;
};

// Input to the Vertex Shader for Point Sprites with Texture Coordinates, Rotation, and Color
struct PointSpriteTextureCoordinatesVertexShaderInput
{
    float4 Position : POSITION0;
	float1 Size		: PSIZE;
	float4 Color	: COLOR0;
	float1 Rotation : NORMAL0;
	float4 TextureCoordinateRange : TEXCOORD0;
};

// Output from the Vertex Shader for Point Sprites with Texture Coordinates, Rotation, and Color
struct PointSpriteTextureCoordinatesVertexShaderOutput
{
    float4 Position	: POSITION0;
    float1 Size 	: PSIZE;
    float4 Rotation	: COLOR0;
    float4 TextureCoordinateRangeAndColor : COLOR1;
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

// Vertex Shader for Point Sprites with no Texture Coordinates
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

// Vertex Shader for Point Sprites with Texture Coordinates, but no Rotation
PointSpriteTextureCoordinatesNoRotationVertexShaderOutput PointSpriteTextureCoordinatesNoRotationVertexShader(PointSpriteTextureCoordinatesNoRotationVertexShaderInput input)
{
    PointSpriteTextureCoordinatesNoRotationVertexShaderOutput Output;

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

	// Pass the Color through unchanged
	Output.Color = input.Color;
  
	// Pass the Texture Coordinates through unchanged
	Output.TextureCoordinateRange = input.TextureCoordinateRange;
  
    return Output;
}

// Vertex Shader for Point Sprites with Texture Coordinates, but no Color
PointSpriteTextureCoordinatesNoColorVertexShaderOutput PointSpriteTextureCoordinatesNoColorVertexShader(PointSpriteTextureCoordinatesNoColorVertexShaderInput input)
{
    PointSpriteTextureCoordinatesNoColorVertexShaderOutput Output;

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
  
	// Pass the Texture Coordinates through unchanged
	Output.TextureCoordinateRange = input.TextureCoordinateRange;
  
    return Output;
}

// Vertex Shader for Point Sprites with Texture Coordinates, Rotation, and Color
PointSpriteTextureCoordinatesVertexShaderOutput PointSpriteTextureCoordinatesVertexShader(PointSpriteTextureCoordinatesVertexShaderInput input)
{
    PointSpriteTextureCoordinatesVertexShaderOutput Output;

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
  
  
	// Because the pixel shader can only accept COLOR0 and COLOR1 semantics in Windows, we
	// have to pack the Color and TextureCoordinates into a single Vector4 of floats.
	// Because a float supports 7 significant digits, we convert the Color to be between
	// 0 - 255, then divide by 1000 to put the 3 digits into the decimal place. The whole
	// digits will be used for the non-normalized texture coordinates between 1 - 5120.
	// So the color uses 3 significant digits, and the texture coordinates use up to 4 significant digits.
	// Also, because the COLOR semantic only passes through values between 0.0 - 1.0, we
	// divide the values by 10,000 before passing them to the pixel shader, so that all 7
	// digits are moved into the decimal portion of the values.
  
	// Convert the Color values to be between 0 - 255 and divide by 1000 to make them fractions
	// so they only occupy 3 digits in the the decimal portion of the values.
	Output.TextureCoordinateRangeAndColor.r = ((int)(input.Color.r * 255)) / 1000.0f;
	Output.TextureCoordinateRangeAndColor.g = ((int)(input.Color.g * 255)) / 1000.0f;
	Output.TextureCoordinateRangeAndColor.b = ((int)(input.Color.b * 255)) / 1000.0f;
	Output.TextureCoordinateRangeAndColor.a = ((int)(input.Color.a * 255)) / 1000.0f;
  
	// Pass the Texture Coordinates through unchanged. We must first un-normalize them so they do
	// not contain any decimal values (all whole numbers).
	input.TextureCoordinateRange.x = (int)(input.TextureCoordinateRange.x * xTextureWidth);
	input.TextureCoordinateRange.y = (int)(input.TextureCoordinateRange.y * xTextureHeight);
	input.TextureCoordinateRange.w = (int)(input.TextureCoordinateRange.w * xTextureWidth);
	input.TextureCoordinateRange.z = (int)(input.TextureCoordinateRange.z * xTextureHeight);
	Output.TextureCoordinateRangeAndColor += input.TextureCoordinateRange;
	
	// Scale the values down so that all digits are in the decimal part of the values, with the first 
	// 4 digits representing the Texture Coordinates, and the last 3 digits representing the Color.
	Output.TextureCoordinateRangeAndColor /= 10000.0f;
  
    return Output;
}

//===================================================================
// Point Sprite Pixel Shader Structures and Methods
//===================================================================

// Pixel Shader input structure for Point Sprites with no Texture Coordinates
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

// Pixel Shader input structure for Point Sprites with Texture Coordinates, but no Rotation
struct PointSpriteTextureCoordinatesNoRotationPixelShaderInput
{
    float4 Color : COLOR0;
    float4 TextureCoordinateRange : COLOR1;
    
    #ifdef XBOX 
        float4 TextureCoordinate : SPRITETEXCOORD; 
    #else 
        float2 TextureCoordinate : TEXCOORD0; 
    #endif 
};

// Pixel Shader input structure for Point Sprites with Texture Coordinates, but no Color
struct PointSpriteTextureCoordinatesNoColorPixelShaderInput
{
    float4 Rotation : COLOR0;
    float4 TextureCoordinateRange : COLOR1;
    
    #ifdef XBOX 
        float4 TextureCoordinate : SPRITETEXCOORD; 
    #else 
        float2 TextureCoordinate : TEXCOORD0; 
    #endif 
};

// Pixel Shader input structure for Point Sprites with Texture Coordinates, Rotation, and Color
struct PointSpriteTextureCoordinatesPixelShaderInput
{
    float4 Rotation : COLOR0;
    float4 TextureCoordinateRangeAndColor : COLOR1;
    
    #ifdef XBOX 
        float4 TextureCoordinate : SPRITETEXCOORD; 
    #else 
        float2 TextureCoordinate : TEXCOORD0; 
    #endif 
};

// Pixel Shader for drawing Point Sprites with no Texture Coordinates
float4 PointSpritePixelShader(PointSpritePixelShaderInput input) : COLOR0
{
	// Get the Texture Coordinate 
    #ifdef XBOX 
        float2 TextureCoordinate = abs(input.TextureCoordinate.zw);
    #else 
        float2 TextureCoordinate = input.TextureCoordinate.xy; 
    #endif 

	// It is not actually possible to rotate a point sprite, so instead we rotate our texture coordinates.
	// Leaving the sprite the regular way up but rotating the texture has the exact same effect as if we 
	// were able to rotate the point sprite itself.

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

// Pixel Shader for drawing Point Sprites with Texture Coordinates, but no Rotation
float4 PointSpriteTextureCoordinatesNoRotationPixelShader(PointSpriteTextureCoordinatesNoRotationPixelShaderInput input) : COLOR0
{
	// Get the Texture Coordinate 
    #ifdef XBOX 
        float2 TextureCoordinate = abs(input.TextureCoordinate.zw);
    #else 
        float2 TextureCoordinate = input.TextureCoordinate.xy; 
    #endif 
   
    // Calculate the Width and Height of the Texture Coordinate Range
    float RangeWidth = input.TextureCoordinateRange.w - input.TextureCoordinateRange.x;
    float RangeHeight = input.TextureCoordinateRange.z - input.TextureCoordinateRange.y;
    
    // Calculate the real Texture Coordinate position within the Texture Coordinate Range
    TextureCoordinate.x = input.TextureCoordinateRange.x + (TextureCoordinate.x * RangeWidth);
    TextureCoordinate.y = input.TextureCoordinateRange.y + (TextureCoordinate.y * RangeHeight);
    

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

// Pixel Shader for drawing Point Sprites with Texture Coordinates, but no Color
float4 PointSpriteTextureCoordinatesNoColorPixelShader(PointSpriteTextureCoordinatesNoColorPixelShaderInput input) : COLOR0
{
	// Get the Texture Coordinate 
    #ifdef XBOX 
        float2 TextureCoordinate = abs(input.TextureCoordinate.zw);
    #else 
        float2 TextureCoordinate = input.TextureCoordinate.xy; 
    #endif    
    
    // It is not actually possible to rotate a point sprite, so instead we rotate our texture coordinates.
	// Leaving the sprite the regular way up but rotating the texture has the exact same effect as if we 
	// were able to rotate the point sprite itself.
    
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
	
	
	// Make sure the Texture Coordinates are still in the range 0 - 1
	TextureCoordinate = clamp(TextureCoordinate, 0.0, 1.0);	
	
	// Calculate the Width and Height of the Texture Coordinate Range
    float RangeWidth = input.TextureCoordinateRange.w - input.TextureCoordinateRange.x;
    float RangeHeight = input.TextureCoordinateRange.z - input.TextureCoordinateRange.y;
    
    // Calculate the real Texture Coordinate position within the Texture Coordinate Range
    TextureCoordinate.x = input.TextureCoordinateRange.x + (TextureCoordinate.x * RangeWidth);
    TextureCoordinate.y = input.TextureCoordinateRange.y + (TextureCoordinate.y * RangeHeight);


	// Get the Color of the Texture at the specific rotated Coordinate
    float4 Color = tex2D(TextureSampler, TextureCoordinate);
    
    return Color;
}

// Pixel Shader for drawing Point Sprites with Texture Coordinates, Rotation, and Color
float4 PointSpriteTextureCoordinatesPixelShader(PointSpriteTextureCoordinatesPixelShaderInput input) : COLOR0
{
	// Get the Texture Coordinate 
    #ifdef XBOX 
        float2 TextureCoordinate = abs(input.TextureCoordinate.zw);
    #else 
        float2 TextureCoordinate = input.TextureCoordinate.xy; 
    #endif
   
    // Scale the values up so that the Texture Coordinates are whole numbers part and the Color is the decimal part (i.e. TTTT.CCC)
    input.TextureCoordinateRangeAndColor *= 10000;
    
    // Unpack the Texture Coordinate Range (whole number part) and Color (decimal part) into their own variables
    float4 InputTextureCoordinateRange;
    float4 InputColor = modf(input.TextureCoordinateRangeAndColor, InputTextureCoordinateRange);

	// Adjust for floating-point errors. 
	// Sometimes when a value such as 0.105000 is passed to the pixel shader, it gets changed to something like 0.104999 due
	// to floating-point errors. Then when we scale it back up by 10,000 we end up with 104.999 instead of 105.000.
	// Because we know that all of the Color values (i.e. last 3 digits) should be between 0 - 255, if the Color value is
	// greater than 256, then we know that this situation has occurred, so we must correct it. We could test for value > 0.9,
	// but since we know that the max the Color value should be is 256, I test for value > 0.3 (should not make a difference).
	// We must test and correct this for all 4 components of the Color and Texture Coordinates.
	if (InputColor.r > 0.3)
	{
		InputColor.r = 0;
		InputTextureCoordinateRange.r++;
	}
	if (InputColor.g > 0.3)
	{
		InputColor.g = 0;
		InputTextureCoordinateRange.g++;
	}
	if (InputColor.b > 0.3)
	{
		InputColor.b = 0;
		InputTextureCoordinateRange.b++;
	}
	if (InputColor.a > 0.3)
	{
		InputColor.a = 0;
		InputTextureCoordinateRange.a++;
	}

    // Normalize the Texture Coordinate Range between 0.0 - 1.0
    InputTextureCoordinateRange.x /= xTextureWidth;
    InputTextureCoordinateRange.y /= xTextureHeight;
    InputTextureCoordinateRange.w /= xTextureWidth;
    InputTextureCoordinateRange.z /= xTextureHeight;

    // Normalize the Color values between 0.0 - 1.0
    InputColor.r = (InputColor.r * 1000) / 255.0f;
    InputColor.g = (InputColor.g * 1000) / 255.0f;
    InputColor.b = (InputColor.b * 1000) / 255.0f;
    InputColor.a = (InputColor.a * 1000) / 255.0f;    
    InputColor = clamp(InputColor, 0.0, 1.0);


    // It is not actually possible to rotate a point sprite, so instead we rotate our texture coordinates.
	// Leaving the sprite the regular way up but rotating the texture has the exact same effect as if we 
	// were able to rotate the point sprite itself.
	
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
	
	
	// Make sure the Texture Coordinates are still in the range 0 - 1
	TextureCoordinate = clamp(TextureCoordinate, 0.0, 1.0);	
	
	// Calculate the Width and Height of the Texture Coordinate Range
    float RangeWidth = InputTextureCoordinateRange.w - InputTextureCoordinateRange.x;
    float RangeHeight = InputTextureCoordinateRange.z - InputTextureCoordinateRange.y;
    
    // Calculate the real Texture Coordinate position within the Texture Coordinate Range
    TextureCoordinate.x = InputTextureCoordinateRange.x + (TextureCoordinate.x * RangeWidth);
    TextureCoordinate.y = InputTextureCoordinateRange.y + (TextureCoordinate.y * RangeHeight);


	// Get the Color of the Texture at the specific rotated Coordinate
    float4 Color = tex2D(TextureSampler, TextureCoordinate);
    
    // Blend the specified Color with the Texture's Color
	Color.rgb = (xColorBlendAmount * InputColor.rgb) + ((1 - xColorBlendAmount) * Color.rgb);

	// If this Pixel should not be completely transparent
	if (Color.a > 0.0)
	{
		// Scale the Texture's Alpha according to the specified Alpha
		Color.a -= ((1 - InputColor.a) * Color.a);
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
	float4 Position						: POSITION0;
	float2 NormalizedTextureCoordinate	: TEXCOORD0;
	float4 Color						: COLOR0;
};

// Output from the Vertex Shader
struct TexturedQuadVertexShaderOutput
{
	float4 Position						: POSITION0;
	float2 NormalizedTextureCoordinate	: TEXCOORD0;
	float4 Color						: COLOR0;
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
	Output.NormalizedTextureCoordinate = input.NormalizedTextureCoordinate;
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
	float2 NormalizedTextureCoordinate : TEXCOORD0;
};

// Pixel Shader for drawing Textured Quads
float4 TexturedQuadPixelShader(TexturedQuadPixelShaderInput input) : COLOR0
{
	// Get the Texture Coordinate
	float2 NormalizedTextureCoordinate = input.NormalizedTextureCoordinate.xy;

	// Get the Color of the Texture at the specific Coordinate
    float4 Color = tex2D(TextureSampler, NormalizedTextureCoordinate);
    
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

technique PointSpritesTextureCoordinatesNoRotation
{
	pass Pass1
	{
		PointSpriteEnable = true;	// Make sure Point Sprites are enabled
        VertexShader = compile vs_2_0 PointSpriteTextureCoordinatesNoRotationVertexShader();
        PixelShader = compile ps_2_0 PointSpriteTextureCoordinatesNoRotationPixelShader();
	}
}

technique PointSpritesTextureCoordinatesNoColor
{
	pass Pass1
	{
		PointSpriteEnable = true;	// Make sure Point Sprites are enabled
        VertexShader = compile vs_2_0 PointSpriteTextureCoordinatesNoColorVertexShader();
        PixelShader = compile ps_2_0 PointSpriteTextureCoordinatesNoColorPixelShader();
	}
}

technique PointSpritesTextureCoordinates
{
	pass Pass1
	{
		PointSpriteEnable = true;	// Make sure Point Sprites are enabled
        VertexShader = compile vs_2_0 PointSpriteTextureCoordinatesVertexShader();
        PixelShader = compile ps_2_0 PointSpriteTextureCoordinatesPixelShader();
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