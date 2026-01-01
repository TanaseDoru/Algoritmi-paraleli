#include <math.h>
#include <pthread.h>
#include <semaphore.h>
#include <stdio.h>
#include <stdlib.h>

int N;
int P;
int printLevel;

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

int resource = 0;

int read_write_check = 0;
int write_write_check = 0;
int read_read_check = 0;
pthread_mutex_t rwmutex = PTHREAD_MUTEX_INITIALIZER;

int get()
{
	read_read_check += 2;
	pthread_mutex_lock(&rwmutex);
	read_write_check += 2;
	pthread_mutex_unlock(&rwmutex);
	return resource;
}

void put(int value)
{
	write_write_check += 2;
	read_write_check += 2;
	resource = value;
}
// HERE IS WHERE YOU NEED TO IMPLEMENT YOUR SOLUTION
sem_t rw_sem;
sem_t reader_sem;

int reader_count = 0;

int get_safe()
{
	int aux;

	sem_wait(&reader_sem);
	reader_count++;
	if (reader_count == 1) {
		sem_wait(&rw_sem);
	}
	sem_post(&reader_sem);

	aux = get();

	sem_wait(&reader_sem);
	reader_count--;
	if (reader_count == 0) {
		sem_post(&rw_sem);
	}
	sem_post(&reader_sem);

	return aux;
}

void put_safe(int value)
{
	sem_wait(&rw_sem);
	pthread_mutex_lock(&rwmutex);
	put(value);
	pthread_mutex_unlock(&rwmutex);
	sem_post(&rw_sem);
}

// END HERE IS WHERE YOU NEED TO IMPLEMENT YOUR SOLUTION

int value;
void* readerThread(void* var)
{
	int i;

	for (i = 0; i < N; i++) {
		value = get_safe();
	}

	return NULL;
}

void* writerThread(void* var)
{
	int i;

	for (i = 0; i < N; i++) {
		put_safe(i);
	}

	return NULL;
}

int main(int argc, char** argv)
{
	getArgs(argc, argv);
	sem_init(&rw_sem, 0, 1);
	sem_init(&reader_sem, 0, 1);
	int i;
	int NREAD = P;
	int NWRITE = P;
	pthread_t tid[NREAD + NWRITE];

	for (i = 0; i < NREAD; i++) {
		pthread_create(&(tid[i]), NULL, readerThread, NULL);
	}

	for (; i < NREAD + NWRITE; i++) {
		pthread_create(&(tid[i]), NULL, writerThread, NULL);
	}

	for (i = 0; i < NREAD + NWRITE; i++) {
		pthread_join(tid[i], NULL);
	}

	if (N * 2 * P == read_read_check && P > 1) {
		printf("Failed two simultaneous readers\n");
		return 1;
	}
	if (N * 2 * P * 2 != read_write_check) {
		printf("Failed read when write %i %i \n", N * 2 * P * 2, read_write_check);
		return 1;
	}
	if (N * 2 * P != write_write_check) {
		printf("Failed write when write\n");
		return 1;
	}
	printf("Passed all\n");

	return 0;
}
