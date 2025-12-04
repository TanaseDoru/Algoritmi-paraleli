
# Exercitiul 1
Codul se afla in oets-par.c

# Exercitiul 2
## Sanity check
./testCorrectnessIntensive.sh bubbleSort-seq oets-par 1000 10  
rm: cannot remove 'out*': No such file or directory
The result of your parallel program is
======================================
[Numbers...]
Sorted correctly
======================================
Running intensive correctness test with threads
Test 1/10
Test 2/10
Test 3/10
Test 4/10
Test 5/10
Test 6/10
Test 7/10
Test 8/10
Test 9/10
Test 10/10
Output correct on intensive test

## Timpi
Pentru N = 90000:
P = 1: 15,5s
P = 2: 9,1s
P = 4: 6,8s

# Exercitiu 3
## Pentru test correctiveness avem:

Sorted correctly
======================================
Running intensive correctness test with threads
Test 1/10
Test 2/10
Test 3/10
Test 4/10
Test 5/10
Test 6/10
Test 7/10
Test 8/10
Test 9/10
Test 10/10
Output correct on intensive test

## Timpi

Pentru N = 268435456:
P = 1: 27,7s
P = 2: 21,2s
P = 4: 18,2s


# Exercitiul 5 - Shear Sort
## Timpi
P = 1: 10,9s
P = 2: 7,5s
P = 4: 5,6s

## Test Correctiveness


======================================
Running intensive correctness test with threads
Test 1/10
Test 2/10
Test 3/10
Test 4/10
Test 5/10
Test 6/10
Test 7/10
Test 8/10
Test 9/10
Test 10/10
Output correct on intensive test

