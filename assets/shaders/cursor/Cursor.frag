#version 440

out vec4 outputColor;
in vec3 Normal;
in vec4 localPosition;
in vec2 texCoord;


uniform float breakPct;


uniform sampler2D albedo0;
uniform sampler2D albedo1;
uniform sampler2D albedo2;
uniform sampler2D albedo3;
uniform sampler2D albedo4;
uniform sampler2D albedo5;
uniform sampler2D albedo6;
uniform sampler2D albedo7;
uniform sampler2D albedo8;
uniform sampler2D albedo9;

uniform float opacity;
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
    
    if(((pos.x < border || pos.x > 1.0-border) || (pos.z < border || pos.z > 1.0-border)) && ((pos.y < border || pos.y > 1.0-border) || (pos.x < border || pos.x > 1.0-border)) && ((pos.y < border || pos.y > 1.0-border) || (pos.z < border || pos.z > 1.0-border)))
    {
        outputColor = vec4(color.xyz, 1.0);
    }
    else
    {
        int texidx = int(breakPct * 10.0);

        switch(texidx)
        {
            case 0:
                outputColor = texture(albedo0, texCoord);
                break;
            case 1:
                outputColor = texture(albedo1, texCoord);
                break;
            case 2:
                outputColor = texture(albedo2, texCoord);
                break;
            case 3:
                outputColor = texture(albedo3, texCoord);
                break;
            case 4:
                outputColor = texture(albedo4, texCoord);
                break;
            case 5:
                outputColor = texture(albedo5, texCoord);
                break;
            case 6:
                outputColor = texture(albedo6, texCoord);
                break;

            case 7:
                outputColor = texture(albedo7, texCoord);
                break;

            case 8:
                outputColor = texture(albedo8, texCoord);
                break;

            case 9:
                outputColor = texture(albedo9, texCoord);
                break;

            default:
                outputColor = texture(albedo9, texCoord);
                break;
        }

        outputColor.w *= opacity;
    }
}