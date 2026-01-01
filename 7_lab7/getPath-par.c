#include <math.h>
#include <pthread.h>
#include <semaphore.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>

#include "Workers.h"

int startVertex = 0;
int destinationVertex = 3;

int N = 10;
int printLevel;
int numVertices = 10;
int numEdges = 30;
int graphDefault[][2] = {{0, 1}, {0, 4}, {0, 5}, {1, 0}, {1, 2}, {1, 6}, {2, 1}, {2, 3}, {2, 7}, {3, 2}, {3, 4}, {3, 8}, {4, 0}, {4, 3}, {4, 9}, {5, 0}, {5, 7}, {5, 8}, {6, 1}, {6, 8}, {6, 9}, {7, 2}, {7, 5}, {7, 9}, {8, 3}, {8, 5}, {8, 6}, {9, 4}, {9, 6}, {9, 7}};
int** graph;

typedef struct {
	int* partialPath;
	int step;
	int destination;
} PathArgs;

void getPathTask(void* args, int thread_id);

void initDefaultGraph()
{
	graph = (int**)malloc(sizeof(int*) * numEdges);
	for (int i = 0; i < numEdges; i++) {
		graph[i] = (int*)malloc(sizeof(int) * 2);
		graph[i][0] = graphDefault[i][0];
		graph[i][1] = graphDefault[i][1];
	}
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

int vectorContains(int* v, int size, int value)
{
	for (int i = 0; i < size; i++)
		if (v[i] == value)
			return 1;
	return 0;
}

void getPathTask(void* args, int thread_id)
{
	PathArgs* pArgs = (PathArgs*)args;
	int step = pArgs->step;
	int destination = pArgs->destination;
	int* partialPath = pArgs->partialPath;

	if (partialPath[step - 1] == destination) {
		print(partialPath, step);
		free(partialPath);
		free(pArgs);
		return;
	}

	int lastNodeInPath = partialPath[step - 1];
	for (int i = 0; i < numEdges; i++) {
		if (graph[i][0] == lastNodeInPath) {
			if (vectorContains(partialPath, step, graph[i][1]))
				continue;

			PathArgs* newArgs = malloc(sizeof(PathArgs));
			newArgs->partialPath = malloc(sizeof(int) * numVertices);
			memcpy(newArgs->partialPath, partialPath, sizeof(int) * numVertices);
			newArgs->partialPath[step] = graph[i][1];
			newArgs->step = step + 1;
			newArgs->destination = destination;

			if (step < 4) {
				Task t;
				t.runTask = getPathTask;
				t.data = newArgs;
				putTask(t);
			} else {
				getPathTask(newArgs, thread_id);
			}
		}
	}
	free(partialPath);
	free(pArgs);
}

int main(int argc, char** argv)
{
	getArgs(argc, argv);
	initDefaultGraph();
	// generateGraph(10), 95);
	startWorkers();

	PathArgs* initialArgs = malloc(sizeof(PathArgs));
	initialArgs->partialPath = malloc(sizeof(int) * numVertices);
	initialArgs->partialPath[0] = startVertex;
	initialArgs->step = 1;
	initialArgs->destination = destinationVertex;

	Task t;
	t.runTask = getPathTask;
	t.data = initialArgs;
	putTask(t);

	while (putTasks != gotTasks) {
		usleep(10000);
	}

	forceShutDownWorkers();
	joinWorkerThreads();
	return 0;
}