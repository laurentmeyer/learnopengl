// tetrahedron: https://www.shadertoy.com/view/ltVGzy

#define FOV 60.0

#define MAX_STEP   32
#define MAX_STEP_F 32.0

// constants
const float infinity = 1.0/0.000000001;
const float PI = asin(1.0)*2.0;


// min/max vec
float max4(in vec4 v4) {
    return max( max(v4.x, v4.y), max(v4.z, v4.w) );
}

float min4(in vec4 v4) {
    return min( min(v4.x, v4.y), min(v4.z, v4.w) );
}

// rotations
vec3 rotateX(in vec3 vec, in float rad) {
    
    return vec3(
        vec[0],
        vec[1]*cos(rad) - vec[2]*sin(rad),
        vec[1]*sin(rad) + vec[2]*cos(rad)
    );
        
}

vec3 rotateY(in vec3 vec, in float rad) {
    
    return vec3(
        vec[2]*sin(rad) + vec[0]*cos(rad),
        vec[1],
        vec[2]*cos(rad) - vec[0]*sin(rad)
    );
}

vec3 rotateZ(in vec3 vec, in float rad) {
    
    return vec3(
        vec[0]*cos(rad) - vec[1]*sin(rad),
        vec[0]*sin(rad) + vec[1]*cos(rad),
        vec[2]
    );
}

// return the vector closest to `p`
vec3 closestTo(in vec3 p, in vec3 a, in vec3 b, in vec3 c, in vec3 d) {
    
    vec4 ll = vec4(
    	distance(a,p),
        distance(b,p),
        distance(c,p),
        distance(d,p)
    );
    
    float sh = min4(ll);
    
    vec4 eq = vec4(equal(ll, vec4(sh)));
    
    //return mix(a, mix( b, mix(c, d, eq.z ), eq.y ), eq.x ); !! with notEqual
    return eq.x*a + eq.y*b + eq.z*c + eq.w*d;
    
}


/***************

Tetrahedron start

***************/

// normals
const vec3 norm1 = vec3(
	0.4714045226573944,
    0.3333333432674408,
    -0.8164966106414795
);
const vec3 norm2 = vec3(
	-0.9428090453147888,
    0.3333333432674408,
    0.0
);
const vec3 norm3 = vec3(
	0.4714045524597168,
    0.3333333432674408,
    0.8164966106414795
);

// points
const vec3 point0 = vec3( 0.0,                 1.0,                 0.0);
const vec3 point1 = vec3( 0.9428090453147888, -0.3333333432674408,  0.0);
const vec3 point2 = vec3(-0.4714045226573944, -0.3333333432674408, -0.8164966106414795);
const vec3 point3 = vec3(-0.4714045226573944, -0.3333333432674408,  0.8164966106414795);

float planedist(in vec3 point, in vec3 norm) {

    return dot(point, norm) - norm.y;

}

float tet(in vec3 point, in float size) {
    
    point = point/size;

    // subtracting the planes from a sphere
    float dist = max4(vec4(
        planedist(point, norm1),
        planedist(point, norm2),
        planedist(point, norm3),
        -(1.0/3.0+point.y)
    ));

    // the sphere has a `size` radius
    dist = max(length(point)-1.0, dist)*size;

    return dist;
    
}

/***************

Tetrahedron end

***************/

// the distance estimator function
const int STEPS = 12;
float DE(in vec3 pos) {
    
    // rotation
    float rot = (iTime*0.25)*PI*2.0;
    pos = rotateY(pos, rot);
    
    // steps
    vec3 p = pos;
    for(int i=0; i<STEPS; i++) {
        
        // scale the current point, and find the closest point vertex
        p *= 2.0;
        p = p-closestTo(p, point0, point1, point2, point3);
        
    }
    
    // calculate the distance to the last found tetrahedron ( for STEPS=0 -> tet(pos, 1.0) )
    return tet(p, 1.0)/exp2(float(STEPS));
    
}

const vec3 shadowColor = vec3(0.1, 0.05, 0.0);
const vec3 backColor = vec3(0.2, 0.25, 0.2);
const vec3 tetColor = vec3(0.2, 0.75, 0.95);

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    
    // camera
	vec2 view = fragCoord.xy / iResolution.xy * 2.0 - 1.0;
    
    float aspect = iResolution.x/iResolution.y;
    float a = tan(FOV * (PI/180.0) / 2.0);
    vec2 ab = vec2(a * aspect, a);
    
    // the start point, and the direction of the ray
    vec3 dir = normalize(vec3(ab*view, -1.0));
    vec3 point = vec3(0.0, 0.0, 1.5+sin(iTime)*1.5);
    
    // ray-march
    float closest = infinity;

    for(int i = 0; i<MAX_STEP; i++) {

        vec3 pos = point;

        float dist = DE(pos);

        if(dist < 0.0025) {
            fragColor = vec4(mix(
                tetColor,
                shadowColor,
                sqrt(float(i)/MAX_STEP_F)
            ), 1.0);
            return;
        }

        closest = min(closest, dist);

        point += dir*dist;

    }

    fragColor = vec4(mix(
        shadowColor,
        backColor,
        sqrt(clamp(closest/0.5, 0.0, 1.0))
    ), 1.0);
    
}