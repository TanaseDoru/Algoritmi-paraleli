#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <threads.h>
#define N 3

int P;

int mat1[N][N] =
    {{1, 2, 3},
     {4, 5, 6},
     {7, 8, 9}};

int mat2[N][N] =
    {{1, 2, 3},
     {4, 5, 6},
     {7, 8, 9}};
int* V1;
int* V2;

int result[N][N] = {};
int resultV[N * N] = {};

int* matrixToVector(int mat[N][N], int n)
{
	int* v = malloc(n * n * sizeof(int));
	for (int i = 0; i < n; i++) {
		for (int j = 0; j < n; j++) {
			v[i * n + j] = mat[i][j];
		}
	}
	return v;
}

void* threadFunc(void* args)
{
	int tid = *(int*)args;
	int chunk = N * N / P;
	int startIndex = chunk * tid;
	int endIndex = chunk * (tid + 1);
	int rest = N * N % P;
	if (tid == P - 1) {
		endIndex += rest;
	}
	for (int i = startIndex; i < endIndex; i++) {
		resultV[i] = V1[i] + V2[i];
	}
	return NULL;
}

void vectToMatrix(int v[N * N], int mat[N][N])
{


	for (int i = 0; i < N; i++) {
		for (int j = 0; j < N; j++) {
			mat[i][j] = v[i * N + j];
		}
	}
	return;
}

int main()
{
	// for(int i = 0; i < N; i++)
	//   for(int j = 0; j < N; j++)
	//     result[i][j] = mat1[i][j] + mat2[i][j];

	P = 2;
	V1 = matrixToVector(mat1, N);
	V2 = matrixToVector(mat2, N);
	pthread_t tid[P];
	int threadIDS[P];

	for (int i = 0; i < P; i++) {
		threadIDS[i] = i;
		pthread_create(&tid[i], NULL, threadFunc, &threadIDS[i]);
	}

	for (int i = 0; i < P; i++) {
		pthread_join(tid[i], NULL);
	}
	vectToMatrix(resultV, result);

	for (int i = 0; i < N; i++) {
		for (int j = 0; j < N; j++)
			printf("%d ", result[i][j]);
		printf("\n");
	}
	return 0;
}
