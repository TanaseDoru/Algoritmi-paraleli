#include <math.h>
#include <pthread.h>
#include <semaphore.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>

#include "Workers.h"

int N;
int printLevel;
int COLORS = 3;
int numVertices = 10;
int numEdges = 30;
int graphDefault[][2] = {{0, 1}, {0, 4}, {0, 5}, {1, 0}, {1, 2}, {1, 6}, {2, 1}, {2, 3}, {2, 7}, {3, 2}, {3, 4}, {3, 8}, {4, 0}, {4, 3}, {4, 9}, {5, 0}, {5, 7}, {5, 8}, {6, 1}, {6, 8}, {6, 9}, {7, 2}, {7, 5}, {7, 9}, {8, 3}, {8, 5}, {8, 6}, {9, 4}, {9, 6}, {9, 7}};
int** graph;

typedef struct {
	int* colors;
	int step;
} ColorArgs;

void colorGraphTask(void* args, int thread_id);

void initDefaultGraph()
{
	graph = (int**)malloc(sizeof(int*) * numEdges);
	for (int i = 0; i < numEdges; i++) {
		graph[i] = (int*)malloc(sizeof(int) * 2);
		graph[i][0] = graphDefault[i][0];
		graph[i][1] = graphDefault[i][1];
	}
}

void getArgs(int argc, char** argv)
{
	if (argc < 4) {
		printf("Not enough paramters: ./program N printLevel P\nprintLevel: 0=no, 1=some, 2=verbouse\n");
		exit(1);
	}
	N = atoi(argv[1]);
	printLevel = atoi(argv[2]);
	P = atoi(argv[3]);
}

void printAll(int* v, int N)
{
	int i;
	int max = 1;
	for (i = 0; i < N; i++)
		if (max < v[i])
			max = v[i];

	int displayWidth = 2 + log10(max);
	char* aux = calloc(1000, sizeof(char));
	char* vectorValue = malloc(10 * sizeof(char));
	for (i = 0; i < N; i++) {
		sprintf(vectorValue, "%*i", displayWidth, v[i]);
		strcat(aux, vectorValue);
		strcat(aux, " ");
	}
	printf("%s\n", aux);
	free(aux);
	free(vectorValue);
}

void print(int* v, int N)
{
	if (printLevel == 0) return;
	printAll(v, N);
}

int isEdge(int a, int b)
{
	for (int i = 0; i < numEdges; i++) {
		if (graph[i][0] == a && graph[i][1] == b)
			return 1;
	}
	return 0;
}

int verifyColors(int colors[], int step)
{
	for (int i = 0; i < step; i++) {
		if (colors[i] == colors[step] && isEdge(i, step))
			return 0;
	}
	return 1;
}

void colorGraphTask(void* args, int thread_id)
{
	ColorArgs* cArgs = (ColorArgs*)args;
	int step = cArgs->step;
	int* colors = cArgs->colors;

	if (step == numVertices) {
		print(colors, step);
		free(colors);
		free(cArgs);
		return;
	}

	for (int i = 0; i < COLORS; i++) {
		int* newColors = (int*)malloc(sizeof(int) * numVertices);
		memcpy(newColors, colors, sizeof(int) * numVertices);

		newColors[step] = i;
		if (verifyColors(newColors, step)) {
			ColorArgs* nextArgs = malloc(sizeof(ColorArgs));
			nextArgs->colors = newColors;
			nextArgs->step = step + 1;

			if (step < 3) {
				Task t;
				t.runTask = colorGraphTask;
				t.data = nextArgs;
				putTask(t);
			} else {
				colorGraphTask(nextArgs, thread_id);
			}
		} else {
			free(newColors);
		}
	}
	free(colors);
	free(cArgs);
}

void generateGraph(int nVertices, int nEdges)
{
	srand(42);
	numVertices = nVertices;
	numEdges = nEdges;
	graph = (int**)malloc(sizeof(int*) * numEdges);
	for (int i = 0; i < numEdges; i++) {
		graph[i] = (int*)malloc(sizeof(int) * 2);
		graph[i][0] = rand() % numVertices;
		graph[i][1] = rand() % numVertices;
		if (graph[i][0] == graph[i][1])
			i--;
	}
}

int main(int argc, char** argv)
{
	getArgs(argc, argv);
	// initDefaultGraph();
	generateGraph(20, 60);
	startWorkers();

	ColorArgs* initialArgs = malloc(sizeof(ColorArgs));
	initialArgs->colors = calloc(numVertices, sizeof(int));
	initialArgs->step = 0;

	Task t;
	t.runTask = colorGraphTask;
	t.data = initialArgs;
	putTask(t);

	while (putTasks != gotTasks) {
		usleep(1000);
	}

	forceShutDownWorkers();
	joinWorkerThreads();
	return 0;
}