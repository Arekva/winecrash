#version 330

//opengl
out vec4 outputColor;
in vec2 texCoord;
in vec3 Normal;
in vec3 dir;

//set by camera
uniform vec4 mainLightColor;
uniform vec4 mainLightAmbiant;
uniform vec3 mainLightDirection;

// set by user
uniform vec4 horizonColourDay;
uniform vec4 horizonColourSunset;
uniform vec4 nightColour;
uniform vec4 highAtmosphereColour;
uniform vec4 groundAtmosphereColour;

uniform float sunSize;
uniform vec4 sunInnerColor;
uniform vec4 sunOuterColor;



void main()
{
    float dist = distance(mainLightDirection, dir);
    if(dir.y > 0.0 && dist < sunSize)
    {
        outputColor = mix(sunInnerColor, sunOuterColor, pow(dist / sunSize, 5));
        return;
    }

    else
    {
        vec4 col = vec4(0.0,0.0,0.0,1.0);

        float sunAltitude = 1.0;

        bool negative = mainLightDirection.y < 0.0;
        sunAltitude = abs(mainLightDirection.y);

        vec4 DenseAtmosphereColour = mix(horizonColourSunset, horizonColourDay, sqrt(sunAltitude));
        vec4 HighAtmosphereColour = mix(nightColour, highAtmosphereColour, pow(sunAltitude, 0.6));
        vec4 GroundAtmosphereColour = mix(nightColour, groundAtmosphereColour, pow(sunAltitude, 0.6));

        if(negative)
        {
            DenseAtmosphereColour = mix(horizonColourSunset, nightColour, sqrt(sunAltitude));
            HighAtmosphereColour = nightColour;
            GroundAtmosphereColour = nightColour;
        }

        if(dir.y > 0.0)
        {
            col = mix(DenseAtmosphereColour, HighAtmosphereColour, pow(dir.y, 0.8));
        }

        else
        {
            col = mix(DenseAtmosphereColour, GroundAtmosphereColour, pow(abs(dir.y), 0.25));
        }

        outputColor = col;
    }
}