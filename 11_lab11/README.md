# Ex 1
- Programul executa pe rand o unctie asincrona GetNumbersAsync, dupa care face secvential DoOtherWork()
## Modificari
Am adaugat ` Task otherWorkTask = Task.Run(() => DoOtherWork());`


```c
DoOtherWork(0)...
Processing value 0
DoOtherWork(1)...
Processing value 1
DoOtherWork(2)...
Processing value 2
DoOtherWork(3)...
Processing value 3
DoOtherWork(4)...
DoOtherWork(5)...
Processing value 4
DoOtherWork(6)...
Processing value 5
DoOtherWork(7)...
Processing value 6
DoOtherWork(8)...
DoOtherWork(9)...
Processing value 7
DoOtherWork(10)...
Processing value 8
Processing value 9
DoOtherWork(0)...
```

# Ex 2
```rust
Processing product 1: Name=Smartphone X Pro, Category=Electronics, Description=Experience the latest in mobile technology with the Smartphone X Pro. Featuring a stunning display, powerful camera, and sleek design, this device is perfect for tech enthusiasts., Price=799.99
Processing product 2: Name=Fashionable Denim Jeans, Category=Apparel, Description=Stay on trend with our Fashionable Denim Jeans. Comfortable and stylish, these jeans are a must-have for your casual wardrobe. Perfect for any occasion., Price=59.99
Processing product 3: Name=Premium Coffee Maker, Category=Appliances, Description=Start your day right with our Premium Coffee Maker. Brew your favorite coffee blends with ease and enjoy the rich aroma and flavor every morning., Price=129.99
Processing product 4: Name=Interactive Robot Toy, Category=Toys, Description=Introduce your child to the world of robotics with our Interactive Robot Toy. This educational and entertaining toy provides hours of fun and learning for kids of all ages., Price=39.99
Processing product 5: Name=Luxurious Anti-Aging Cream, Category=Beauty, Description=Revitalize your skin with our Luxurious Anti-Aging Cream. Formulated with premium ingredients, this cream helps reduce fine lines and wrinkles, leaving your skin looking radiant and youthful., Price=89.99
Toate produsele au fost procesate.
```

# Ex 3
- Produsele avand preturi diferite, nu am reusit sa gasesc mai mult de un produs pentru un anumit pret
- Cautarea a fost facute pe baza a unui filtru aplicat pe toate produsele intoarse de catre server
- Daca s-au gasit deja *K* produse, atunci facem Cancel pe CancellationToken
```md
ex03: Cautare primele K produse cu un pret dat

Cautam primele 2 produse cu pretul 39.99...

[1/2] Found: 4 | Interactive Robot Toy | Category: Toys | Price: 39.99

Program terminat.
```

# Ex 4
## Explicatie program
- Programul ia un vector de ARRAY_SIZE elemente de la 1 la ARRAY_SIZE
- Apui afiseaza numerele pare de forma `{number} is EVEN`
## Explicatie Parallel.Foreach
- Functiia Parallel.ForEach ia fiecare element din *numbers* si le executa in paralel
- Explicatie conform documentatiei:
`Executes a foreach (For Each in Visual Basic) operation on an System.Collections.IEnumerable in which iterations may run in parallel.`

# Ex 5
- Am gasit **78498** numere prime
```cs
 Parallel.ForEach(v, i =>
 {
     if (IsPrime(i))
     {
         primes.Add(i);
     }
 });
```
- Si am definit pe primes ca
```cs
var primes = new ConcurrentBag<int>();
```

# Ex 6
- In functie de rulare am gasit urmatoarele variante:
```c
Am gasit: 250007

Am gasit: 83339

Am gasit: 166667
```
- Deci in functie de rulare Parallel.ForEach poate intoarce rezultate diferinte, depinde de care numar se proceseaza primul
- Am folosit
```cs
var cts = new CancellationTokenSource();
var options = new ParallelOptions { CancellationToken = cts.Token };
```

# Ex 7
```c
Am gasit: 2
```
- Am folosit `state.Stop();`

# Ex 8
- Programul afiseaza rezultatul `120`
- Programul fac inmultirea valorilor de la 1 la 5 in mod paralel si il returneaza in variabila rezult

# Ex 9
- Rezultat afisat: `Numar nr prime: 78498`
- Functioneaza de fiecare data la fel, afiseaza acelasi rezultat

# Ex 10
- Rezultat afisat:
```c
[0, 5] => 0
[5, 10] => 5
[0, 5] => 1
[5, 10] => 6
[0, 5] => 2
[5, 10] => 7
[5, 10] => 8
[0, 5] => 3
[0, 5] => 4
[5, 10] => 9
```
- Parallel.Invoke porneste task-urile si face wait la fiecare in parte
- Invoke executa task-uri independente in acelasi timp si asteapta ca aceasta sa se temrine

# Ex 11
- Dupa rularea programului am gasit
`Gasite 25 numere prime`
- Am impartit vectorul in Array_size / Section_count si avem nr de task-uri paralelizabile
- Am facut un vector de functii lambda in care este functia de testare a nr prime din vector intre *start* si *end*
- La final am apelat Parallel.Invoke(actions)
```cs
Action[] actions = new Action[SECTION_COUNT];

for (int i = 0; i < SECTION_COUNT; i++)
{
    int start = i * sectionSize;
    int end = (i == SECTION_COUNT - 1) ? ARRAY_SIZE : (i + 1) * sectionSize;

    actions[i] = () => ProcessSection(v, start, end, primes);
}
```



