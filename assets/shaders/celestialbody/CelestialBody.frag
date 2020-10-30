#version 330

out vec4 outputColor;
in vec2 texCoord;
in vec3 Normal;

uniform sampler2D albedo;
uniform vec4 color; 
uniform vec2 offset;
uniform vec2 tiling;

uniform vec3 lightDir;

uniform vec4 colorMult;

void main()
{
    vec4 diffuse = texture(albedo, (texCoord * tiling) + offset);

    vec4 multipliedColor = color + colorMult;

    outputColor = diffuse * multipliedColor;
}