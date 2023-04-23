# Feliz.React.Msal

A wrapper around Microsofts MSAL library for react. This package can be used with Azure AD OR Azure B2C, simply change config object to refelct right endpoints.
[Here](https://github.com/rasheedaboud/Feliz.Auth.Examples/blob/master/README.md) is a minimal repo showinf step by step how to get started using Feliz minimal template. It does assume you register you application in AD or B2C in advance.

#### Getting Started 

1. Install npm dependencies

    npm:
    ```fs
    dotnet add package Feliz.React.Msal
    npm install @azure/msal-react
    ```

    femto:
    ```fs
      femto install Feliz.React.Msal
    ```
    
2. Initialize msal config (this is an example using B2C with sign in flow)

    ```fs
        open Feliz.React.Msal
    
        let msalConfig ={
            auth={
                  clientId=""
                  authority="https://<domain>.b2clogin.com/<domain>.onmicrosoft.com/<Sign in flow>"
                  knownAuthorities=[|"https://<domain>.b2clogin.com"|]
                  redirectUri= "https://localhost:8080/"
                  postLogoutRedirectUri = "https://localhost:8080/"};
            cache={cacheLocation="sessionStorage"; storeAuthStateInCookie=false}
          }
        let client:IPublicClientApplication = createClient msalConfig
        
        
    ```
    
3. Pass client into msal component
    ```fs
  
        [<ReactComponent>]
        let App() =
            MsalProvider.create[
                MsalProvider.instance client
                MsalProvider.children[
                ]
            ]
             
    ```
    
    
#### Protecting sections of site

Use ``Authenticated/Unauthenticated`` template to show or hide sections of UI. Alternatively  use ``useIsAuthenticated()`` hook if use ``if XXX then xxx else `` block to show or hide UI.

```fs
      AuthenticatedTemplate.create [
          AuthenticatedTemplate.children [
          ]
      ]

      UnauthenticatedTemplate.create [
          UnauthenticatedTemplate.children [
          ]
      ]
          
```



If you are using Elmish you will need to use the ``useEffect()`` react hook in conjuction with ``useIsAuthenticated()`` or ``useMsal()`` hooks to verify user is logged in and dispatch an event.

```fs
[<ReactComponent>]
let Component () =

    let client = useMsal()

    let isAuthenticated = useIsAuthenticated(None)

    //Dispatch you event here
    let someEffect() = async {
        if isAuthenticated then
            let account = client.accounts[0]
            setUser  Some account |> dispatch
        else 
            setUser  None |> dispatch
    }

    //Re runs effect isAuthenticated changes
    React.useEffect(someEffect >> Async.StartImmediate, [| box isAuthenticated |])
```

#### Hooks 

Support for the following custon msal hooks are supported under the 'Hooks' module;

1. useAccount
2. useIsAuthenticated
3. useMsal
4. useMsalAuthentication

#### Links

1. MSAL react library can be found on [NPM](https://www.npmjs.com/package/@azure/msal-react).
2. Microsoft github page for [MSAL](https://github.com/AzureAD/microsoft-authentication-library-for-js).
3. Microsoft docs showing example usage of MSAL react library can be found [here](https://docs.microsoft.com/en-us/azure/active-directory/develop/tutorial-v2-react).


