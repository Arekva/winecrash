#version 330

out vec4 outputColor;
in vec2 texCoord;
in vec3 Normal;
in vec3 vertPos;

uniform vec4 outlineColor;
uniform vec4 fillColor;

uniform vec2 resolution;

uniform vec3 extents;

float remap(float value, float low1, float high1, float low2, float high2)
{
    return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
}

void main()
{
    vec3 maxExtents = extents;
    vec3 minExtents = -maxExtents;


    float ratio = extents.x / extents.y;
    
    float xValue = abs(texCoord.x-0.5)*2.0;
    float yValue = abs(texCoord.y-0.5)*2.0;


    float outlineStrength = max(xValue,yValue);

    float outlinePower = 50.0;
    float outlineBlend = 3.0;

    
    vec4 outline = mix(fillColor, outlineColor, pow(outlineStrength,outlinePower) * outlineBlend);//vec4(resolution.x /1920, resolution.y / 1080,0.0,1.0);//outlineColor;
    //outline.w = outlineStrength * outlinePower;
    outputColor = outline;
}