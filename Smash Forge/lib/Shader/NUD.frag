#version 330

in vec3 viewPosition;
in vec3 objectPosition;
in vec2 texCoord;
in vec2 texCoord2;
in vec2 texCoord3;
in vec2 normaltexCoord;
in vec4 vertexColor;
in vec3 normal;
in vec3 viewNormal;
in vec3 tangent;
in vec3 bitangent;

layout (location = 0) out vec4 fragColor;
layout (location = 1) out vec4 fragColorBright;

#include MISC_UNIFORMS

#include STAGE_LIGHTING_UNIFORMS

#include NU_UNIFORMS

// Constants
#define gamma 2.2
#define PI 3.14159

#include SMASH_SHADER

void main()
{
    fragColor = SmashShader();
    // Separate bright and regular color for bloom calculations.
    fragColorBright = vec4(vec3(0), 1);
    if (Luminance(fragColor.rgb) > bloomThreshold)
        fragColorBright.rgb = fragColor.rgb;
}