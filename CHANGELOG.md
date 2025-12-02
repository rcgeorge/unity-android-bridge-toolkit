# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-12-02

### Added

#### Core Features
- **Bridge Generator**: Automatic C# bridge generation from Java source code
  - Parse Java classes, methods, and parameters
  - Convert Java types to C# types automatically
  - Generate Unity-compatible AndroidJavaClass/AndroidJavaObject code
  - Support for static and instance methods
  - XML documentation comments
  - PascalCase method naming for C# conventions

#### UI Components
- Main toolkit window with tabbed interface
- Bridge Generator window with split-view editor
  - Java input panel with syntax highlighting
  - C# output panel with generated code
  - Load Java files from disk
  - Copy generated code to clipboard
  - Save as C# file directly
- Settings window for toolkit configuration
  - Auto-refresh preferences
  - Notification settings
  - Code generation options
  - Default output paths

#### Documentation
- Comprehensive README with features and examples
- Quick Start Guide with step-by-step tutorial
- Sample Java file (SimpleSDK.java) for testing
- Real-world Viture XR example
- Troubleshooting section

#### Project Structure
- Unity Package Manager support (package.json)
- Assembly definitions for proper Unity integration
- MIT License with Instemic copyright
- Contributing guidelines
- Proper .gitignore for Unity projects

### Technical Details

#### Core Components
- `JavaParser.cs`: Regex-based Java source code parser
- `TypeConverter.cs`: Java to C# type conversion
- `BridgeGenerator.cs`: Code generation engine
- `JavaClass.cs`: Data models for parsed Java structures

#### Supported Features
- Static method bridging
- Instance method bridging (with getInstance pattern)
- Primitive type conversion (int, float, bool, etc.)
- String handling
- Array support
- AndroidJavaObject for complex types
- Method parameters with multiple arguments

### Known Limitations

- No support for Java generics yet (stripped during parsing)
- Instance method initialization assumes getInstance() pattern
- No field/property generation yet
- No callback/listener support yet
- No nested class support yet

### Coming in v1.1

- AAR Builder integration
- APK Decompiler with JADX
- Improved Java parser with proper AST
- Generic type support
- Callback/listener pattern support
- Batch processing for multiple files
- More code generation templates

---

## [Unreleased]

### Planned Features

#### v1.1 (Next Release)
- [ ] AAR Builder without Android Studio
- [ ] APK Decompiler integration
- [ ] Improved type handling (generics)
- [ ] Callback support (UnityPlayer.UnitySendMessage)
- [ ] Batch bridge generation
- [ ] Code templates library

#### v1.2
- [ ] Field/property generation
- [ ] Constructor bridging
- [ ] Nested class support
- [ ] Annotation processing
- [ ] Error handling generation

#### v2.0 (Future)
- [ ] Visual scripting node generation
- [ ] iOS bridge support (Objective-C)
- [ ] Cloud build integration
- [ ] AI-assisted code generation
- [ ] Package manager for common SDKs

---

## Development Notes

### Architecture Decisions

**Why Regex Parser Instead of Full AST?**
- Simpler implementation for v1.0
- Covers 80% of common use cases
- Faster for simple classes
- Full AST parser planned for v1.2

**Why Template-Based Generation?**
- Predictable output
- Easy to customize
- No AI dependencies
- Deterministic results

**Why No JADX Bundling in v1.0?**
- Reduces package size
- User can install separately if needed
- Pure C# solution for core features
- JADX integration coming in v1.1

---

For detailed information, see:
- [README.md](README.md)
- [Quick Start Guide](Documentation/QuickStart.md)
- [GitHub Releases](https://github.com/rcgeorge/unity-android-bridge-toolkit/releases)
