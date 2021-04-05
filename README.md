# LaCaliforniaBot

Twitch chat bot made with C#/.Net Core 3.1 that plays messages using TTS.
Uses google cloud for text to speech and logging.

<br>

The bot can be used by the broadcaster, mods, subs and VIPs of the channel the bot is in. If a command says 'Mods only' it means the command can only be used by the broadcaster or mods.

<br>

## Text to speech

Plays a message using text to speech.

```
!k <message>
```


## Info

The bot will give information about its status if text to speech is disabled or there is slow mode enabled.

```
!slowinfo
```


## Toggle - Mods only

Enables (on) and disables (off) the use of the text to speech command (!k).

```
!tts <option>
```


## Slow mode - Mods only

Activates slow mode so users have to wait the specified amount of seconds between messages.

```
!tts <seconds>
```


## Permit - Mods only

Allows someone during the specified amount of minutes to use text to speech if they have no privilege to use it. You can deny the usage before the time runs out.

```
!allowtts <user> <minutes>
!denytts <user>
```


## Max characters - Mods only

Allows moderators to change the max lenght of the message. Default is 100 characters.

```
!ttsmax <maxcharacters>
```

