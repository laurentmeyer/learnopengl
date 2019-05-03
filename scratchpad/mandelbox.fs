// https://en.wikipedia.org/wiki/Mandelbox

mat4 translate(vec3 v) {
  mat4 res = mat4(1.);
  res[3].xyz = v;
  return res;
}

float sphereDistance(vec3 point, float sphereRadius) {
  return length(point) - sphereRadius;
}

float mandelDistance(vec3 p, float scale, float c){
  const vec3 constants = vec3(1., -1., 0.); // https://gamedev.stackexchange.com/a/77534
  bvec3 isGreater = greaterThan(p, constants.xxx);
  bvec3 isLower = lessThan(p, constants.yyy);
  vec3 higher = 2. - p;
  vec3 lower = -2. - p;
  p = mix(p, higher, isGreater);
  p = mix(p, lower, isLower);
  float magnitude = length(p);
  if (magnitude < 0.5)
    p *= 4.;
  else if (magnitude < 1.)
    p /= magnitude * magnitude;
  p = p * scale + c;

  return 0.;
}

float signedDistance(vec3 point) {
  mat4 transform = translate(vec3(0., 0., 3));
  vec3 transformedPoint = (inverse(transform) * vec4(point, 1.)).xyz;
  float mandel = sphereDistance((inverse(transform) * vec4(transformedPoint, 1.)).xyz, 1.);
  // float mandel = mandelDistance(transformedPoint, 1.5, 0.1);
  return mandel;
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
  const float hitPrecision = 0.01;
  const int maxSteps = 10;

  float totalDistance = 0.0;
  vec3 p = rayOrigin;
  int step = 0;
  while (step < maxSteps)
  {
    float distance = signedDistance(p);
    totalDistance += distance;
    if (distance < hitPrecision)
      break;
    p = rayOrigin + totalDistance * rayDirection;
    step++;
  }
  vec3 normal = calculateNormal(p);
  vec3 directionToLight = normalize(lightPosition - p);
  float diffuseIntensity = max(0., dot(normal, directionToLight));
  vec3 col = vec3(diffuseIntensity, diffuseIntensity, 0.);
  //col = vec3(1. - float(step) / float(maxSteps));
  fragColor = vec4(col, 1.0);
}