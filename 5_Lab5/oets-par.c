#include <math.h>
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>

int printLevel;
int N;
int P;
int* v;
int* vQSort;
int* vNew;
pthread_barrier_t GP_barrier;

void compareVectors(int* a, int* b)
{
	// DO NOT MODIFY
	int i;
	for (i = 0; i < N; i++) {
		if (a[i] != b[i]) {
			printf("Sorted incorrectly\n");
			return;
		}
	}
	printf("Sorted correctly\n");
}

void displayVector(int* v)
{
	// DO NOT MODIFY
	int i;
	int max = 1;
	for (i = 0; i < N; i++)
		if (max < log10(v[i]))
			max = log10(v[i]);
	int displayWidth = 2 + max;
	for (i = 0; i < N; i++) {
		printf("%*i", displayWidth, v[i]);
		if (!((i + 1) % 20))
			printf("\n");
	}
	printf("\n");
}

int cmp(const void* a, const void* b)
{
	// DO NOT MODIFY
	int A = *(int*)a;
	int B = *(int*)b;
	return A - B;
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

void init()
{
	int i;
	v = malloc(sizeof(int) * N);
	vQSort = malloc(sizeof(int) * N);
	vNew = malloc(sizeof(int) * N);
	if (v == NULL) {
		printf("malloc failed!");
		exit(1);
	}

	// generate the vector v with random values
	// DO NOT MODIFY
	srand(42);
	for (i = 0; i < N; i++)
		v[i] = rand() % N;
}

void printPartial()
{
	compareVectors(v, vQSort);
}

void printAll()
{
	displayVector(v);
	displayVector(vQSort);
	compareVectors(v, vQSort);
}

void print()
{
	if (printLevel == 0)
		return;
	else if (printLevel == 1)
		printPartial();
	else
		printAll();
}

void* threadFunction(void* arg)
{
	int thread_id = *((int*)arg);
	int phase = 0;
	int aux;
	// phase folosit la indexarea in functie (ca sa nu facem 2 for-uri separate)
	// int chunk =  c

	for (int it = 0; it < N; it++) {
		for (int i = (phase % 2) + thread_id * 2; i < N - 1; i += 2*P) {
			if (v[i] > v[i + 1]) {
				aux = v[i];
				v[i] = v[i + 1];
				v[i + 1] = aux;
			}
		}
		pthread_barrier_wait(&GP_barrier);
		phase++;
	}

	// int sorted = 0;
	// while(!sorted) {
	// 	sorted = 1;
	// 	for(i = 0; i < N-1; i++) {
	// 		if(v[i] > v[i + 1]) {
	// 			aux = v[i];
	// 			v[i] = v[i + 1];
	// 			v[i + 1] = aux;
	// 			sorted = 0;
	// 		}
	// 	}
	// }

	return NULL;
}

int main(int argc, char* argv[])
{
	int i;
	getArgs(argc, argv);
	init();
	pthread_barrier_init(&GP_barrier, NULL, P);

	// make copy to check it against qsort
	// DO NOT MODIFY
	for (i = 0; i < N; i++)
		vQSort[i] = v[i];
	qsort(vQSort, N, sizeof(int), cmp);

	// sort the vector v
	// PARALLELIZE ME

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

	print();

	return 0;
}
