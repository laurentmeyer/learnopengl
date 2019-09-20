//https://www.shadertoy.com/view/ltVGzy
#define PI 3.141592653
#define FOV 60.0

#define MAX_STEP   32
#define MAX_STEP_F 32.0

const float infinity = 1.0/0.00000000001;

#define epsilon 0.0025


float sum(in vec3 v3) {
 	return dot(v3, vec3(1.0));   
}

struct Hit {
    float dist;
    vec3 norm;
    vec3 col;
};
    
struct DirLight {
	vec3 dir;
    vec3 col;
    float intensity;
};
    
vec3 applyLight(in DirLight light, in vec3 pos, in vec3 cam, in Hit hit) {
	
    vec3 surfaceToLight = light.dir;

    float diffuseCoefficient = max(0.0, dot(hit.norm, surfaceToLight));
    
    vec3 diffuse = diffuseCoefficient * hit.col*light.col * light.intensity;

    // specular
    vec3 surfaceToCamera = normalize(cam - pos);

    float specularCoefficient = pow(
        max(0.0, dot(surfaceToCamera, reflect(-surfaceToLight, hit.norm))),
        1.0/0.0025/*materialShininess*/
    );

    vec3 specular = specularCoefficient * vec3(1.0, 1.0, 1.0)/*materialSpecularColor*/ * light.intensity;
	
    // ambient
    vec3 ambient = hit.col*light.col*0.05;
    
    // result
    return diffuse + specular + ambient;
    
}

// rotations
mat4 rotationMatrix(vec3 axis, float angle) {
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return mat4(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
                0.0,                                0.0,                                0.0,                                1.0);
}

// matrix functions

vec3 applyMat( in vec3 v, in mat4 mat) {
    return (vec4(v, 1.0) * mat).xyz;
}

//
const vec3 shadowColor = vec3(0.1, 0.0, 0.1);
const vec3 backColor = vec3(0.5, 0.5, 0.5);
const vec3 tetColor = vec3(0.0, 0.75, 0.0);

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
const vec3 norm4 = vec3(
	 0.0,
    -1.0,
     0.0
);

// points
const vec3 point0 = vec3( 0.0,                 1.0,                 0.0);
const vec3 point1 = vec3( 0.9428090453147888, -0.3333333432674408,  0.0);
const vec3 point2 = vec3(-0.4714045226573944, -0.3333333432674408, -0.8164966106414795);
const vec3 point3 = vec3(-0.4714045226573944, -0.3333333432674408,  0.8164966106414795);

float planedist(in vec3 point, in vec3 norm) {
    return dot(point, norm) - norm.y;
}

const float size = 3.0;
Hit tet(in vec3 point) {
    
    Hit hit;
    point = point/size;

    // subtracting the planes from a sphere
    vec4 distances = vec4(
        planedist(point, norm1),
        planedist(point, norm2),
        planedist(point, norm3),
        -(1.0/3.0+point.y)
    );

    float dist = max( max(distances.x, distances.y), max(distances.z, distances.w) );

    // the sphere has a `size` radius
    hit.dist = max(length(point)-1.0, dist)*size;
    
    if(hit.dist < epsilon) {
        
        // select the appropriate norm
        vec4 eq = vec4(equal(distances, vec4(dist)));
        
        hit.norm = eq.x*norm1 + eq.y*norm2 + eq.z*norm3 + eq.w*norm4;
        
        hit.col = tetColor;
        
    }

    return hit;
    
}

/***************

Tetrahedron end

***************/

//Hit DE(in vec3 pos, in mat4 trans, in mat4 invTrans) {
Hit DE(in vec3 pos, in mat4 invTrans) {
    
    //pos = applyMat(pos, invTrans);
    
    Hit hit = tet(pos);
    
    // // convert back normal coordinates to global
    // hit.norm = normalize(applyMat(hit.norm, trans) - applyMat(vec3(0.0, 0.0, 0.0), trans));
    
    return hit;
    
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    
    // camera
	vec2 view = fragCoord.xy / iResolution.xy * 2.0 - 1.0;
    
    float aspect = iResolution.x/iResolution.y;
    float a = tan(FOV * (PI/180.0) / 2.0);
    vec2 ab = vec2(a * aspect, a);
    
    // the start point, and the direction of the ray
    vec3 dir = normalize(vec3(ab*view, -1.0));
    vec3 point = vec3(0.0, 0.0, 10.0+sin(iTime)*4.0);
    vec3 pos = point;
    
    // light
    DirLight light;
    light.dir = normalize(vec3(0.25, 0.25, 1.0));
    light.col = vec3(1.0, 1.0, 1.0);
    light.intensity = 1.0;
    
    // trans
    // float rot = (iTime*0.25)*PI*2.0;
    
    // mat4 trans = rotationMatrix(vec3(1.0, 0.0, 0.0), rot)*rotationMatrix(vec3(0.0, 1.0, 0.0), rot)*rotationMatrix(vec3(0.0, 0.0, 1.0), rot);
    // mat4 invTrans = inverse(trans);
    
    // ray-march
    float closest = infinity;

    for(int i = 0; i<MAX_STEP; i++) {
		
        Hit hit = DE(pos, trans, invTrans);
        float dist = hit.dist;

        if(dist < epsilon) {
            /*
            fragColor = vec4(mix(
                tetColor,
                shadowColor,
                sqrt(float(i)/MAX_STEP_F)
            ), 1.0);
            */
            
            fragColor = vec4(applyLight(light, pos, point, hit), 1.0);
            
            return;
        }

        closest = min(closest, dist);

        pos += dir*dist;

    }

    fragColor = vec4(mix(
        shadowColor,
        backColor,
        sqrt(clamp(closest/2.0, 0.0, 1.0))
    ), 1.0);
    
}