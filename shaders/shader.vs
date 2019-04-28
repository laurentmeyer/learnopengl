#version 330 core
    layout (location = 0) in vec3 aPos;
    out vec2 fragCoord;
    uniform vec3 iResolution;
    void main()
    {
        gl_Position = vec4(aPos, 1.f);
        fragCoord = (gl_Position.xy / 2.f + 0.5f) * iResolution.xy;
    }