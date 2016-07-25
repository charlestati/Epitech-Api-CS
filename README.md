# {API#TECH} - Epitech API Client Library for .NET
[![Build status](https://ci.appveyor.com/api/projects/status/j3sjbecy331b02fx?svg=true)](https://ci.appveyor.com/project/pheonyx/epitech-api-cs)

**{API#TECH}** is an API Client. Its goal is to **provide an easy way** to communicate with the Intranet Website of [Epitech school](http://www.epitech.eu/). It can be used to improve the development of **Mobile** or **Desktop applications** for Students, like Studients' Schedule manager.

***

## Table of Content

  - [Usage examples](#usage-examples)
  - [Supported Platforms](#supported-platforms)
  - [Build](#build)
  - [API Reference](#api-reference)
  - [Contribution](#contribution)
    - [How to contribute?](#how-to-contribute-)
    - [Discussion](#discussion)
    - [Bug reports](#bug-reports)
    - [Pull requests](#pull-requests)
    - [Changelog](#changelog)
  - [Tasks](#tasks)
  - [Copyright and License](#copyright-and-license)

## Usage examples

```c#
var api = new EpitechApi(new [] {HttpStatusCode.InternalServerError});
api.ConnectTo(ConnectionManager.Classic, "https://intra.epitech.eu/?format=json", login, password);
api.ConfigureApi(new List<string>
{
      {"... JSON CONFIGURATION ..."}
});
api.LoadData(new Dictionnary<string, object>
{
      {"KEY-ONE", "VALUE"},
      {"SPECIAL-KEY", new List<string>()
        {"LIST", "OF", "VALUES"}
      },
      {"ANOTHER-KEY", "FINAL-VALUE"}
});

var results = api.Database;
```

## Supported Platforms

* .NET 4.6 (Desktop)

## Build

**build.cmd** not currently available.

## API Reference

See [API Reference document](https://github.com/pheonyx/Epitech-Api-CS/blob/master/API-REFERENCE.md).

## Contribution

### How to contribute ?
There are many ways to contribute to {API#TECH}.  The easiest way is to participate in discussions of features and issues. You can also contribute by sending pull requests of features or bug fixes to us.

### Discussion

You can participate into discussions and ask questions about this API at our [GitHub Issue](https://github.com/pheonyx/Epitech-Api-CS/issues) or our [Slack page](https://apitech-discuss.slack.com).

### Bug reports

When reporting a bug at the issue tracker, please use the [following template](https://github.com/aspnet/Home/wiki/Functional-bug-template). Simply copy and paste the following template into your new issue and please do not alter the template or use it incorrectly.

```
### Title
* A short description of the bug that becomes the issue title *  

### Description
* Does the bug result in any actual functional issue, if so, which one? *

### Minimal repro steps
* What is the smallest, simplest set of steps to reproduce the issue. If needed, provide a project that demonstrates the issue. *  

### Expected result
* What would you expect to happen if there wasn't a bug *  

### Actual result
* What is actually happening*  

### Further technical details
* Optional, details of the root cause if known *  
```
### Pull requests
Pull request of features and bug fixes are both welcomed. Before you send a pull request to us, there are a few steps you need to make sure you've followed.

1. Create a forked repository of [https://github.com/pheonyx/Epitech-Api-CS.git](https://github.com/pheonyx/Epitech-Api-CS)
2. Clone the forked repository into your local environment
3. Make code changes on your local environment
4. Test the changes code with unit test (and, for features, create new unit test)
5. Commit changed code to local repository with clear message
6. Rebase the code to upstream via command git pull --rebase upstream master and resolve conflicts if there is any then continue rebase via command git pull --rebase continue
7. Push local commit to the forked repository
8. Create pull request from forked repository via comparing with upstream

## Changelog

## Todo

### Release v1.1
- [ ] Choice of configuration file to load

### Release v1.2
- [ ] Connection with Office 365
- [ ] Change Database system (Use inheritance of JSON.Net class)

### Release v1.3
- [ ] Improve performance (Memory and Processor)

### Release v2
- [ ] Add cross data features
- [ ] Add multi-thread system

## Copyright and License

Copyright 2016 Phoenyx

Licensed under the [MIT License](https://github.com/pheonyx/Epitech-Api-CS/blob/master/LICENSE)
