#include <stdlib.h>
#include <stdio.h>
#define N 3

int mat1[N][N] = 
{{1, 2, 3},
 {4, 5, 6},
 {7, 8, 9}};

int mat2[N][N] =
{{1, 2, 3},
 {4, 5, 6},
 {7, 8, 9}};

int* matrixToVector(int **mat, int n)
{
  int *v = malloc(n*n*sizeof(int));
  for(int i = 0; i < n; i++)
  {
    for(int j = 0; j < n; j++)
    {
      v[i*n + j] = mat[i][j];
    }
  }
  return v;
}

int main()
{
  int result[N][N] = {};
  for(int i = 0; i < N; i++)
    for(int j = 0; j < N; j++)
      result[i][j] = mat1[i][j] + mat2[i][j];
  for(int i = 0; i < N; i++)
  {
    for(int j = 0; j < N; j++)
      printf("%d ", result[i][j]);
    printf("\n");
  }
  return 0;
}
