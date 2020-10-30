#version 330

//opengl
out vec4 outputColor;
in vec2 texCoord;
in vec3 Normal;
in vec3 dir;

//set by camera
uniform vec4 mainLightColor;
uniform vec4 mainLightAmbiant;
uniform vec3 mainLightDirection;

// set by user

uniform vec4 nightColour;

uniform vec4 highColorDay;
uniform vec4 horizonColorDay;

void main()
{
    vec3 viewDir = dir;

    vec4 color = horizonColorDay;

    if(viewDir.y > 0)
    {
        color = mix(horizonColorDay, highColorDay, pow(viewDir.y, 0.4));
    }

    outputColor = color;//vec4(viewDir,1.0);
}