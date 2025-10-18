#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int P;
char str[] = "Ana are mere!";
char substr[] = "are";
int pos = -1;

void getSubstr(char* str, char* substr)
{
    int index = 0;
    int finished = strlen(substr);
    for(int i = 0; i < strlen(str); i++)
    {
        if(str[i] == substr[index])
        {
            index++;
            if(index == finished)
            {
                pos = i - index + 1;
                break;
            }
        }
        else
            index = 0;
    }
}


int main()
{
    getSubstr(str, substr);
    if(pos != -1)
    {
        printf("Found substr at pos %d\n", pos);
    }
    else
    {
        printf("Substr not found!\n");
    }
    return 0;
}