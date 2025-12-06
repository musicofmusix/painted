uniform mat4 view;
uniform mat4 proj;

layout(location = 0) in vec3 pos_vs_in;
layout(location = 1) in vec2 uv_vs_in;
layout(location = 2) in vec3 normal_vs_in;

out vec4 pos_cs_in;
out vec2 uv_cs_in;
out vec3 normal_cs_in;
out vec3 pos_obj_tcs;

void main() {
    pos_cs_in = proj * view * vec4(pos_vs_in, 1.0);
    uv_cs_in = uv_vs_in;
    normal_cs_in = normal_vs_in;
    pos_obj_tcs = pos_vs_in;
    gl_Position = pos_cs_in;
}
