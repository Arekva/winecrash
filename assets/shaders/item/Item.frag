#version 440

out vec4 outputColor;
in vec2 texCoord;
in vec3 objectNormal;
in vec3 worldNormal;
in vec3 viewNormal;
in vec3 objPos;

uniform sampler2D albedo;
uniform vec4 color;
uniform float showPercent;

uniform float minLight;
uniform float maxLight;

#define LIGHT_MAX_LEVEL 15.0

// the number of light data is packed into a single uint. a light data is 4 bits
// and a uint is 32, so there is 8 lights packed into one int.
#define UINT_SIZE 32
#define LIGHT_DATA_SIZE 4

#define LIGHT_PACK_SIZE (UINT_SIZE/LIGHT_DATA_SIZE)

/*uint getLightLevel(int x, int y, int z)
{
    bool north = false;
    bool south = false;
    bool east = false;
    bool west = false;

    if(x < 0)
    {
        x = 15;
        west = true;
    }

    else if(x > CHUNK_WIDTH - 1)
    {
        x = 0;
        east = true;
    }

    if(z < 0)
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
            mask = 0x0000000F;
            break;   
        case 1:
            mask = 0x000000F0;
            break;
        case 2:
            mask = 0x00000F00;
            break;
        case 3:
            mask = 0x0000F000;
            break;
        case 4:
            mask = 0x000F0000;
            break;
        case 5:
            mask = 0x00F00000;
            break;
        case 6:
            mask = 0x0F000000;
            break;
        case 7:
            mask = 0xF0000000;
            break;
    }

    if(west)
    {
        return (west_lights[basePackedIndex] & mask) >> (LIGHT_DATA_SIZE * shiftPackedIndex);
    }

    if(east)
    {
        return (east_lights[basePackedIndex] & mask) >> (LIGHT_DATA_SIZE * shiftPackedIndex);
    }

    if(north)
    {
        return (north_lights[basePackedIndex] & mask) >> (LIGHT_DATA_SIZE * shiftPackedIndex);
    }

    if(south)
    {
        return (south_lights[basePackedIndex] & mask) >> (LIGHT_DATA_SIZE * shiftPackedIndex);
    }

    return (lights[basePackedIndex] & mask) >> (LIGHT_DATA_SIZE * shiftPackedIndex);
}

float getLight(float x, float y, float z)
{
    return float(getLightLevel(int(x), int(y), int(z)));
}*/

vec4 lerp(vec4 v0, vec4 v1, float t)
{
  return (1.0 - t) * v0 + t * v1;
}

float remap(float value, float low1, float high1, float low2, float high2)
{
    return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
}

void main()
{
    vec4 diffuse = texture(albedo, texCoord);
    float alpha = (diffuse * color).w;

    vec3 spotDir = normalize(vec3(1.0));

    float light = dot(worldNormal, spotDir);
    


    vec4 chunkCol = vec4(diffuse.xyz, 1.0) * color * remap(light, 0.0, 1.0, minLight, maxLight);//getLight(0,0,0);
    chunkCol.a = alpha;

    outputColor = chunkCol;
}