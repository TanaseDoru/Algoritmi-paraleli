# Ex 1
Daca join-urile sunt comentate atunci afiseaza mesajul "All threads ..." inainte ca toate thread-urile
sa isi termine executia
Thread-ul principal isi termina executia dupa ce afiseaza mesajul "All threads..."

# Ex 2
Shared value are valoarea 0 la final
Dupa incrementarea Nr de interatii la 10000 avem Shared Variable: 84
Dupa decomentare: Shared Variable: 0

# Ex 3
Daca comentez sleep
Working... 0
Worker thread was interrupted.
Worker thread cleanup.
Main program has finished.
Se arunca o exceptie ThreadInterrupt pentru ca threadFunction nu s-a terminat

Daca fac numarul de iteratii 100000 se va face interrupt si cu sleep si fara sleep deoarece nu are destul timp ca working sa se termine

# Ex 4

====================Program Start====================
Current time: 17:46:14:4614
Thread 0: [0, 125000] => 11734
Thread 1: [125000, 250000] => 10310
Elapsed Time is 24721 ms
Current time: 17:46:39:4639
====================Program END====================

====================Program Start====================
Current time: 17:47:51:4751
Thread 0: [0, 31250] => 3368
Thread 1: [31250, 62500] => 2907
Thread 2: [62500, 93750] => 2772
Thread 3: [93750, 125000] => 2687
Thread 4: [125000, 156250] => 2642
Thread 5: [156250, 187500] => 2588
Thread 6: [187500, 218750] => 2542
Thread 7: [218750, 250000] => 2538
Elapsed Time is 12541 ms
Current time: 17:48:04:484
====================Program END====================

# Ex 5
## Cu Join
Current time: 18:23:59:2359
Thread 0: [0, 31250] => 3368
Thread 1: [31250, 62500] => 2907
Thread 2: [62500, 93750] => 2772
Thread 3: [93750, 125000] => 2687
Thread 4: [125000, 156250] => 2642
Thread 5: [156250, 187500] => 2588
Thread 6: [187500, 218750] => 2542
Thread 7: [218750, 250000] => 2538
Elapsed Time is 8353 ms
Current time: 18:24:07:247

## Fara Join
Current time: 18:25:07:257
Elapsed Time is 25 ms
Current time: 18:25:07:257


## Observatii
- Am observat ca ruleaza mai rapid cu isBackground activ
- Fara join programul se opreste prematur

# Ex 6

- Programul se opreste dupa un numar de rulari deoarece functia doStuff arunca o exceptie
- Monitor.enter face lock si nu apuca sa faca unlock  pentru ca arunca o exceptie
- => DEADLOCK
- Monitor este similar cu lock
- Daca folosim lock in loc de Monitor.enter si monitor.exit atunci codul functioneaza corect Shared Variable: 0

# Ex 7

## Solutie
- In catch am pus Monitor.Exit(...) pentru a nu intra in deadlock

# Ex 8
## Descriere
- Functia scan simuleaza un scan pe 50 de fisiere pentru a le cataloga ca VIRUS, MALWARE sau CLEAN
- Log-urile le afiseaza in terminal si le face append intr-un fisier de logs.txt
- Mutex-ul este unul static in memorie, deci alte instante ale codului vor folosi acelasi mutex

## Rulare cu mai multe instante
- Eroarea afisata este:
```c
[8008] A scanning session is already running
[8008] Releasing the mutex...
Unhandled exception. System.ApplicationException: Object synchronization method was called from an unsynchronized block of code.
   at System.Threading.Mutex.Release
```

# Ex 9
## Solutie
- In Finally putem vedea daca putem da release la Mutex sau nu
- Trebuie sa verificam daca mutex-ul a fost obtinut si facem release doar daca il detin
- 
## Afisare
```c
[8856] A scanning session is already running
[8856] Mutex was not owned. Nothing to release.
Press any key to close the program.
```'

# Ex 10
Unhandled exception. System.Exception: READ_WRITE_CHECK: 15946 != 16000
- Nu exista nici un lock real pentru _mapAreas
- _READ_READ_CHECK, _WRITE_WRITE_CHECK, _READ_WRITE_CHECK se executa in acelasi timp
## Decomentare linii
- Unhandled exception. System.Exception: READ_WRITE_CHECK: 15932 != 16000
- Scriitorii si cititorii intra in acelasi timp peste dictionar

## Solutie:
- Am adaugat _rwLock.EnterWriteLock(); si _rwLock.ExitWriteLock(); la *SET*
- _rwLock.EnterReadLock(); si _rwLock.ExitReadLock(); la *GET*
- Am Facut N = 10000 ca sa se poata face o citire/scriere corespunzatoare