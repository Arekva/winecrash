#version 330

out vec4 outputColor;
in vec3 Normal;
in vec4 localPosition;
in vec2 texCoord;
//in float border; 

uniform vec4 color;


#define border 0.005
float remap(float value, float low1, float high1, float low2, float high2)
{
    return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
}

void main()
{
    vec4 pos = localPosition + vec4(0.5);

    outputColor = pos;
    
    float alpha = 0.0;
    if(((pos.x < border || pos.x > 1.0-border) || (pos.z < border || pos.z > 1.0-border)) && ((pos.y < border || pos.y > 1.0-border) || (pos.x < border || pos.x > 1.0-border)) && ((pos.y < border || pos.y > 1.0-border) || (pos.z < border || pos.z > 1.0-border)))
    {
        alpha = 1.0;
    }
    outputColor = vec4(color.xyz, alpha);
}