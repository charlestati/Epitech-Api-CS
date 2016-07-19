# API Reference
{API#TECH} is an **Epitech School API**. It is compatible with **classic** authentification (*UNIX Login/Password*), but not currently with **Office365** authentification.
It is **fully configurable** in data to recover as well as their organization.

## New API instance
It's really simple to create new instance, but you need some explanation to configure it. First of all, three arguments are possible but optional :
* **Maximum execution time** of web request *(default: 1'30)*
* **Web agent** of API *(default: ".NET Epitech API")*
* **HttpStatusCode(s) to ignore** during a WebRequest Exeption *(default: none)*

*NB: I recommend ignoring `HttpStatusCode.InternalServerError`, it happens recurrently*

### Sample
```C#
var api = new EpitechApi(new[] { HttpStatusCode.InternalServerError }); // I use this example to the rest of the manual
var withWebAgent = new EpitechApi("My Personnal Web Agent");
var fullyBro = new EpitechApi(new TimeSpan(0, 0, 0, 0, 10), "No Agent", new[] { HttpStatusCode.InternalServerError, HttpStatusCode.RequestEntityTooLarge } )
```

## Authentification
* Choisir son module de connexion
  * Office365 pour se connecter avec un compte Microsoft Compatible
  * Classique pour se connecter avec les identifiants habituels (Login / Password UNIX)
* ConnectTo() => Permet de connecter l'API. Si il y a une erreur de connexion (url, identifiants), une exception sera faite et l'API ne sera pas connectée.
* IsConnected => Propriété permetant de savoir si l'API est connectée

## Configure your API
* ConfigureApi => Configure l'API. Prend en parametre le contenu des fichiers de config JSON (See [Configure JSON file section](#configure-json-file)).
* IsConfigured => Propriété permetant de savoir si l'API est configurée
* ClearApi() => Nettoie l'API (Fichiers de configuration / Base de données)

## Fill database API with results
* LoadData() => Charge les données dans la base de données en fonctions du fichier de configuration
  * Varible pour charger les données (remplacement)
* Verrouille automatiquement la BDD ou éviter toutes modifications externe
* Database => Propriété qui retourne la base de donnée chargée

## Use database results

### Database objects
* ```EQuery```: Basic data interface. Contains **useful methods** to access data (and for **LINQ**).
  * ```Type```: Get **EQueryType** of current data.
  * ```Parent```: Get **EQuery** parent of current data.
  * ```IsLocked```: Property determining if the data is locked.
  * ```IsNull```: Property determining if the data is null (**ENull** class type)
  * ```AccessTo()```: Access a data defined by a path **EPath**.
* ```EObject```: Object representing data related to keys (like C# Dictionnary).
* ```EArray```: Object representing array of data (like C# Array or List).
* ```EValue```: Object containing value (C# classic type).
* ```ENull```: Special object representing null or error value.
  * ```Reason```: Reason for why we are ENull here.
  * ```Message```: Message containing description of error.

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
          * LoadData() => ```{ "NameOfVar", "Value" }```
          * Sample => "This is {GREEK}!" + ```{ "GREEK", "SPARTA !!" }``` => "This is SPARTA !!!"
        * Variable récursive possible : ([&test={MultiVar}])
          * LoadData() => ```{ "MultiVar" , new List<String>() { "Value One", "Value Two" } }```
          * Sample => "Where is Brian ? Brian is([ in {Where}])" + ```{ "Where" , new List<String>() { "the kitchen,", "his house,", "his country,", "...", }}``` = "Where is Brian ? Brian is in the kitchen, in his house, in his country, in ..."
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
