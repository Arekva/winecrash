#version 330

out vec4 outputColor;

in vec2 texCoord;

in vec3 Normal;

uniform sampler2D albedo;
uniform vec4 color;
uniform vec4 mainLightColor;
uniform vec4 mainLightAmbiant;
uniform vec3 mainLightDirection;

uniform int debug;
//uniform sampler2D texture1;

float remap(float value, float oldLow, float oldHigh, float newLow, float newHigh)
{
    return newLow + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
}

vec3 remap_v3(vec3 value, float oldLow, float oldHigh, float newLow, float newHigh)
{
    value.x = remap(value.x, oldLow, oldHigh, newLow, newHigh);
    value.y = remap(value.y, oldLow, oldHigh, newLow, newHigh);
    value.z = remap(value.z, oldLow, oldHigh, newLow, newHigh);
    return value;
}
void main()
{
    // debug normals : outputColor = vec4(remap_v3(normalize(Normal), -1.0, 1.0, 0.0, 1.0), 1.0);
    // debug uvs : outputColor = vec4(texCoord, 0.0, 1.0);

    vec4 diffuse = texture(albedo, texCoord);

    //apply light
    //diffuse = color * mainLightColor * diffuse * max(dot(mainLightDirection, Normal), 0.0) + mainLightAmbiant;

    if(debug == 1)
    {
        outputColor = vec4(remap_v3(normalize(Normal), -1.0, 1.0, 0.0, 1.0), 1.0);
    }

    else
    {
        outputColor = vec4(diffuse.xyz, 1.0);
    }
}