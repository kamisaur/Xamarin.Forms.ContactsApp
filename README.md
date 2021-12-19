# Xamarin.Forms.ContactsApp

Simple implementation of Contacts app.
Contacts are imported from phonebook and stored in cache using sglite.


## UI States
Main page has 3 states:
1. Normal - displays list of contacts and sync info.
2. Permission Denied - when attempting to sync contacts, if permission was not granted or was revoked the UI will display permission view where app can request for contact permission again.
3. Empty - is show when there are no contacts available. Includes a button for syncing.

Sync info has 3 states:
1. Completed - shown when the sync is completed. Includes information for last performed sync date.
2. Loading - shown when the contacs are being synced
3. Error - shown when an exception is thrown.

For ease of testsing there are toolbar buttons added to main page which can be used to clear cache, sync, throw exception.
