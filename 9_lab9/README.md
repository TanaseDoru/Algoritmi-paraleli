# Ex 1
CancellationToken inseamna ca programul se opreste in momentul cand acel token este activat(.Cancel())
Daca decomentam linia 32 atunci actiunea de cancel se paote face mai devereme. Adica in loc sa asteptam 2 cicluri de Thread.Sleep(), vom astepta maxim unul. Dupa primul Sleep se face iar verificare pe cancellationToken

# Ex 2
Threads = 1: Elapsed time: 722ms
Threads = 2: Elapsed time: 514ms
Threads = 4: Elapsed time: 235ms
Threads = 8: Elapsed time: 315ms


# Ex 3
=== Encrypting with 1 threads
Time: 398 ms

=== Encrypting with 2 threads
Time: 175 ms

=== Encrypting with 4 threads
Time: 150 ms

=== Encrypting with 8 threads
Time: 93 ms

# Ex 4
=== Decrypting with 1 threads
Decryption time: 923 ms
Decryption finished.

=== Decrypting with 2 threads
Decryption time: 477 ms
Decryption finished.

=== Decrypting with 4 threads
Decryption time: 385 ms
Decryption finished.

=== Decrypting with 8 threads
Decryption time: 234 ms
Decryption finished.

# Ex 5
[Requester] Eroare/Retry: Raspuns vid de la server. A?tept 1s...
[Requester] Eroare/Retry: Raspuns vid de la server. A?tept 2s...
[Requester] Eroare/Retry: Raspuns vid de la server. A?tept 4s...

<b>Server-ul nu raspunde</b> 
