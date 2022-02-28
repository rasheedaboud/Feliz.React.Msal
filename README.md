# Feliz.React.Msal

A wrapper around Microsofts MSAL library for react.

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
        let msal:IPublicClientApplication = createClient msalConfig
        
        
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

Use Authenticated/Unauthenticated template to show or hide sections of ui


    
          AuthenticatedTemplate.create [
              AuthenticatedTemplate.children [
              ]
          ]
    
          UnauthenticatedTemplate.create [
              UnauthenticatedTemplate.children [
              ]
          ]
          


Or use useIsAuthenticated() hook.


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


