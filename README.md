# HexagonalUI

[![Docs](https://github.com/mpewsey/HexagonalUI/actions/workflows/docs.yml/badge.svg?event=push)](https://mpewsey.github.io/HexagonalUI)

## Purpose

Unity currently does not offer a built-in layout group for creating stackings of hexagonal elements, such as those sometimes used for skill grids. This package aims to fill that gap by providing a hexagonal layout group that will page UI elements into a grid.

![HexLayoutGroup](https://user-images.githubusercontent.com/23442063/163601049-a1522652-6063-4976-ae87-35b92b21187e.png)

## Installation

To add the package to a project, in Unity, select `Window > Package Manager`.

![HexagonalUI](https://user-images.githubusercontent.com/23442063/163601100-191d8699-f4fd-42cc-96d4-f6aa5a8ae29b.png)

Select `Add package from git URL...` and paste the following URL:

```
https://github.com/mpewsey/HexagonalUI.git
```

NOTE: To lock into a specific version, append `#{VERSION_TAG}` to the end of the URL. For example:

```
https://github.com/mpewsey/HexagonalUI.git#v1.3.0
```

## Usage

To use, simply attach the `HexLayoutGroup` component to a Game Object, as you would with Unity's built-in layout group components. The cell orientation component setting is based on the direction of the hexagonal element's long diagonal. In the image above, the hexagons on the top feature a horizontal cell orientation, whereas the hexagons on the bottom feature a vertical orientation.

Due to the staggering of the hexagonal elements, the default `Button` component input navigation tends to navigate randomly. Therefore, if buttons will serve as the children of the layout group, it is recommended that the `HexButton` component, which provides more regular navigation behaviour, be used instead.

## Tile Art Creation Guidance

To match the dimensions assigned by the hexagonal layout group, it is recommended that hexagonal tile art have their long diagonals to short diagonals proportioned to 1 : 0.86602540378. For instance, the example tile images in this package are 100 px by 87 px, including the required rounding to the nearest pixel.
