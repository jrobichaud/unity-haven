# unity-haven
Various tools or code snippets I developed when I was a Unity Game Developer

## Exporting all the tools to your project
1. Select _Core.Complete_ asset
2. Press on _Build and Pack_ in the Inspector
3. Wait for the compilation process to finish
4. Save the resulting Unity Asset Package
  * There are "normal" error displaying in Unity's console after the build is completed.
    * :exclamation: `Reload Assembly called from managed code directly. This will cause a crash. You should never refresh assets in synchronous mode or enter playmode synchronously from script code.`

    * :exclamation: `InvalidOperationException: Operation is not valid due to the current state of the object`
5. Import this package to your project
6. Follow the extra step of [Core.Drawing Installation Notes](#coredrawing-installation-notes)

## Core

### ExtendedInspector
It is highly recommended to checkout the project and experience it yourself.
[Source](Assets/Core/Demo/Attributes.cs)

#### Reorderable
![](screenshots/Core/reorderable.gif)

#### ToolTip
![](screenshots/Core/tooltip.gif)

#### All
![](screenshots/Core/attributes.png)


### Find References
1. Right-click on an asset
2. Select _Find Reference(s)_
3. Look at Unity's console for this kind of output:

```
[ Selected File(s) ]
Assets/Varia/Stencils/3DimensionCube/Shader/3DStandardMask.shader

[ Direct Reference(s) ]
Assets/Varia/Stencils/3DimensionCube/Material/XMask.mat
Assets/Varia/Stencils/3DimensionCube/Material/YMask.mat
Assets/Varia/Stencils/3DimensionCube/Material/ZMask.mat

[ Indirect Reference(s) or Rare Assets that have both direct AND indirect reference on the selected asset(s) ]
Assets/Varia/Stencils/3DimensionCube/3DimensionCube.prefab
Assets/Varia/UnityUICommonMistakes/FindTheMistakes.unity
```

## Core.AssemblyPacker

## Core.Drawing

### Ditherer
| Original  | RGBA Without Dithering  | RGBA With Dithering |
|-----------|-------------------------|---------------------|
|![Original](Assets/Core.Drawing/Demo/gradient.png). |![Without dithering](screenshots/Core.Drawing/without_dithering.png) | ![With dithering](screenshots/Core.Drawing/with_dithering.png)|

This tool automatically converts 16bit images assets using [dithering](https://en.wikipedia.org/wiki/Dither) through [ImageMagick](https://www.imagemagick.org).

#### Core.Drawing Installation Notes

1. After installing the Unity Asset Package in your project
2. locate the zip file located in [Assets/Core.Drawing/ExternalTools.zip](Assets/Core.Drawing/ExternalTools.zip)
3. Unzip this archive at the root of your project folder (At the same level of `Assets`, `Library`, `ProjectSettings`, etc.)
4. Delete the folder `Assets/Core.Drawing`

## Core.UI

### Large Outline
![Comparison of unity outline with this outline](screenshots/Core.UI/large_outline.png)

### Shear UI
### SpriteUV1

Adds a second normalized coordinate relative to the UI element. It allows to create special shaders in order to overlay the element (or else).

## Notes

Special thanks to [@elaberge](https://github.com/elaberge) for several contributions.

Note: The :trollface: image belongs to it's original owner and therefore is not part of this repository's licence.
