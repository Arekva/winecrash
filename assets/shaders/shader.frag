#version 400 core
out vec4 FragColor;
in vec3 gl_FragCoord;

float remap(float value, float oldLow, float oldHigh, float newLow, float newHigh)
{
    return newLow + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
}

void main()
{
    float x = remap(gl_FragCoord.x, -1.0f, 1.0f, 0.0f, 1.0f);
    float y = remap(gl_FragCoord.y, -1.0f, 1.0f, 0.0f, 1.0f);
    float z = remap(gl_FragCoord.z, -1.0f, 1.0f, 0.0f, 1.0f);

    FragColor = vec4(z, z, z, 1.0f);
}

