Sure is an exciting start for Jimmy again

Make `appsettings.json` file in the same folder as the binary and put your token there
```json
{
  "Discord": {
    "Token": "Something something token"
  },
  "Imgbb": {
    "Key": "Something something key"
  },
  "TheCatApi": {
    "Key": "Something something key optional"
  }
}
```

## Plan
- [ ] Better `README.MD`.
- [ ] `/config set` - Conveniences command that allows for a more user-friendly way of configuring server settings through interactive buttons.
- [ ] `/rules set` - Conveniences command that allows for a more user-friendly way of configuring rules through interactive buttons.
- [ ] Ability to log roles after a member leaves.
- [ ] Mod command channel config – If set, the command confirmation prompt will be sent there instead and if the command is run somewhere outside this, it will make the response of the bot ephemeral.
- [ ] For god’s sake, please refactor the code.