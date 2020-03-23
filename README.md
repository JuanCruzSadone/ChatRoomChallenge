# ChatRoomChallenge

Simple browser-based chat application using .NET core 3.1.

This application allows several users to talk in a chatroom and also to get stock quotes
from an API using a specific command.

## Installation

Create Migration

```bash
PM> add-migration MigracionInicial
```

Run Migration

```bash
PM> Update-Database
```

## Supported Commands

```
/stock={stock_name}
```

