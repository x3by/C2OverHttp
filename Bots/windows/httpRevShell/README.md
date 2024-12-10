# Windows Bots

The C# program presents itself to the C2 Flask process through the `/welcome` URI and passing in POST a `json` (or a `x-www-form-urlencoded`) containing its hostname:

```json
{
    "botname": "Windows-01"
}
```

Then the bot every 800 milliseconds makes a GET request to `/c2` to ask for instructions, the response is formulated as follows:

```json
{
    "bot": "Windows-01",
    "cmd": "whoami",
    "rev": "0"
}
```

- **bot**: Who must carry out the instructions
- **cmd**: What command should be executed
- **rev**: This parameter is used to avoid multiple execution fo the same command

If the subject of this response is the bot, and the revision (rev) is different from the one saved by the bot, the bot will execute the command, update the rev, and POST the following body to `/c2`:

```json
{
    "bot": "Windows-01",
    "result": "aHR0cHM6Ly93d3cueW91dHViZS5jb20vd2F0Y2g/dj1kUXc0dzlXZ1hjUQ=="
}
```