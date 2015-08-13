# Itavio Sdk
## Best Practices - Unity

### Initializing
It is best to initialize Itavio as early as possible, in the `Start` method of a `MonoBehaviour` that appears early in the game is recommended. Once it is initialized it will instantiate a a persistent `GameObject` named `ItavioManager`.

### Check for the Parent App
Check for the parent app and if necessary present the user with the option to get the parent app every time the IAP storefront is opened. In the case that the user has removed the Parent App, they will be presented with the option to disassociate the plugin from their account.

### Showing the Get App Dialog
If the user does not have the Parent App, the Get App Dialog should be shown every time the user accesses the IAP storefront until the user completes a transaction.

### Linking on **iOS**
Linking should be performed when the IAP storefront is opened, if the parent app is present. This should only be done once, however there should be a button (Get Itavio/Link Itavio) available on the storefront or the options.
