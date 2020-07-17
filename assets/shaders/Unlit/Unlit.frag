#version 330

out vec4 outputColor;
in vec2 texCoord;
in vec3 Normal;

uniform sampler2D albedo;
uniform vec4 color;

void main()
{
    vec4 diffuse = texture(albedo, texCoord);

    outputColor = diffuse * color;
}