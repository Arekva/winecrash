#version 330

out vec4 outputColor;
in vec2 texCoord;
in vec3 Normal;

uniform sampler2D albedo;
uniform vec4 color; 
uniform vec2 offset;
uniform vec2 tiling;

uniform vec3 lightDir;

void main()
{
    vec4 diffuse = texture(albedo, texCoord);

    outputColor = diffuse;// * color;
    outputColor.a = 1.0;
}