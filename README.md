# Realtin.Xdsl
A lightweight, high-performance XML-like Domain Specific Language (XDSL) library for .NET.

## Features
- Fast and memory-efficient XDSL document parsing and writing
- Support for elements, attributes, tags, and comments
- Resource loading capabilities 
- Streaming support for large documents
- Indented and non-indented output formats
- Async operations support
- Docker integration ready
- Benchmarking tools included

## Project Structure
The solution consists of three main projects:

- **Realtin.Xdsl**: Core library containing the base XDSL implementation
- **Realtin.Xdsl.Experimental**: Extended features and experimental implementations
- **Realtin.Xdsl.Lab**: Benchmarking and testing project for performance analysis

## Getting Started
### Installation
Add a reference to the `Realtin.Xdsl` package in your project:

```bash
dotnet add package Realtin.Xdsl
```
Or
```xml
<PackageReference Include="Realtin.Xdsl" Version="1.2.0" />
```


### Basic Usage

```csharp
// Create a new XDSL document
var document = new XdslDocument();

// Create and add elements 
document.CreateElement("Person", person => {
    person.CreateElement("FirstName", x => x.Text = "Java");
    person.CreateElement("LastName", x => x.Text = "Script");
    person.CreateElement("Age", x => x.Text = "30");
});

// Write document to string 
var prettyOutput = document.WriteToString(writeIndented: true);

Console.WriteLine(prettyOutput);
```

### Serialize an Object to Xdsl

```csharp
using Realtin.Xdsl;
using Realtin.Xdsl.Serialization;

var user = new
{
    Name = "User",
    Email = "user@example.com",
    Age = 20
};

var document = XdslSerializer.Serialize(user);
Console.WriteLine(document.WriteToString(writeIndented: true));
```

**Output:**

```xml
<User>
  <Name>Realtin</Name>
  <Email>realtin@example.com</Email>
  <Age>20</Age>
</User>
```

### XQL Queries
```csharp
var usersResult = XqlExpression
    .Compile(@"WHERE X:NAME == ""User"" SELECT")
    .Execute(doc, XqlPermissions.Read);

if (usersResult.TryCast<IEnumerable<XdslElement>>(out var users)) {
    foreach (var user in users) {
        Console.WriteLine(user.Text);
    }
}
```

## Key Components

### XdslElement
Base class for all XDSL elements, supporting:
- Hierarchical node structure
- Attribute management
- Resource loading
- Text content

### XdslDocument
Represents an XDSL document with capabilities for:
- Loading and saving documents
- Managing document structure
- Version control
- Document type specification

### XdslTextWriter
Provides efficient writing capabilities with:
- Indented output support
- Streaming capabilities
- StringBuilder integration
- Performance optimizations

## Performance
The library is designed with performance in mind:
- Aggressive inlining of critical methods
- Minimal allocations
- Optional pooling for frequently used objects
- Optimized string operations

### Performance Benchmarks

Xdsl offers industry-leading performance for serialization and deserialization. Benchmarks compared to popular formats:

|Serializer|Time (ns)|Size (Bytes)|
|---|---|---|
|**Xdsl Custom Serializer**|236.1|600|
|**Proto Serializer**|774.7|504|
|**Newtonsoft JSON**|2,043.5|4,160|


## Requirements
- .NET Standard 2.1 or higher
- C# 13.0 features supported

## License
MIT - See LICENSE file for details

## Authors
- Realtin (CodingBoson)
- BosonWare, Technologies

## Contributing
Contributions are welcome! Please feel free to submit a Pull Request.
