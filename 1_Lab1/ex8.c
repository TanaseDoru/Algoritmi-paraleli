#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

void *threadFunction(void *arg)
{
	sleep(1000);
	return NULL;
}

int main()
{
	pthread_t tid;
	int count = 0;
	int err;

	while (err == 0) {
		err = pthread_create(&tid, NULL, threadFunction, NULL);
		if (err)
			perror("pthread");
		count++;
	}
	printf(
	    "Maximum number of thread within a Process is : %d\n",
	    count);

	return 0;
}
