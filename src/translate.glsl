// Inputs:
//   t  3D vector by which to translate
// Return a 4x4 matrix that translates and 3D point by the given 3D vector
mat4 translate(vec3 t)
{
  // what? apparently OpenGL does not use our friendly, intuitive row-order matrix notation
  // just transpose.
  
  return transpose(mat4(
  1,0,0,t.x,
  0,1,0,t.y,
  0,0,1,t.z,
  0,0,0,1));
}

