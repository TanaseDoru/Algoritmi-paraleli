# Ex1 
## Output:
```c
// Rulare 1
TID[1] 
TID[7] => 0
TID[7]
TID[8] => 1
TID[7] => 2
TID[7] => 3
TID[7] => 4
TID[7]
TID[7]

// Rulare 2
TID[1]
TID[7] => 0
TID[9]
TID[9] => 1
TID[9] => 2
TID[9] => 3
TID[9] => 4
TID[9]
TID[9]
```

## Explicatie
- Tid[1] -> thread-ul principal/main
- Tid[7] -> Thread-ul care face Task.Run
- Tid[8] -> Continua executia dupa apelarea lui await.

## Schimare tid
- Tid se schimba deoarece await Task,Delay este nonblocant, deci thread-ul cu id-ul 7 face alt task, in timp ce urmatorul thread liber este cel cu tid 9, care continua numararea

## Decomentare linie
- Daca decomentam linia 32 observam ca se asteapta finalizarea numaratorii de 50 de elemente si functia nu se inchide prematur
```c
TID[1]
TID[9] => 0
TID[10] => 1
TID[9]
TID[9] => 2
TID[9] => 3
TID[9] => 4
TID[7]
TID[7]
TID[7] => 5
TID[7] => 6
// ...
```
# Ex 2



