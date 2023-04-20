# MiniContainer

[![Run tests](https://github.com/Voresh/MiniContainer/actions/workflows/runTests.yml/badge.svg?branch=main)](https://github.com/Voresh/MiniContainer/actions/workflows/runTests.yml)

**MiniContainer** is fast and lightweight **DI Container** designed to simplify dependency injection in your codebase.
With a small code base and easy-to-learn API, it's a great choice for developers who want a fast and reliable solution without the complexity of larger frameworks.

### Key Features

- **Constructor injection:** Ensures reliable dependency management.
- **Scoping:** Allows defining the lifetime of dependencies.
- **Factories:** Uses `Func` as factories for dynamic dependency creation.
- **Open generics:** Supports registering and resolving of generic types.
- **Source Generators:** Uses Source Generators to improve Resolve performance up to 5-10 times.
- **Loosely coupled:** Promotes loosely-coupled architecture, so your code can remain decoupled from the framework.
- **Platform Agnostic:** Can be used outside of the Unity platform. 

## Table of Contents

- [Getting Started](#getting-started)
- [Scoping](#scoping)
- [Factories](#factories)
- [Open Generics](#open-generics)
- [Performance](#performance)
  - [Code generation](#code-generation)
- [Installation](#installation)
  - [Install via Git URL](#install-via-git-url)
- [License](#license)

## Getting Started

To begin using Unity Injector, you first need to build your container by registering all the dependencies you will need.

Once your container is built, you can run your entry point and use your registered dependencies.

```csharp
public class Program {
    [RuntimeInitializeOnLoadMethod]
    private static void Main() {
        var container = Build();
        Start(container);
    }

    private static Container Build() {
        var container = new Container();
        container.Register<Service>();
        container.Register<AnotherService, IAnotherService>();
        return container;
    }

    private static void Start(Container container) {
        container.Resolve<Service>();
    }
}

public class Service {
    public Service(IAnotherService anotherService) {
        Debug.Log($"constructor call, got {anotherService}");
    }
}    
```

In the following example, Service is registered as self and AnotherService is registered as IAnotherService.

## Scoping

Unity Injector supports registering dependencies in scoped containers, which can be useful for organizing dependencies in different scopes, such as game or menu scopes.

Here's an example of registering and resolving a dependency in a scoped container:

```csharp
using (var scopeContainer = new Container(container)) {
    scopeContainer.Register<ScopedService>();
    
    scopeContainer.Resolve<ScopedService>();
}
```

Scoped containers can also have a parent container, which allows them to access dependencies registered in the parent container.

When a scoped container is disposed, Unity Injector will automatically dispose all registered `IDisposable` dependencies within that scope.

## Factories

Unity Injector allows you to use Func as a factory to create dependencies. You can register the factory as an instance and use it as a dependency.

Here's an example of registering a Func factory that can create a FactoryProduction instance:

```csharp
var container = new Container();
container.RegisterInstance<Func<int, FactoryProduction>>(_ => new FactoryProduction(container.Resolve<AnotherService>(), _));

public Service(Func<int, FactoryProduction> productionFactory) {
    var production = productionFactory(1);
}
```

## Open Generics

Unity Injector allows registration of open generic types, which can be useful for situations where you want to define a dependency based on a generic type, but without specifying the type argument.

Here's an example of registering and resolving a generic type with a type parameter that's not specified:

```csharp
container.Register(typeof(Logger<>), typeof(ILogger<>));

public Service(ILogger<Service> logger) {
    logger.Log("constructor call");
}
```

## Performance

### Code generation

Unity Injector leverages Source Generators to eliminate reflection and create dependencies directly using constructor, with a fallback to reflection when necessary.
To use source-generated code, you'll need to register your instance constructors, these constructors are named as `"AssemblyName"_GeneratedInstanceConstructor`.

Here's an example of setting the instance constructors:

```csharp
Container.SetInstanceConstructors(
    new AssemblyCSharp_GeneratedInstanceConstructor(),
    new ReflectionInstanceConstructor());
```

## Installation

Requires `Unity 2021.3+`

### Install via git URL

You can select "Add package from git URL" in Package Manager and paste `https://github.com/Voresh/MiniContainer.git?path=Unity/Assets/MiniContainer` there.

Or you can add `"com.voresh.minicontainer": "https://github.com/Voresh/MiniContainer.git?path=Unity/Assets/MiniContainer",` to `Packages/manifest.json`

## License

    MIT License
    
    Copyright (c) 2023 Vladimir Oreshkov aka Voresh
    
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
