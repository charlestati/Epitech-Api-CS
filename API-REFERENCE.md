# API Reference
{API#TECH} is an **Epitech School API**. It is compatible with **classic** authentification (*UNIX Login/Password*), but not currently with **Office365** authentification.
It is **fully configurable** in data to recover as well as their organization.


## New API instance
It's really simple to create new instance, but you need some explanation to configure it. First of all, three arguments are possible but optional :
* **Maximum execution time** of web request *(default: 1'30)*
* **Web agent** of API *(default: ".NET Epitech API")*
* **HttpStatusCode(s) to ignore** during a WebRequest Exeption *(default: none)*

*NB: I recommend ignoring `HttpStatusCode.InternalServerError`, it happens recurrently*

### Sample Code
```C#
var api = new EpitechApi(new[] { HttpStatusCode.InternalServerError }); // I use this example for the rest of the manual
var withWebAgent = new EpitechApi("My Personnal Web Agent");
var fullyBro = new EpitechApi(new TimeSpan(0, 0, 0, 0, 10), "No Agent", new[] { HttpStatusCode.InternalServerError, HttpStatusCode.RequestEntityTooLarge } )
```


## Authentification
There are two ways to authenticate:
* With **Office365 authentification**, *but it's currently not implemented*.
* With **Classic authentification**, *ie.* with Epitech Unix credentials (Login / Unix password)

To authenticate the API, just one method is required: `ConnectTo()`.

For that, specify :
  - [X] `ConnectionManager` (**Office365** or **Classic**)
  - [X] **URL** of Epitech login page
  - [X] **User login** *(email address for Office365)*
  - [X] **User password**

> **CAUTION**: When an error occurs, either the method returns `false` or there is an `Exception` *(depending on the source of the error)*. I advise to `try … catch` this method.

It also exist a property of `EpitechApi` that indicates whether the API is connected or not : `IsConnected` *(really difficult to find it, no ?)*

### Sample Code
```C#
try
{
  api.ConnectTo(ConnectionManager.Classic, "https://intra.epitech.eu/?format=json", "user_l", "maybemypassword");
}
catch (Exception e)
{
  Console.WriteLine($"Exception: {e}");
}
Console.WriteLine("I'm connected ? {api.IsConnected}"); // I'm connected ? true
```


## Configure your API
The configuration of {API#TECH} is mainly made through files JSON which let the free user choose which elements to get back (See [Configure JSON file](#configure-json-file)).

In C#, this step is *really simple*. You must just use one method *(like the others steps, I know)*.
`ConfigureApi()` just take a `List<String>`, in which **every row** is the content of one of your **JSON configuration file**. *(Yes, you must read it before)*.

After that, the property `IsConfigured` can let you know if the **configuration is valid**.
Finally, **to clean API** *(configuration and database loaded)*, `ClearApi()` can be your friend.

### Sample Code
```C#
try
{
  api.ConfigureApi(new List<String>() {
      "… JSON file content …",
      "… Another JSON file …"
    });
}
catch (Exception e)
{
  Console.WriteLine($"Exception: {e}");
}
Console.WriteLine("I'm configured ? {api.IsConfigured}"); // I'm configured ? true
api.ClearApi();
Console.WriteLine("I'm configured ? {api.IsConfigured}"); // I'm configured ? false
```


## Fill database API with results
Now that the API is almost set, it's time to **retrieve the data**. For that, `LoadData()` allows you to receive the **desired data**.
I describe the parameter in the [Configure JSON file](#configure-json-file).

> **CAUTION**: Like [Authentification](authentification), when an error occurs, an `Exception` is sent. I advise to `try … catch` this method.

Finally, `Database` property allows **to retrieve** the loaded database.

> **CAUTION**: To prevent unwanted changes, the database is locked with a `LockerManager`.

### Sample Code
```C#
try
{
  api.LoadData(new Dictionnary<String, Object>() {
      {"Var_One", "Value"},
      {"Var_Two", new List<String>() {"Value_One", "Value_Two"}}
    });
}
catch (Exception e)
{
  Console.WriteLine($"Exception: {e}");
}
var db = api.Database;
```


## Use database results

### Database objects
* `EQuery`: Basic data interface. Contains **useful methods** to access data (and for **LINQ**).
  * `Type`: Get **EQueryType** of current data.
  * `Parent`: Get **EQuery** parent of current data.
  * `IsLocked`: Property determining if the data is locked.
  * `IsNull`: Property determining if the data is null (**ENull** class type)
  * `AccessTo()`: Accessing data defined by a path **EPath** *(you can use `String`)*.
* `EObject`: Object representing data related to keys *(like C# Dictionnary)*.
* `EArray`: Object representing array of data *(like C# Array or List)*.
* `EValue`: Object containing value *(C# basic type)*.
  * `Value`: Return objet representing the Value data.
  * `Value<Type>()`: Return Value data typed with **Type**.
* `ENull`: Special object representing null or error value.
  * `Reason`: Reason for why we are ENull here.
  * `Message`: Message containing description of error.

### Usage
So, now, you are the loaded database. Great, but what to do with ? How to get data ?

**Three way** is offered to you:
* Usable like an `Array`. For example: To access *User → (Row) 2 → Name* in **db**
```C#
db["User"][2]["Name"]
```
* Usable with `AccessTo()`. For example: To access *User → (Row) 2 → Name* in **db**
```C#
db.AccessTo("User[2].Name")
```
* Usable with [`LINQ`](https://msdn.microsoft.com/en-us/library/bb397933.aspx). For example: To retrieve all name in *Users* if the name doesn't start with 'A' ⇒
```C#
db["User"]
  .Where(u => u.Name is EValue && u.name.Type == String)
  .Select(u => u.Name.Value<String>())
  .Where(n => !n.StartsWith("A"))
```


## Configure JSON file

This part is the most abstract part of this, but also the most important to properly configure {API#TECH}. Toute la configuration des données à recevoir, ainsi que l'architecture de la base de donnée est décrite dans un fichier de configuration JSON.
This one is divided in two part:

***
### Mandatory Part
This part contains all informations to link the API with the JSON file.

```json
{
  "API-local-config": {
    "API-dbName": "MyDBName",
    "API-modules": [
      {
        "Name": "MyModuleName",
        "Url": "UrlModule"
      },
      {
        "Name": "MySecondModuleName",
        "Url": "SecondUrlModule"
      }
    ]
  }
}
```

* API-dbName: Name of the configuration file (Usefull for the [Release v1.1](https://github.com/pheonyx/Epitech-Api-CS#release-v11))
* API-modules: List of user module.
  * Name: Name of the module. This is also the name of module root in the [second part](#module-part).
  * Url: Url where module informations are located.

If you want, you can create variable URL.
For that, two tools are available for you :

Simple variable : `{VarName}`
---

To use this, you must describe a definition in the parameter of [`LoadData()`](#fill-database-api-with-results) like `{ "VarName", "ValueName" }`.

For example:

**JSON**
```json
{
  "Name": "SimpleVar",
  "Url": "This is {GREEK}!"
}
```

**C#**
```c#
api.LoadData(new Dictionnary() {
    { "GREEK", "SPARTA !!" }
  });
```

**Result**
```c#
Url = "This is SPARTA !!!"
```

Loop variable : `([... {VarName}])`
---

Loop variable is like simple variable, but you can define some value for just one key.

So, to use it, you must describe a definition in the parameter of [`LoadData()`](#fill-database-api-with-results) like `{ "VarName", new List<String>() {"ValueNameOne", "ValueNameTwo"} }`.

For example:

**JSON**
```json
{
  "Name": "LoopVar",
  "Url": "Where is Brian ? Brian is([ in {Where}])"
}
```

**C#**
```c#
api.LoadData(new Dictionnary() {
    { "Where" , new List<String>()
        { "the kitchen,", "his house,", "...", }
    }
  });
```

**Result**
```c#
Url = "Where is Brian ? Brian is in the kitchen, in his house, in ..."
```

Url variable combination
---

You can use also one variable for more than one url and you can use multiple variable in one url.

For example:

**JSON**
```json
{
  "Name": "ModuleOne",
  "Url": "{SimpleVar} with ([{MultiVar}, ])end"
},
{
  "Name": "ModuleTwo",
  "Url": "([{MultiVar}] | ) {SimpleVar} | {AnotherVar}"
}
```

**C#**
```c#
api.LoadData(new Dictionnary() {
  { "SimpleVar", "SV" },
  { "MultiVar", new List<String>() {"A", "B", "C", "D"}},
  { "AnotherVar", "Another variable"}
  });
```

**Result**
```c#
ModuleOne.Url = "SV with A, B, C, D, end"
ModuleTwo.Url = "A | B | C | D | SV | Another variable"
```

***
### Module Part

This part has only one obligation: the module root must bear the name specified in module `API-local-config.API-modules.Name`. The rest depends only on you.

You choose what to put where you want, ~~when you want~~.
To retrieve the data, simply respected the architecture you created before.

Oh and the end path value (you see what I mean ? No ? Look then the example), must be a way ["JSONPath"](#path-tools) heading towards the desired value contained in the file downloaded from the intranet.

#### Sample

```Json
"Module" : {
  "User" : {
    "Name" : "root.path.to.name.value",
    "Promo" : "path.currentpromo (+) (path.to.promo.history|,)",
    "Password" : "we.can.t.found.this"
  },
  "Netsoul" : {
    "Active" : "path.to.netsoul",
    "Dark" : "path.to.black.hole"
  }
}
```

```C#
// Configure and load this JSON module
var db = api.Database;
var user = db.AccessTo("User.Name"); //EValue
var promo = db["User"]["Promo"]; //EArray containing promotion value
var none = db["None"]; //ENull containing the reason AccessFailure
```

***
### Path Tools

To facilitate the parsing of the values, the path to find the values are compatible [JSONPath](http://goessner.net/articles/JsonPath/).

Two **extensions** are availables: *Append* and *Split*.

* **Append** : `target.path.value (+) target.path.anther.value`
  * The result is an **EArray** containing the values appended.
  * You can append **EArr**ay and **EValue** (for example `target.to.an.array (+) target.to.an.another.array (+) target.path.value`)
* **Split** : `(target.to.value|key)`
  * The result is an **EArray** containing the values splitted with key.
  * You can compine **Split** and **Apend**: `(target.to.value|key) (+) target.to.an.another.array (+) target.path.value`

## Epitech JSON API

Under construction, coming soon
