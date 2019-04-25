CC = 		clang++
CFLAGS =	-g -Wall -Wextra -Werror
BINARY =	learnOpenGL
BUILDDIR =	builds
SOURCEDIR =	srcs
HEADERDIR = includes
SRCFILES =						\
			main.cpp			\




GLFW = 		./libs/GLFW
GLEW = 		./libs/GLEW

CCHEADERS = -I./$(HEADERDIR) \
			-I$(GLFW)/include/ \
			-I$(GLEW)/include/ \
			-I$(LIBFT)/libft/includes \

CCLIBS =	-L$(GLFW)/lib -lglfw		\
			-L$(GLEW)/lib -lGLEW		\

CCFRAMEWORKS = -framework Cocoa -framework OpenGL -framework IOKit -framework CoreVideo

SOURCES = $(SRCFILES:%.cpp=$(SOURCEDIR)/%.cpp)
OBJECTS = $(SOURCES:$(SOURCEDIR)/%.cpp=$(BUILDDIR)/%.o)

.PHONY: clean fclean all re norme $(LIBFT)/libft.a

all : $(BINARY)

$(BINARY) : $(OBJECTS)
	@$(CC) $(CCHEADERS) $(CCLIBS) $(OBJECTS) $(CCFRAMEWORKS) -o $(BINARY)

$(BUILDDIR)/%.o : $(SOURCEDIR)/%.cpp
	mkdir -p $(@D)
	$(CC) $(CFLAGS) $(CCHEADERS) -c $< -o $@

clean:
	rm -f $(OBJECTS)

fclean: clean
	rm -f $(BINARY)

re: fclean all