void mainImage(out vec4 fragColor, in vec2 fragCoord) {
    vec2 uv = fragCoord / iResolution.xy;

    const int maxIteration = 100;
    vec2 c = iMouse.xy / iResolution.xy * 2. - 1.;
    vec2 z = uv * 2. - 1.;

    int iteration = 0;
    while (z.x * z.x + z.y * z.y < 4. && iteration < maxIteration)
    {
        float xtemp = z.x * z.x - z.y * z.y;
        z.y = 2. * z.x * z.y + c.y;
        z.x = xtemp + c.x;
        iteration++;
    }

    vec3 col = vec3(0.);
    if (iteration != maxIteration)
     col = vec3(fract(float(iteration) / 2.), fract(float(iteration) / 3.), fract(float(iteration) / 5.));

    fragColor = vec4(col, 1.0);
}