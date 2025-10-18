#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <time.h>

void* threadFunction(void *var)
{
	srand(time(NULL));
	int thread_id = *(int*)var;
	int number = thread_id;
	while(number != -1)
	{
		number = rand();
	}
	return NULL;
}

int main(int argc, char **argv)
{
	int P = 12;
	int i;

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

	return 0;
}
