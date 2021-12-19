# Xamarin.Forms.ContactsApp

Simple implementation of Contacts app.
Contacts are imported from phonebook and stored in cache using sglite.


## UI States
Main page has 3 states:
1. Normal - displays list of contacts and sync info.
2. Permission Denied - when attempting to sync contacts, if permission was not granted or was revoked the UI will display permission view where app can request for contact permission again.
3. Empty - is show when there are no contacts available. Includes a button for syncing.

<html>
  <table style="width:100%">
    <tr>
      <td><img src="https://github.com/kamisaur/Xamarin.Forms.ContactsApp/blob/main/Screenshots/normal_state.jpg"></td>
      <td><img src="https://github.com/kamisaur/Xamarin.Forms.ContactsApp/blob/main/Screenshots/permission_state.jpg"></td>
      <td><img src="https://github.com/kamisaur/Xamarin.Forms.ContactsApp/blob/main/Screenshots/empty_state.jpg"></td>
    </tr>
  </table>
</html>

Sync info has 3 states:
1. Completed - shown when the sync is completed. Includes information for last performed sync date.
2. Loading - shown when the contacs are being synced
3. Error - shown when an exception is thrown.

<html>
  <table style="width:100%">
    <tr>
      <td><img src="https://github.com/kamisaur/Xamarin.Forms.ContactsApp/blob/main/Screenshots/normal_state.jpg"></td>
      <td><img src="https://github.com/kamisaur/Xamarin.Forms.ContactsApp/blob/main/Screenshots/normal_loading_state.jpg"></td>
      <td><img src="https://github.com/kamisaur/Xamarin.Forms.ContactsApp/blob/main/Screenshots/error_state.jpg"></td>
    </tr>
  </table>
</html>

For ease of testsing there are toolbar buttons added to main page which can be used to clear cache, sync, error (throw exception).

<html>
  <table style="width:100%">
    <tr>
      <td><img src="https://github.com/kamisaur/Xamarin.Forms.ContactsApp/blob/main/Screenshots/toolbar_buttons.jpg" width="320px" height="auto"></td>
    </tr>
  </table>
</html>
