void main()
{
        // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord / iResolution.xy;

    // Time varying pixel color
    //vec3 col = 0.5 + 0.5 * cos(iTime + uv.xyx + vec3(0,2,4));
    vec3 col = vec3(uv, 0.f);

    // Output to screen
    fragColor = vec4(col,1.0);
}