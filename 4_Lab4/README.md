# Multiply-matrixes out
## Explicatie:
- Am folosit un localC care este o matrice unde se face adunarea pentru o bucata din matricea mare (chunk linii)
## Test:
Rulare cu N = 1500:
P = 1: 22s
P = 2: 11,3s
P = 4: 7,1s

# Exercitiul 2
```c
The result of your parallel program is
======================================
1	2	3	4	5	
0	1	2	3	4	
0	0	1	2	3	
0	0	0	1	2	
0	0	0	0	1	
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
```
# multiplyMatrices-mid
# Exercitiul 4
Pentru N = 5 si 5 teste:
```c
The result of your parallel program is
======================================
1	2	3	4	5	
0	1	2	3	4	
0	0	1	2	3	
0	0	0	1	2	
0	0	0	0	1	
======================================
Running intensive correctness test with threads
Test 1/5
Test 2/5
Test 3/5
Test 4/5
Test 5/5
Output correct on intensive test
```

## Test
- P = 1: 23,6s
- P = 2: 11,8s
- P = 4: 7,4s

# multiplyMatrices-in
# Exercitiul 6
Pentru N = 5 si 5 teste:
```c

The result of your parallel program is
======================================
1	2	3	4	5	
0	1	2	3	4	
0	0	1	2	3	
0	0	0	1	2	
0	0	0	0	1	
======================================
Running intensive correctness test with threads
Test 1/5
Test 2/5
Test 3/5
Test 4/5
Test 5/5
Output correct on intensive test
```

## Test
- P = 1: 23,3s
- P = 2: 9,7s
- P = 4: 5,4s

 
