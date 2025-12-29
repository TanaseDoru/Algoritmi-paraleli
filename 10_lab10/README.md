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
```c
Incep colectarea URL-urilor de imagini...

[1/5] SUCCESS => https://images.freeimages.com/images/large-previews/ee6/screaming-cat-1404453.jpg
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
[2/5] SUCCESS => https://images.freeimages.com/images/large-previews/5c9/cat-1058028.jpg
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
[3/5] SUCCESS => https://images.freeimages.com/images/large-previews/42b/white-cat-4-1362395.jpg
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
Raspuns invalid: Status = RETRY-LATER. 
[4/5] SUCCESS => https://images.freeimages.com/images/large-previews/d3f/cat-1507244.jpg
Raspuns invalid: Status = RETRY-LATER. 
[5/5] SUCCESS => https://hips.hearstapps.com/ghk.h-cdn.co/assets/17/30/1280x1919/gallery-gettyimages-485681215.jpg

Am colectat 5 URL-uri din 5 dorite.

Incep descarcarea imaginilor în paralel...
Salvata: image_2.jpg
Salvata: image_5.jpg
Salvata: image_4.jpg
Salvata: image_1.jpg
Salvata: image_3.jpg
```

# Ex 3

- Programul are o clasa de Progress si incrementeaza acel progres cu 10% la fiecare rulare pana ajunge la 10(100%)
- Se observa ca AFTER, TID si BEFORE arata toate 3 aceleasi valori

## Decomentare linie
- Se observa un delay mai mare deoarece s-a adaugat un await in plus
```c
BEFORE ProgressChanged => 10%
AFTER ProgressChanged => 10%
TID 10: ProgressChanged => 10%
BEFORE ProgressChanged => 20%
TID 9: ProgressChanged => 20%
AFTER ProgressChanged => 20%
// ...
```

# Ex 4
## Explicatie cod
- Toate task-urile ruleaza pentru a completa un progress pana la 100%
- Task1 face wait cu i * 200 ms
- Task 2 face wait cu i * 100 ms
- Task 3 face throw la o exceptie de notImplemented

## Diferentiere task-uri
- Inainte de a se face progress.report ama adaugat un Console.Write cu numarul task-ului
```c
Task3 throwing exception...
Task2: TID 9: ProgressChanged => 10%
Task1: TID 9: ProgressChanged => 10%
Task2: TID 9: ProgressChanged => 20%
Task1: TID 9: ProgressChanged => 20%
Task2: TID 9: ProgressChanged => 30%
Task2: TID 9: ProgressChanged => 40%
Task1: TID 10: ProgressChanged => 30%
Task2: TID 9: ProgressChanged => 50%
Task1: TID 9: ProgressChanged => 40%
Task2: TID 7: ProgressChanged => 60%
Task2: TID 7: ProgressChanged => 70%
Task1: TID 7: ProgressChanged => 50%
Task2: TID 7: ProgressChanged => 80%
Task1: TID 7: ProgressChanged => 60%
Task2: TID 7: ProgressChanged => 90%
Task2: TID 7: ProgressChanged => 100%
Task1: TID 7: ProgressChanged => 70%
Task1: TID 7: ProgressChanged => 80%
Task1: TID 7: ProgressChanged => 90%
Task1: TID 10: ProgressChanged => 100%
Something went wrong! => The method or operation is not implemented.
```

## Modificare cod

Am salvat rezultatul de la task1 si task2 si le-am afisat in *catch*
```c
=== Rezultate finale ===
Rezultatul task_1 (int): 65
Rezultatul task_2 (string): 12345678910
```

# Ex 5
## Explicatie
- Programul ruleaza 3 task-uri simulate folosind delay-uri
- Result reprezinta rezultatul primului task care se termina astfel:
```c
Final result: [3] => 12345678910
```
- Deci primul task care s-a terminat a fost 3 si a intors numerele concatenate de la 1 la 10

## Decomentare
```c
[1] working 8...
[3] working 9...
[2] working 9...
[1] working 9...
[3] working 10...
Final result: [3] => 12345678910
[1] working 10...
[2] working 10...
```
Am observat ca rezultatul s-a intors inainte ca toate task-urile sa se termine => Dupa ce s-a terminat primul task si celelalte task-uri au putut sa se termine la timp si astfel am vazut toate mesajele de output

## Modificare cod
-            using var cts = new CancellationTokenSource();
- Am adaugat un CancellationToken pentru a opri automat si celelate task-uri dupa ce am primit rezultatul de la primul task care si-a terminat executia

# Ex 6
```
Task cu 2 iteratii => 01
Task cu 5 iteratii => 01234
Task cu 10 iteratii => 0123456789
```

Am adaugat:
```c
while (tasks.Length > 0)
{
    Task<string> completedTask = await Task.WhenAny(tasks);

    string result = await completedTask;
    Console.WriteLine(result);

    tasks = tasks.Where(t => t != completedTask).ToArray();
}
```

# Ex 7
## Modificare cod + explicatie
- Programul ruleaza doua task-uri async
- Fiecare task ruleaza de 50 de ori, asteapta un timp si apoi face increment pe progresss, aduna numerele de la 1 la 50
- Se foloseste si CancellationToken pentru a opri al task-uri care se termina mai tarziu
- Dupa ce se termina primul task, se face cancel si se afiseaza rezultatul
```c
DoWork_2_Async: 2%
DoWork_1_Async: 2%
DoWork_2_Async: 4%
DoWork_1_Async: 4%
DoWork_1_Async: 6%
DoWork_2_Async: 6%

// ...

DoWork_1_Async: 96%
DoWork_2_Async: 86%
DoWork_1_Async: 98%
DoWork_1_Async: 100%

Primul task terminat a returnat rezultatul: 1275
```
- Se observa cum Task1 s-a temrinat si Task2 s-a oprit prematur

# Ex 8
- Am folosit API-uri de la alphaadvantage si financialmodel
```json
Primul raspuns succes: {
    "Global Quote": {
        "01. symbol": "AAPL",
        "02. open": "274.1600",
        "03. high": "275.3700",
        "04. low": "272.8600",
        "05. price": "273.4000",
        "06. volume": "21521802",
        "07. latest trading day": "2025-12-26",
        "08. previous close": "273.8100",
        "09. change": "-0.4100",
        "10. change percent": "-0.1497%"
    }
}

Sau

Primul raspuns succes: [
  {
    "symbol": "APLY.NE",
    "name": "Apple (AAPL) Yield Shares Purpose ETF",
    "currency": "CAD",
    "exchangeFullName": "CBOE CA",
    "exchange": "NEO"
  },
  {
    "symbol": "APLY",
    "name": "YieldMax AAPL Option Income Strategy ETF",
    "currency": "USD",
    "exchangeFullName": "New York Stock Exchange Arca",
    "exchange": "AMEX"
  },
  {
    "symbol": "AAPW",
    "name": "Roundhill AAPL WeeklyPay ETF",
    "currency": "USD",
    "exchangeFullName": "Chicago Board Options Exchange",
    "exchange": "CBOE"
  },
  {
    "symbol": "AAPD",
    "name": "Direxion Daily AAPL Bear 1X Shares",
    "currency": "USD",
    "exchangeFullName": "NASDAQ Global Market",
    "exchange": "NASDAQ"
  },
  {
    "symbol": "AAPU",
    "name": "Direxion Daily AAPL Bull 1.5X Shares",
    "currency": "USD",
    "exchangeFullName": "NASDAQ Global Market",
    "exchange": "NASDAQ"
  },
  {
    "symbol": "AAPB",
    "name": "GraniteShares 2x Long AAPL Daily ETF",
    "currency": "USD",
    "exchangeFullName": "NASDAQ Global Market",
    "exchange": "NASDAQ"
  },
  {
    "symbol": "AAPS.L",
    "name": "Leverage Shares -3x Short Apple (AAPL) ETP Securities",
    "currency": "USD",
    "exchangeFullName": "London Stock Exchange",
    "exchange": "LSE"
  },
  {
    "symbol": "AAPY",
    "name": "Kurv Yield Premium Strategy Apple (AAPL) ETF",
    "currency": "USD",
    "exchangeFullName": "Chicago Board Options Exchange",
    "exchange": "CBOE"
  }
]
```