#ifndef MESH_TO_VAO_H
#define MESH_TO_VAO_H
#include "gl.h"
#include <Eigen/Core>

// Send a triangle mesh to the GPU using a vertex array object.
//
// Inputs:
//   V  #V by 3 list of 3D mesh vertex positions
//   F  #F by 3 list of triangle indices into V
//   UV #V by 2 list of 2D mesh vertex uv coordinates
//   N  #V by 3 list of 3D mesh vertex normals
// Outputs:
//   VAO  identifier of compiled vertex array object.
inline void mesh_to_vao(
  const Eigen::Matrix<float, Eigen::Dynamic, 3, Eigen::RowMajor> &V,
  const Eigen::Matrix<GLuint, Eigen::Dynamic, 3, Eigen::RowMajor> &F,
  const Eigen::Matrix<float, Eigen::Dynamic, 2, Eigen::RowMajor> &UV,
  const Eigen::Matrix<float, Eigen::Dynamic, 3, Eigen::RowMajor> &N,
  GLuint &VAO);

// Implementation
inline void mesh_to_vao(
  const Eigen::Matrix<float, Eigen::Dynamic, 3, Eigen::RowMajor> &V,
  const Eigen::Matrix<GLuint, Eigen::Dynamic, 3, Eigen::RowMajor> &F,
  const Eigen::Matrix<float, Eigen::Dynamic, 2, Eigen::RowMajor> &UV,
  const Eigen::Matrix<float, Eigen::Dynamic, 3, Eigen::RowMajor> &N,
  GLuint &VAO)
{
    glGenVertexArrays(1, &VAO);
    glBindVertexArray(VAO);

    GLuint VBO_pos, VBO_uv, VBO_norm, EBO;
    glGenBuffers(1, &VBO_pos);
    glGenBuffers(1, &VBO_uv);
    glGenBuffers(1, &VBO_norm);
    glGenBuffers(1, &EBO);

    glBindBuffer(GL_ARRAY_BUFFER, VBO_pos);
    glBufferData(GL_ARRAY_BUFFER, sizeof(float) * V.size(), V.data(), GL_STATIC_DRAW);
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (GLvoid*)0);
    glEnableVertexAttribArray(0);

    glBindBuffer(GL_ARRAY_BUFFER, VBO_uv);
    glBufferData(GL_ARRAY_BUFFER, sizeof(float) * UV.size(), UV.data(), GL_STATIC_DRAW);
    glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), (GLvoid*)0);
    glEnableVertexAttribArray(1);

    glBindBuffer(GL_ARRAY_BUFFER, VBO_norm);
    glBufferData(GL_ARRAY_BUFFER, sizeof(float) * N.size(), N.data(), GL_STATIC_DRAW);
    glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (GLvoid*)0);
    glEnableVertexAttribArray(2);

    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(GLuint) * F.size(), F.data(), GL_STATIC_DRAW);

    glBindVertexArray(0);
}

#endif
