#version 330 core

layout(location = 0) in vec3 position;
layout(location = 1) in vec2 uv;
layout(location = 2) in vec3 normal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 worldView;
uniform mat4 projection;
uniform vec4 rotation;

out mat4 transform;
out vec2 texCoord;

out vec3 objectNormal;
out vec3 viewNormal;
out vec3 worldNormal;

out vec3 worldSpacePosition;
//out vec3 worldSpacePosition;

#define DEG_TO_RAD 0.01745329251

vec4 rotFromAngle(float xAngle,float yAngle,float zAngle);
vec3 multVec4Vec3(vec4 rot, vec3 dir);
vec3 rotateAround(vec3 point, vec3 pivot, vec4 rot);

void main(void)
{
    mat4 mv = model * view;
    mat4 worldMv = model * worldView;
    transform = mv * projection;

    gl_Position = vec4(position, 1.0) * transform;
    vec4 worldPosition = vec4(position, 1.0) * worldMv;

    worldSpacePosition = vec3(worldPosition.x, worldPosition.y, worldPosition.z);

    // fun with spherical world
    //float dist = length(worldSpacePosition);
    //vec4 sphereRot = rotFromAngle(dist * 0.01, 0, 0);
    //vec3 sphericalPos = rotateAround(worldSpacePosition, vec3(0, -512, 0), sphereRot);

    //gl_Position.xyz -= sphericalPos;


    texCoord = uv;
    objectNormal = normalize(normal);

    viewNormal = normalize(transpose(inverse(mat3(transform))) * objectNormal);
    worldNormal = multVec4Vec3(rotation, objectNormal);

    
}

vec3 rotateAround(vec3 point, vec3 pivot, vec4 rot)
{
    return multVec4Vec3(rot, (point - pivot)) + pivot;
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

vec4 rotFromAngle(float xAngle, float yAngle, float zAngle)
{
    //xAngle *= DEG_TO_RAD;
    //yAngle *= DEG_TO_RAD;
    //zAngle *= DEG_TO_RAD;

    //  Roll first, about axis the object is facing, then
    //  pitch upward, then yaw to face into the new heading
    float sr, cr, sp, cp, sy, cy;

    float halfRoll = zAngle * 0.5;
    sr = sin(halfRoll);
    cr = cos(halfRoll);

    float halfPitch = xAngle * 0.5;
    sp = sin(halfPitch);
    cp = cos(halfPitch);

    float halfYaw = yAngle * 0.5;
    sy = sin(halfYaw);
    cy = cos(halfYaw);

    float x = cy * sp * cr + sy * cp * sr;
    float y = sy * cp * cr - cy * sp * sr;
    float z = cy * cp * sr - sy * sp * cr;
    float w = cy * cp * cr + sy * sp * sr;

    return vec4(x,y,z,w);
}