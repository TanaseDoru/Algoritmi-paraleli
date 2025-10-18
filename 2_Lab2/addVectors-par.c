#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>

int NReps = 100000;
int printLevel;
int N;
int P;
int* a;
int* b;
int* c;

void getArgs(int argc, char **argv)
{
	if(argc < 4) {
		printf("Not enough paramters: ./program N printLevel P\nprintLevel: 0=no, 1=some, 2=verbouse\n");
		exit(1);
	}
	N = atoi(argv[1]);
	printLevel = atoi(argv[2]);
	P = atoi(argv[3]);
}

void init()
{
	a = malloc(sizeof(int) * N);
	b = malloc(sizeof(int) * N);
	c = calloc(sizeof(int) , N);
	if(a == NULL || b == NULL || c == NULL) {
		printf("malloc failed!");
		exit(1);
	}

	int i;
	for(i = 0; i < N; i++) {
		a[i] = i%500;
		b[i] = i%500;
	}
}

void printPartial()
{
	int i;
	for(i = 0; i < 10; i++)
		printf("%i ", c[i]);
	printf("\n");
	for(i = 20; i < N; i+=N/10)
		printf("%i ", c[i]);
	printf("\n");
	for(i = N - 10; i < N; i++)
		printf("%i ", c[i]);
	printf("\n");
}

void printAll()
{
	int i;
	for(i = 0; i < N; i++)
		printf("%i ", c[i]);
	printf("\n");
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

void* sumVector(void* var)
{
	int threadId = *(int*)var;
	int chunk = N / P;
	int rest = N % P;
	
	int startIndex = threadId * chunk;

	int endIndex = startIndex + chunk;
	if (threadId == P - 1) {
	    endIndex += rest;
	}
	for(int rep = 0; rep < NReps; rep++)
	{
		for (int i = startIndex; i  < endIndex; i++) {
  		c[i] += a[i] + b[i];
  	}
  }
	return NULL;	
}

int main(int argc, char *argv[])
{
	int i;
	getArgs(argc, argv);
	init();
	
	pthread_t tid[P];
	int thread_id[P];
	for(i = 0 ; i < P; i++)
	{
		thread_id[i] = i;
	}
	
	for(i = 0; i < P; i++)
	{
		pthread_create(&(tid[i]), NULL, sumVector, &(thread_id[i]));
	}
	
	for(i = 0; i < P; i++)
	{
		pthread_join(tid[i],NULL );
	}
	print();


	return 0;
}
