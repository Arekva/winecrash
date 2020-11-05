#version 330 core

layout(location = 0) in vec3 position;
layout(location = 1) in vec2 uv;
layout(location = 2) in vec3 normal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out mat4 transform;
out vec2 texCoord;
out vec3 Normal;

out vec3 dir;

void main(void)
{
    transform = model * view * projection;
    gl_Position = vec4(position, 1.0) * transform;

    dir = normalize(position);
    texCoord = uv;
    Normal = normalize(normal);
}