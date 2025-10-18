#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int P;
char str[] = "Ana are mer";
int N;
char substr[] = "are";
int pos = -1;

pthread_mutex_t posMutex = PTHREAD_MUTEX_INITIALIZER;

void* getSubstr(void* var)
{
	int tid = *(int*)var;
	int index = 0;
	int finished = strlen(substr);

	int chunk = N / P;
	int startPos = chunk * tid;
	int endPos = chunk * (tid + 1);
	int rest = N % P;

	if (tid == P - 1) {
		endPos += rest;
	}

	for (int i = startPos; i < endPos; i++) {
		if (str[i] == substr[index]) {
			index++;
			if (index == finished) {
				pthread_mutex_lock(&posMutex);
				if (pos == -1 || pos > i)
					pos = i - index + 1;
				pthread_mutex_unlock(&posMutex);
				break;
			}
		} else
			index = 0;
	}
    // in caz ca subsirul se afla dupa ce se termina endPos(chunk-ul)
	if (index > 0 && endPos < strlen(str)) {
		int i = endPos;
		while (str[i] == substr[index]) {
			index++;
			if (index == finished) {
				pthread_mutex_lock(&posMutex);
				if (pos == -1 || pos > i)
					pos = i - index + 1;
				pthread_mutex_unlock(&posMutex);
				break;
			}
			i++;
		}
	}
	return NULL;
}

int main()
{
	N = strlen(str);
	P = 2;

	pthread_t tid[P];
	int tids[P];
	for (int i = 0; i < P; i++) {
		tids[i] = i;
		pthread_create(&tid[i], NULL, getSubstr, &tids[i]);
	}

	for (int i = 0; i < P; i++) {
		pthread_join(tid[i], NULL);
	}
	if (pos != -1) {
		printf("Found substr at pos %d\n", pos);
	} else {
		printf("Substr not found!\n");
	}
	return 0;
}