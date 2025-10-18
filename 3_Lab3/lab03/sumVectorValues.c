#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>

int printLevel;
int N;
int P;
long long* v;
long long sum;
pthread_mutex_t GPmutex = PTHREAD_MUTEX_INITIALIZER;

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

long long * allocVector(int N) {
	long long *v = malloc(sizeof(long long) * N);
	if(v == NULL) {
		printf("malloc failed!");
		exit(1);
	}
	return v;
}

void init()
{
	v = allocVector(N);

	long long i;
	for(i = 0; i < N; i++) {
		v[i] = i+1;
	}
}

void printPartial()
{
	printf("Sum: %lli \n", sum);
}

void printAll()
{
	printPartial();
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

void* thread_func(void* arg)
{
	int threadId = *((int*)arg);
	long long local_sum = 0;

	int chunk = N / P;
	int rest = N % P;
	int start = threadId * chunk;
	int end = (threadId + 1) * chunk;
	if(threadId == P - 1)
	{
		end += rest;
	}
	for (int i = start; i < end; i++) {
  	local_sum += v[i];
  }
	
	pthread_mutex_lock(&GPmutex);
	sum += local_sum;
	pthread_mutex_unlock(&GPmutex);
	return NULL;
}

int main(int argc, char *argv[])
{
	getArgs(argc, argv);
	init();

	long long i;
	pthread_t pids[P];
	int thread_id[P];

	sum = 0;
	for (int i = 0; i < P ; i++) {
  	thread_id[i] = i;
  }

	for(int i = 0; i < P; i++)
	{
		pthread_create(&(pids[i]), NULL, thread_func, &(thread_id[i]));	
	}
	
	for(int i = 0; i < P; i++)
	{
		pthread_join(pids[i], NULL);
	}
	// for(i = 0; i < N; i++)
	// 	sum += v[i];

	print();

	return 0;
}
