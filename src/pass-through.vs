in vec4 pos_vs_in;
in vec2 uv_vs_in;
in vec3 normal_vs_in;

out vec4 pos_cs_in;
out vec2 uv_cs_in;
out vec3 normal_cs_in;
void main()
{
  pos_cs_in = pos_vs_in;
}
