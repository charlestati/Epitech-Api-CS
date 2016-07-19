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

**CAUTION**: When an error occurs, either the method returns `false` or there is an `Exception` *(depending on the source of the error)*. I advise to `try ... catch` this method.

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
`ConfigureApi()` just take a `List<String>`, in which **every row** is the content of one of your **JSON configuration file**. *(Yes, you must read-it before)*.

After that, the property `IsConfigured` can let you know if the **configuration is valid**.
Finally, **to clean API** *(configuration and database loaded)*, `ClearApi()` can be your friend.

### Sample Code
```C#
try
{
  api.ConfigureApi(new List<String>() {
      "... JSON file content ...",
      "... Another JSON file ..."
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
* LoadData() => Charge les données dans la base de données en fonctions du fichier de configuration
  * Varible pour charger les données (remplacement)
* Verrouille automatiquement la BDD ou éviter toutes modifications externe
* Database => Propriété qui retourne la base de donnée chargée

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
* Les resultats peuvent être utilisée de façon différente :
  * Utilisable comme des tableaux: db["Key"]["Key"][Key]
  * Utilisable avec une méthode: AccessTo("PathEngine")
  * Utilisable avec LINQ: db.Where(q => q is EValue && ((EValue)q).Type == String).Select(q => q.Value<String>()).ToList();

## Sample Code

## Configure JSON file

### Architecture
* Partie configuration => Obligatoire
  * API-local-config
    * API-dbName => Nom du fichier (Utilisable lors du Release v1.1)
    * API-modules => Liste des modules (pour faire plusieurs modules en fonction de se que l'on veut faire)
      * Name => Nom du module
      * Url => Url a utiliser lors du chargement de la BDD
        * Variable possible : {NameOfVar}
          * LoadData() => `{ "NameOfVar", "Value" }`
          * Sample => "This is {GREEK}!" + `{ "GREEK", "SPARTA !!" }` => "This is SPARTA !!!"
        * Variable récursive possible : ([&test={MultiVar}])
          * LoadData() => `{ "MultiVar" , new List<String>() { "Value One", "Value Two" } }`
          * Sample => "Where is Brian ? Brian is([ in {Where}])" + `{ "Where" , new List<String>() { "the kitchen,", "his house,", "his country,", "...", }}` = "Where is Brian ? Brian is in the kitchen, in his house, in his country, in ..."
* Partie module
  * La racine de l'objet doit être forcément un nom de module
  * Le reste dépend de vous: Vous choissez quoi mettre, où voulez, ~~quand vous voulez~~. Pour récupérer les données, il ne suffira que d'utiliser l'architecture que vous avez fait.
  * Exemple: { "ModuleName", { "User" : { "Profil" : "target.path" }} }

### Path Tools
  * Les éléments à recherché peuvent-être trouvé à l'aide du JPath
  * Pour generer des EObject (Dictionnary), il suffit de faire un objet JSON ( "Key" : { })
  * Pour generer des EArray (List), il faut que la cible soit un JSON array. Ensuite, il suffit d'utiliser une syntaxe spéciale: "Key" : [{"targetArray", {"Content0ne" : "one", "ContentTwo" : "two"}}]
  * Pour generer des EValue, il faut juste de faire correspondre la cible à une clé: "Key" : "this.is.a.target"
  * Il y a aussi quelques petits outils:
    * Split d'une chaine (par exemple "a.target" = "a,b,c,d"): ("a.target"|,) => EArray {"a", "b", "c", "d"}
    * Append deux EQuery (seulement EValue ou EArray): "a.target" (+) ("a.target"|,) => EArray {"a,b,c,d", "a", "b", "c", "d"}
