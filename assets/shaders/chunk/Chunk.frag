#version 440

out vec4 outputColor;
in vec2 texCoord;
in vec3 Normal;
in vec3 objPos;

uniform sampler2D albedo;
uniform vec4 color;

uniform float minLight;
uniform float maxLight;

layout (std430, binding=10) buffer chunk_lights
{
    uint lights[8192];
};

layout (std430, binding=11) buffer north_chunk_lights
{
    uint north_lights[8192];
};

layout (std430, binding=12) buffer south_chunk_lights
{
    uint south_lights[8192];
};

layout (std430, binding=13) buffer east_chunk_lights
{
    uint east_lights[8192];
};

layout (std430, binding=14) buffer west_chunk_lights
{
    uint west_lights[8192];
};

#define CHUNK_WIDTH 16
#define CHUNK_HEIGHT 256
#define CHUNK_DEPTH 16

#define LIGHT_MAX_LEVEL 15.0

// the number of light data is packed into a single uint. a light data is 4 bits
// and a uint is 32, so there is 8 lights packed into one int.
#define UINT_SIZE 32
#define LIGHT_DATA_SIZE 4

#define LIGHT_PACK_SIZE (UINT_SIZE/LIGHT_DATA_SIZE)

uint getLightLevel(int x, int y, int z)
{
    bool north = false;
    bool south = false;
    bool east = false;
    bool west = false;

    if(x < 0)
    {
        x = 15;
        east = true;
    }

    else if(x > CHUNK_WIDTH - 1)
    {
        x = 0;
        west = true;
    }

    else if(z < 0)
    {
        z = 15;
        south = true;
    }

    else if(z > CHUNK_DEPTH - 1)
    {
        z = 0;
        north = true;
    }

    int baseFullIndex = x + CHUNK_WIDTH * y + CHUNK_WIDTH * CHUNK_HEIGHT * z;
    
    int basePackedIndex = baseFullIndex / LIGHT_PACK_SIZE;
    int shiftPackedIndex = baseFullIndex % LIGHT_PACK_SIZE;

    uint mask = 0x0;

    switch (shiftPackedIndex)
    {
        case 0:
            mask = 0xF;
            break;   
        case 1:
            mask = 0xF0;
            break;
        case 2:
            mask = 0xF00;
            break;
        case 3:
            mask = 0xF000;
            break;
        case 4:
            mask = 0xF0000;
            break;
        case 5:
            mask = 0xF00000;
            break;
        case 6:
            mask = 0xF000000;
            break;
        case 7:
            mask = 0xF0000000;
            break;
    }

    if(west)
    {
        return (west_lights[basePackedIndex] & mask) >> (LIGHT_DATA_SIZE * shiftPackedIndex);
    }

    else if(east)
    {
        return (east_lights[basePackedIndex] & mask) >> (LIGHT_DATA_SIZE * shiftPackedIndex);
    }

    else if(north)
    {
        return (north_lights[basePackedIndex] & mask) >> (LIGHT_DATA_SIZE * shiftPackedIndex);
    }
    if(south)
    {
        return (south_lights[basePackedIndex] & mask) >> (LIGHT_DATA_SIZE * shiftPackedIndex);
    }

    else
    {
        return (lights[basePackedIndex] & mask) >> (LIGHT_DATA_SIZE * shiftPackedIndex);
    }
    
}

float getLight(float x, float y, float z)
{
    return float(getLightLevel(int(x), int(y), int(z))) / LIGHT_MAX_LEVEL;
}

float remap(float value, float low1, float high1, float low2, float high2)
{
    return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
}

void main()
{
    vec4 diffuse = texture(albedo, texCoord);

    vec3 blockpos = objPos + Normal * 0.5;
    
    float light = getLight(blockpos.x, blockpos.y, blockpos.z);



    outputColor = vec4(diffuse.xyz, 1.0) * color * remap(light, 0.0, 1.0, minLight, maxLight);//getLight(0,0,0);
}