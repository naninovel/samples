![](https://i.gyazo.com/f93f0e73389934bf25226f4000e437eb.gif)

The project shows an example on how you can implement interctive map with Naninovel without any C# scripts.

The map is implemented as a [custom UI](https://naninovel.com/guide/user-interface.html#adding-custom-ui) and stored at `Assets/Prefabs/MapUI`. The locations are regular Unity buttons placed in the UI.

![](https://i.gyazo.com/f421eaf666c9d84b04d23a72d1259f47.png)

The button's click and hover events are handled with Naninovel's [Play Script](https://naninovel.com/guide/user-interface.html#play-script-on-unity-event) component.

![](https://i.gyazo.com/a64ee9beee378c687d0d8093334f4ef7.png)

The availability of the locations is controlled with [Variable Trigger](https://naninovel.com/guide/custom-variables.html#variable-triggers) components attached to the buttons.

![](https://i.gyazo.com/9bcb2107de1fa7873fe1e18ab7c8d625.png)

Be aware, that [Naninovel](https://u3d.as/1pg9) package is not distributed with the project, hence compilation errors will be produced after opening it for the first time; import Naninovel from the Asset Store to resolve the issues.
