#include <complex.h>
#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>

int printLevel;
int N;
int P;
pthread_barrier_t first_barrier; 

void* threadFunction(void *var)
{
	//TODO preserve the correct order by using barriers
	int thread_id = *(int*)var;
	if(thread_id==0){
		pthread_barrier_wait(&first_barrier);
		pthread_barrier_wait(&first_barrier);
		printf("I should be displayed last\n");
	}
	else if(thread_id==1)
	{
		pthread_barrier_wait(&first_barrier);
		printf("I should be displayed in the middle\n");
		pthread_barrier_wait(&first_barrier);
	}
	else if(thread_id==2)
	{
		printf("I should be displayed first\n");
		pthread_barrier_wait(&first_barrier);
		pthread_barrier_wait(&first_barrier);
		
	}
		return NULL;
}

void getArgs(int argc, char **argv)
{

}

void init() 
{
}	

void printAll()
{
}

void printPartial()
{
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
	getArgs(argc, argv);
	init();

	P = 3; // ATTENTION, WE OVERWRITE THE NUMBER OF THREADS. WE ONLY NEED 3
	int i;

	pthread_barrier_init(&first_barrier, NULL, P);
	pthread_t tid[P];
	int thread_id[P];
	for(i = 0;i < P; i++)
		thread_id[i] = i;
	//DO NOT EDIT
	for(i = 0; i < P; i++) {
		pthread_create(&(tid[i]), NULL, threadFunction, &(thread_id[i]));
	}
	//DO NOT EDIT
	for(i = 0; i < P; i++) {
		pthread_join(tid[i], NULL);
	}
	//DO NOT EDIT
	print();

	return 0;
}
