in vec4 pos_fs_in;
in vec2 uv_fs_in;
in vec3 normal_fs_in;
in vec3 pos_obj_fs;

out vec3 color;

// 1D, 2D, and 3D pseudorandom (hash) functions
float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

vec2 hash2(vec2 p) {
    return fract(sin(vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)))) * 43758.5453);
}

vec3 hash3(vec2 p) {
    return fract(sin(vec3(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)), dot(p, vec2(113.5, 271.9)))) * 43758.5453);
}

// data structure to access voronoi results
struct voronoi_result {
    float dist;
    vec2  pos;
    vec3  color;
};

// euclidian voronoi random function
// like perlin noise, this produces a pseudorandom texture map that we can use for all sorts of downstream tasks.
// https://en.wikipedia.org/wiki/Voronoi_diagram
voronoi_result voronoi(vec2 uv) {
    vec2 gv = floor(uv);
    vec2 lv = fract(uv);

    float best = 1e10;
    vec2 best_cell = vec2(0.0);
    vec2 best_pos  = vec2(0.0);

    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            vec2 cell = gv + vec2(x, y);
            vec2 rnd = hash2(cell);
            vec2 p = (vec2(x, y) + rnd) - lv;

            float d = dot(p, p);
            if (d < best) {
                best = d;
                best_cell = cell;
                best_pos = cell + rnd;
            }
        }
    }

    voronoi_result r;
    r.dist = sqrt(best);
    r.pos = best_pos;
    r.color = hash3(best_cell);
    return r;
}

// mapping function a la blender
float map_range(float v, float from_min, float from_max, float to_min, float to_max) {
    float t = (v - from_min) / (from_max - from_min);
    t = clamp(t, 0.0, 1.0);
    return mix(to_min, to_max, t);
}

// step (no interpolation) colorramp function for toon shading, also like blender
vec3 color_ramp(float fac, vec3 c0, vec3 c1, vec3 c2, float t0, float t1) {
    if (fac < t0) {
        return c0;
    }
    else if (fac < t1) {
        return c1;
    }
    else {
        return c2;
    }
}

void main()
{
    // compute a paint-brush-like alpha mask, starting from UV barycentric coords.
    // this is identical to the blender shadergraph shown in the writeup.
    vec2 uv = uv_fs_in;
    vec2 uv_centered = uv - 0.5;

    float scale = 9.0;
    voronoi_result vr = voronoi(uv * scale);

    vec2 feature_pos = vr.pos / scale - 0.5;

    vec2 d  = uv_centered - feature_pos;
    vec2 d2 = d * vr.color.xy;

    float len = length(d2);
    float radius = length(feature_pos);

    float base  = map_range(radius, 0.36, 0.58, 0.04, -0.01);
    float base2 = base + 0.005;

    float mask = map_range(len, base, base2, 1.0, 0.0);

    // compute toon-shaded colour to apply the above mask to.
    // we use n dot l shading, with a fake light (single vector).
    // instead of directly using the dot product,
    // we send it through a step function with custom colours defined at each step => toon shading!
    // this is identical to the blender shadergraph shown in the writeup.
    vec3 col1 = vec3(86.0, 115.0, 57.0) / 255.0;
    vec3 col2 = vec3(129.0, 166.0, 73.0) / 255.0;
    vec3 col3 = vec3(242.0, 227.0, 153.0) / 255.0;
    vec3 col4 = vec3(242.0, 202.0, 179.0) / 255.0;
    vec3 col5 = vec3(242.0, 145.0, 136.0) / 255.0;

    vec3 light = normalize(vec3(-1, 0.5, 1));
    float fac = dot(light, normal_fs_in);

    if (pos_obj_fs.z < -0.5) {
        color = color_ramp(fac, col1, col2, col3, 0.5, 0.55);
    }
    else {
        color = color_ramp(fac, col5, col4, col5, 0.5, 0.55);
    }

    // apply alpha mask
    if (mask < 0.2) discard;
}
