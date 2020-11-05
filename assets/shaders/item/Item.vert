#version 330 core

layout(location = 0) in vec3 position;
layout(location = 1) in vec2 uv;
layout(location = 2) in vec3 normal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec4 rotation;

out mat4 transform;
out vec2 texCoord;

out vec3 objectNormal;
out vec3 viewNormal;
out vec3 worldNormal;

vec3 multVec4Vec3(vec4 rot, vec3 dir);

void main(void)
{
    mat4 mv = model * view;
    transform = mv * projection;

    gl_Position = vec4(position, 1.0) * transform;
    texCoord = uv;
    objectNormal = normalize(normal);
    

    viewNormal = normalize(transpose(inverse(mat3(transform))) * objectNormal);
    worldNormal = multVec4Vec3(rotation, objectNormal);

}

vec3 multVec4Vec3(vec4 rot, vec3 dir)
{
    float X = rotation.x * 2.0;
    float Y = rotation.y * 2.0;
    float Z = rotation.z * 2.0;
    float XX = rotation.x * X;
    float YY = rotation.y * Y;
    float ZZ = rotation.z * Z;
    float XY = rotation.x * Y;
    float XZ = rotation.x * Z;
    float YZ = rotation.y * Z;
    float WX = rotation.w * X;
    float WY = rotation.w * Y;
    float WZ = rotation.w * Z;

    vec3 res;
    res.x = (1.0 - (YY + ZZ)) * dir.x + (XY - WZ) * dir.y + (XZ + WY) * dir.z;
    res.y = (XY + WZ) * dir.x + (1.0 - (XX + ZZ)) * dir.y + (YZ - WX) * dir.z;
    res.z = (XZ - WY) * dir.x + (YZ + WX) * dir.y + (1.0 - (XX + YY)) * dir.z;
    return res;
}