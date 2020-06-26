#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D albedo;
uniform vec4 color;
//uniform sampler2D texture1;

void main()
{
    outputColor = color * texture(albedo, texCoord);//mix(texture(albedo, texCoord), texture(texture1, texCoord), 0.00001);
}