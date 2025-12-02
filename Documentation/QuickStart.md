# Quick Start Guide

## Installation

### Unity Package Manager (Recommended)

1. Open Unity (2021.3 or newer)
2. Open Package Manager: `Window > Package Manager`
3. Click `+` in the top-left corner
4. Select `Add package from git URL...`
5. Enter: `https://github.com/rcgeorge/unity-android-bridge-toolkit.git`
6. Click `Add`

Unity will download and install the package automatically.

### Manual Installation

1. Download the latest release from GitHub
2. Extract to `Assets/AndroidBridgeToolkit/`
3. Unity will automatically import the package

## Your First Bridge

### Step 1: Open the Toolkit

From Unity's menu:
```
Tools > Android Bridge Toolkit
```

This opens the main window with tabs for different features.

### Step 2: Open Bridge Generator

Click on the **Bridge Generator** tab, then click **"Open Bridge Generator Window"**.

You'll see a split view:
- **Left side**: Java input
- **Right side**: Generated C# output

### Step 3: Load Sample Java Code

For this tutorial, copy this sample Java class:

```java
package com.example.sdk;

public class SimpleSDK {
    public static void init() {
        // Initialization code
    }
    
    public static String getMessage() {
        return "Hello from SDK!";
    }
    
    public static int calculate(int a, int b) {
        return a + b;
    }
}
```

Paste it into the left panel.

### Step 4: Generate the Bridge

Click the **"⚡ Generate Bridge"** button.

The right panel will show the generated C# code:

```csharp
using UnityEngine;
using System;

/// <summary>
/// Unity bridge for SimpleSDK
/// Java class: com.example.sdk.SimpleSDK
/// </summary>
public class SimpleSDKBridge
{
    private static AndroidJavaClass javaClass;
    
    static SimpleSDKBridge()
    {
        javaClass = new AndroidJavaClass("com.example.sdk.SimpleSDK");
    }
    
    /// <summary>
    /// Calls init in Java
    /// </summary>
    public static void Init()
    {
        javaClass.CallStatic("init");
    }
    
    /// <summary>
    /// Calls getMessage in Java
    /// </summary>
    /// <returns>string</returns>
    public static string GetMessage()
    {
        return javaClass.CallStatic<string>("getMessage");
    }
    
    /// <summary>
    /// Calls calculate in Java
    /// </summary>
    /// <param name="a">int</param>
    /// <param name="b">int</param>
    /// <returns>int</returns>
    public static int Calculate(int a, int b)
    {
        return javaClass.CallStatic<int>("calculate", a, b);
    }
}
```

### Step 5: Save the Generated Code

Click **"💾 Save as C# File"**.

Choose a location (e.g., `Assets/Scripts/SimpleSDKBridge.cs`) and save.

Unity will automatically import the new C# file!

### Step 6: Use in Unity

Now you can use the bridge in any Unity script:

```csharp
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        // Only works on Android!
        #if UNITY_ANDROID && !UNITY_EDITOR
        
        // Initialize SDK
        SimpleSDKBridge.Init();
        
        // Get message
        string msg = SimpleSDKBridge.GetMessage();
        Debug.Log(msg); // "Hello from SDK!"
        
        // Calculate
        int result = SimpleSDKBridge.Calculate(5, 3);
        Debug.Log($"5 + 3 = {result}"); // "5 + 3 = 8"
        
        #endif
    }
}
```

## Real-World Example: Viture XR

Let's use a real example from the Viture XR SDK:

### Java Code (from decompiled APK):

```java
package com.viture.xr;

public class VitureXR {
    public static void init() {
        // Initialize XR system
    }
    
    public static int getGlassesModel() {
        return 4; // Luma Ultra
    }
    
    public static String getGlassesModelName() {
        return "Luma Ultra";
    }
    
    public static boolean supports6DOF() {
        return true;
    }
}
```

### Generated Bridge:

1. Paste the Java code into Bridge Generator
2. Click "Generate Bridge"
3. Save as `VitureXRBridge.cs`

### Usage:

```csharp
void Start()
{
    #if UNITY_ANDROID && !UNITY_EDITOR
    
    VitureXRBridge.Init();
    
    int model = VitureXRBridge.GetGlassesModel();
    string modelName = VitureXRBridge.GetGlassesModelName();
    bool has6DOF = VitureXRBridge.Supports6DOF();
    
    Debug.Log($"Glasses: {modelName} (Model {model})");
    Debug.Log($"6DOF Support: {has6DOF}");
    
    #endif
}
```

## Tips & Best Practices

### 1. Test on Device

Bridges only work on Android devices, not in the Unity Editor:

```csharp
#if UNITY_ANDROID && !UNITY_EDITOR
    // Bridge code here
#else
    Debug.LogWarning("Android bridge only works on device!");
#endif
```

### 2. Handle Exceptions

```csharp
try
{
    SimpleSDKBridge.Init();
}
catch (System.Exception e)
{
    Debug.LogError($"Failed to init SDK: {e.Message}");
}
```

### 3. Check Availability

Some methods may not be available on all Android versions:

```csharp
if (Application.platform == RuntimePlatform.Android)
{
    // Safe to use bridge
}
```

## Next Steps

- **Learn AAR Building**: Build custom Android libraries
- **APK Decompilation**: Study existing implementations
- **Advanced Features**: Complex types, callbacks, threading

## Troubleshooting

### "Class not found" Error

Make sure:
1. The Java package name is correct
2. The AAR is in `Assets/Plugins/Android/`
3. You're testing on a real Android device

### "Method not found" Error

Check:
1. Method name matches exactly (case-sensitive)
2. Method is `public`
3. For instance methods, object is initialized

### Build Errors

Ensure:
1. Android Build Support is installed
2. Target API level is set correctly
3. Required permissions are in AndroidManifest.xml

## Support

- 📖 [Full Documentation](https://github.com/rcgeorge/unity-android-bridge-toolkit/wiki)
- 🐛 [Report Issues](https://github.com/rcgeorge/unity-android-bridge-toolkit/issues)
- 💬 [Discussions](https://github.com/rcgeorge/unity-android-bridge-toolkit/discussions)

---

**You're ready to create Android bridges!** 🎉
