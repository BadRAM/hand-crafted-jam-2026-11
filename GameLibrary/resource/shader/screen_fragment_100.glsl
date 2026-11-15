#version 100

precision mediump float;

// Input vertex attributes (from vertex shader)
varying vec2 fragTexCoord;
varying vec4 fragColor;

// Input uniform values
uniform sampler2D texture0;
uniform sampler2D mask;




void main()
{
    vec4 maskColour = texture2D(mask, mod(fragTexCoord / (vec2(148, 256) / 15000.0), 1.0)) + 0.15;
    vec4 texelColor = texture2D(texture0, fragTexCoord);

    gl_FragColor = texelColor*maskColour;
    
    //col = mix(vec4(0.0,0.0,0.0,1.0), col, 0.5 + mask.x);
}