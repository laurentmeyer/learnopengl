#ifndef SHADER_CLASS_H
# define SHADER_CLASS_H

# include "learnopengl.h"
# include <string>
# include <fstream>
# include <sstream>
# include <iostream>
# include <cassert>

class Shader
{
  public:
    GLuint ID;

    Shader() { assert(false); };
    Shader(const GLchar *vertexPath, const GLchar *fragmentPath)
    {
        std::string vertexCode;
        std::string fragmentCode;
        std::ifstream vShaderFile;
        std::ifstream fShaderFile;

        // verifier ce que fait ce truc
        vShaderFile.exceptions(std::ifstream::failbit | std::ifstream::badbit);
        fShaderFile.exceptions(std::ifstream::failbit | std::ifstream::badbit);
        try
        {
            vShaderFile.open(vertexPath);
            fShaderFile.open(fragmentPath);

            std::stringstream fShaderStream;
            std::stringstream vShaderStream;
            vShaderStream << vShaderFile.rdbuf();

            fShaderStream << fShaderFile.rdbuf();

            vShaderFile.close();
            fShaderFile.close();

            vertexCode = vShaderStream.str();
            fragmentCode = fShaderStream.str();
        }
        catch (std::ifstream::failure e)
        {
            std::cout << "ERROR::SHADER::FILE_NOT_SUCCESFULLY_READ" << e.what() << std::endl;
        }
        const char *vShaderCode = vertexCode.c_str();
        const char *fShaderCode = fragmentCode.c_str();

        unsigned int vertex;
        unsigned int fragment;
        int success;
        char infoLog[512];

        vertex = glCreateShader(GL_VERTEX_SHADER);
        glShaderSource(vertex, 1, &vShaderCode, NULL);
        glCompileShader(vertex);
        glGetShaderiv(vertex, GL_COMPILE_STATUS, &success);
        if (!success)
        {
            glGetShaderInfoLog(vertex, 512, NULL, infoLog);
            std::cout << "ERROR::SHADER::VERTEX::COMPILATION_FAILED\n"
                      << infoLog << std::endl;
        };

        fragment = glCreateShader(GL_FRAGMENT_SHADER);
        glShaderSource(fragment, 1, &fShaderCode, NULL);
        glCompileShader(fragment);
        glGetShaderiv(fragment, GL_COMPILE_STATUS, &success);
        if (!success)
        {
            glGetShaderInfoLog(fragment, 512, NULL, infoLog);
            std::cout << "ERROR::SHADER::FRAGMENT::COMPILATION_FAILED\n"
                      << infoLog << std::endl;
        };

        ID = glCreateProgram();
        glAttachShader(ID, vertex);
        glAttachShader(ID, fragment);
        glLinkProgram(ID);

        glGetProgramiv(ID, GL_LINK_STATUS, &success);
        if (!success)
        {
            glGetProgramInfoLog(ID, 512, NULL, infoLog);
            std::cout << "ERROR::SHADER::PROGRAM::LINKING_FAILED\n"
                      << infoLog << std::endl;
        }

        glDeleteShader(vertex);
        glDeleteShader(fragment);
    }
    ~Shader(){};

    void use() {glUseProgram(ID);}

    void setBool(const std::string &name, bool value) const { glUniform1i(glGetUniformLocation(ID, name.c_str()), (int)value); }
    void setInt(const std::string &name, int value) const { glUniform1i(glGetUniformLocation(ID, name.c_str()), value); }
    void setFloat(const std::string &name, float value) const { glUniform1f(glGetUniformLocation(ID, name.c_str()), value); }
    //void setVec3i(const std::string &name, GLint value[3]) const { glUniform3iv(glGetUniformLocation(ID, name.c_str()), 1, value); }
    void setVec3(const std::string &name, GLfloat value[3]) const { glUniform3fv(glGetUniformLocation(ID, name.c_str()), 1, value); }
    void setVec4(const std::string &name, GLfloat value[4]) const { glUniform4fv(glGetUniformLocation(ID, name.c_str()), 1, value); }

    void setUniforms(GLFWwindow *window)
    {
        // uniform float iTimeDelta;
        // uniform float iFrame;
        // uniform float iChannelTime[4];
        // uniform vec4 iMouse;
        // uniform vec4 iDate;
        // uniform float iSampleRate;
        // uniform vec3 iChannelResolution[4];
        // uniform samplerXX iChanneli;

        // uniform float iTime;
        setFloat("iTime", glfwGetTime());

        // uniform vec3 iResolution;
        int width;
        int height;
        glfwGetWindowSize(window, &width, &height);
        GLfloat resolution[3] = {(GLfloat)width, (GLfloat)height, 1.f};
        setVec3("iResolution", resolution);
    }
};

#endif // SHADER_CLASS_H