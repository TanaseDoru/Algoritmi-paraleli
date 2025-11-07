#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <math.h>

int printLevel;
int N;
int *v;
int *vQSort;
int n; // matrix dimension (sqrt(N))
int numThreads;
pthread_barrier_t barrier;

/* ========== Utility functions ========== */

/* comparator for ascending ints (safe) */
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

void compareVectors(int *a, int *b) {
    for (int i = 0; i < N; i++) {
        if (a[i] != b[i]) {
            printf("Sorted incorrectly\n");
            return;
        }
    }
    printf("Sorted correctly\n");
}

/* display vector in the same style as your example */
void displayVector(int *vec) {
    int i;
    int max_digits = 1;
    for (i = 0; i < N; i++) {
        if (vec[i] > 0) {
            int digits = (int)log10((double)vec[i]) + 1;
            if (digits > max_digits) max_digits = digits;
        }
    }
    int displayWidth = 2 + max_digits;
    for (i = 0; i < N; i++) {
        printf("%*i", displayWidth, vec[i]);
        if (!((i + 1) % 20))
            printf("\n");
    }
    printf("\n");
}

/* ========== Sorting helpers ========== */
void sort_row(int *matrix, int row, int ascending) {
    int *start = matrix + row * n;
    if (ascending)
        qsort(start, n, sizeof(int), cmp_asc);
    else
        qsort(start, n, sizeof(int), cmp_desc);
}

void sort_col(int *matrix, int col) {
    int *temp = malloc(sizeof(int) * n);
    if (!temp) { printf("malloc failed\n"); exit(1); }
    for (int i = 0; i < n; i++)
        temp[i] = matrix[i * n + col];
    qsort(temp, n, sizeof(int), cmp_asc);
    for (int i = 0; i < n; i++)
        matrix[i * n + col] = temp[i];
    free(temp);
}

/* ========== Thread function ========== */
typedef struct {
    int id;
} ThreadArg;

void *thread_func(void *arg) {
    ThreadArg *targ = (ThreadArg *)arg;
    int id = targ->id;

    /* partition rows/cols among threads */
    int start = id * n / numThreads;
    int end   = (id + 1) * n / numThreads;

    int phases = (int)ceil(log2((double)n)) + 1;

    for (int p = 0; p < phases; p++) {
        /* Row phase */
        for (int r = start; r < end; r++) {
            int ascending = (r % 2 == 0);
            sort_row(v, r, ascending);
        }

        pthread_barrier_wait(&barrier);

        /* Column phase */
        for (int c = start; c < end; c++) {
            sort_col(v, c);
        }

        pthread_barrier_wait(&barrier);
    }

    /* Final row sort */
    for (int r = start; r < end; r++) {
        int ascending = (r % 2 == 0);
        sort_row(v, r, ascending);
    }

    pthread_barrier_wait(&barrier);
    return NULL;
}

/* ========== Initialization and utilities ========== */
void getArgs(int argc, char **argv) {
    /* Usage: ./program N printLevel numThreads
       - N: total elements (must be perfect square)
       - printLevel: 0=no print, 1=partial (just check), 2=full display
       - numThreads: number of threads to use (1..n)
    */
    if (argc < 4) {
        printf("Usage: %s N printLevel numThreads\n", argv[0]);
        exit(1);
    }
    N = atoi(argv[1]);
    if (N <= 0) {
        printf("N must be positive\n");
        exit(1);
    }
    if (sqrt(N) != (int)sqrt(N)) {
        printf("N must be a perfect square (e.g., 16, 64, 256)\n");
        exit(1);
    }
    n = (int)sqrt(N);
    printLevel = atoi(argv[2]);
    numThreads = atoi(argv[3]);
    if (numThreads <= 0 || numThreads > n) {
        printf("numThreads must be between 1 and n (sqrt(N) = %d)\n", n);
        exit(1);
    }
}

void init() {
    v = malloc(sizeof(int) * N);
    vQSort = malloc(sizeof(int) * N);
    if (v == NULL || vQSort == NULL) {
        printf("malloc failed!\n");
        exit(1);
    }
    srand(42);
    for (int i = 0; i < N; i++)
        v[i] = rand() % N;
}

/* ========== Linearize snake & printing ========== */
void linearize_snake(int *matrix) {
    int *tmp = malloc(sizeof(int) * N);
    if (!tmp) { printf("malloc failed\n"); exit(1); }
    int idx = 0;
    for (int r = 0; r < n; r++) {
        if (r % 2 == 0)
            for (int c = 0; c < n; c++)
                tmp[idx++] = matrix[r * n + c];
        else
            for (int c = n - 1; c >= 0; c--)
                tmp[idx++] = matrix[r * n + c];
    }
    for (int i = 0; i < N; i++)
        matrix[i] = tmp[i];
    free(tmp);
}

void printPartial() { compareVectors(v, vQSort); }

void printAll() {
    displayVector(v);
    displayVector(vQSort);
    compareVectors(v, vQSort);
}

void printResult() {
    if (printLevel == 0)
        return;
    else if (printLevel == 1)
        printPartial();
    else
        printAll();
}

/* ========== Main ========== */
int main(int argc, char *argv[]) {
    getArgs(argc, argv);
    init();

    for (int i = 0; i < N; i++)
        vQSort[i] = v[i];
    qsort(vQSort, N, sizeof(int), cmp_asc);

    pthread_t *threads = malloc(sizeof(pthread_t) * numThreads);
    ThreadArg *args = malloc(sizeof(ThreadArg) * numThreads);
    if (!threads || !args) { printf("malloc failed\n"); exit(1); }

    if (pthread_barrier_init(&barrier, NULL, numThreads) != 0) {
        printf("Failed to init barrier\n");
        exit(1);
    }

    for (int i = 0; i < numThreads; i++) {
        args[i].id = i;
        if (pthread_create(&threads[i], NULL, thread_func, &args[i]) != 0) {
            printf("Failed to create thread %d\n", i);
            exit(1);
        }
    }

    for (int i = 0; i < numThreads; i++)
        pthread_join(threads[i], NULL);

    pthread_barrier_destroy(&barrier);
    free(threads);
    free(args);

    /* Convert matrix into snake-linearized array for final comparison/printing */
    linearize_snake(v);
    printResult();

    free(v);
    free(vQSort);
    return 0;
}
