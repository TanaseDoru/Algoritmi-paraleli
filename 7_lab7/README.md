# Ex 1
## Sample-seq
- Afiseaza numere de la 1 la N, sub forma de something {i}
## Sample-par
- Afiseaza numere de la 1 la N, la fel ca la sample-seq doar ca spune si din ce thread a provenit si cate task-uri a facut thread-ul respectiv
## Workers
- Prima data se face initializare pe semafoare din functia StartWorkers cu numarul de thread-uri
- Practic avem problema producator/consumator
- Si apoi se creaza thread-uri care stau in starea idle pana cand exista un task care poate fi procesat
- Astfel avem simulat un thread pool, care fiecare task se afla in lock pana cand se face post pe sem_empty

# Ex 2
- Am transformat problema intr-o problema producer/consumer prin coada de task-uri
- Am creat o structura PathArgs si am folosit-o pentru a o da ca parametru
- Am transformat din recursiv in task-uri pentru workeri
## Output
```c
 0  5  8  3 
 0  4  9  6  8  3 
 0  4  3 
 0  4  9  6  1  2  3 
 0  4  9  7  2  3 
 0  5  8  6  9  4  3 
 0  4  9  7  5  8  3 
 0  5  8  6  9  7  2  3 
 0  1  2  3 
 0  4  9  7  2  1  6  8  3 
 0  5  7  9  4  3 
 0  5  7  2  3 
 0  4  9  6  8  5  7  2  3 
 0  5  7  9  6  8  3 
 0  4  9  6  1  2  7  5  8  3 
 0  5  8  6  1  2  7  9  4  3 
 0  1  6  9  4  3 
 0  5  7  2  1  6  8  3 
 0  5  7  9  6  1  2  3 
 0  1  2  7  5  8  3 
 0  1  2  7  9  4  3 
 0  1  6  9  7  2  3 
 0  5  7  2  1  6  9  4  3 
 0  1  2  7  9  6  8  3 
 0  1  6  8  3 
 0  4  9  7  5  8  6  1  2  3 
 0  1  6  8  5  7  9  4  3 
 0  1  2  7  5  8  6  9  4  3 
 0  5  8  6  1  2  3 
 0  1  6  8  5  7  2  3 
 0  1  6  9  7  5  8  3 
 ```

# Ex 3
Vertices = 20
Edges = 95
P = 1: 0.35
P = 2: 0.2
P = 3: 0.12
- Fiind paralelizabil, testCorecctiveness nu afiseaza acelasi rezultat, deoarece output-urile se afla in ordine diferita

# Ex 5
Vertices = 20
Edges = 60
P = 1: 0.6
P = 2: 0.3
P = 4: 0.19

