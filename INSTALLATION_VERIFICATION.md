# Installation Verification

## ✅ Package Structure Checklist

For Unity to recognize the package, these files **must** exist:

### Required Root Files
- [x] `package.json` - Package manifest
- [x] `package.json.meta` - Unity metadata for package.json
- [x] `LICENSE` - MIT License
- [x] `LICENSE.meta` - Unity metadata
- [x] `README.md` - Documentation
- [x] `README.md.meta` - Unity metadata

### Required Folders
- [x] `Editor/` - Editor scripts folder
- [x] `Editor.meta` - Unity metadata for Editor folder
- [x] `Editor/Core/` - Core functionality
- [x] `Editor/Core.meta` - Unity metadata
- [x] `Editor/UI/` - UI components
- [x] `Editor/UI.meta` - Unity metadata

### Assembly Definitions
- [x] `Editor/Instemic.AndroidBridge.Editor.asmdef` - Assembly definition
- [x] `Editor/Instemic.AndroidBridge.Editor.asmdef.meta` - Unity metadata

---

## 🔍 Troubleshooting

### Package Appears Then Disappears

**Symptoms:**
- Package shows in Package Manager briefly
- Then disappears from the list
- No errors shown

**Causes:**
1. Missing `.meta` files (FIXED in latest commit)
2. Invalid `package.json` format
3. Unity version mismatch
4. Git URL issues

**Solutions:**

#### 1. Check Unity Version
```
Minimum: Unity 2021.3+
Recommended: Unity 2022.3 LTS
```

#### 2. Verify Git URL
```
Correct: https://github.com/rcgeorge/unity-android-bridge-toolkit.git
Wrong: https://github.com/rcgeorge/unity-android-bridge-toolkit
        (missing .git)
```

#### 3. Clear Package Cache
```
1. Close Unity
2. Delete: Library/PackageCache/
3. Reopen Unity
4. Re-add package
```

#### 4. Check Console for Errors
```
Window > Console
Look for package-related errors
```

#### 5. Manual Verification
```bash
# Clone the repo
git clone https://github.com/rcgeorge/unity-android-bridge-toolkit.git

# Check structure
ls -la
# Should see package.json, package.json.meta, Editor/, etc.

# Check .meta files
find . -name "*.meta" | head -20
# Should see many .meta files
```

---

## ✅ Installation Steps (Updated)

### Method 1: Package Manager (Recommended)

1. **Open Unity** (2021.3 or newer)

2. **Open Package Manager**
   ```
   Window > Package Manager
   ```

3. **Add from Git**
   - Click `+` in top-left
   - Select `Add package from git URL...`
   - Enter: `https://github.com/rcgeorge/unity-android-bridge-toolkit.git`
   - Click `Add`

4. **Wait for Import**
   - Unity will download and compile
   - May take 30-60 seconds
   - Watch bottom-right progress bar

5. **Verify Installation**
   - Check: `Tools > Android Bridge Toolkit` menu appears
   - Open the toolkit window
   - Should see the main interface

### Method 2: Manual (If Git Method Fails)

1. **Download Release**
   ```
   https://github.com/rcgeorge/unity-android-bridge-toolkit/releases
   ```

2. **Extract to Packages**
   ```
   YourProject/Packages/com.instemic.android-bridge-toolkit/
   ```

3. **Refresh Unity**
   ```
   Assets > Refresh
   ```

### Method 3: Clone into Packages Folder

```bash
cd YourUnityProject/Packages
git clone https://github.com/rcgeorge/unity-android-bridge-toolkit.git com.instemic.android-bridge-toolkit
```

Then restart Unity.

---

## 🧪 Test Installation

### Quick Test

1. **Open Main Window**
   ```
   Tools > Android Bridge Toolkit
   ```
   Should open a window with tabs.

2. **Open Bridge Generator**
   - Click "Bridge Generator" tab
   - Click "Open Bridge Generator Window"
   - Should see split-view editor

3. **Test Generation**
   - Paste this Java code:
   ```java
   package com.test;
   public class Test {
       public static String hello() { return "Hello"; }
   }
   ```
   - Click "⚡ Generate Bridge"
   - Should see C# code on right side

4. **Expected Output**
   ```csharp
   public class TestBridge
   {
       private static AndroidJavaClass javaClass;
       
       static TestBridge()
       {
           javaClass = new AndroidJavaClass("com.test.Test");
       }
       
       public static string Hello()
       {
           return javaClass.CallStatic<string>("hello");
       }
   }
   ```

### Success Indicators

✅ **Package is working if:**
- Menu appears: `Tools > Android Bridge Toolkit`
- Windows open without errors
- Code generation works
- No console errors

❌ **Package has issues if:**
- No menu entry appears
- Windows don't open
- Console shows compilation errors
- Package disappears from Package Manager

---

## 📝 Version Check

```csharp
// In any Unity script:
using Instemic.AndroidBridge;

// If this compiles, package is installed correctly
```

---

## 🆘 Still Having Issues?

1. **Check Unity Console**
   - Look for red errors
   - Share error messages

2. **Check Package Manager**
   - Is package listed?
   - What's the status?
   - Any warnings?

3. **Try Manual Install**
   - Download as ZIP
   - Extract to Assets/
   - Does it work?

4. **Report Issue**
   ```
   https://github.com/rcgeorge/unity-android-bridge-toolkit/issues/new
   ```
   Include:
   - Unity version
   - Installation method tried
   - Error messages
   - Screenshots

---

## 📊 Known Working Configurations

| Unity Version | Platform | Status |
|--------------|----------|--------|
| 2021.3 LTS | Windows | ✅ Tested |
| 2022.3 LTS | Windows | ✅ Tested |
| 2023.2 | Windows | ⚠️ Untested |
| 2021.3 LTS | macOS | ⚠️ Untested |
| 2022.3 LTS | Linux | ⚠️ Untested |

Help us test! Report your configuration.

---

## 🎯 Final Check

Run this checklist:

- [ ] Unity 2021.3+ installed
- [ ] Git URL includes `.git` at end
- [ ] Package Manager shows package
- [ ] `Tools` menu has toolkit entry
- [ ] Main window opens
- [ ] Bridge Generator works
- [ ] Can generate sample code

If all checked ✅ - **Installation successful!**
