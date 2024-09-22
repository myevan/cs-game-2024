# C# RPC 서버

## 성능

### 스펙

lg-gram-i7-1165G7-2.8GHz-4c-8t

### 정적

초당 8만

nginx

```
$ wrk -t1 -c100 -d10s http://localhost/
Running 10s test @ http://localhost
  1 threads and 100 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     2.16ms    3.01ms  23.75ms   87.25%
    Req/Sec    80.77k    32.66k  129.14k    48.00%
  802402 requests in 10.00s, 657.29MB read
Requests/sec:  80218.80
Transfer/sec:     65.71MB
```

### 평문 응답

초당 13만?!

```
$ wrk -t1 -c100 -d10s http://localhost:5000/
Running 10s test @ http://localhost:5000/
  1 threads and 100 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   654.40us  628.25us  20.53ms   96.53%
    Req/Sec   139.58k     9.15k  163.56k    69.00%
  1388760 requests in 10.00s, 217.21MB read
Requests/sec: 138863.70
Transfer/sec:     21.72MB
```

### 평문 요청-응답

초당 10만

```
$ wrk -t1 -c100 -d10s http://localhost:5000/rpc/benchmark-hello?name=Jaru
Running 10s test @ http://localhost:5000/rpc/benchmark-hello?name=Jaru
  1 threads and 100 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   807.95us  658.83us  34.42ms   96.29%
    Req/Sec   103.19k     9.08k  128.06k    71.00%
  1026924 requests in 10.01s, 229.17MB read
Requests/sec: 102589.22
Transfer/sec:     22.89MB
```