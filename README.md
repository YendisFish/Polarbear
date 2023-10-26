# PolarbearDB
Polarbear is an in memory database designed to be simple and
fast to use. The point of it is to be self contained within
an application without adding extreme waiting times.

# Dependencies
Polarbear only relies upon <a href="https://dotnet.microsoft.com/en-us/">dotnet 7</a> 
(or higher) to run.

# Basic Usage
It is very easy to use Polarbear! you simply import the library
into your program:

```csharp
using Polarbear;
```

After importing Polarbear you can proceed to create a class
in which to store in the database:

```csharp
/*
    the Enterable object contains
    an "Id" string that must be
    unique to the object. By default
    it is set to be a Guid.
*/

class MyData : Enterable
{
    int myCustomData { get; set; }
    
    pubic MyData(int num)
    {
        myCustonData = num;
    }
}
```

We can then create our DB and insert an object into it:

```csharp
PolarbearDB db = new PolarbearDB();

MyData data = new MyData(5);
data.Id = "Hi!";

db.Insert(data);
```

Now to query the database for our object we can do this:

```csharp
MyData query = new MyData(100 /*this doesnt matter when querying by id*/);
data.Id = "Hi!";

MyData obj = db.QueryById(query);
```

# Roadmap
- [ ] Implement database wrapper to make scripting possible
- [ ] Add threading in order to increase speed
- [ ] Incorporate compression in order to increase efficiency