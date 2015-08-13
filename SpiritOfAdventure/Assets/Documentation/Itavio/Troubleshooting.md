# Itavio Sdk
## Troubleshooting
Common missteps while implementing Itavio

### Initializing
1.  Itavio needs to be initialized before any of the other methods will work. After initializing on **iOS**  you will also need to link with the parent app.
2.  Verify that the `secret key` and the `secret key id` are correct.

### Linking on **iOS**
1.  Before Itavio can be used, it will need to be linked to the parent app. This only needs to happen once to associate the the user's account to the game. If the user changes they will need to relink after logging into the parent app.

### Purchases
1.  Make sure that all purchase calls to the store are passed through the `itavioManager.startDebit` method.
    * You can subscribe to the `itavioManager.OnStartDebit` event. This is fired immediately before firing the purchase delegate.
2.  When a debit has completed or been cancelled, it is necessary to finalize the transaction with `itavioManager.finalizeDebit`.

### Errors
1.  Subscribing to the OnError event to handle any problems that arise is recommended.
    * **Android** : Errors in logcat have the `ItavioSdk` tag.
