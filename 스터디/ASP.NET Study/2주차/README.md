# ASP.NET-Study

http://deok9.com/Yes

### BODY! 읽고 싶어요

```
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});
```

Body를 읽으려고 했지만 무수한 실패\
구글링 끝에 찾아낸 코드\
벼락치기로 인해 무슨 의미인지 아직 파악하지 못했습니다

### POST
```
{
    "seqid": 1,
    "uid": "TEST001",
    "actions":
    [{
        "action": "echo",
        "text": "Hello World"
    },
    {
        "action": "add",
        "a": 3,
        "b": 7
    },
        {
        "action": "multiply",
        "a": 35,
        "b": 71
    },
    {
        "action": "blablabla"
    }]
}
```

### RESULT
```
[
  {
    "action": "echo",
    "text": "Hello World",
    "status": "success"
  },
  {
    "action": "add",
    "sum": "10",
    "status": "success"
  },
  {
    "action": "multiply",
    "result": "2,485",
    "status": "success"
  },
  {
    "action": "blablabla",
    "status": "INVALID_REQUEST"
  }
]
```

### 스샷
![스샷](https://github.com/hjine01/ASP.NET-Study/blob/main/2%EC%A3%BC%EC%B0%A8/2%EC%A3%BC%EC%B0%A8%20%EA%B2%B0%EA%B3%BC.png)

### 으악 이게 아니구나

지금까지 Map을 사용해 함수 한개에 다 때려박고있었습니다..

http://deok9.com/api/Test2

용님 코드를 보고 다시 구조를 맞춰서 생성했습니다
