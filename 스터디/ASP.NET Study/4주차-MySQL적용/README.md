# ASP.NET-Study

http://deok9.com/api/Test2

### 목표
점수 저장에 MySQL 적용
, uid로 구별하기

### POST
```
{
    "seqid": 1,
    "uid": "deok9",
    "actions":
    [{
        "action": "get_player_score"
    },
    {
        "action": "set_player_score",
        "score": 131
    },
    {
        "action": "get_player_score"
    },
    {
        "action": "set_player_score",
        "score": 30
    },
    {
        "action": "get_player_score"
    },
    ]
}
```

### RESULT
```
[
  {
    "score": "",
    "action": "get_player_score",
    "status": "success"
  },
  {
    "status": "1",
    "action": "set_player_score"
  },
  {
    "score": "131",
    "action": "get_player_score",
    "status": "success"
  },
  {
    "status": "2",
    "action": "set_player_score"
  },
  {
    "score": "30",
    "action": "get_player_score",
    "status": "success"
  }
]
```

### 참고
- https://cntechsystems.tistory.com/89