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
Add a reference to the Realtin.Xdsl package in your project:

```xml
<PackageReference Include="Realtin.Xdsl" Version="1.0.0" />
```


### Basic Usage

```csharp
// Create a new XDSL document
var document = new XdslDocument();
// Create and add elements 
var root = document.CreateElement("root", element => { element.CreateElement("child", child => { child.AddAttribute("name", "value"); child.Text = "Hello XDSL!"; }); });
// Write document to string 
string output = document.WriteToString(indented: true);
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

## Requirements
- .NET Standard 2.1 or higher
- C# 13.0 features supported
- Docker (optional, for containerized testing)

## License
GNU GENERAL PUBLIC LICENSE - See LICENSE file for details

## Authors
- Realtin (CodingBoson)
- BosonWare Technologies

## Contributing
Contributions are welcome! Please feel free to submit a Pull Request.
