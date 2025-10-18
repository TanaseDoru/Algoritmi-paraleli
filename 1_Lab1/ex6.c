#include <math.h>
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

int P;
void *threadFunction(void *var)
{
	int thread_id = *(int *)var;
	int hellos_per_thread = 100 / P;
	int iter = thread_id * hellos_per_thread;

	// printf("Hello world from thread %i\n", thread_id);
	for (int i = iter; i < (thread_id == P - 1 ? 101 : (hellos_per_thread * (1 + thread_id))); i++) {
		printf("Hello world; iter: %d\n", i);
	}
}

int main(int argc, char **argv)
{
	printf("%ld processes\n", sysconf(_SC_NPROCESSORS_ONLN));
	P = sysconf(_SC_NPROCESSORS_ONLN);
	int i;

	pthread_t tid[P];
	int thread_id[P];
	for (i = 0; i < P; i++)
		thread_id[i] = i;

	for (i = 0; i < P; i++) {
		pthread_create(&(tid[i]), NULL, threadFunction, &(thread_id[i]));
	}

	for (i = 0; i < P; i++) {
		pthread_join(tid[i], NULL);
	}

	return 0;
}
