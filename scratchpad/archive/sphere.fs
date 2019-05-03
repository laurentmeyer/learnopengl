// http://www.michaelwalczyk.com/blog/2017/5/25/ray-marching

mat4 translate(vec3 v) {
  mat4 res = mat4(1.);
  res[3].xyz = v;
  return res;
}

float sphereDistance(vec3 point, float sphereRadius) {
  return length(point) - sphereRadius;
}

float signedDistance(vec3 point) {
  const float radius = 1.;
  mat4 transform = translate(vec3(0., -1., 1. + 2. * 3. * iMouse.y / iResolution.y));
  float sphere_0 = sphereDistance((inverse(transform) * vec4(point, 1.)).xyz, radius);
  float displacement = 0.;
  displacement = sin(iTime + 5. * point.x) * sin(3. * iTime + point.y) * sin(7. * point.z) * .25;
  return sphere_0 + displacement;
}

vec3 calculateNormal(vec3 point) // http://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm
{
  const float epsilon = 0.001;
  const vec2 k = vec2(1., -1.);
  return normalize(k.xyy * signedDistance(point + epsilon * k.xyy) +
                   k.yyx * signedDistance(point + epsilon * k.yyx) +
                   k.yxy * signedDistance(point + epsilon * k.yxy) +
                   k.xxx * signedDistance(point + epsilon * k.xxx));
}

void mainImage(out vec4 fragColor, in vec2 fragCoord) {
  // Fragment
  vec2 centeredCoord = 2. * fragCoord - iResolution.xy;
  float maxResolution = max(iResolution.x, iResolution.y);
  vec2 uv = (centeredCoord / maxResolution);

  // Camera and Ray
  vec3 rayOrigin = vec3(uv, 0.);
  const float cameraDepth = .5;
  vec3 cameraPosition = vec3(0., 0., -cameraDepth);
  vec3 rayDirection = normalize(rayOrigin - cameraPosition);

  // Light
  const vec3 lightPosition = vec3(3.0, 2.0, .0);

  // Raymarching
  const float step = .1;
  const float maxDistance = 30.;
  const float hitPrecision = 0.01;
  const int maxSteps = int(maxDistance / step);
  vec3 col = vec3(0.);

  int iteration = 0;
  vec3 point = rayOrigin;
  while (iteration < maxSteps)
  {
    float distance = signedDistance(point);
    if (distance <= hitPrecision)
    {
      vec3 normal = calculateNormal(point);
      vec3 directionToLight = normalize(lightPosition - point);
      float diffuseIntensity = max(0., dot(normal, directionToLight));
      col = vec3(diffuseIntensity, diffuseIntensity, 0.);
      //col = normal / 2. + 0.5;
      break;
    }
    point += distance * rayDirection;
    iteration++;
  }

  fragColor = vec4(col, 1.0);
}