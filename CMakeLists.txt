cmake_minimum_required(VERSION 3.20)
project(shaderpipeline)

# C++ standard
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# OpenGL
if(NOT CMAKE_VERSION VERSION_LESS "3.11")
  cmake_policy(SET CMP0072 NEW)
endif()
find_package(OpenGL REQUIRED) # provides OpenGL::GL


# Include GLFW and GLAD cmake projects
if(NOT TARGET glfw)
  # Turn off building examples, tests, docs, and install rules.
  # Just include the library.
  set(GLFW_BUILD_EXAMPLES OFF CACHE BOOL " " FORCE)
  set(GLFW_BUILD_TESTS OFF CACHE BOOL " " FORCE)
  set(GLFW_BUILD_DOCS OFF CACHE BOOL " " FORCE)
  set(GLFW_INSTALL OFF CACHE BOOL " " FORCE)
  add_subdirectory("${CMAKE_CURRENT_SOURCE_DIR}/glfw")
endif()
add_subdirectory("${CMAKE_CURRENT_SOURCE_DIR}/glad")

# Executable target
add_executable(${PROJECT_NAME} main.cpp)

# Use solution or src code
option(USE_SOLUTION "Build with solution code enabled" OFF)
if(USE_SOLUTION)
  target_compile_definitions(${PROJECT_NAME} PRIVATE USE_SOLUTION)
endif()

# Add project headers
target_include_directories(${PROJECT_NAME}
  PUBLIC
    "${CMAKE_CURRENT_SOURCE_DIR}/include"
)

# Eigen
target_include_directories(${PROJECT_NAME} SYSTEM PUBLIC "${CMAKE_CURRENT_SOURCE_DIR}/eigen")

# Link libraries
target_link_libraries(${PROJECT_NAME}
  PRIVATE
    glfw
    glad
    OpenGL::GL
)

# Warnings
if(MSVC)
  target_compile_options(${PROJECT_NAME} PRIVATE /W4 /permissive-)
else()
  target_compile_options(${PROJECT_NAME} PRIVATE -Wall -Wextra -Wpedantic)
endif()

if(MSVC)
  # Use M_PI for windows
  target_compile_definitions(${PROJECT_NAME} PRIVATE _USE_MATH_DEFINES)
endif()
