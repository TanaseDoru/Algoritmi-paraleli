#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>

int printLevel;
int N;
int P;
int** a;
int** b;
int** c;

pthread_mutex_t GPmutex = PTHREAD_MUTEX_INITIALIZER; 
void* threadFunction(void *args)
{
	// TODO: Implement parallel multiply of matrices: C = A * B
	// using P threads by splitting the inner loop.

	int thread_id = *(int*)args;

	int** localC = (int**)calloc(N, sizeof(int*));
	for(int i = 0 ; i < N; i ++)
		localC[i] = (int*)calloc(N, sizeof(int));
	
	int chunk = N / P;	
	int rest = N % P;
	int startIndex = chunk * thread_id;
	int endIndex = chunk * (thread_id + 1);
	if (thread_id == P - 1)
	{
		endIndex += rest;
	}

	for(int i = 0; i < N; i++)
	{
		for(int j = startIndex; j < endIndex; j++)
		{
			for(int k = 0; k < N; k++)
			{
				localC[i][j] += a[i][k] * b[k][j];
			}
		}
	}

	pthread_mutex_lock(&GPmutex);
	for(int i = startIndex; i < endIndex; i++)
	{
		for(int j = 0; j < N; j++)
		{
			c[i][j] = localC[i][j];
		}
	}
	pthread_mutex_unlock(&GPmutex);
	

	for(int i = 0; i < N; i++)
	{
		free(localC[i]);
	}
	free(localC);
	
	/*
	for(i = 0; i < N; i++) {
		for(j = 0; j < N; j++) {
			for(k = 0; k < N; k++) {
				c[i][j] += a[i][k] * b[k][j];
			}
		}
	}
	*/

	return NULL;
}

void getArgs(int argc, char **argv)
{
	if(argc < 4) {
		printf("Not enough paramters: ./program N printLevel P\n");
		exit(1);
	}
	N = atoi(argv[1]);
	printLevel = atoi(argv[2]);
	P = atoi(argv[3]);
}

void init()
{
	a = malloc(sizeof(int *) * N);
	b = malloc(sizeof(int *) * N);
	c = malloc(sizeof(int *) * N);
	if(a == NULL || b == NULL || c == NULL) {
		printf("malloc failed!");
		exit(1);
	}

	int i, j;
	for(i = 0; i < N; i++) {
		a[i] = malloc(sizeof(int) * N);
		b[i] = malloc(sizeof(int) * N);
		c[i] = malloc(sizeof(int) * N);
		if(a[i] == NULL || b[i] == NULL || c[i] == NULL) {
			printf("malloc failed!");
			exit(1);
		}
	}

	for(i = 0; i < N; i++) {
		for(j = 0; j < N; j++) {
			c[i][j] = 0;
			if(i <= j) {
				a[i][j] = 1;
				b[i][j] = 1;
			}
			else {
				a[i][j] = 0;
				b[i][j] = 0;
			}
		}
	}
}

void printAll()
{
	int i, j;
	for(i = 0; i < N; i++) {
		for(j = 0; j < N; j++) {
			printf("%i\t",c[i][j]);
		}
		printf("\n");
	}
}

void printPartial()
{
	printAll();
}

void print()
{
	if(printLevel == 0)
		return;
	else if(printLevel == 1)
		printPartial();
	else
		printAll();
}

int main(int argc, char *argv[])
{
	int i, j, k;
	getArgs(argc, argv);
	init();

	pthread_t tid[P];
	int thread_id[P];
	for(i = 0;i < P; i++)
		thread_id[i] = i;

	for(i = 0; i < P; i++) {
		pthread_create(&(tid[i]), NULL, threadFunction, &(thread_id[i]));
	}

	for(i = 0; i < P; i++) {
		pthread_join(tid[i], NULL);
	}

	print();

	return 0;
}
