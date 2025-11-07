#include <stdio.h>
#include <stdlib.h>
#include <math.h>

int printLevel;
int N;
int *v;
int *vQSort;

void compareVectors(int *a, int *b) {
    for (int i = 0; i < N; i++) {
        if (a[i] != b[i]) {
            printf("Sorted incorrectly\n");
            return;
        }
    }
    printf("Sorted correctly\n");
}

void displayVector(int *v) {
    int i;
    int max = 1;
    for (i = 0; i < N; i++)
        if (v[i] > 0 && max < (int)log10(v[i]))
            max = (int)log10(v[i]);
    int displayWidth = 2 + max;
    for (i = 0; i < N; i++) {
        printf("%*i", displayWidth, v[i]);
        if (!((i + 1) % 20))
            printf("\n");
    }
    printf("\n");
}

int cmp(const void *a, const void *b) {
    int A = *(int *)a;
    int B = *(int *)b;
    return A - B;
}

void getArgs(int argc, char **argv) {
    if (argc < 3) {
        printf("Not enough parameters: ./program N printLevel\n");
        exit(1);
    }
    N = atoi(argv[1]);
    if (sqrt(N) != (int)sqrt(N)) {
        printf("N must be a perfect square (e.g., 16, 64, 256)\n");
        exit(1);
    }
    printLevel = atoi(argv[2]);
}

void init() {
    int i;
    v = malloc(sizeof(int) * N);
    vQSort = malloc(sizeof(int) * N);
    if (v == NULL || vQSort == NULL) {
        printf("malloc failed!");
        exit(1);
    }

    srand(42);
    for (i = 0; i < N; i++)
        v[i] = rand() % N;
}

void printPartial() {
    compareVectors(v, vQSort);
}

void printAll() {
    displayVector(v);
    displayVector(vQSort);
    compareVectors(v, vQSort);
}

void print() {
    if (printLevel == 0)
        return;
    else if (printLevel == 1)
        printPartial();
    else
        printAll();
}

/* --- Shear sort helpers using qsort for correctness --- */

/* comparator for ascending ints */
int cmp_asc(const void *a, const void *b) {
    int A = *(int *)a;
    int B = *(int *)b;
    return (A > B) - (A < B);
}

/* comparator for descending ints */
int cmp_desc(const void *a, const void *b) {
    int A = *(int *)a;
    int B = *(int *)b;
    return (B > A) - (B < A);
}

/* sort a single row (row index, 0-based), ascending if ascending!=0, else descending */
void sort_row_qsort(int *matrix, int row, int n, int ascending) {
    int *start = matrix + row * n;
    if (ascending)
        qsort(start, n, sizeof(int), cmp_asc);
    else
        qsort(start, n, sizeof(int), cmp_desc);
}

/* sort a single column (col index, 0-based), ascending */
void sort_col_qsort(int *matrix, int col, int n) {
    int *temp = malloc(sizeof(int) * n);
    if (!temp) { printf("malloc failed\n"); exit(1); }
    for (int i = 0; i < n; i++)
        temp[i] = matrix[i * n + col];
    qsort(temp, n, sizeof(int), cmp_asc);
    for (int i = 0; i < n; i++)
        matrix[i * n + col] = temp[i];
    free(temp);
}

/* perform shear sort on n x n matrix stored row-major in 'matrix' */
void shear_sort(int *matrix, int n) {
    /* number of iterations: ceil(log2(n)) + 1 (each iteration = row-phase + col-phase) */
    int phases = (int)ceil(log2((double)n)) + 1;

    for (int p = 0; p < phases; p++) {
        /* Row phase: sort each row; even rows ascending, odd rows descending */
        for (int r = 0; r < n; r++) {
            int ascending = (r % 2 == 0);
            sort_row_qsort(matrix, r, n, ascending);
        }

        /* Column phase: sort each column ascending */
        for (int c = 0; c < n; c++) {
            sort_col_qsort(matrix, c, n);
        }
    }

    /* Final row-phase to ensure rows are locally sorted in snake order */
    for (int r = 0; r < n; r++) {
        int ascending = (r % 2 == 0);
        sort_row_qsort(matrix, r, n, ascending);
    }

    /* Linearize into standard ascending order expected by compareVectors:
       read rows in snake order and write them back left-to-right into matrix (flattened) */
    int *tmp = malloc(sizeof(int) * n * n);
    if (!tmp) { printf("malloc failed\n"); exit(1); }
    int idx = 0;
    for (int r = 0; r < n; r++) {
        if (r % 2 == 0) {
            /* left -> right */
            for (int c = 0; c < n; c++)
                tmp[idx++] = matrix[r * n + c];
        } else {
            /* right -> left */
            for (int c = n - 1; c >= 0; c--)
                tmp[idx++] = matrix[r * n + c];
        }
    }
    /* copy back to matrix (v) so compareVectors sees a normal ascending flattening */
    for (int i = 0; i < n * n; i++)
        matrix[i] = tmp[i];

    free(tmp);
}

int main(int argc, char *argv[]) {
    getArgs(argc, argv);
    init();

    /* Copy for qsort validation */
    for (int i = 0; i < N; i++)
        vQSort[i] = v[i];
    qsort(vQSort, N, sizeof(int), cmp);

    int n = (int)sqrt(N);
    shear_sort(v, n);

    print();

    free(v);
    free(vQSort);
    return 0;
}

