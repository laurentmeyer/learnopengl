#include "learnopengl.h"
#include "ShaderClass.hpp"
#include <iostream>
#include <cmath>

void processInput(GLFWwindow *window)
{
    if(glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
        glfwSetWindowShouldClose(window, true);
}

GLfloat vertices[] = {
     1.f,  1.f, 0.0f,  // top right
     1.f, -1.f, 0.0f,  // bottom right
    -1.f, -1.f, 0.0f,  // bottom left
    -1.f,  1.f, 0.0f   // top left 
};

GLuint indices[] = {
    0, 1, 3,
    1, 2, 3
};

// const char *vertexShaderSource = "#version 330 core\n"
//     "layout (location = 0) in vec3 aPos;\n"
//     "\n"
//     "out vec4 fragCoord;\n"
//     "\n"
//     "void main()\n"
//     "{\n"
//     "gl_Position = vec4(aPos, 1.0);\n"
//     //"vertexColor = gl_Position;\n"
//     // "vertexColor = vec4(0.5, 0.0, 0.0, 1.0);\n"
//     "}";

// const char *fragmentShaderSource = "#version 330 core\n"
//     "in vec4 fragCoord;\n"
//     "out vec4 fragColor;\n"
//     "uniform vec4 ourColor;\n"
//     "void main()\n"
//     "{\n"
//     "   fragColor = ourColor;\n"
//     "}\n\0";


int main() {
	GLFWwindow *window = initWindow();
	if (!window)
	{
		std::cout << "Could not initialize window" << std::endl;
		return -1;
	}

    Shader shader("shaders/shader.vs", "shaders/shader.fs");

    GLuint VAO;
    GLuint VBO;
    GLuint EBO; // element buffer object
    glGenVertexArrays(1, &VAO);
    glGenBuffers(1, &VBO);
    glGenBuffers(1, &EBO);
    // bind the Vertex Array Object first, then bind and set vertex buffer(s), and then configure vertex attributes(s).
    glBindVertexArray(VAO);

    glBindBuffer(GL_ARRAY_BUFFER, VBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);

    // note that this is allowed, the call to glVertexAttribPointer registered VBO as the vertex attribute's bound vertex buffer object so afterwards we can safely unbind
    glBindBuffer(GL_ARRAY_BUFFER, 0); 

    // remember: do NOT unbind the EBO while a VAO is active as the bound element buffer object IS stored in the VAO; keep the EBO bound.
    //glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);

    // You can unbind the VAO afterwards so other VAO calls won't accidentally modify this VAO, but this rarely happens. Modifying other
    // VAOs requires a call to glBindVertexArray anyways so we generally don't unbind VAOs (nor VBOs) when it's not directly necessary.
    //glBindVertexArray(0); 


	while (!glfwWindowShouldClose(window))
    {
        // input
        // -----
        processInput(window);

        // render
        // ------
        glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        glClear(GL_COLOR_BUFFER_BIT);


        // draw our first triangle

        shader.use();
        shader.setUniforms(window);
        glBindVertexArray(VAO);
        glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
 
        // glfw: swap buffers and poll IO events (keys pressed/released, mouse moved etc.)
        // -------------------------------------------------------------------------------
        glfwSwapBuffers(window);
        glfwPollEvents();
    }


	glfwTerminate();

	return 0;
}