# API Reference
{API#TECH} is an **Epitech School API**. It is compatible with **classic** authentification (*UNIX Login/Password*), but not currently with **Office365** authentification.
It is **fully configurable** in data to recover as well as their organization.

## New API instance
* Création d'une instance de l'API
* On peut parameter:
  * le temps de reponse maxi d'une requête
  * l'agent web utilisé par l'API
  * les erreurs HTML à ignorer (Arrive souvent)

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
* Verrouille automatiquement la BDD ou éviter toutes modifications externe
* Database => Propriété qui retourne la base de donnée chargée

## Use database results
* Les resultats peuvent être utilisée de façon différente :
  * Utilisable comme des tableaux: db["Key"]["Key"][Key]
  * Utilisable avec 

## Sample Code

## Configure JSON file
