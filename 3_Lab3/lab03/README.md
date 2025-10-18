# raceCondition
Observatii vazute:
  - Daca se ruleaza cu -O3 nu exista diferente in rularea fara mutex
  - Daca se sterge -O3 atunci se observa diferente de output intre rularea seq si par
Solutie:
  - Am creat un mutex_sum pentru a face lock inainte de for si unlock dupa for


# Barrier
Observatii:
  - Fara bariere se afiseaza toate intr-o ordine aleatoare
Solutie:
  - Folosirea de bariere pentru a seta ordinea corecta
  - Am dublat bariera pentru a astepta ordinea corecta a thread-urilor

# semaphore Signal
Solutie:
  - Am folosit semafoare pentru a seta ordinea corecta:
    - thread_id 1 si 2 fac post dupa ce afiseaza
    - thread_id 0 are 2 wait-uri pentru a trece doar in momentul in care celelalte 2 au terminat de afisat

# deadLock1
Se face deadlock deoarece mutexul comun se face lock si nu poate celalalt sa treaca de acel pas
Solutie:
   - Facem un trylock si atunci apelul de lock devine nonblocant si trece de acea instructiune
  - Sau alta solutie este sa facem un unlock imediat dupa ce se face lock

# deadLock2
Se face deadlock deoarece primul thread face lock la MutexA, asteapta putin si dupa vrea sa faca lock la mutexB. Intre timp, thread-ul 2 face lock la mutexB si dupa incearca sa faca lock la mutexA, ambele incercand sa faca lock la mutex care sunt deja in lock.<br>
Daca stergem acel sleep atunci nu se mai face deadlock, deoarece daca primul thread face lock la mutexA va face imediat thread la mutexB si astfel va bloca ambele mutex-uri, pe cand thread2 va sta in astepare pana cand thread1 face unlock

```bash
#!/bin/bash
while [[ 1 ]]; do
  ./deadLock2
  echo "Redoing..."
done
```
Pentru a testa daca se mai face deadlock am facut un script care verifica acest lucru.
<br>
S-a observat ca tot se mai face deadlock, dar doar in anumite cazuri, deoarece in unele momente thread1 nu face lock la ambele mutex si atunci intra in cazul in care aveam sleep si se face deadlock

# deadLock3
Se face deadlock deoarece se afla lock pe acelasi mutex unul dupa celalalt<br>
Solutie:
  - Facem unlock in thread-ul 2 de 2 ori
Observatii:
  - Daca rulam codul de suficient de multe ori se va face deadLock
  Solutie:
    - Am putea folosi o bariera care sa fie inainte de lock

# sumVectorValues
## Fara optimizare
Programul nu este scalabil deoarece am folosit un mutex si astfel este blocat accesul la acea variabila globala
Pentru a-l face scalabil pot face cate o variabila locala de suma si pe aceasta sa o adun la suma principala
<br>Pentru P = 1, P = 2, P = 4 avem rezultate similare:

## Paralelizare:
Am folosit variabile de tip local_sum si la final un mutex pentru a le aduna la suma globala
<br>
Rezultate:
  - 1 thread: 0.7s
  - 2 thread: 0.6s
  - 4 thread: 0.5s
Am folosit doar 
100000000 pentru 'n' deoarece daca puneam mai mare aveam problema ca imi dadea 'killed' pe proces din cauza la malloc  

# prepStrassen
- Am facut 4 thread-uri ficare corespunzand pentru calculul cate unui C.
- In fiecare thread se calculeza 2 variabile M si dupa aceea se va astepta la o bariera, dupa care se va calcula rezultatul final
- Folosind script-ul de testare, rezultatul a iesit corespunzator
