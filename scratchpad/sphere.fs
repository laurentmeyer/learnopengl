// http://www.michaelwalczyk.com/blog/2017/5/25/ray-marching

float sphereDistance(vec3 point, float sphereRadius) {
  return length(point) - sphereRadius;
}

float mapAroundPoint(vec3 point) {
  const float radius = 3.;
  float sphere_0 = sphereDistance(point, radius);
  return sphere_0;
}

vec3 calculateNormal(vec3 point)
{
  const vec2 smallStep = vec2(0.001, 0.);
  float gradient_x = mapAroundPoint(point + smallStep.xyy) - mapAroundPoint(point - smallStep.xyy);
  float gradient_y = mapAroundPoint(point + smallStep.yxy) - mapAroundPoint(point - smallStep.yxy);
  float gradient_z = mapAroundPoint(point + smallStep.yyx) - mapAroundPoint(point - smallStep.yyx);
  return normalize(vec3(gradient_x, gradient_y, gradient_z));
}

void mainImage(out vec4 fragColor, in vec2 fragCoord) {
  vec2 centeredCoord = 2. * fragCoord - iResolution.xy;
  float maxResolution = max(iResolution.x, iResolution.y);
  vec2 uv = (centeredCoord / maxResolution);

  const float distanceFromSphere = 5.;
  const float cameraDepth = .5;
  vec3 rayOrigin = vec3(uv, -distanceFromSphere);
  //vec3 rayOrigin = vec3(fragCoord * 2. - iResolution.xy, -distanceFromSphere);
  vec3 cameraPosition = vec3(0., 0., rayOrigin.z - cameraDepth);
  vec3 rayDirection = normalize(rayOrigin - cameraPosition);

  const float step = .1;
  const float max = 10.;
  vec3 col = vec3(1., 1., 1.);

  vec3 point = rayOrigin;
  while (length(point - rayOrigin) < max)
  {
    if (mapAroundPoint(point) < 0.)
    {
      col = calculateNormal(point);
      break;
    }
    point += step * rayDirection;
  }

  fragColor = vec4(col, 1.0);
  //fragColor = vec4(uv, 0., 1.);
}