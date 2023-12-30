This project shows how to manually expose Naninovel resources to [addressable provider](https://naninovel.com/guide/resource-providers.html#addressable) (without using resource editor menus) and serve specific assets from a remote host.

Be aware, that Naninovel package is not distributed with the project, hence compilation errors will be produced after opening it for the first time; import Naninovel from the Asset Store to resolve the issues.

There are two resources, that are manually exposed to the addressables: 
 - Assets/Backgrounds/1
 - Assets/Backgrounds/2

Notice, that while they are not assigned in backgrounds resources manager menu:

![](https://i.gyazo.com/8c1b37362bf58d26f18e4e61ffe2957c.png)

— they are still accessible in Naninovel scripts in the same way:

```
@back 1
@back 2
```

Furthermore, those two backgrounds are split between asset bundles by labels corresponding to episodes (EP1, EP2, EP3) and are loaded from a remote host:

![](https://i.gyazo.com/ebbd2c4a19b11aefeeac48d8ccc16403.png)

This way when you click a button to start a specific episode in the title menu, only the assets used in that episode will be downloaded from the remote host.

![](https://i.gyazo.com/4042870d6acfc8b78446df4ac791bb8e.png)

The built project for WebGL: https://naninovel.com/addressable 

Actual assets uploaded to the host: https://github.com/Naninovel/Documentation/tree/gh-pages/addressable

If you have any Naninovel-specific questions or issues, [contact the support](https://naninovel.com/support). Be aware, that we're not providing any tutorials or support for Unity's addressable asset system itself, like setting up a remote web hosting for you assets or other deploy/serving scenarios.
