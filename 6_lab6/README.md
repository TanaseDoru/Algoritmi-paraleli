# oneProducerOneConsumerOneBuffer
```sh
./testCorrectnessIntensive.sh oneProducerOneConsumer-fakeForScriptSeq oneProducerOneConsumerOneBuffer 1000 10
rm: cannot remove 'out*': No such file or directory
The result of your parallel program is
======================================
I finished Correctly
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

# oneProducerOneConsumerFiveBuffer
```sh
./testCorrectnessIntensive.sh oneProducerOneConsumer-fakeForScriptSeq oneProducerOneConsumerFiveBuffer 100 10
The result of your parallel program is
======================================
I finished Correctly
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

# multipleProducersMultipleConsumers
```sh
./testCorrectnessIntensive.sh multipleProducersMultipleConsumers-fakeForScriptSeq multipleProducersMultipleConsumers 1000 10 4
======================================
CORRECT
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
# readersWriters
```sh
./testCorrectnessIntensive.sh readersWriters-fakeForScriptSeq readersWriters 1000 8 4
The result of your parallel program is
======================================
Passed all
======================================
Running intensive correctness test with threads
Test 1/8
Test 2/8
Test 3/8
Test 4/8
Test 5/8
Test 6/8
Test 7/8
Test 8/8
Output correct on intensive test
```

# Problema fumatorilor
- Nu are solutie care sa satisfaca





