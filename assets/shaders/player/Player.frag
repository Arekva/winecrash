#version 330

out vec4 outputColor;
in vec2 texCoord;
in vec3 objectNormal;
in vec3 worldNormal;

uniform sampler2D albedo;
uniform vec4 color; 
uniform vec2 offset;
uniform vec2 tiling;

uniform vec3 lightDir;

void main()
{
    vec3 lightdir = normalize(vec3(1.0));

    float lightPct = dot(vec3(abs(worldNormal.x), abs(worldNormal.y), abs(worldNormal.z)), lightDir);
    
    vec4 diffuse = texture(albedo, texCoord);

    outputColor = diffuse;//vec4(worldNormal, 1.0);//diffuse * lightPct;// * color;
    outputColor.a = 1.0;
}