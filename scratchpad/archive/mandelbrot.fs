void mainImage(out vec4 fragColor, in vec2 fragCoord) {
    vec2 uv = fragCoord / iResolution.xy;

    const int maxIteration = 100;
    vec2 c = uv * 4. - 2.;
    //vec2 c = iMouse.xy / iResolution.xy * 2. - 1.;

    int iteration = 0;
    vec2 z = vec2(0.);
    const float threshold = 50.;
    const float k = 2.;
    while (z.x * z.x + z.y * z.y < threshold && iteration < maxIteration)
    {
        float xtemp = z.x * z.x - z.y * z.y;
        z.y = 2. * z.x * z.y + c.y;
        z.x = xtemp + c.x;
        iteration++;
    }

    float smoothCol =
        float(iteration)
        - log2(log2(dot(z, z)) / (log2(threshold)))
        / log2(k);

    vec3 col = vec3(0.);
    if (iteration != maxIteration)
     col += 0.5 + 0.5*cos( 3.0 + smoothCol * 0.075 * k + vec3(0.0,0.6,1.0));

    fragColor = vec4(col, 1.0);
}