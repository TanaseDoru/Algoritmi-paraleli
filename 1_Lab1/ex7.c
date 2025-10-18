#include <math.h>
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>

void *threadFunction(void *var)
{
	int thread_id = *(int *)var;

	printf("Hello world from thread %i\n", thread_id);
}

void *threadFunction2(void *var)
{
	int thread_id = *(int *)var;

	printf("Salutare Planeta\n");
}

int main(int argc, char **argv)
{
	int P = 2;
	int i;

	pthread_t tid[P];
	int thread_id[P];
	for (i = 0; i < P; i++)
		thread_id[i] = i;

	pthread_create(&(tid[0]), NULL, threadFunction, &(thread_id[0]));
	pthread_create(&(tid[1]), NULL, threadFunction2, &(thread_id[1]));

	for (i = 0; i < P; i++) {
		pthread_join(tid[i], NULL);
	}

	return 0;
}
