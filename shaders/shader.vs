#version 330 core
layout(location = 0) in vec3 aPos;

in vec4 MCvertex;
in vec3 MCnormal;

uniform vec3 LightPosition;
uniform mat4 MVMatrix;
uniform mat4 MVPMatrix;
uniform mat4 NormalMatrix;

const float SpecularContribution = 0.3;
const float DiffuseContribution = 1.0 - SpecularContribution;

out vec2 fragCoord;
uniform vec3 iResolution;

void main() {
  gl_Position = vec4(aPos, 1.f);
  fragCoord = (gl_Position.xy / 2.f + 0.5f) * iResolution.xy;
}