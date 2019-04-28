#include "learnopengl.h"

void framebufferSizeCallback(GLFWwindow* window, int width, int height)
{
	(void)window;
	glViewport(0, 0, width, height);
}

GLFWwindow* initWindow()
{
	GLFWwindow *window;

	glfwInit();
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
	window = glfwCreateWindow(800, 600, "LearnOpenGL", nullptr, nullptr);
	if (window)
	{
		glfwMakeContextCurrent(window);
		glewInit();
		glViewport(0, 0, 800, 600);
		glfwSetFramebufferSizeCallback(window, framebufferSizeCallback);
	}
	//glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
	return window;
}