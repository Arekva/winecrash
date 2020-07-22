#version 330

out vec4 outputColor;
in vec2 texCoord;
in vec3 Normal;

uniform sampler2D albedo;
uniform vec4 color; 
uniform vec2 offset;
uniform vec2 tiling;

uniform vec3 lightDir;
uniform vec4 ambiant;
uniform vec4 lightColor;

void main()
{
    vec3 dir = normalize(lightDir);
    vec4 diffuse = texture(albedo, (texCoord * tiling) + offset);
    float alpha = diffuse.a;
    diffuse = color * lightColor * diffuse * max(dot(dir, Normal), 0.0) + ambiant;


    outputColor = vec4(diffuse.xyz, alpha);
}